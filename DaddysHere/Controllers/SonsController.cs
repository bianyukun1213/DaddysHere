using DaddysHere.Models;
using DaddysHere.Resources;
using DaddysHere.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

// https://docs.microsoft.com/zh-cn/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-6.0&tabs=visual-studio#add-a-controller
namespace DaddysHere.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SonsController : ControllerBase
    {
        private readonly SonsService _sonsService;
        private readonly bool _enableFullCRUD;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly ILogger<SonsController> _logger;

        public SonsController(SonsService sonsService, IOptions<DaddysHereGeneralSettings> daddysHereGeneralSettings, IStringLocalizer<SharedResource> localizer, ILogger<SonsController> logger)
        {
            _sonsService = sonsService;
            _enableFullCRUD = daddysHereGeneralSettings.Value.EnableFullCRUD;
            _localizer = localizer;
            _logger = logger;
            _logger.LogDebug(1, "NLog 已注入到 SonsController。");
        }
        [HttpGet("get-all")]
        public async Task<StandardReturn> GetSonsAsync()
        {
            _logger.LogInformation("调用 GetSonsAsync。");
            if (!_enableFullCRUD)
            {
                _logger.LogInformation("未启用完整增删查改，无权操作。");
                return new StandardReturn(errorType: StandardReturn.ErrorType.PermissionDenied, localizer: _localizer);
            }
            var sons = await _sonsService.GetSonsAsync();
            if (sons.Count == 0)
            {
                _logger.LogInformation("数据不存在。");
                return new StandardReturn(errorType: StandardReturn.ErrorType.DataNotFound, localizer: _localizer);
            }
            return new StandardReturn(result: sons, localizer: _localizer);
        }
        [HttpGet("get-by-name/{name:maxlength(10)}")]
        public async Task<StandardReturn> GetSonsByNameAsync(string name)
        {
            _logger.LogInformation("调用 GetSonsByNameAsync，名字：{name}。", name);
            var sons = await _sonsService.GetSonsByNameAsync(name);
            if (sons.Count == 0)
            {
                _logger.LogInformation("数据不存在。");
                return new StandardReturn(errorType: StandardReturn.ErrorType.DataNotFound, localizer: _localizer);
            }
            return new StandardReturn(result: sons, localizer: _localizer);
        }
        [HttpGet("get-by-daddy/{daddyName:maxlength(10)}")]
        public async Task<StandardReturn> GetSonsByDaddyAsync(string daddyName)
        {
            _logger.LogInformation("调用 GetSonsByDaddyAsync，爹：{daddyName}。", daddyName);
            if (!_enableFullCRUD)
            {
                _logger.LogInformation("未启用完整增删查改，无权操作。");
                return new StandardReturn(errorType: StandardReturn.ErrorType.PermissionDenied, localizer: _localizer);
            }
            var sons = await _sonsService.GetSonsByDaddyAsync(daddyName);
            if (sons.Count == 0)
            {
                _logger.LogInformation("数据不存在。");
                return new StandardReturn(errorType: StandardReturn.ErrorType.DataNotFound, localizer: _localizer);
            }
            return new StandardReturn(result: sons, localizer: _localizer);
        }
        [HttpGet("get-by-name-and-daddy/{daddyName:maxlength(10)}/{name:maxlength(10)}")]
        public async Task<StandardReturn> GetSonByNameAndDaddyAsync(string name, string daddyName)
        {
            _logger.LogInformation("调用 GetSonsByNameAndDaddyAsync，名字：{name}，爹：{daddyName}。", name, daddyName);
            if (!_enableFullCRUD)
            {
                _logger.LogInformation("未启用完整增删查改，无权操作。");
                return new StandardReturn(errorType: StandardReturn.ErrorType.PermissionDenied, localizer: _localizer);
            }
            var son = await _sonsService.GetSonByNameAndDaddyAsync(name, daddyName);
            if (son is null)
            {
                _logger.LogInformation("数据不存在。");
                return new StandardReturn(errorType: StandardReturn.ErrorType.DataNotFound, localizer: _localizer);
            }
            return new StandardReturn(result: son, localizer: _localizer);
        }
        [HttpGet("get-by-id/{id:length(24)}")]
        public async Task<StandardReturn> GetSonByIdAsync(string id)
        {
            _logger.LogInformation("调用 GetSonByIdAsync，Id：{id}。", id);
            if (!_enableFullCRUD)
            {
                _logger.LogInformation("未启用完整增删查改，无权操作。");
                return new StandardReturn(errorType: StandardReturn.ErrorType.PermissionDenied, localizer: _localizer);
            }
            var son = await _sonsService.GetSonByIdAsync(id);
            if (son is null)
            {
                _logger.LogInformation("数据不存在。");
                return new StandardReturn(errorType: StandardReturn.ErrorType.DataNotFound, localizer: _localizer);
            }
            return new StandardReturn(result: son, localizer: _localizer);
        }
        [HttpPost("create")]
        public async Task<StandardReturn> CreateSonAsync([FromBody] Son newSon)
        {
            _logger.LogInformation("调用 CreateSonAsync，儿子：{son}。", newSon);
            if (!_enableFullCRUD)
            {
                _logger.LogInformation("未启用完整增删查改，无权操作。");
                return new StandardReturn(errorType: StandardReturn.ErrorType.PermissionDenied, localizer: _localizer);
            }
            if (await _sonsService.DoesSonsCountReachLimitValueAsync(newSon.Name) || await _sonsService.DoesSonsCountReachLimitValueAsync())
            {
                _logger.LogInformation("儿子数量达到限值。");
                return new StandardReturn(errorType: StandardReturn.ErrorType.LimitValueReached, localizer: _localizer);
            }
            var son = await _sonsService.GetSonByNameAndDaddyAsync(newSon.Name, newSon.Daddy);
            if (son is null)
            {
                newSon.Id = null;
                if (!_sonsService.IsSonValid(newSon))
                {
                    _logger.LogInformation("儿子 {son} 非法。", newSon);
                    return new StandardReturn(errorType: StandardReturn.ErrorType.WrongData, localizer: _localizer);
                }
                newSon.Expiration = DateTime.Today.AddDays(30);
                newSon.Reserved = false; // 通过 API 创建的 Son，Reserved 全部设为 false
                _logger.LogInformation("新建儿子：{son}。", newSon);
                await _sonsService.CreateSonAsync(newSon);
                return new StandardReturn(localizer: _localizer);
            }
            _logger.LogInformation("儿子 {son} 重复。", newSon);
            return new StandardReturn(errorType: StandardReturn.ErrorType.RepeatedSubmissionNotAllowed, localizer: _localizer);
        }
        [HttpPut("update-by-id/{id:length(24)}")]
        public async Task<StandardReturn> UpdateSonByIdAsync(string id, [FromBody] Son updatedSon)
        {
            _logger.LogInformation("调用 UpdateSonByIdAsync，Id：{id}，儿子：{son}。", id, updatedSon);
            if (!_enableFullCRUD)
            {
                _logger.LogInformation("未启用完整增删查改，无权操作。");
                return new StandardReturn(errorType: StandardReturn.ErrorType.PermissionDenied, localizer: _localizer);
            }
            var son = await _sonsService.GetSonByIdAsync(id);
            if (son is null)
            {
                _logger.LogInformation("数据不存在。");
                return new StandardReturn(errorType: StandardReturn.ErrorType.DataNotFound, localizer: _localizer);
            }
            if (!_sonsService.IsSonValid(updatedSon))
            {
                _logger.LogInformation("儿子 {son} 非法。", updatedSon);
                return new StandardReturn(errorType: StandardReturn.ErrorType.WrongData, localizer: _localizer);
            }
            var potentialSon = await _sonsService.GetSonByNameAndDaddyAsync(updatedSon.Name, updatedSon.Daddy);
            if (potentialSon is not null && potentialSon.Id != id) // 已有使用另一个 Id 的相同父子
            {
                _logger.LogInformation("儿子 {son} 重复。", updatedSon);
                return new StandardReturn(errorType: StandardReturn.ErrorType.RepeatedSubmissionNotAllowed, localizer: _localizer);
            }
            updatedSon.Id = son.Id;
            updatedSon.Expiration = DateTime.Today.AddDays(30);
            updatedSon.Reserved = son.Reserved;
            _logger.LogInformation("更新儿子：{son}。", updatedSon);
            await _sonsService.UpdateSonByIdAsync(id, updatedSon);
            return new StandardReturn(localizer: _localizer);
        }
        [HttpDelete("delete-by-id/{id:length(24)}")]
        public async Task<StandardReturn> DeleteSonByIdAsync(string id)
        {
            _logger.LogInformation("调用 DeleteSonByIdAsync，Id：{id}。", id);
            if (!_enableFullCRUD)
            {
                _logger.LogInformation("未启用完整增删查改，无权操作。");
                return new StandardReturn(errorType: StandardReturn.ErrorType.PermissionDenied, localizer: _localizer);
            }
            var son = await _sonsService.GetSonByIdAsync(id);
            if (son is null)
            {
                _logger.LogInformation("数据不存在。");
                return new StandardReturn(errorType: StandardReturn.ErrorType.DataNotFound, localizer: _localizer);
            }
            if (son.Reserved)
            {
                _logger.LogInformation("儿子 {son} 保留，无权操作。", son);
                return new StandardReturn(errorType: StandardReturn.ErrorType.PermissionDenied, localizer: _localizer);
            }
            _logger.LogInformation("删除儿子：{son}。", son);
            await _sonsService.DeleteSonByIdAsync(id);
            return new StandardReturn(localizer: _localizer);
        }
    }
}
