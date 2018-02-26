var Item = function() {

    return {
        name: '',
        price: 0
    };
};

var RateService = function() {

    return {
        btcrur: 0, 
        ethrur: 0,
        rurbtc: 0, 
        rureth: 0,
        type: 'btcrur',
        blockchainName: 'BTC',
        sellingCurrencyName: 'BTC',
        purchasingCurrencyName: 'RUB',
        onRefresh: document.createEvent('Event'),

        init: function() {

            this.onRefresh.initEvent('refresh', true, true);

            var getRate = $.get("api/rate").then(function(data, status, xhr) { return data; });
            
            $.when(getRate).done(function(data) {
                                
                this.btcrur = data.btcrur;
                this.ethrur = data.ethrur;
                this.rurbtc = data.btcrur;
                this.rureth = data.ethrur;

                document.dispatchEvent(this.onRefresh);
                
            }.bind(this));
        },

        getCurrentRate: function() {
            
            if (this.sellingCurrencyName == 'BTC' && this.purchasingCurrencyName == 'RUB')
            {
                return this.btcrur;
            }

            if (this.sellingCurrencyName == 'ETH' && this.purchasingCurrencyName == 'RUB')
            {
                return this.ethrur;
            }

            if (this.sellingCurrencyName == 'RUB' && this.purchasingCurrencyName == 'BTC')
            {
                return this.rurbtc;
            }

            if (this.sellingCurrencyName == 'RUB' && this.purchasingCurrencyName == 'ETH')
            {
                return this.rureth;
            }

        },

        toggle: function() {
            var a = new String(this.sellingCurrencyName);
            var b = new String(this.purchasingCurrencyName);

            this.sellingCurrencyName = b;
            this.purchasingCurrencyName = a;
        }
    };
}

$(document).ready(function() {

    var rate = new RateService();
    rate.init();
    
    document.addEventListener('refresh', function(e) {
        refresh();
    });

    function refresh() {

        var currentRate = rate.getCurrentRate();

        $("#rate").attr("data", currentRate);
        $("#rate").text((currentRate).toFixed(2).toLocaleString('en'));

        var amount = $("#amount").val().replace(new RegExp(",", 'g'), "");
        var bitcoin = 0;
        
        if (rate.purchasingCurrencyName == 'RUB') {
            bitcoin = amount / currentRate;
        } else {
            bitcoin = amount * currentRate;
        }

        $("#bitcoin").html(Number(bitcoin).toLocaleString('en'));
        $("#sellingCurrencyName").html(rate.sellingCurrencyName);
        $("#blockchainName").html(rate.blockchainName);
        $("#purchasingCurrencyName").html(rate.purchasingCurrencyName);

        console.log("amount: " + amount);
        console.log("rate: " + currentRate);
        console.log("bitcoin: " + bitcoin);
    };

    amount.focus();

    $("#amount").on("keyup", function() {
        refresh();
    });

    $("#createInvoice").on("click", function() {

        var amount = $("#amount").val().replace(new RegExp(",", 'g'), "");
        var email = $("#email").val();
        var bitcointWalletNumber = $("#bitcointWalletNumber").val();
        var bankName = $('input[name=bankName]:checked').val();

        var invoiceRequest = $.post("api/invoice", { Email: email, BitcointWalletNumber: bitcointWalletNumber, BankName: bankName, Amount: amount }).then(function(data, status, xhr) { return data; });
        
        $.when(invoiceRequest).done(function(invoice) {

            console.log("amount: " + invoice);

            $("#invoiceNumber").html(invoice.number);
            $("#rubAmount").html(Number(invoice.rubAmount).toLocaleString('en'));
            $("#rubAmount2").html(Number(invoice.rubAmount).toLocaleString('en'));
            $("#bitcoinAmount").html(Number(invoice.bitcoinAmount).toLocaleString('en'));
            $("#invoiceRate").html(Number(invoice.rate).toLocaleString('en'));
            $("#bankName").html(invoice.payment.bankName);

            $("#approve").css("display","block");
            
        });
    });

    var inputNumeral = DOM.select('.input-numeral');

    var cleaveNumeral = new Cleave(inputNumeral, {
        numeral: true
    });

    $("#toggleArrow").on("click", function() {
        $("#toggleArrow").toggleClass('flip');
        rate.toggle();

        refresh();
    });

    $(".select-bank").on("click", function() {
        $(".select-bank").addClass('inactive');
        $(".select-bank").attr('data-select', false);

        $(this).removeClass('inactive');
        $(this).attr('data-select', true);

        invoice.bankName = $(this).attr('name');
    });

    $(".select-blockchain").on("click", function() {
        $(".select-blockchain").addClass('inactive');
        $(".select-blockchain").attr('data-select', false);

        $(this).removeClass('inactive');
        $(this).attr('data-select', true);

        rate.sellingCurrencyName = $(this).attr('name');
        rate.blockchainName = $(this).attr('name');

        refresh();
    });

    /*
    selectNumeral.addEventListener('change', function () {
        cleaveNumeral = new Cleave(inputNumeral, {
            numeral:                    true,
            numeralThousandsGroupStyle: this.value
        });

        DOM.html(selectNumeralCoverTitle, 'Style: ' + this.value);
        DOM.html(codeNumeral, DOM.html(codeNumeral).replace(/(<span class="code-grouping-style">(wan|thousand|lakh)<\/span>)|(thousand)/g, '<span class="code-grouping-style">' + this.value + '</span>'));
        inputNumeral.focus();
    });*/
});
