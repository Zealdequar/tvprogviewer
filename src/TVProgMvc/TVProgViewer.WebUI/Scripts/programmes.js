var idx;
var pairKeys;

$(function () {
    fillSelects();
    $('#tabs').tabs({
        activate: function (event, ui) {
            idx = ui.newTab.index()

            if (idx === 2 || idx === 3) {
                setTree(idx);
                $('.sticky').removeClass('sticky');
                $('.sticky2').removeClass('sticky2');
            }
        }
    });
    setTree(getSelectedTabIndex());
    fillFooter();
    $('#TVProgProvider').change(function () {
        fillSelects();
        $('#TVProgrammeNowGrid').jqGrid('GridUnload');
        $('#TVProgrammeNextGrid').jqGrid('GridUnload');
        setGrids();
        setTree(getSelectedTabIndex());
        fillFooter();
    });
    $('#TVProgType').change(function () {
        $('#TVProgrammeNowGrid').jqGrid('GridUnload');
        $('#TVProgrammeNextGrid').jqGrid('GridUnload');
        setGrids();
        setTree(getSelectedTabIndex());
        fillFooter();
    });
    $('#TVProgCategories').change(function () {
        $('#TVProgrammeNowGrid').jqGrid('GridUnload');
        $('#TVProgrammeNextGrid').jqGrid('GridUnload');
        setGrids();
        if (idx === 2) {
            fillUserByDay(pairKeys[0], pairKeys[1]);
        } else if (idx === 3) {
            fillUserByChannels(pairKeys[0], pairKeys[1]);
        }
    });
    setGrids();
    $("#anonsTool").click(function () {
        $('#anonsDescr').toggle(100);
    });
    $("#anonsToolNext").click(function () {
        $('#anonsDescrNext').toggle(100);
    });

    $("#anonsToolByDays").click(function () {
        $('#anonsDescrByDays').toggle(100);
    });
    $("#anonsToolByChannels").click(function () {
        $('#anonsDescrByChannels').toggle(100);
    });
$("#btnSearch").click(function () {
        searchProgramme($('#TVProgType option:selected').val(), $('#tbContains').val());
    });

$("#containerByDays")
    .on('open_node.jstree', function (evt, data) {
        if (data.node.parents.length === 3)
            data.instance.set_icon(data.node, (data.node.icon.indexOf('_exp') === -1) ? [data.node.icon.slice(0, 11), '_exp', data.node.icon.slice(11)].join('') : data.node.icon);
    })
    .on('close_node.jstree', function (evt, data) {
        if (data.node.parents.length === 3)
            data.instance.set_icon(data.node, data.node.icon.replace('_exp', ''));
    });
$('#containerByDays').on('select_node.jstree', function (e, data) {
    if (data.node.parents.length === 4) {
        $("#pic").hide(100);
        $("#pic").attr("src", data.node.icon.replace("/small/", "/large/"));
        $("#pic").show(100);
        $("#channelText").text(data.node.text);
        var selectedNodeId = data.node.id;
        pairKeys = selectedNodeId.split('_');
        $("#dtFull").hide();
        var dtFull = new Date(pairKeys[0].replace(/(\d+).(\d+).(\d+)/, '$2/$1/$3'));
        var days = ['Вс', 'Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб'];
        var month = ['января', 'февраля', 'марта', 'апреля', 'мая', 'июня', 'июля', 'августа', 'сентября', 'октября', 'ноября', 'декабря'];
        var d = { day: "numeric" };
        var y = { year: "numeric" };
        $("#dtFull").text(dtFull.toLocaleDateString('ru-Ru', d) + ' ' + month[dtFull.getMonth()] + ' '
                          + dtFull.toLocaleDateString('ru-Ru', y) + ' (' + days[dtFull.getDay()] + ')');
        $("#dtFull").show();
        fillUserByDay(pairKeys[0], pairKeys[1]);
    }
    });
    $('#containerByChannels').on('select_node.jstree', function (e, data) {
        if (data.node.parents.length === 4) {
            var selectedNodeId = data.node.id;
            pairKeys = selectedNodeId.split('_');
            fillUserByChannels(pairKeys[0], pairKeys[1]);
        }
    });
    $("#choicePnl").show();
    $("#tabs").show();
});

