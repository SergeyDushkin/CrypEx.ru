using System;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using MailChimp.Net;
using MailChimp.Net.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace blockchain.rate.service.Controllers
{
    [Route("api/invoice")]
    public class InvoiceController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public InvoiceController(IHostingEnvironment hostingEnvironment) 
        {
            _hostingEnvironment = hostingEnvironment;
        }
        
        [HttpPost]
        public async Task<ActionResult> Post(CreateInvice create)
        {
            var service = new InvoiceService(new BitcoinRateService(), new CurrencyRateService());
            var invoice = await service.CreateAsync(create);

            var path = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "data", invoice.Number + ".xml");

            using (XmlTextWriter xml = new XmlTextWriter(path, new UTF8Encoding(false)))
            {
                XmlSerializer xs = new XmlSerializer(typeof(Invoice));
                xs.Serialize(xml, invoice);
            }

            //IMailChimpManager manager = new MailChimpManager("f235cd845b6d50846d5ff7c7adb001d7-us17");
            
            return Ok(invoice);
        }
    }

    public class CreateInvice 
    {
        public string Email { get; set; }
        public string BitcointWalletNumber { get; set; }
        public string BankName { get; set; }
        public float Amount { get; set; }
    }

    public class Invoice 
    {
        public string Number { get; set; }
        public string Email { get; set; }
        public string BitcointWalletNumber { get; set; }
        public PaymentInfo Payment { get; set; }
        public float Rate { get; set; }
        public float BitcoinRate { get; set; }
        public float UsdRate { get; set; }
        public float RubAmount { get; set; }
        public float BitcoinAmount { get; set; }
    }

    public class PaymentInfo
    {
        public string BankName { get; set; }
        public string AccontNumber { get; set; }
        public string Message { get; set; }
    }

    public class InvoiceService
    {
        private readonly BitcoinRateService bitcoinRateService;
        private readonly CurrencyRateService currencyRateService;

        public InvoiceService(BitcoinRateService bitcoinRateService, CurrencyRateService currencyRateService)
        {
            this.bitcoinRateService = bitcoinRateService;
            this.currencyRateService = currencyRateService;
        }

        public async Task<Invoice> CreateAsync(CreateInvice create)
        {
            var bitcoinRate = await bitcoinRateService.GetRateAsync();
            var currencyRate = await currencyRateService.GetRateAsync();
            var rate = (bitcoinRate * currencyRate) * 1.02f;
            var bitcoinAmount = create.Amount / rate;

            var invoice = new Invoice
            {
                Number = DateTime.Now.ToString("yyyyMMddHHmmss"),
                Email = create.Email,
                RubAmount = create.Amount,
                BitcoinAmount = bitcoinAmount,
                Rate = rate,
                BitcoinRate = bitcoinRate,
                UsdRate = currencyRate,
                BitcointWalletNumber = create.BitcointWalletNumber,
                Payment = new PaymentInfo {
                    BankName = create.BankName
                }
            };

            return invoice;
        }
    }
}
