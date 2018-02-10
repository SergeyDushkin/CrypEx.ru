using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace blockchain.rate.service.Controllers
{
    [Route("rate/usd")]
    public class CurrencyRateController : Controller
    {
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var service = new CurrencyRateService();
            var rate = await service.GetRateAsync();

            return Ok(rate);
        }   
    }
    
}