// Заполнение раскрывающихся списков
function fillSelects() {
    $.ajax({
        url: "/Programme/GetTvProviderList",
        dataType: 'json',
        async: false,
        type: 'Get',
        contentType: 'application/json; charset=utf-8',
        success: function (response) {
            $('#TVProgType').empty();
            for (var i = 0; i < response.length; i++) {
                if (!$('#TVProgProvider option').filter(function () { return $(this).html() === response[i].ProviderName; }).val())
                    $('#TVProgProvider').append($('<option>', { value: response[i].TVProgProviderID })
                        .text(response[i].ProviderName));
            }

            for (var i = 0; i < response.length; i++) {
                if (!$('#TVProgType option').filter(function () { return $(this).html() === response[i].TypeProgID }).val() &&
                    $('#TVProgProvider option:selected').filter(function () { return $(this).html() === response[i].ProviderName; }).val())
                    $('#TVProgType').append($('<option>', { value: response[i].TypeProgID })
                        .text(response[i].TypeName));
            }
        },
        error: function (jqXHR, exception) {
        var msg = '';
        if (jqXHR.status === 0) {
            msg = 'Not connect.\n Verify Network.';
        } else if (jqXHR.status == 404) {
            msg = 'Requested page not found. [404]';
        } else if (jqXHR.status == 500) {
            msg = 'Internal Server Error [500].';
        } else if (exception === 'parsererror') {
            msg = 'Requested JSON parse failed.';
        } else if (exception === 'timeout') {
            msg = 'Time out error.';
        } else if (exception === 'abort') {
            msg = 'Ajax request aborted.';
        } else {
            msg = 'Uncaught Error.\n' + jqXHR.responseText;
        }
      }
    });
    $.ajax({
        url: "/Programme/GetCategories",
        dataType: 'json',
        async: false,
        type: 'Get',
        contentType: 'application/json; charset=utf-8',
        success: function (response) {
            $('#TVProgCategories').empty();
            $('#TVProgCategories').append($('<option>', { value: 'null' }).text('Все категории'));
            for (var i = 1; i < response.length; i++) {
                $('#TVProgCategories').append($('<option>', { value: response[i]})
                    .text(response[i]));
            }
        }
    });
}

// Заполенение подвала
function fillFooter() {
    if (!$('#TVProgType option:selected'))
        return;

    $.ajax({
        url: "/Programme/GetSystemProgrammePeriod?progType=" + $('#TVProgType option:selected').val(),
        dataType: 'json',
        type: 'Get',
        contentType: 'application/json; charset=utf-8',
        success: function (response) {
            var startDate = new Date(parseInt(response.dtStart.substr(6)));
            var endDate = new Date(parseInt(response.dtEnd.substr(6)));
            $("#period").text(formatDateString(startDate) + "—" + formatDateString(endDate));
        },
    }); 
}

// Преобразование к формату даты (Ru-ru)
function formatDateString(date) {
    var day = ("0" + date.getDate()).slice(-2);
    var month = ("0" + (date.getMonth() + 1)).slice(-2);
    var year = date.getFullYear();
    return day + "." + month + "." + year;
}

//Получение тега пиктограммы
function getImgTag(s, str) {
    return "<img src='" + s + "' alt='" + str + "' />";
}

// Пиктограмма для рейтнигов
function imgRating(s) {
    if (s)
        return getImgTag(s, "Рейтинг");
    return " ";
}

// Пиктограмма для жанров
function imgGenre(s) {
    if (s)
        return getImgTag(s, "Жанр");
    return " ";
}

// Пиктограмма для анонсов
function imgAnons(s) {
    if (s)
        return getImgTag(s, "Анонс");
    return " ";
}

// Пиктограмма для каналов
function imgChannel(s) {
    if (s)
        return getImgTag(s, "Эмблема канала");
    return "<img src='/imgs/i/satellite_25.png' alt='Эмблема канала' />";
}

