using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace blockchain.rate.service.Controllers
{
    [Route("rate/bitcoin")]
    public class BitcoinRateController : Controller
    {
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            using(var client = new HttpClient())
            {
                var result = await client.GetAsync($"https://api.bitfinex.com/v1/ticker/btcusd"); 
    
                var stream = await result.Content.ReadAsStringAsync(); 

                var rate = JObject.Parse(stream).Property("last_price").Value.ToString();
    
                return Ok(rate);
            }
        }
    }
}
