var Item = function() {

    return {
        name: '',
        price: 0
    };
};

var InvoiceService = function() {
    return {
        data: [],
        onRefresh: document.createEvent('Event'),
        
        init: function() {

            this.onRefresh.initEvent('invoicesLoaded', true, true);

            var getRate = $.get("api/invoice").then(function(data, status, xhr) { return data; });
            
            $.when(getRate).done(function(data) {
                                
                this.data = data;
                document.dispatchEvent(this.onRefresh);
                
            }.bind(this));
        }
    }
}

var RateService = function() {

    return {
        btcrur: undefined, 
        ethrur: undefined,
        rurbtc: undefined, 
        rureth: undefined,
        item: 'btcrur',

        onRefresh: document.createEvent('Event'),

        init: function() {

            this.onRefresh.initEvent('refresh', true, true);

            var getRate = $.get("api/rate").then(function(data, status, xhr) { return data; });
            
            $.when(getRate).done(function(data) {
                                
                this.btcrur = { sellingCurrencyName: 'BTC', purchasingCurrencyName: 'RUB', blockchainName: 'BTC', rate: data.btcrur };
                this.ethrur = { sellingCurrencyName: 'ETH', purchasingCurrencyName: 'RUB', blockchainName: 'ETH', rate: data.ethrur };
                this.rurbtc = { sellingCurrencyName: 'RUB', purchasingCurrencyName: 'BTC', blockchainName: 'BTC', rate: data.rurbtc };
                this.rureth = { sellingCurrencyName: 'RUB', purchasingCurrencyName: 'ETH', blockchainName: 'ETH', rate: data.rureth };

                document.dispatchEvent(this.onRefresh);
                
            }.bind(this));
        },

        setItem: function(item) {
            this.item = item;
            document.dispatchEvent(this.onRefresh);
        },

        getCurrentRate: function() {
            return this[this.item];
        }
    };
}

var OrderService = function() {
    return {
        amount: 0
    };
};

$(document).ready(function() {

    var rate = new RateService();
    var order = new OrderService();
    
    rate.init();
    amount.focus();
    
    document.addEventListener('refresh', function(e) {
        refresh();
    });

    var cleaveNumeral = new Cleave(document.querySelector('.input-numeral'), {
        numeral: true
    });

    function refresh() {

        var currentRate = rate.getCurrentRate();

        $("#rate").attr("data", currentRate.rate);
        $("#rate").text((currentRate.rate).toFixed(2).toLocaleString('en'));

        var amount = $("#amount").val().replace(new RegExp(",", 'g'), "");
        var bitcoin =  amount * currentRate.rate;

        $("#bitcoin").html(Number(bitcoin).toLocaleString('en'));
        $("#sellingCurrencyName").html(currentRate.sellingCurrencyName);
        $("#blockchainName").html(currentRate.blockchainName);
        $("#purchasingCurrencyName").html(currentRate.purchasingCurrencyName);
    };


    $("#amount").on("keyup", function() {
        refresh();
    });

    $("#createInvoice").on("click", function() {

        var quantity = $("#amount").val().replace(new RegExp(",", 'g'), "");
        var email = $("#email").val();
        var bitcointWalletNumber = $("#bitcointWalletNumber").val();
        var bankName = $('input[name=bankName]:checked').val();

        var invoiceRequest = $.post("api/invoice", { Email: email, BitcointWalletNumber: bitcointWalletNumber, BankName: bankName, Item: rate.item, Quantity: quantity }).then(function(data, status, xhr) { return data; });
        
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

    $("#acceptButton").on("change", function (el, data) {
        allowSendButton();
        //var hasNotInvalidInputs = !$("input[required]").hasClass('invalid');
        //var isAccepted = el.target.checked;
        //$("#1createInvoice").prop('disabled', !(hasNotInvalidInputs && isAccepted));
    });

    $("input[required]").on("change", function (el, data) {
        allowSendButton();
        //var hasNotInvalidInputs = !$("input[required]").hasClass('invalid');
        //var isAccepted = el.target.checked;
        //$("#1createInvoice").prop('disabled', !(hasNotInvalidInputs && isAccepted));
    });

    function allowSendButton() {
        var hasNotInvalidInputs = !$("input[required]").hasClass('invalid');
        var isAccepted = $("#acceptButton").is(':checked');

        $("#createInvoice").prop('disabled', !(hasNotInvalidInputs && isAccepted));
    }

    $("#toggleArrow").on("click", function() {
        $("#toggleArrow").toggleClass('flip');

        var a = $("#toggleArrow").attr("data-select");
        var b = a == "data-item-1" ? "data-item-2" : "data-item-1";

        $("#toggleArrow").attr("data-select", b);
        
        selectItem();
    });

    $(".select-bank").on("click", function() {
        $(".select-bank img").addClass('inactive');
        $(".select-bank img").attr('data-select', false);
        $(".select-bank").css('color', '#fff');

        $(this).find('img').removeClass('inactive');
        $(this).find('img').attr('data-select', true);
        $(this).css('color', 'red');
    });

    $(".select-blockchain").on("click", function() {
        $(".select-blockchain").addClass('inactive');
        $(".select-blockchain").attr('data-select', false);

        $(this).removeClass('inactive');
        $(this).attr('data-select', true);
        
        selectItem();
    });

    function selectItem() {

        var a = $("#toggleArrow").attr("data-select");
        var b = $(".select-blockchain[data-select='true']").attr(a);

        rate.setItem(b);
    }
});
