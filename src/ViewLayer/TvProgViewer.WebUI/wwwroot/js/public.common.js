﻿/*
** TvProgViewer custom js functions
*/

function OpenWindow(query, w, h, scroll) {
    var l = (screen.width - w) / 2;
    var t = (screen.height - h) / 2;

    winprops = 'resizable=0, height=' + h + ',width=' + w + ',top=' + t + ',left=' + l + 'w';
    if (scroll) winprops += ',scrollbars=1';
    var f = window.open(query, "_blank", winprops);
}

function setLocation(url) {
    window.location.href = url;
}

function setLocationWithPart(urlWithPart) {
    window.location.href = urlWithPart.split(';')[0];
}

function displayAjaxLoading(display) {
    if (display) {
        $('.ajax-loading-block-window').show();
    }
    else {
        $('.ajax-loading-block-window').hide('slow');
    }
}

function displayPopupNotification(message, messagetype, modal) {
    //types: success, error, warning
    var container;
    if (messagetype == 'success') {
        //success
        container = $('#dialog-notifications-success');
    }
    else if (messagetype == 'error') {
        //error
        container = $('#dialog-notifications-error');
    }
    else if (messagetype == 'warning') {
        //warning
        container = $('#dialog-notifications-warning');
    }
    else {
        //other
        container = $('#dialog-notifications-success');
    }

    //we do not encode displayed message
    var htmlcode = '';
    if ((typeof message) == 'string') {
        htmlcode = '<p>' + message + '</p>';
    } else {
        for (var i = 0; i < message.length; i++) {
            htmlcode = htmlcode + '<p>' + message[i] + '</p>';
        }
    }

    container.html(htmlcode);

    var isModal = (modal ? true : false);
    container.dialog({
        modal: isModal,
        width: 350
    });
}
function displayJoinedPopupNotifications(notes) {
    if (Object.keys(notes).length === 0) return;

    var container = $('#dialog-notifications-success');
    var htmlcode = document.createElement('div');

    for (var note in notes) {
        if (notes.hasOwnProperty(note)) {
            var messages = notes[note];

            for (var i = 0; i < messages.length; ++i) {
                var elem = document.createElement("div");
                elem.innerHTML = messages[i];
                elem.classList.add('popup-notification');
                elem.classList.add(note);

                htmlcode.append(elem);
            }
        }
    }

    container.html(htmlcode);
    container.dialog({
        width: 350,
        modal: true
    });
}
function displayPopupContentFromUrl(url, title, modal, width) {
    var isModal = (modal ? true : false);
    var targetWidth = (width ? width : 550);
    var maxHeight = $(window).height() - 20;

    $('<div></div>').load(url)
        .dialog({
            modal: isModal,
            width: targetWidth,
            maxHeight: maxHeight,
            title: title,
            close: function(event, ui) {
                $(this).dialog('destroy').remove();
            }
        });
}

function displayBarNotification(message, messagetype, timeout) {
    var notificationTimeout;

    var messages = typeof message === 'string' ? [message] : message;
    if (messages.length === 0)
        return;

    //types: success, error, warning
    var cssclass = ['success', 'error', 'warning'].indexOf(messagetype) !== -1 ? messagetype : 'success';

    //remove previous CSS classes and notifications
    $('#bar-notification')
      .removeClass('success')
      .removeClass('error')
      .removeClass('warning');
    $('.bar-notification').remove();

    //add new notifications
    var htmlcode = document.createElement('div');

    //IE11 Does not support miltiple parameters for the add() & remove() methods
    htmlcode.classList.add('bar-notification', cssclass);
    htmlcode.classList.add(cssclass);

    //add close button for notification
    var close = document.createElement('span');
    close.classList.add('close');
    close.setAttribute('title', document.getElementById('bar-notification').dataset.close);

    for (var i = 0; i < messages.length; i++) {
        var content = document.createElement('p');
        content.classList.add('content');
        content.innerHTML = messages[i];

      htmlcode.appendChild(content);
    }
    
    htmlcode.appendChild(close);

    $('#bar-notification')
      .append(htmlcode);

    $(htmlcode)
        .fadeIn('slow')
        .on('mouseenter', function() {
            clearTimeout(notificationTimeout);
        });

    //callback for notification removing
    var removeNoteItem = function () {
        $(htmlcode).remove();
    };

    $(close).on('click', function () {
        $(htmlcode).fadeOut('slow', removeNoteItem);
    });

    //timeout (if set)
    if (timeout > 0) {
        notificationTimeout = setTimeout(function () {
            $(htmlcode).fadeOut('slow', removeNoteItem);
        }, timeout);
    }
}

