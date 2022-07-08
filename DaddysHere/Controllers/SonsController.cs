using DaddysHere.Models;
using DaddysHere.Resources;
using DaddysHere.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

// https://docs.microsoft.com/zh-cn/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-6.0&tabs=visual-studio#add-a-controller
namespace DaddysHere.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SonsController : ControllerBase
    {
        private readonly SonsService _sonsService;
        private readonly IStringLocalizer<SharedResource> _localizer;
        public SonsController(SonsService sonsService, IStringLocalizer<SharedResource> localizer)
        {
            _sonsService = sonsService;
            _localizer = localizer;
        }
        [HttpGet("get-all")]
        public async Task<StandardReturn> GetSonsAsync()
        {
            var sons = await _sonsService.GetSonsAsync();
            if (sons.Count == 0)
            {
                return new StandardReturn(code: 20001, localizer: _localizer);
            }
            return new StandardReturn(result: sons, localizer: _localizer);
        }
        [HttpGet("get-by-name/{name}")]
        public async Task<StandardReturn> GetSonsByNameAsync(string name)
        {
            var sons = await _sonsService.GetSonsByNameAsync(name);
            if (sons.Count == 0)
            {
                return new StandardReturn(code: 20001, localizer: _localizer);
            }
            return new StandardReturn(result: sons, localizer: _localizer);
        }
        [HttpGet("get-by-daddy/{daddyName}")]
        public async Task<StandardReturn> GetSonsByDaddyAsync(string daddyName)
        {
            var sons = await _sonsService.GetSonsByDaddyAsync(daddyName);
            if (sons.Count == 0)
            {
                return new StandardReturn(code: 20001, localizer: _localizer);
            }
            return new StandardReturn(result: sons, localizer: _localizer);
        }
        [HttpGet("get-by-name-and-daddy/{daddyName}/{name}")]
        public async Task<StandardReturn> GetSonByNameAndDaddyAsync(string name, string daddyName)
        {
            var son = await _sonsService.GetSonByNameAndDaddyAsync(name, daddyName);
            if (son is null)
            {
                return new StandardReturn(code: 20001, localizer: _localizer);
            }
            return new StandardReturn(result: son, localizer: _localizer);
        }
        [HttpGet("get-by-id/{id:length(24)}")]
        public async Task<StandardReturn> GetSonByIdAsync(string id)
        {
            var son = await _sonsService.GetSonByIdAsync(id);
            if (son is null)
            {
                return new StandardReturn(code: 20001, localizer: _localizer);
            }
            return new StandardReturn(result: son, localizer: _localizer);
        }
        [HttpPost("create")]
        public async Task<StandardReturn> CreateSonAsync([FromBody] Son newSon)
        {
            var son = await _sonsService.GetSonByNameAndDaddyAsync(newSon.Name, newSon.Daddy);
            if (son is null)
            {
                newSon.Id = null;
                await _sonsService.CreateSonAsync(newSon);
                return new StandardReturn(localizer: _localizer);
            }
            return new StandardReturn(code: 20004, localizer: _localizer);
        }
        [HttpPut("update-by-id/{id:length(24)}")]
        public async Task<StandardReturn> UpdateSonByIdAsync(string id, [FromBody] Son updatedSon)
        {
            var son = await _sonsService.GetSonByIdAsync(id);
            if (son is null)
            {
                return new StandardReturn(code: 20001, localizer: _localizer);
            }
            updatedSon.Id = son.Id;
            await _sonsService.UpdateSonByIdAsync(id, updatedSon);
            return new StandardReturn(localizer: _localizer);
        }
        [HttpDelete("delete-by-id/{id:length(24)}")]
        public async Task<StandardReturn> DeleteSonByIdAsync(string id)
        {
            var son = await _sonsService.GetSonByIdAsync(id);
            if (son is null)
            {
                return new StandardReturn(code: 20001, localizer: _localizer);
            }
            await _sonsService.DeleteSonByIdAsync(id);
            return new StandardReturn(localizer: _localizer);
        }
    }
}
