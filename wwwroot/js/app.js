var DOM = {
    offset: function (el) {
        var rect = el.getBoundingClientRect();

        return {
            top:  rect.top + document.body.scrollTop,
            left: rect.left + document.body.scrollLeft
        };
    },

    select: function (selector) {
        return document.querySelector(selector);
    },

    html: function (ele, html) {
        if (!html) {
            return ele.innerHTML;
        }

        ele.innerHTML = html;
    },

    selectAll: function (selector) {
        return document.querySelectorAll(selector);
    },

    removeClass: function (el, className) {
        if (el.classList) {
            el.classList.remove(className);
        } else {
            el.className = el.className.replace(new RegExp('(^|\\b)' + className.split(' ').join('|') + '(\\b|$)', 'gi'), ' ');
        }
    },

    addClass: function (el, className) {
        if (el.classList) {
            if (className.indexOf(' ') >= 0) {
                var list = className.split(' ');
                _.each(list, function (element) {
                    el.classList.add(element);
                });
            } else {
                el.classList.add(className);
            }
        } else {
            el.className += ' ' + className;
        }
    }
};

$(document).ready(function() {

    var rate = {
        btcrur: 0, 
        ethrur: 0,
        type: 'btcrur',
        sellingCurrencyName: 'BTC',

        getCurrentRate: function() {
            
            if (this.sellingCurrencyName == 'BTC')
            {
                return this.btcrur;
            }

            if (this.sellingCurrencyName == 'ETH')
            {
                return this.ethrur;
            }

        }
    };

    var invoice = {

        bankName: 'alfabank',
        blockchain: 'BTC',
        type: 'RURBTC',

    };

    function refresh() {

        var currentRate = rate.getCurrentRate();

        $("#rate").attr("data", currentRate);
        $("#rate").text((currentRate).toFixed(2));

        var amount = $("#amount").val().replace(new RegExp(",", 'g'), "");
        var bitcoin = amount / currentRate;
        $("#bitcoin").html(Number(bitcoin).toLocaleString('en'));
        $("#sellingCurrencyName").html(rate.sellingCurrencyName);

        console.log("amount: " + amount);
        console.log("rate: " + currentRate);
        console.log("bitcoin: " + bitcoin);
    };

    amount.focus();

    var getRate = $.get("api/rate").then(function(data, status, xhr) { return data; });

    $.when(getRate).done(function(data) {
        
        rate.btcrur = data.btcrur;
        rate.ethrur = data.ethrur;
        
        refresh();
    });

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
