using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentEmail.Mailgun;
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
            var service = new InvoiceService(new BitcoinRateService(), new EthereumRateService(), new CurrencyRateService());
            var invoice = await service.CreateAsync(create);

            var path = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "data", invoice.Number + ".xml");

            using (XmlTextWriter xml = new XmlTextWriter(path, new UTF8Encoding(false)))
            {
                XmlSerializer xs = new XmlSerializer(typeof(Invoice));
                xs.Serialize(xml, invoice);
            }

            var body = @"
Здравствуйте Сергей

<a href='https://cryptoX.space/OLjrzcMbn88FXulGfhrOlucTIvXB76GbU38'>Заявка #%OrderNum% от %OrderDateTime% - %Status%</a>
Заявка #123 от 26.02.18 14:30 - Ожидает перевода

Покупка %ToAmount% %ToCur%
Покупка 0.085 BTC

1 %FromCur% = %Rate% %ToCur%
1 RUR = 0.00008893 BTC

Адрес отправителя: %Phone vs CryptoAddress%
Адрес отправителя: +7 925 878 8681

Адрес получателя: %Phone vs CryptoAddress%
Адрес получателя: 17XSs9yq5M9cRDU4XGtz45UTEcqMEdm6RF



Crypto eXchange Space
hi@cryptoX.space";

            var sender = new MailgunSender(
                "cryptox.space", // Mailgun Domain
                "key-f942a01bbc9473142f4c8dace3c34f78" // Mailgun API Key
            );

            var to = new List<Address>() 
            { 
                new Address("leirbythe@gmail.com"), 
                new Address("rodionov.sergey.v@gmail.com") 
            };

            var subject = "cryptoX.space: Заявка #124 - Ожидает перевода";

            Email.DefaultSender = sender;

            var email = Email
                .From("leirbythe@gmail.com") //hi@cryptoX.space
                .To(to)
                .Subject(subject)
                .Body(body);

            var response = await email.SendAsync();

            return Ok(invoice);
        }

        
        [HttpGet]
        public ActionResult Get()
        {
            var path = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "data");

            var files = System.IO.Directory.GetFiles(path, "*.xml");
            var invoices = files.Select(f => {
                using (var stream = new System.IO.StreamReader(f))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(Invoice));
                    return (Invoice)xs.Deserialize(stream);
                }
            }).ToList();
            
            return Ok(invoices);
        }

        
        [HttpGet("{id}")]
        public ActionResult Get(string id)
        {
            var path = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "data", id + ".xml");

            using (var stream = new System.IO.StreamReader(path))
            {
                XmlSerializer xs = new XmlSerializer(typeof(Invoice));
                return Ok((Invoice)xs.Deserialize(stream));
            }
        }
    }

    public class CreateInvice 
    {
        public string Email { get; set; }
        public string BitcointWalletNumber { get; set; }
        public string BankName { get; set; }
        public string Item { get; set; }
        public float Quantity { get; set; }
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
        private readonly EthereumRateService ethereumRateService;
        private readonly CurrencyRateService currencyRateService;

        public InvoiceService(BitcoinRateService bitcoinRateService, EthereumRateService ethereumRateService, CurrencyRateService currencyRateService)
        {
            this.bitcoinRateService = bitcoinRateService;
            this.ethereumRateService = ethereumRateService;
            this.currencyRateService = currencyRateService;
        }

        public async Task<Invoice> CreateAsync(CreateInvice create)
        {
            var bitcoinRate = await bitcoinRateService.GetRateAsync();
            var ethereumRate = await ethereumRateService.GetRateAsync();
            var currencyRate = await currencyRateService.GetRateAsync();

            var btcrur = (1f / (currencyRate * bitcoinRate)) * 1.02f;
            var ethrur = (1f / (currencyRate * ethereumRate)) * 1.02f;
            var rurbtc = (currencyRate * bitcoinRate) * 0.98f;
            var rureth = (currencyRate * ethereumRate) * 0.98f;

            var rate = 0f;

            switch(create.Item) {
                case "btcrur": 
                    rate = btcrur; 
                    break;
                case "ethrur": 
                    rate = ethrur; 
                    break;
                case "rurbtc": 
                    rate = rurbtc; 
                    break;
                case "rureth": 
                    rate = rureth; 
                    break;
            }

            var bitcoinAmount = create.Quantity * rate;

            var invoice = new Invoice
            {
                Number = DateTime.Now.ToString("yyyyMMddHHmmss"),
                Email = create.Email,
                RubAmount = create.Quantity,
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