// Установка табличек
function setGrids() {
    // Табличка сейчас в эфире
    $('#TVProgrammeNowGrid').jqGrid(
        {
            url: "/Programme/GetSystemProgrammeAtNow?progType=" + $('#TVProgType option:selected').val() + "&category=" + $('#TVProgCategories option:selected').val(),
            datatype: 'json',
            mtype: 'Get',
            success: function () { },
            colNames: ["Рейтинг", "Название рейтинга", "Жанр", "Название жранра", "Анонс", "Эмблема канала", "Название канала", "Передача", "Начало", "Окончание", "Осталось, %", ""],
            colModel: [
                {
                    key: false, name: 'RatingContent', index: 'RatingContent', sortable: true, width: "2%", align: "center", formatter: imgRating,
                    cellattr: function (rowId, val, rawObject, cm, rdata) {
                        return 'title="' + rawObject.RatingName + '"';
                    }
                },
                {
                    key: false, name: 'RatingName', index: 'RatingName', sortable: true, widht: "0", align: "center"
                },
                {
                    key: false, name: 'GenreContent', index: 'GenreContent', sortable: true, width: "2%", align: "center", formatter: imgGenre,
                    cellattr: function (rowId, val, rawObject, cm, rdata) {
                        return 'title="' + rawObject.GenreName + '"';
                    } 
                },
                {
                    key: false, name: 'GenreName', index: 'GenreName', sortable: true, width: "0", align: "center"
                },
                {
                    key: false, name: 'AnonsContent', index: 'AnonsContent', sortable: true, width: "2%", align: "center", formatter: imgAnons
                },
                {
                    key: false, name: 'ChannelContent', index: 'ChannelContent', sortable: true, width: "3%", align: "center", formatter: imgChannel
                },
                { key: false, name: 'ChannelName', index: 'ChannelName', sortable: true, width: "5%" },
                { key: false, name: 'TelecastTitle', index: 'TelecastTitle', sortable: true, width: "20%" },
                {
                    key: false, name: 'Start', index: 'Start', sortable: true, align: "center", width: "3%", sorttype: 'datetime', formatter: "date", formatoptions: { srcformat: 'ISO8601Long', newformat: 'H:i' }
                },
                {
                    key: false, name: 'Stop', index: 'Stop', sortable: true, align: "center", width: "3%", sorttype: 'datetime', formatter: "date", formatoptions: { srcformat: 'ISO8601Long', newformat: 'H:i' }
                },
                {
                    key: false, name: 'Remain', index: 'Remain', sortable: true, width: "5%", formatter: function (cellVal) {
                        return "<div style='position: relative'><div style='text-align: center; position: absolute; top: 1px; left: 42%; right: 42%; z-index: 1000'>" + cellVal + "%</div><div class='progress-bar progress-bar-striped' role='progressbar' aria-valuemin='100' aria-valuemax='1' style='width: " + cellVal + "%; position: relative; height: 20px;'></div></div>";
                    }
                },
                {
                    key: false, name: 'TelecastDescr', index: 'TelecastDescr', hidden: true
                }
            ],
            rowNum: 20,
            beforeSelectRow: function (rowid, e) {
                $('#TVProgrammeNowGrid').jqGrid('resetSelection');
                return (true);
            },
            loadComplete: function () {
                $("tr.jqgrow:odd").css("background", "#EFFFEF");
                
                $("tr.jqgrow td input", "#TVProgrammeNowGrid").click(function () {
                    if ($(this).closest('tr').find('td:nth-child(4)').find('img').length) {
                        $("#anonsTool").show(50);
                        $("#anonsDescr").html($(this).closest('tr').find('td:nth-child(13)').attr('title'));
                    }
                    else {
                        $("#anonsTool").hide(50);
                        $("#anonsDescr").hide(150);
                    }
                });
            },
            rowList: [10, 20, 30, 40, 50, 100, 500, 1000],
            height: 'auto',
            width: null,
            autowidth: true,
            shrinkToFit: true,
            autoResizing: { minColWidth: 80 },
            viewrecords: true,
            caption: 'Программа передач',
            emptyrecords: 'Программа передач не обнаружена',
            pager: '#TVProgrammePager',
            loadonce: false,
            forceClientSorting: true,
            multiselect: true,
        }).navGrid('#TVProgrammePager',
        {
            edit: false, add: false, del: false, search: true,
            searchtext: "Поиск передачи", refresh: true
        },
        {
            zIndex: 100,
            caption: "Поиск передачи",
            sopt: ['cn']
        });
    jQuery("#TVProgrammeNowGrid").jqGrid('hideCol', "GenreName");
    jQuery("#TVProgrammeNowGrid").jqGrid('hideCol', "RatingName");
    if (uv == 0) {
       jQuery("#TVProgrammeNowGrid").jqGrid('hideCol', "RatingContent");
    }

    // Табличка затем в эфире
    $('#TVProgrammeNextGrid').jqGrid(
        {
            url: "/Programme/GetSystemProgrammeAtNext?progType=" + $('#TVProgType option:selected').val() + "&category=" + $('#TVProgCategories option:selected').val(),
            datatype: 'json',
            mtype: 'Get',
            colNames: ["Рейтинг", "Название рейтинга", "Жанр", "Название жанра", "Анонс", "Эмблема канала", "Название канала", "Передача", "Начало", "Окончание", "Осталось", ""],
            colModel: [
                {
                    key: false, name: 'RatingContent', index: 'RatingContent', sortable: true, width: 40, align: "center", formatter: imgRating,
                    cellattr: function (rowId, val, rawObject, cm, rdata) {
                        return 'title="' + rawObject.RatingName + '"';
                    }
                },
                {
                    key: false, name: 'RatingName', index: 'RatingName', sortable: true, widht: "0", align: "center"
                },
                {
                    key: false, name: 'GenreContent', index: 'GenreContent', sortable: true, width: 40, align: "center", formatter: imgGenre,
                    cellattr: function (rowId, val, rawObject, cm, rdata) {
                        return 'title="' + rawObject.GenreName + '"';
                    }
                },
                {
                    key: false, name: 'GenreName', index: 'GenreName', sortable: true, width: "0", align: "center"
                },
                {
                    key: false, name: 'AnonsContent', index: 'AnonsContent', sortable: true, width: 40, align: "center", formatter: imgAnons
                },
                {
                    key: false, name: 'ChannelContent', index: 'ChannelContent', sortable: true, width: 80, align: "center", formatter: imgChannel
                },
                { key: false, name: 'ChannelName', index: 'ChannelName', sortable: true },
                { key: false, name: 'TelecastTitle', index: 'TelecastTitle', width: 520, sortable: true },
                {
                    key: false, name: 'Start', index: 'Start', sortable: true, align: "center", width: 80, sorttype: 'datetime', formatter: "date", formatoptions: { srcformat: 'ISO8601Long', newformat: 'H:i' }
                },
                {
                    key: false, name: 'Stop', index: 'Stop', sortable: true, align: "center", width: 80, sorttype: 'datetime', formatter: "date", formatoptions: { srcformat: 'ISO8601Long', newformat: 'H:i' }
                },
                {
                    key: false, name: 'Remain', index: 'Remain', sortable: true, align: "center", width: 80, sorttype: 'datetime', formatter: function (cellval, opts, rowObject, action) {
                        return $.fn.fmatter.call(
                            this,
                            "date",
                            new Date(cellval * 1000 - 3*60*60*1000),
                            $.extend({}, $.jgrid.formatter.date, opts),
                            rowObject,
                            action);
                    }
                    , formatoptions: { srcformat: 'ISO8601Long', newformat: 'H:i' }
                },
                {
                    key: false, name: 'TelecastDescr', index: 'TelecastDescr', hidden: true
                }
            ],
            rowNum: 20,
            beforeSelectRow: function (rowid, e) {
                $('#TVProgrammeNextGrid').jqGrid('resetSelection');
                return (true);
            },
            loadComplete: function () {
                $("tr.jqgrow:odd").css("background", "#EFFFEF");

                $("tr.jqgrow td input", "#TVProgrammeNextGrid").click(function () {
                    if ($(this).closest('tr').find('td:nth-child(4)').find('img').length) {
                        $("#anonsToolNext").show(50);
                        $("#anonsDescrNext").html($(this).closest('tr').find('td:nth-child(13)').attr('title'));
                    }
                    else {
                        $("#anonsToolNext").hide(50);
                        $("#anonsDescrNext").hide(150);
                    }
                });
            },
            rowList: [10, 20, 30, 40, 50, 100, 500, 1000],
            height: 'auto',
            width: null,
            autowidth: true,
            shrinkToFit: true,
            autoResizing: { minColWidth: 80 },
            viewrecords: true,
            caption: 'Программа передач',
            emptyrecords: 'Программа передач не обнаружена',
            /*jsonReader:
            {
                repeatitems: false,
                root: function (obj) { return obj; },
                page: function (obj) { return 1; },
                total: function (obj) { return obj.rowNum / obj.height; },
                records: function (obj) { return obj.length; }
            },*/
            pager: '#TVProgrammeNextPager',
            loadonce: false,
            multiselect: true,
        }).navGrid('#TVProgrammeNextPager',
        {
            edit: false, add: false, del: false, search: true,
            searchtext: "Поиск передачи", refresh: true
        },
        {
            zIndex: 100,
            caption: "Поиск передачи",
            sopt: ['cn']
        });
    jQuery("#TVProgrammeNextGrid").jqGrid('hideCol', "GenreName");
    jQuery("#TVProgrammeNextGrid").jqGrid('hideCol', "RatingName");
    if (uv == 0) {
        jQuery("#TVProgrammeNextGrid").jqGrid('hideCol', "RatingContent");
    }
}

