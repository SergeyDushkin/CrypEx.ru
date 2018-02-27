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

            var btcrur = (1f / (currencyRate * bitcoinRate)) * 1.02f;   // меняем рубли на биткоины
            var ethrur = (1f / (currencyRate * ethereumRate)) * 1.02f;  // меняем рубли на эфир
            var rurbtc = (currencyRate * bitcoinRate) * 0.98f;          // меняем биткоины на рубли
            var rureth = (currencyRate * ethereumRate) * 0.98f;         // меняем эфир на рубли

            return Ok(new { btcrur, ethrur, rurbtc, rureth });
        }
    }
}
