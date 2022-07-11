using DaddysHere.Models;
using DaddysHere.Services;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

using NLog;
using NLog.Web;
using AspNetCoreRateLimit;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("初始化……");

try
{
    var builder = WebApplication.CreateBuilder(args);

    #region 各种配置
    builder.Services.AddOptions();
    builder.Services.AddMemoryCache();
    builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
    builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
    builder.Services.AddInMemoryRateLimiting();
    builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
    builder.Services.Configure<DaddysHereDatabaseSettings>(builder.Configuration.GetSection("DaddysHereDatabase"));
    builder.Services.Configure<DaddysHereGeneralSettings>(builder.Configuration.GetSection("DaddysHereGeneral"));
    builder.Services.AddSingleton<SonsService>();
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins("https://我是你爹.com", "https://*.我是你爹.com", "https://hoppscotch.io", "http://localhost", "http://127.0.0.1", "https://localhost", "https://127.0.0.1").AllowAnyHeader().AllowAnyMethod();
        });
    });
    builder.Services.Configure<RequestLocalizationOptions>(options =>
    {
        options.DefaultRequestCulture = new RequestCulture("zh-Hanse");
    });
    builder.Services.AddLocalization();
    var migrationOptions = new MongoMigrationOptions
    {
        MigrationStrategy = new MigrateMongoMigrationStrategy(),
        BackupStrategy = new CollectionMongoBackupStrategy(),
    };
    var storageOptions = new MongoStorageOptions
    {
        MigrationOptions = migrationOptions,
        CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection
    };
    var config = new ConfigurationBuilder().SetBasePath(Environment.CurrentDirectory).AddJsonFile("appsettings.json").AddInMemoryCollection().Build();
    var connectionString = config["DaddysHereDatabase:ConnectionString"];
    var databaseName = config["DaddysHereDatabase:DatabaseName"];
    string hangfireDBConnectionStr = $"{connectionString}/{databaseName}";
    logger.Debug($"Hangfire 数据库连接字符串：{hangfireDBConnectionStr}");
    builder.Services.AddHangfire(options => options.UseMongoStorage(hangfireDBConnectionStr, storageOptions));
    builder.Services.AddHangfireServer(options => { options.WorkerCount = 1; });
    var fullCRUDEnabledStr = config["DaddysHereGeneral:EnableFullCRUD"];
    bool parseRes = bool.TryParse(fullCRUDEnabledStr, out bool fullCRUDEnabled);
    if (parseRes)
    {
        logger.Debug($"启用完整增删查改：{fullCRUDEnabled}");
    }
    else
    {
        logger.Error("启用完整增删查改 设置项解析失败。");
    }
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();
    #endregion

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    #region 国际化、请求频率限制、Hangfire 定时任务
    var support = new List<CultureInfo>()
    {
        new CultureInfo("zh-Hans")
    };
    app.UseRequestLocalization(x =>
    {
        x.SetDefaultCulture("zh-Hans");
        x.SupportedCultures = support;
        x.SupportedUICultures = support;
        x.AddInitialRequestCultureProvider(new AcceptLanguageHeaderRequestCultureProvider());
    });
    app.UseIpRateLimiting();
    app.UseHangfireDashboard();
    RecurringJob.AddOrUpdate<SonsService>(x => x.DeleteExpiredSons(), Cron.Daily, TimeZoneInfo.Local);
    app.UseCors(); // 放在 UseAuthorization 之前
    #endregion

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    #region
    if (app.Environment.IsProduction())  // 只在生产环境使用 Https 重定向
    {
        app.UseHttpsRedirection();
    }
    #endregion

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception exception)
{
    logger.Error(exception, "Program 异常，已停止。");
    throw;
}
finally
{
    LogManager.Shutdown();
}