// Поиск по всей программе передач
function searchProgramme(typeProgID, findTitle) {
    $('#SearchedTVProgramme').jqGrid('GridUnload');
    $('#SearchedTVProgramme').jqGrid(
        {
            url: "/Programme/SearchProgramme?progType=" + typeProgID + "&findTitle=" + findTitle,
            datatype: 'json',
            mtype: 'Get',
            colNames: ["Рейтинг", "Название рейтинга", "Жанр", "Название жанра", "Анонс", "Эмблема канала", "Название канала", "День", "От", "До", "Передачи"],
            colModel: [
                {
                    key: false, name: 'RatingContent', index: 'RatingContent', sortable: true, width: 40, align: "center", formatter: imgRating,
                    cellattr: function (rowId, val, rawObject, cm, rdata) {
                        return 'title="' + rawObject.RatingName + '"';
                    }
                },
                {
                    key: false, name: 'RatingName', index: 'RatingName', sortable: true, widht: "0", align: "center"
                },
                {
                    key: false, name: 'GenreContent', index: 'GenreContent', sortable: true, width: "46px", align: "center", formatter: imgGenre,
                    cellattr: function (rowId, val, rawObject, cm, rdata) {
                        return 'title="' + rawObject.GenreName + '"';
                    }
                },
                {
                    key: false, name: 'GenreName', index: 'GenreName', sortable: true, width: "0", align: "center"
                },
                {
                    key: false, name: 'AnonsContent', index: 'AnonsContent', sortable: true, width: "43px", align: "center", formatter: imgAnons
                },
                {
                    key: false, name: 'ChannelContent', index: 'ChannelContent', sortable: true, align: "center", formatter: imgChannel
                },
                { key: false, name: 'ChannelName', index: 'ChannelName', sortable: true },
                { key: false, name: 'DayMonth', index: 'DayMonth', sortable: true, align: "center", width: 100 } ,
                {
                    key: false, name: 'Start', index: 'Start', sortable: true, align: "center", width: 55, sorttype: 'datetime', formatter: "date", formatoptions: { srcformat: 'ISO8601Long', newformat: 'H:i' }
                },
                {
                    key: false, name: 'Stop', index: 'Stop', sortable: true, align: "center", width: 55, sorttype: 'datetime', formatter: "date", formatoptions: { srcformat: 'ISO8601Long', newformat: 'H:i' }
                },
                { key: false, name: 'TelecastTitle', index: 'TelecastTitle', sortable: true, width: 556 },
            ],
            rowNum: 20,
            loadComplete: function () {
                $("tr.jqgrow:odd").css("background", "#EFFFEF");
            },
            afterInsertRow: function (rowid, rowData, rowelem) {
                var date = new Date(1970, 0, 1, 3, 0, 0);
                date.setMilliseconds(rowData['Stop'].substring(6, rowData['Stop'].length - 2));
                var today = new Date();
                if (today > date) {
                    $(this).jqGrid('setRowData', rowid, false, {
                        color: '#696969'
                    });
                }
                else {
                    $(this).jqGrid('setRowData', rowid, false, { color: '#000' });
                }
            },
            rowList: [10, 20, 30, 40, 50, 100, 500, 1000],
            height: 'auto',
            width: null,
            autowidth: true,
            shrinkToFit: true,
            viewrecords: true,
            caption: 'Программа передач',
            emptyrecords: 'Программа передач не обнаружена',
            jsonReader:
            {
                repeatitems: false,
                root: function (obj) { return obj; },
                page: function (obj) { return 1; },
                total: function (obj) { return obj.rowNum / obj.height; },
                records: function (obj) { return obj.length; }
            },
            pager: '#TVProgrammeSearchPager',
            loadonce: true,
            multiselect: true,
        }).navGrid('#TVProgrammeSearchPager',
        {
            edit: false, add: false, del: false, search: true,
            searchtext: "Поиск передачи", refresh: true
        },
        {
            zIndex: 100,
            caption: "Поиск передачи",
            sopt: ['cn']
        });
    jQuery("#SearchedTVProgramme").jqGrid('hideCol', "GenreName");
    jQuery("#SearchedTVProgramme").jqGrid('hideCol', "RatingName");
    if (uv == 0) {
        jQuery("#SearchedTVProgramme").jqGrid('hideCol', "RatingContent");
    }
}

