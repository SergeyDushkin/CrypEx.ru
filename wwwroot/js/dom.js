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
