using System;
using System.Collections.Generic;
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
            var service = new InvoiceService(new BitcoinRateService(), new CurrencyRateService());
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
                "sandbox2de279b82e6f443bbcc3e6cea7ebca3b.mailgun.org", // Mailgun Domain
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
                .From("hi@cryptoX.space")
                .To(to)
                .Subject(subject)
                .Body(body);

            var response = await email.SendAsync();

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
