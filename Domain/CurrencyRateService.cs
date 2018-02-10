using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace blockchain.rate.service
{
    public class CurrencyRateService: IRateService
    {
        public async Task<float> GetRateAsync()
        {
            using (var client = new HttpClient())
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                var result = await client.GetAsync($"http://www.moex.com/export/derivatives/currency-rate.aspx?language=ru&currency=USD/RUB&moment_start={DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd")}&moment_end={DateTime.Today.ToString("yyyy-MM-dd")}"); 
    
                var stream = await result.Content.ReadAsStringAsync(); 

                var correctDocument = XDocument.Parse(stream);

                var rate = correctDocument.Descendants("rate").FirstOrDefault().Attribute("value").Value;

                rate = rate.Replace(",", CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator);
                return float.Parse(rate, NumberStyles.Any, CultureInfo.InvariantCulture);
            }
        }
    }
}
