using System;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace blockchain.rate.service.Controllers
{
    [Route("api/rate")]
    public class RateController : Controller
    {        
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var bitcoinRateService = new BitcoinRateService();
            var ethereumRateService = new EthereumRateService();
            var currencyRateService = new CurrencyRateService();

            var bitcoinRate = await bitcoinRateService.GetRateAsync();
            var ethereumRate = await ethereumRateService.GetRateAsync();
            var currencyRate = await currencyRateService.GetRateAsync();

            var btcrur = (bitcoinRate * currencyRate) * 1.02f;
            var ethrur = (ethereumRate * currencyRate) * 1.02f;

            return Ok(new { btcrur, ethrur });
        }
    }
}
