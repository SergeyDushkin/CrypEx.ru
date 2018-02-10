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

    amount.focus();

    var getUsd = $.get("rate/usd").then(function(data, status, xhr) { return data; });
    var getBitcoin = $.get("rate/bitcoin").then(function(data, status, xhr) { return data; });
    
    $.when(getUsd, getBitcoin).done(function(usd, bitcoin) {
        
        $("#rate").attr("data", usd * bitcoin);
        $("#rate").text((usd * bitcoin).toFixed(2));
    });

    $("#amount").on("keyup", function() {
        var amount = this.value.replace(new RegExp(",", 'g'), "");
        var rate = parseFloat($("#rate").attr("data"));
        var bitcoin = amount / rate;
        $("#bitcoin").html(bitcoin)

        console.log("amount: " + amount);
        console.log("rate: " + rate);
        console.log("bitcoin: " + bitcoin);
    });

    var inputNumeral = DOM.select('.input-numeral');

    var cleaveNumeral = new Cleave(inputNumeral, {
        numeral: true
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
