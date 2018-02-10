using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace blockchain.rate.service.Controllers
{
    [Route("rate/bitcoin")]
    public class BitcoinRateController : Controller
    {
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var service = new BitcoinRateService();
            var rate = await service.GetRateAsync();

            return Ok(rate);
        }
    }
}
