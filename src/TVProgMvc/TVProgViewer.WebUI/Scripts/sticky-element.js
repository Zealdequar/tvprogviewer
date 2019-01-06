(function () {
    var a = document.querySelector('#aside1'), b = null, P = 10;  // если ноль заменить на число, то блок будет прилипать до того, как верхний край окна браузера дойдёт до верхнего края элемента. Может быть отрицательным числом
    window.addEventListener('scroll', Ascroll1, false);
    document.body.addEventListener('scroll', Ascroll1, false);
    function Ascroll1() {
        if (b == null) {
            var Sa = getComputedStyle(a, ''), s = '';
            for (var i = 0; i < Sa.length; i++) {
                if (Sa[i].indexOf('overflow') == 0 || Sa[i].indexOf('padding') == 0 || Sa[i].indexOf('border') == 0 || Sa[i].indexOf('outline') == 0 || Sa[i].indexOf('box-shadow') == 0 || Sa[i].indexOf('background') == 0) {
                    s += Sa[i] + ': ' + Sa.getPropertyValue(Sa[i]) + '; '
                }
            }
            b = document.createElement('div');
            b.style.cssText = s + ' box-sizing: border-box; width: ' + a.offsetWidth + 'px;';
            a.insertBefore(b, a.firstChild);
            var l = a.childNodes.length;
            for (var i = 1; i < l; i++) {
                b.appendChild(a.childNodes[1]);
            }
            a.style.height = b.getBoundingClientRect().height + 'px';
            a.style.padding = '0';
            a.style.border = '0';
        }
        var Ra = a.getBoundingClientRect(),
            R = Math.round(Ra.top + b.getBoundingClientRect().height - document.querySelector('footer').getBoundingClientRect().top + 0);  // селектор блока, при достижении верхнего края которого нужно открепить прилипающий элемент;  Math.round() только для IE; если ноль заменить на число, то блок будет прилипать до того, как нижний край элемента дойдёт до футера
        if ((Ra.top - P) <= 0) {
            if ((Ra.top - P) <= R) {
                b.className = 'stop';
                b.style.top = - R + 'px';
            } else {
                b.className = 'sticky';
                b.style.top = P + 'px';
            }
        } else {
            b.className = '';
            b.style.top = '';
        }
        window.addEventListener('resize', function () {
            a.children[0].style.width = getComputedStyle(a, '').width
        }, false);
    }
   var a2 = document.querySelector('#aside2'), b2 = null, P2 = 10;  // если ноль заменить на число, то блок будет прилипать до того, как верхний край окна браузера дойдёт до верхнего края элемента. Может быть отрицательным числом
    window.addEventListener('scroll', Ascroll2, false);
    document.body.addEventListener('scroll', Ascroll2, false);
    function Ascroll2() {
        if (b2 == null) {
            var Sa2 = getComputedStyle(a2, ''), s2 = '';
            for (var i = 0; i < Sa2.length; i++) {
                if (Sa2[i].indexOf('overflow') == 0 || Sa2[i].indexOf('padding') == 0 || Sa2[i].indexOf('border') == 0 || Sa2[i].indexOf('outline') == 0 || Sa2[i].indexOf('box-shadow') == 0 || Sa2[i].indexOf('background') == 0) {
                    s2 += Sa2[i] + ': ' + Sa2.getPropertyValue(Sa2[i]) + '; '
                }
            }
            b2 = document.createElement('div');
            b2.style.cssText = s2 + ' box-sizing: border-box; width: ' + a2.offsetWidth + 'px;';
            a2.insertBefore(b2, a2.firstChild);
            var l = a2.childNodes.length;
            for (var i = 1; i < l; i++) {
                b2.appendChild(a2.childNodes[1]);
            }
            a2.style.height = b2.getBoundingClientRect().height + 'px';
            a2.style.padding = '0';
            a2.style.border = '0';
        }
        var Ra2 = a2.getBoundingClientRect(),
            R2 = Math.round(Ra2.top + b2.getBoundingClientRect().height - document.querySelector('footer').getBoundingClientRect().top + 0);  // селектор блока, при достижении верхнего края которого нужно открепить прилипающий элемент;  Math.round() только для IE; если ноль заменить на число, то блок будет прилипать до того, как нижний край элемента дойдёт до футера
        if ((Ra2.top - P2) <= 0) {
            if ((Ra2.top - P2) <= R2) {
                b2.className = 'stop2';
                b2.style.top = - R2 + 'px';
            } else {
                b2.className = 'sticky2';
                b2.style.top = P2 + 'px';
            }
        } else {
            b2.className = '';
            b2.style.top = '';
        }
        window.addEventListener('resize', function () {
            a2.children[0].style.width = getComputedStyle(a2, '').width
        }, false);
    }
})()