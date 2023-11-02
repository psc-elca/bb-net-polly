using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace Service.Controllers
{
    [ApiController]
    [Route("/")]
    public class ServiceWithTokenController : ControllerBase
    {
        private IThirdPartyServiceWithToken ThirdPartyServiceWithToken { get; }

        public ServiceWithTokenController(IThirdPartyServiceWithToken thirdPartyServiceWithToken)
        {
            ThirdPartyServiceWithToken = thirdPartyServiceWithToken;
        }

        [HttpGet("with-token")]
        public async Task<ActionResult<string>> GetWithToken(CancellationToken cancellationToken = default)
        {
            return await ThirdPartyServiceWithToken.Get("with-token", cancellationToken);
        }
    }
}