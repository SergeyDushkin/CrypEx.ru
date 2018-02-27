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

            var btcrur = (1f / (currencyRate * bitcoinRate)) * 1.02f;
            var ethrur = (1f / (currencyRate * ethereumRate)) * 1.02f;
            var rurbtc = (currencyRate * bitcoinRate) * 0.98f;
            var rureth = (currencyRate * ethereumRate) * 0.98f;

            return Ok(new { btcrur, ethrur, rurbtc, rureth });
        }
    }
}
