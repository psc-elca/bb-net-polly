using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace Service.Controllers
{
    [ApiController]
    [Route("/")]
    public class ServiceController : ControllerBase
    {
        private IThirdPartyService ThirdPartyService { get; }

        public ServiceController(IThirdPartyService thirdPartyService)
        {
            ThirdPartyService = thirdPartyService;
        }

        [HttpGet("success200")]
        public async Task<ActionResult<string>> Success200(CancellationToken cancellationToken = default)
        {
            return await ThirdPartyService.Get("success200", cancellationToken);
        }

        [HttpGet("timeout")]
        public async Task<ActionResult<string>> GetTimeout(CancellationToken cancellationToken = default)
        {
            return await ThirdPartyService.Get("timeout", cancellationToken);
        }

        [HttpGet("error500")]
        public async Task<ActionResult<string>> GetError500(CancellationToken cancellationToken = default)
        {
            return await ThirdPartyService.Get("error500", cancellationToken);
        }

        [HttpGet("error503")]
        public async Task<ActionResult<string>> GetError503(CancellationToken cancellationToken = default)
        {
            return await ThirdPartyService.Get("error503", cancellationToken);
        }

        [HttpGet("retries-then-200")]
        public async Task<ActionResult<string>> GetRetriesThen200(CancellationToken cancellationToken = default)
        {
            return await ThirdPartyService.Get("retries-then-200", cancellationToken);
        }
    }
}