function htmlEncode(value) {
    return $('<div/>').text(value).html();
}

function htmlDecode(value) {
    return $('<div/>').html(value).text();
}


// CSRF (XSRF) security
function addAntiForgeryToken(data) {
    //if the object is undefined, create a new one.
    if (!data) {
        data = {};
    }
    //add token
    var tokenInput = $('input[name=__RequestVerificationToken]');
    if (tokenInput.length) {
        data.__RequestVerificationToken = tokenInput.val();
    }
    return data;
};

// Получение списка настроенных пользователем каналов
function getStorageChannels() {
    let storageChannels = window.localStorage.getItem("optChans");
    let result;
    if (storageChannels && storageChannels.length > 0) {
        result = JSON.parse(storageChannels);
    }
    return result;
}

function GetGenres(btnActive) {
    var ids_genres = $(btnActive).map(function () {
        return this.id;
    }).get();

    return ids_genres.join(";");
}


//Получение тега пиктограммы
function getImgTag(s, str) {
    return "<img src='" + s + "' alt='" + str + "' height='25px' width='25px' />";
}

// Пиктограмма для рейтнигов
function imgRating(cellvalue, options, rowObject) {
    if (cellvalue)
        return getImgTag(cellvalue, rowObject.RatingName);
    return " ";
}

// Пиктограмма для жанров
function imgGenre(cellvalue, options, rowObject) {
    if (cellvalue)
        return getImgTag(cellvalue, rowObject.GenreName);
    return " ";
}

// Пиктограмма для анонсов
function imgAnons(cellvalue, options, rowObject) {
    if (cellvalue)
        return "<img src='" + cellvalue + "' alt='Анонс' height='21px' width='17px' />";
    return " ";
}

// Пиктограмма для каналов
function imgChannel(cellvalue, options, rowObject) {
    if (cellvalue)
        return getImgTag(cellvalue, "Эмблема телеканала " + rowObject.ChannelName);
    return "<img src='/images/i/satellite_25.png' alt='Эмблема телеканала' height='25px' width='25px' />";
}

// Установка класса для прошедшей, текущей и будущей передачи:
function getRowClassByDates(startDateStr, stopDateStr) {
    const dateFrom = new Date(Date.parse(startDateStr));
    const dateTo = new Date(Date.parse(stopDateStr));
    dayjs.extend(dayjs_plugin_utc);
    dayjs.extend(dayjs_plugin_timezone);   

    // Преобразование времени из UTC в MSK:
    const mskNow = dayjs().utc(new Date()).tz('Europe/Moscow');

    // Логика определения класса:
    if (dateTo < mskNow) {
        return { "class": "grayColor" };
    }
    if (dateFrom <= mskNow && mskNow < dateTo) {
        return { "class": "blackColor" };
    }
    if (dateFrom > mskNow) {
        return { "class": "greenColor" };
    }
    return {}; // На случай, если ни одно условие не сработало
}

// Установка класса для прошедшей и будущей передачи:
function getRowClassBySearches(stopDateStr) {
    var stopDate = new Date(Date.parse(stopDateStr));
    dayjs.extend(dayjs_plugin_utc);
    dayjs.extend(dayjs_plugin_timezone);

    // Преобразование времени из UTC в MSK:
    const mskNow = dayjs().utc(new Date()).tz('Europe/Moscow');

    // Логика определения класса:
    if (mskNow > stopDate)
        return { "class": "grayColor" }
    else
        return { "class": "blackColor" };
}