// Установка деревьев
function setTree(index) {
    var url = '/Tree/GetTreeData?providerId=' + $('#TVProgProvider option:selected').val() + '&typeProg=' + $('#TVProgType option:selected').val() + '&mode=' + (index - 1);
    if (index === 2) {
        
        $('#containerByDays').jstree({
            'core': {
                'data': {
                    'url': url,
                    'dataType': 'json'
                }
            }
        });
        $('#containerByDays').jstree(true).settings.core.data.url = url;
        $('#containerByDays').jstree('refresh');
        
    }
    else if (index === 3) {

        $('#containerByChannels').jstree({
            'core': {
                'data': {
                    'url': url,
                    'dataType': 'json'
                }
            }
        });
        $('#containerByChannels').jstree(true).settings.core.data.url = url;
        $('#containerByChannels').jstree('refresh');
    }
}

// Получить индекс вкладки
function getSelectedTabIndex() {
    return idx;
}

// Установка таблички под дням
function fillUserByDay(date, channelId) {
    $('#TVProgrammeByDaysGrid').jqGrid('GridUnload');
    $('#TVProgrammeByDaysGrid').jqGrid(
        {
            url: "/Programme/GetUserProgrammeOfDay?progTypeID=" + $('#TVProgType option:selected').val() + '&cid=' + channelId + '&tsDate=' +
                                                                  date + "&category=" + $('#TVProgCategories option:selected').val(),
            datatype: 'json',
            mtype: 'Get',
            colNames: ["Рейтинг", "Название рейтинга", "Жанр", "Название жанра", "Анонс", "От", "До", "Передача", ""],
            colModel: [
                {
                    key: false, name: 'RatingContent', index: 'RatingContent', sortable: true, width: 40, align: "center", formatter: imgRating,
                    cellattr: function (rowId, val, rawObject, cm, rdata) {
                        return 'title="' + rawObject.RatingName + '"';
                    }
                },
                {
                    key: false, name: 'RatingName', index: 'RatingName', sortable: true, widht: "0", align: "center"
                },
                {
                    key: false, name: 'GenreContent', index: 'GenreContent', sortable: true, width: 40, align: "center", formatter: imgGenre,
                    cellattr: function (rowId, val, rawObject, cm, rdata) {
                        return 'title="' + rawObject.GenreName + '"';
                    }
                },
                {
                    key: false, name: 'GenreName', index: 'GenreName', sortable: true, width: "0", align: "center"
                },
                {
                    key: false, name: 'AnonsContent', index: 'AnonsContent', sortable: true, width: 40, align: "center", formatter: imgAnons
                },
                {
                    key: false, name: 'Start', index: 'Start', sortable: true, align: "center", width: 80, sorttype: 'datetime', formatter: "date", formatoptions: { srcformat: 'ISO8601Long', newformat: 'H:i' }
                },
                {
                    key: false, name: 'Stop', index: 'Stop', sortable: true, align: "center", width: 80, sorttype: 'datetime', formatter: "date", formatoptions: { srcformat: 'ISO8601Long', newformat: 'H:i' }
                },
                {   key: false, name: 'TelecastTitle', index: 'TelecastTitle', width: 850, sortable: true },
                {
                    key: false, name: 'TelecastDescr', index: 'TelecastDescr', hidden: true
                }
            ],
            rowNum: 20,
            beforeSelectRow: function (rowid, e) {
                $('#TVProgrammeByDaysGrid').jqGrid('resetSelection');
                return (true);
            },
            afterInsertRow: function (rowid, rowData, rowelem) {
                var dateFrom = new Date(1970, 0, 1, 3, 0, 0);
                dateFrom.setMilliseconds(rowData['Start'].substring(6, rowData['Start'].length - 2));
                var dateTo = new Date(1970, 0, 1, 3, 0, 0);
                dateTo.setMilliseconds(rowData['Stop'].substring(6, rowData['Stop'].length - 2));
                var today = new Date();
                if (dateTo < today) {
                    $(this).jqGrid('setRowData', rowid, false, {
                        color: '#696969'
                    });
                }
                if (dateFrom <= today && dateTo > today)
                {
                    $(this).jqGrid('setRowData', rowid, false, { color: '#000' });
                }
                if (dateFrom > today)
                {
                    $(this).jqGrid('setRowData', rowid, false, { color: '#006400' });
                }
            },
            loadComplete: function () {
                $("tr.jqgrow:odd").css("background", "#EFFFEF");

                $("tr.jqgrow td input", "#TVProgrammeByDaysGrid").click(function () {
                    if ($(this).closest('tr').find('td:nth-child(4)').find('img').length) {
                        $("#anonsToolByDays").show(50);
                        $("#anonsDescrByDays").html($(this).closest('tr').find('td:nth-child(10)').attr('title'));
                    }
                    else {
                        $("#anonsToolByDays").hide(50);
                        $("#anonsDescrByDays").hide(150);
                    }
                });
            },
            rowList: [10, 20, 30, 40, 50, 100, 500, 1000],
            height: 'auto',
            width: null,
            autowidth: true,
            shrinkToFit: true,
            viewrecords: true,
            caption: 'Программа передач',
            shrinkToFit: true,
            emptyrecords: 'Программа передач не обнаружена',
            jsonReader:
            {
                repeatitems: false,
                root: function (obj) { return obj; },
                page: function (obj) { return 1; },
                total: function (obj) { return obj.rowNum / obj.height; },
                records: function (obj) { return obj.length; }
            },
            pager: '#TVProgrammeByDaysPager',
            loadonce: true,
            multiselect: true,
        }).navGrid('#TVProgrammeByDaysPager',
        {
            edit: false, add: false, del: false, search: true,
            searchtext: "Поиск передачи", refresh: true
        },
        {
            zIndex: 100,
            caption: "Поиск передачи",
            sopt: ['cn']
        });
    jQuery('#TVProgrammeByDaysGrid').jqGrid('hideCol', "GenreName");
    jQuery('#TVProgrammeByDaysGrid').jqGrid('hideCol', "RatingName");
    if (uv == 0) {
        jQuery("#TVProgrammeByDaysGrid").jqGrid('hideCol', "RatingContent");
    }
}

