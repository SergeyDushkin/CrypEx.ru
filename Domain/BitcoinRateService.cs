using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace blockchain.rate.service
{
    public class BitcoinRateService: IRateService
    {
        public async Task<float> GetRateAsync()
        {
            using(var client = new HttpClient())
            {
                var result = await client.GetAsync($"https://api.bitfinex.com/v1/ticker/btcusd"); 
    
                var stream = await result.Content.ReadAsStringAsync(); 

                var rate = JObject.Parse(stream).Property("last_price").Value.ToString();
                
                rate = rate.Replace(",", CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator);
                return float.Parse(rate, NumberStyles.Any, CultureInfo.InvariantCulture);
            }
        }
    }
}
