using DaddysHere.Models;
using DaddysHere.Services;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<SonDatabaseSettings>(builder.Configuration.GetSection("SonDatabase"));
builder.Services.AddSingleton<SonsService>();
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("zh-Hanse");
});
builder.Services.AddLocalization();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