// Установка таблички по каналам
function fillUserByChannels(date, channelId) {
    $('#TVProgrammeByChannelsGrid').jqGrid('GridUnload');
    $('#TVProgrammeByChannelsGrid').jqGrid(
        {
            url: "/Programme/GetUserProgrammeOfDay?progTypeID=" + $('#TVProgType option:selected').val() + '&cid=' + channelId + '&tsDate=' +
            date + "&category=" + $('#TVProgCategories option:selected').val(),
            datatype: 'json',
            mtype: 'Get',
            colNames: ["Рейтинг", "Название рейтинга", "Жанр", "Название жанра", "Анонс", "От", "До", "Передача", ""],
            colModel: [
                {
                    key: false, name: 'RatingContent', index: 'RatingContent', sortable: true, width: 40, align: "center", formatter: imgRating,
                    cellattr: function (rowId, val, rawObject, cm, rdata) {
                        return 'title="' + rawObject.RatingName + '"';
                    }
                },
                {
                    key: false, name: 'RatingName', index: 'RatingName', sortable: true, widht: "0", align: "center"
                },
                {
                    key: false, name: 'GenreContent', index: 'GenreContent', sortable: true, width: 40, align: "center", formatter: imgGenre,
                    cellattr: function (rowId, val, rawObject, cm, rdata) {
                        return 'title="' + rawObject.GenreName + '"';
                    }
                },
                {
                    key: false, name: 'GenreName', index: 'GenreName', sortable: true, width: "0", align: "center"
                },
                {
                    key: false, name: 'AnonsContent', index: 'AnonsContent', sortable: true, width: 40, align: "center", formatter: imgAnons
                },
                {
                    key: false, name: 'Start', index: 'Start', sortable: true, align: "center", width: 80, sorttype: 'datetime', formatter: "date", formatoptions: { srcformat: 'ISO8601Long', newformat: 'H:i' }
                },
                {
                    key: false, name: 'Stop', index: 'Stop', sortable: true, align: "center", width: 80, sorttype: 'datetime', formatter: "date", formatoptions: { srcformat: 'ISO8601Long', newformat: 'H:i' }
                },
                { key: false, name: 'TelecastTitle', index: 'TelecastTitle', width: 850, sortable: true },
                {
                    key: false, name: 'TelecastDescr', index: 'TelecastDescr', hidden: true
                }
            ],
            rowNum: 20,
            beforeSelectRow: function (rowid, e) {
                $('#TVProgrammeByChannelsGrid').jqGrid('resetSelection');
                return (true);
            },
            afterInsertRow: function (rowid, rowData, rowelem) {
                var dateFrom = new Date(1970, 0, 1, 3, 0, 0);
                dateFrom.setMilliseconds(rowData['Start'].substring(6, rowData['Start'].length - 2));
                var dateTo = new Date(1970, 0, 1, 3, 0, 0);
                dateTo.setMilliseconds(rowData['Stop'].substring(6, rowData['Stop'].length - 2));
                var today = new Date();
                if (dateTo < today) {
                    $(this).jqGrid('setRowData', rowid, false, {
                        color: '#696969'
                    });
                }
                if (dateFrom <= today && dateTo > today) {
                    $(this).jqGrid('setRowData', rowid, false, { color: '#000' });
                }
                if (dateFrom > today) {
                    $(this).jqGrid('setRowData', rowid, false, { color: '#006400' });
                }
            },
            loadComplete: function () {
                $("tr.jqgrow:odd").css("background", "#EFFFEF");

                $("tr.jqgrow td input", "#TVProgrammeByChannelsGrid").click(function () {
                    if ($(this).closest('tr').find('td:nth-child(4)').find('img').length) {
                        $("#anonsToolByChannels").show(50);
                        $("#anonsDescrByChannels").html($(this).closest('tr').find('td:nth-child(10)').attr('title'));
                    }
                    else {
                        $("#anonsToolByChannels").hide(50);
                        $("#anonsDescrByChannels").hide(150);
                    }
                });
            },
            rowList: [10, 20, 30, 40, 50, 100, 500, 1000],
            height: 'auto',
            width: null,
            autowidth: true,
            shrinkToFit: true,
            viewrecords: true,
            caption: 'Программа передач',
            emptyrecords: 'Программа передач не обнаружена',
            jsonReader:
            {
                repeatitems: false,
                root: function (obj) { return obj; },
                page: function (obj) { return 1; },
                total: function (obj) { return obj.rowNum / obj.height; },
                records: function (obj) { return obj.length; }
            },
            pager: '#TVProgrammeByChannelsPager',
            loadonce: true,
            multiselect: true,
        }).navGrid('#TVProgrammeByChannelsPager',
        {
            edit: false, add: false, del: false, search: true,
            searchtext: "Поиск передачи", refresh: true
        },
        {
            zIndex: 100,
            caption: "Поиск передачи",
            sopt: ['cn']
        });
    jQuery('#TVProgrammeByChannelsGrid').jqGrid('hideCol', "GenreName");
    jQuery('#TVProgrammeByChannelsGrid').jqGrid('hideCol', "RatingName");
    if (uv == 0) {
        jQuery("#TVProgrammeByChannelsGrid").jqGrid('hideCol', "RatingContent");
    }
}