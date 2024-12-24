var idx;
var pairKeys;
var incSearch = 1;
var incChannelByDay = 1;
var incDayByChannel = 1;
let chansArr = [];
$(function () {
    $.jgrid.no_legacy_api = true;
    $.jgrid.useJSON = true;
    
    chansArr = getStorageChannels();
    if (chansArr && chansArr.length > 0) {
        $(".tabChannels, .tabNow, .tabNext, .tabSearch").removeClass("active disable");
        $(".tabNow").addClass("active");
        $(".byDays, .byChannels").show();
        $("#category-editor").show(500);
    } else {
        $(".tabNow, .tabNext, .tabSearch").addClass("disable");
        $(".tabChannels").addClass("active");
        $("#category-editor").hide(500);
        return;
    }
        
    
    fillFooter();
    fillGenresToolNow();
    fillGenresToolNext();
    fillGenresToolSearch();
    fillSearchTab();
    fillDatesToolSearch();
    fillChannelsToolSearch();
    
    setGrids();
    $('#tabs').tabs({
        show: function (event, ui) {
            if (grid = $('.ui-jqgrid-btable:visible')) {
                grid.each(function (index) {
                    gridId = $(this).attr('id');
                    gridParentWidth = $('#gbox_' + gridId).parent().width();
                    $('#' + gridId).setGridWidth(gridParentWidth);
                });
            }
        },
        activate: function (event, ui) {
            idx = ui.newTab.index();
            if (idx === 2 || idx === 3) {
                setTree(idx);
            }
            if (idx === 4) {
                var datesToolSearch = document.getElementById("datesToolSearch");
                datesToolSearch.scrollTop = datesToolSearch.scrollHeight - datesToolSearch.clientHeight;
            }
            if (idx === 5) {
                $("#category-editor").hide(500);
            } else {
                $("#category-editor").show(500);
            }
        }
    });
    
    $("#anonsTool").on('click', function () {
        $('#anonsDescr').toggle(100);
    });
    $("#anonsToolNext").on('click', function () {
        $('#anonsDescrNext').toggle(100);
    });

    $("#anonsToolSearch").on('click', function () {
        $('#anonsDescrSearch').toggle(100);
    });
    $("#chkAll").on('click', function () {
        checkSearchAll();
    });

    $("#chkUncheckAll").on('click', function () {
        uncheckSearchAll();
    });

    $("#chkInvert").on('click', function () {
        invertSearchAll();
    });
    $("#anonsToolByDays").on('click', function () {
        $('#anonsDescrByDays').toggle(100);
    });
    $("#anonsToolByChannels").on('click', function () {
        $('#anonsDescrByChannels').toggle(100);
    });
    $("#btnSearch").on('click', function () {
        searchProgramme($('#tbContains').val());
    });

    $("#containerByDays")
    .on('open_node.jstree', function (evt, data) {
        if (data.node.parents.length === 3)
            data.instance.set_icon(data.node, (data.node.icon.indexOf('_exp') === -1) ? [data.node.icon.slice(0, 13), '_exp', data.node.icon.slice(13)].join('') : data.node.icon);
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
    $("#dragToolGenreNow").draggable();
    $("#dragToolGenreNext").draggable();
});


// Заполенение подвала
function fillFooter() {
    $.ajax({
        url: "Home/GetSystemProgrammePeriod?progType=1",
        dataType: 'json',
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        success: function (response) {
            if (response) {
                var startDate = new Date(Date.parse(response.dtStart));
                var endDate = new Date(Date.parse(response.dtEnd));
                $("#period").text(formatDateString(startDate) + " — " + formatDateString(endDate));
            }
        },
        error: function (err) {
            err.statusText;
        }
    }); 
}

// Преобразование к формату даты (Ru-ru)
function formatDateString(date) {
    var day = ("0" + date.getDate()).slice(-2);
    var month = ("0" + (date.getMonth() + 1)).slice(-2);
    var year = date.getFullYear();
    return day + "." + month + "." + year;
}

// Преобразование к формату даты (202001010)
function numDateString(date) {
    var day = ("0" + date.getDate()).slice(-2);
    var month = ("0" + (date.getMonth() + 1)).slice(-2);
    var year = date.getFullYear();
    return year + '' + month + '' + day;
}



// Установка табличек
function setGrids() {
    
    // Табличка сейчас в эфире
    $('#TVProgrammeNowGrid').jqGrid(
        {
            url: "Home/GetSystemProgrammeAtNow?progType=1&category=" + $('#userCategory option:selected').val().split(';')[1] +
                "&genres=" + GetGenres(".btn-genre-now.active") + "&channels=" + ((chansArr) ? chansArr.map(ch => ch.ChannelId).join(";") : ""),
            datatype: 'json',
            type: 'GET',
            success: function () { },
            error: function () { console.log("ERROR"); },
            colNames: ["Рейтинг", "Название рейтинга", "Жанр", "Название жранра", "Анонс", "Эмблема канала", "Название канала", "Передача", "Начало", "Окончание", "Осталось, %", "", ""],
            colModel: [
                {
                    key: false, name: 'RatingContent', index: 'RatingContent', sortable: true, width: "2%", align: "center", formatter: imgRating,
                    cellattr: function (rowId, val, rawObject, cm, rdata) {
                        return 'title="' + rawObject.RatingName + '"';
                    }
                },
                {
                    key: false, name: 'RatingName', index: 'RatingName', sortable: true, width: "0", align: "center"
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
                        return "<div style='position: relative'><div class='remain-progress'>" + cellVal + "%</div><div class='progress-bar progress-bar-striped' role='progressbar' aria-valuemin='100' aria-valuemax='1' style='width: " + cellVal + "%; position: relative; height: 23px; float: right;'></div></div>";
                    }
                },
                {
                    key: false, name: 'TelecastDescr', index: 'TelecastDescr', hidden: true
                },
                {
                    key: false, name: 'OrderCol', index: 'OrderCol', hidden: true
                }
            ],
            rowNum: 20,
            beforeSelectRow: function (rowid, e) {
                $('#TVProgrammeNowGrid').jqGrid('resetSelection');
                return (true);
            },            
            loadComplete: function () {
                $(this).find("tr.jqgrow:odd").addClass("alt-green-background");
                $("tr.jqgrow td input", "#TVProgrammeNowGrid").on('click', function () {
                    if ($(this).closest('tr').find('td:nth-child(6)').find('img').length) {
                        $("#mainToolNow").show(50); 
                        $("#anonsTool").show(50);
                        $("#anonsDescr").html($(this).closest('tr').find('td:nth-child(13)').attr('title'));
                    }
                    else {
                        $("#anonsTool").hide(50);
                        $("#mainToolNow").hide(50);
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
            caption: 'Программа передач на сейчас | Что сейчас показывают на телевидении | Сколько процентов от времени передачи осталось до окончания',
            emptyrecords: 'Программа передач не обнаружена',
            pager: '#TVProgrammePager',
            loadonce: false,
            forceClientSorting: true,
            multiselect: true
        }).navGrid('#TVProgrammePager',
        {
            edit: false, add: false, del: false, search: false,
            searchtext: "Поиск передачи", refresh: true
        },
         {
            zIndex: 100,
            caption: "Поиск передачи",
            sopt: ['cn']
        });
    jQuery("#TVProgrammeNowGrid").jqGrid('hideCol', "GenreName");
    jQuery("#TVProgrammeNowGrid").jqGrid('hideCol', "RatingName");
    //if (uv == 0) {
    jQuery("#TVProgrammeNowGrid").jqGrid('hideCol', "RatingContent");
    //}
    jQuery('#TVProgrammeNowGrid').jqGrid('setGridWidth', $("#mw").width()); 
    jQuery("#TVProgrammeNowGrid").jqGrid("setGridHeight", 548);
    $(".drag-tool-genre").width($("#mw").width());

    // Табличка затем в эфире
    $('#TVProgrammeNextGrid').jqGrid(
     {
            url: "Home/GetSystemProgrammeAtNext?progType=1&category=" + $('#userCategory option:selected').val().split(';')[1] +
                "&genres=" + GetGenres(".btn-genre-next.active") + "&channels=" + ((chansArr) ? chansArr.map(ch => ch.ChannelId).join(";") : ""),
            datatype: 'json',
            type: 'GET',
            colNames: ["Рейтинг", "Название рейтинга", "Жанр", "Название жанра", "Анонс", "Эмблема канала", "Название канала", "Передача", "Начало", "Окончание", "Осталось", "", ""],
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
                    key: false, name: 'Remain', index: 'Remain', sortable: true, align: "center", width: "5%", sorttype: 'datetime', formatter: function (cellval, opts, rowObject, action) {
                        return $.fn.fmatter.call(
                            this,
                            "date",
                            new Date(cellval * 1000 - 3*60*60*1000),
                            opts,
                            rowObject,
                            action);
                    }
                    , formatoptions: { srcformat: 'ISO8601Long', newformat: 'H:i' }
                },
                {
                    key: false, name: 'TelecastDescr', index: 'TelecastDescr', hidden: true
                },
                {
                    key: false, name: 'OrderCol', index: 'OrderCol', hidden: true
                }
            ],
            rowNum: 20,
            beforeSelectRow: function (rowid, e) {
                $('#TVProgrammeNextGrid').jqGrid('resetSelection');
                return (true);
            },
            loadComplete: function () {
                $(this).find("tr.jqgrow:odd").addClass("alt-green-background");

                $("tr.jqgrow td input", "#TVProgrammeNextGrid").on('click', function () {
                    if ($(this).closest('tr').find('td:nth-child(6)').find('img').length) {
                        $("#mainToolNext").show(50);
                        $("#anonsToolNext").show(50);
                        $("#anonsDescrNext").html($(this).closest('tr').find('td:nth-child(13)').attr('title'));
                    }
                    else {
                        $("#mainToolNext").hide(50);
                        $("#anonsToolNext").hide(50);
                        $("#anonsDescrNext").hide(150);
                    }
                });
            },
            rowList: [10, 20, 30, 40, 50, 100, 500, 1000],
            height: 'auto',
            width: null,
            autowidth: false,
            shrinkToFit: true,
            autoResizing: { minColWidth: 80 },
            viewrecords: true,
            caption: 'Телепередачи после текущих передач | Что последует в телеэфире | Через сколько в часах и минутах они начнутся',
            emptyrecords: 'Программа передач не обнаружена',
            pager: '#TVProgrammeNextPager',
            loadonce: false,
            multiselect: true,
        }).navGrid('#TVProgrammeNextPager',
        {
            edit: false, add: false, del: false, search: false,
            searchtext: "Поиск передачи", refresh: true
        },
        {
            zIndex: 100,
            caption: "Поиск передачи",
            sopt: ['cn']
        });
    jQuery("#TVProgrammeNextGrid").jqGrid('hideCol', "GenreName");
    jQuery("#TVProgrammeNextGrid").jqGrid('hideCol', "RatingName");
    //if (uv == 0) {
        jQuery("#TVProgrammeNextGrid").jqGrid('hideCol', "RatingContent");
    //}
    jQuery('#TVProgrammeNextGrid').jqGrid('setGridWidth', $("#mw2").width());
    jQuery("#TVProgrammeNextGrid").jqGrid("setGridHeight", 548);
    $(".drag-tool-genre").width($("#mw2").width());
}

// Поиск по всей программе передач
function searchProgramme(findTitle) {
    if (incSearch == 1)
    {
        $('#SearchedTVProgramme').jqGrid(
            {
                url: "Home/SearchProgramme?progType=1&findTitle=" + findTitle + "&category=" + $('#userCategory option:selected').val().split(';')[1] +
                    "&genres=" + getSearchGenres(".chkGenres:checkbox:checked") + "&dates=" + getSearchDates(".chkDates:checkbox:checked") +
                    "&channels=" + getSearchChannels(".chkChannels:checkbox:checked"),
                datatype: 'json',
                type: 'GET',
                colNames: ["Рейтинг", "Название рейтинга", "Жанр", "Название жанра", "Анонс", "Эмблема канала", "Название канала", "День", "От", "До", "Передачи", ""],
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
                    { key: false, name: 'DayMonth', index: 'DayMonth', sortable: true, align: "center", width: 100 },
                    {
                        key: false, name: 'Start', index: 'Start', sortable: true, align: "center", width: 55, sorttype: 'datetime', formatter: "date", formatoptions: { srcformat: 'ISO8601Long', newformat: 'H:i' }
                    },
                    {
                        key: false, name: 'Stop', index: 'Stop', sortable: true, align: "center", width: 55, sorttype: 'datetime', formatter: "date", formatoptions: { srcformat: 'ISO8601Long', newformat: 'H:i' }
                    },
                    { key: false, name: 'TelecastTitle', index: 'TelecastTitle', sortable: true, width: 556 },
                    {
                        key: false, name: 'TelecastDescr', index: 'TelecastDescr', hidden: true
                    }
                ],
                rowNum: 20,
                beforeSelectRow: function (rowid, e) {
                    $('#SearchedTVProgramme').jqGrid('resetSelection');
                    return (true);
                },
                loadComplete: function () {
                    $(this).find("tr.jqgrow:odd").addClass("alt-green-background");

                    $("tr.jqgrow td input", "#SearchedTVProgramme").on('click', function () {
                        if ($(this).closest('tr').find('td:nth-child(6)').find('img').length) {
                            $("#mainToolSearch").show(50);
                            $("#anonsToolSearch").show(50);
                            $("#anonsDescrSearch").html($(this).closest('tr').find('td:nth-child(13)').attr('title'));
                        }
                        else {
                            $("#anonsToolSearch").hide(50);
                            $("#mainToolSearch").hide(50);
                            $("#anonsDescrSearch").hide(150);
                        }
                    });
                },
                rowList: [10, 20, 30, 40, 50, 100, 500, 1000],
                height: 'auto',
                width: null,
                autowidth: true,
                shrinkToFit: true,
                viewrecords: true,
                caption: 'Список найденных передач из эфирной сетки | Телепередачи, которые вы ищете | Результаты поиска по программе передач',
                emptyrecords: 'Программа передач не обнаружена',
                pager: '#TVProgrammeSearchPager',
                loadonce: false,
                forceClientSorting: true,
                multiselect: true,
                gridview: true,
                rowattr: function (rd) {
                    var stopDate = new Date(Date.parse(rd.Stop));
                    var today = new Date();
                    if (today > stopDate)
                        return { "class": "grayColor" }
                    else
                        return { "class": "blackColor" };
                }
            }).navGrid('#TVProgrammeSearchPager',
                {
                    edit: false, add: false, del: false, search: false,
                    searchtext: "Поиск передачи", refresh: true
                },
                {
                    zIndex: 100,
                    caption: "Поиск передачи",
                    sopt: ['cn']
                });
        jQuery("#SearchedTVProgramme").jqGrid('hideCol', "GenreName");
        jQuery("#SearchedTVProgramme").jqGrid('hideCol', "RatingName");
        /*if (uv == 0) {
            jQuery("#SearchedTVProgramme").jqGrid('hideCol', "RatingContent");
        }*/
        jQuery('#SearchedTVProgramme').jqGrid('setGridWidth', $("#mw3").width());
        jQuery("#SearchedTVProgramme").jqGrid("setGridHeight", 548);
    }
    else
    {
        $("#SearchedTVProgramme").setGridParam({
            url: "Home/SearchProgramme?progType=1&findTitle=" + findTitle + "&category=" + $('#userCategory option:selected').val().split(';')[1] +
                "&genres=" + getSearchGenres(".chkGenres:checkbox:checked") + "&dates=" + getSearchDates(".chkDates:checkbox:checked") +
                "&channels=" + getSearchChannels(".chkChannels:checkbox:checked")
        });
        $("#SearchedTVProgramme").trigger("reloadGrid");
    }
    incSearch++;
}

// Установка деревьев
function setTree(index) {
    var url = "Home/GetTreeData?providerId=1&typeProg=1&jsonChannels=" + window.localStorage.getItem("optChans") + '&mode=' + (index - 1);
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
    if (incChannelByDay == 1) {
        $("#TVProgrammeByDaysGrid").jqGrid(
            {
                url: "Home/GetUserProgrammeOfDay?progTypeID=1&cid=" + channelId + "&tsDate=" +
                    date + "&category=" + $('#userCategory option:selected').val().split(';')[1],
                datatype: 'json',
                type: 'GET',
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
                rowNum: 500,
                beforeSelectRow: function (rowid, e) {
                    $('#TVProgrammeByDaysGrid').jqGrid('resetSelection');
                    return (true);
                },
                loadComplete: function () {
                    $(this).find("tr.jqgrow:odd").addClass("alt-green-background");

                    $("tr.jqgrow td input", "#TVProgrammeByDaysGrid").on('click', function () {
                        if ($(this).closest('tr').find('td:nth-child(6)').find('img').length) {
                            $("#mainToolByDays").show(50); 
                            $("#anonsToolByDays").show(50);
                            $("#anonsDescrByDays").html($(this).closest('tr').find('td:nth-child(10)').attr('title'));
                        }
                        else {
                            $("#mainToolByDays").hide(50); 
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
                caption: 'Программа передач по телеканалу на определённый день',
                shrinkToFit: true,
                emptyrecords: 'Программа передач не обнаружена',
                pager: '#TVProgrammeByDaysPager',
                loadonce: false,
                multiselect: true,
                gridview: true,
                rowattr: function (rd) {
                    var dateFrom = new Date(Date.parse(rd.Start));
                    var dateTo = new Date(Date.parse(rd.Stop));
                    var today = new Date();
                    if (dateTo < today) {
                        return { "class": "grayColor" };
                    }
                    if (dateFrom <= today && dateTo > today) {
                        return { "class": "blackColor" };
                    }
                    if (dateFrom > today) {
                        return { "class": "greenColor" };
                    }
                }
            }).navGrid('#TVProgrammeByDaysPager',
                {
                    edit: false, add: false, del: false, search: false,
                    searchtext: "Поиск передачи", refresh: true
                },
                {
                    zIndex: 100,
                    caption: "Поиск передачи",
                    sopt: ['cn']
                });
        $("#TVProgrammeByDaysGrid").jqGrid('hideCol', "GenreName");
        $("#TVProgrammeByDaysGrid").jqGrid('hideCol', "RatingName");
        jQuery('#TVProgrammeByDaysGrid').jqGrid('setGridWidth', $("#mw5").width());
        jQuery("#TVProgrammeByDaysGrid").jqGrid("setGridHeight", 548);
    } else {
        $("#TVProgrammeByDaysGrid").setGridParam({
            url: "Home/GetUserProgrammeOfDay?progTypeID=1&cid=" + channelId + "&tsDate=" +
                date + "&category=" + $('#userCategory option:selected').val().split(';')[1]
        });
        $("#TVProgrammeByDaysGrid").trigger("reloadGrid");
    }
    incChannelByDay++;
  jQuery("#TVProgrammeByDaysGrid").jqGrid('setGridWidth', $("#mw5").width());
    jQuery("#TVProgrammeByDaysGrid").jqGrid("setGridHeight", 548);    
    $(".drag-tool-genre").width($("#mw5").width());
    /*if (uv == 0) {
        jQuery("#TVProgrammeByDaysGrid").jqGrid('hideCol', "RatingContent");
    }*/
}

// Установка таблички по каналам
function fillUserByChannels(date, channelId) {
    if (incDayByChannel == 1) {
        $('#TVProgrammeByChannelsGrid').jqGrid(
            {
                url: "Home/GetUserProgrammeOfDay?progTypeID=1&cid=" + channelId + "&tsDate=" +
                    date + "&category=" + $('#userCategory option:selected').val().split(';')[1],
                datatype: 'json',
                type: 'GET',
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
                rowNum: 500,
                beforeSelectRow: function (rowid, e) {
                    $('#TVProgrammeByChannelsGrid').jqGrid('resetSelection');
                    return (true);
                },
                loadComplete: function () {
                    $("tr.jqgrow:odd").css("background", "#EFFFEF");

                    $("tr.jqgrow td input", "#TVProgrammeByChannelsGrid").on('click', function () {
                        if ($(this).closest('tr').find('td:nth-child(6)').find('img').length) {
                            $("#mainToolByChannels").show(50);
                            $("#anonsToolByChannels").show(50);
                            $("#anonsDescrByChannels").html($(this).closest('tr').find('td:nth-child(10)').attr('title'));
                        }
                        else {
                            $("#mainToolByChannels").hide(50);
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
                caption: 'Программа передач по дням на определённый телеканал',
                emptyrecords: 'Программа передач не обнаружена',
                pager: '#TVProgrammeByChannelsPager',
                loadonce: false,
                multiselect: true,
                gridview: true,
                rowattr: function (rd) {
                    var dateFrom = new Date(Date.parse(rd.Start));
                    var dateTo = new Date(Date.parse(rd.Stop));
                    var today = new Date();
                    if (dateTo < today) {
                        return { "class": "grayColor" };
                    }
                    if (dateFrom <= today && dateTo > today) {
                        return { "class": "blackColor" };
                    }
                    if (dateFrom > today) {
                        return { "class": "greenColor" };
                    }
                }
            }).navGrid('#TVProgrammeByChannelsPager',
                {
                    edit: false, add: false, del: false, search: false,
                    searchtext: "Поиск передачи", refresh: true
                },
                {
                    zIndex: 100,
                    caption: "Поиск передачи",
                    sopt: ['cn']
                });
        jQuery('#TVProgrammeByChannelsGrid').jqGrid('hideCol', "GenreName");
        jQuery('#TVProgrammeByChannelsGrid').jqGrid('hideCol', "RatingName");
    } else {
        $('#TVProgrammeByChannelsGrid').setGridParam({
            url: "Home/GetUserProgrammeOfDay?progTypeID=1&cid=" + channelId + "&tsDate=" +
                date + "&category=" + $('#userCategory option:selected').val().split(';')[1]
        });
        $('#TVProgrammeByChannelsGrid').trigger("reloadGrid");
    }
  incDayByChannel++;
  jQuery("#TVProgrammeByChannelsGrid").jqGrid('setGridWidth', $("#mw6").width());
    jQuery("#TVProgrammeByChannelsGrid").jqGrid("setGridHeight", 548);  
    $(".drag-tool-genre").width($("#mw6").width());
    /*if (uv == 0) {
        jQuery("#TVProgrammeByChannelsGrid").jqGrid('hideCol', "RatingContent");
    }*/
}

// Получение отмеченных жанров
function getSearchGenres(chbGenreChecked) {
    var ids_genres = $(chbGenreChecked).map(function () {
        return this.id.replace('Genre', '');
    }).get();

    return ids_genres.join(";");
}

// Получение отмеченных дат
function getSearchDates(chbChecked) {
    var ids_dates = $(chbChecked).map(function () {
        return this.id.replace('Date', '');
    }).get();

    return ids_dates.join(";");
}

// Получение отмеченных каналов
function getSearchChannels(chbChannelChecked) {
    var ids_channels = $(chbChannelChecked).map(function () {
        return this.id.replace('Channel', '');
    }).get();

    return ids_channels.join(";");
}

function fillGenresToolNow() {
    $('#genresToolNow').hide();
    $.ajax({
        url: "Home/GetGenres",
        dataType: 'json',
        async: false,
        type: 'Get',
        contentType: 'application/json; charset=utf-8',
        success: function (response) {
            $('#genresToolNow').empty();
            for (var i = 0; i < response.length; i++) {
                var b = $('<button id="' + response[i].GenreId + '" class="btn btn-default btn-genre-now">');

                $('#genresToolNow').append(
                    b.html('<img src="' + response[i].GenrePath + '" title="' + response[i].GenreName + '" alt="' + response[i].GenreName + '" height="24px" width="24px">'));


            }
            $('.btn-group-genres-now').on('click', '.btn', function (e) {
                e.preventDefault();
                $(this).toggleClass("active");
                $("#TVProgrammeNowGrid").setGridParam({
                    url: "Home/GetSystemProgrammeAtNow?progType=1&category=" + $('#userCategory option:selected').val().split(';')[1] + "&genres=" + GetGenres(".btn-genre-now.active") +
                        "&channels=" + ((chansArr) ? chansArr.map(ch => ch.ChannelId).join(";") : "")
                });
                $("#TVProgrammeNowGrid").trigger("reloadGrid");
            });
            window.setTimeout(function () { $('#genresToolNow').show(); }, 500);
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
}

function fillGenresToolNext() {
    $('#genresToolNext').hide();
    $.ajax({
        url: "Home/GetGenres",
        dataType: 'json',
        async: false,
        type: 'Get',
        contentType: 'application/json; charset=utf-8',
        success: function (response) {
            $('#genresToolNext').empty();
            for (var i = 0; i < response.length; i++) {
                var b = $('<button id="' + response[i].GenreId + '" class="btn btn-default btn-genre-next">');

                $('#genresToolNext').append(
                    b.html('<img src="' + response[i].GenrePath + '" title="' + response[i].GenreName + '" alt="' + response[i].GenreName +
                        '" height="24px" width="24px">'));


            }
            $('.btn-group-genres-next').on('click', '.btn', function (e) {
                e.preventDefault();
                $(this).toggleClass("active");
                $("#TVProgrammeNextGrid").setGridParam({
                    url: "Home/GetSystemProgrammeAtNext?progType=1&category=" + $('#userCategory option:selected').val().split(';')[1] +
                        "&genres=" + GetGenres(".btn-genre-next.active") +
                        "&channels=" + ((chansArr) ? chansArr.map(ch => ch.ChannelId).join(";") : ""),
                });
                $("#TVProgrammeNextGrid").trigger("reloadGrid");
            });
            window.setTimeout(function () { $('#genresToolNext').show(); }, 3000);
            
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
}

function fillGenresToolSearch() {
    
        $.ajax({
            url: "Home/GetGenres",
            dataType: 'json',
            async: false,
            type: 'Get',
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                $('#genreToolSearch').empty();
                for (var i = 0; i < response.length; i++) {
                    $("#genreToolSearch").append('<div class="row search-conditions">' +
                        appendGenreColumn(response[i]) +
                        '</div>');
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

}

function fillChannelsToolSearch() {
   $.ajax({
        url: "Home/GetChannels?providerId=1&jsonChannels=" + window.localStorage.getItem("optChans"),
        dataType: 'json',
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        success: function (response) {
            var arrChannels = response;
            for (var i = 1; i <= arrChannels.length; i++) {
                $("#channelToolSearch").append('<div class="row search-conditions">' +
                    appendChannelColumn(arrChannels[i - 1]) +
                    '</div>');
            }
        }
    });
}

function fillDatesToolSearch() {
    $.ajax({
        url: "Home/GetSystemProgrammePeriod?progType=1",
        dataType: 'json',
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        success: function (response) {
            var startDate = new Date(Date.parse(response.dtStart));
            var endDate = new Date(Date.parse(response.dtEnd));
            var arrDates = getDates(startDate, endDate);
            for (var i = 1; i <= arrDates.length; i++) {
                $("#datesToolSearch").append('<div class="row search-conditions">' +
                    appendDateColumn(arrDates[i - 1]) +
                                              '</div>');
            }
        }
    });
}

Date.prototype.yyyymmdd = function () {
    var mm = this.getMonth() + 1; // getMonth() is zero-based
    var dd = this.getDate();

    return [this.getFullYear(),
    (mm > 9 ? '' : '0') + mm,
    (dd > 9 ? '' : '0') + dd
    ].join('');
};

Date.prototype.addDays = function (days) {
    var date = new Date(this.valueOf());
    date.setDate(date.getDate() + days);
    return date;
};

function appendDateColumn(dt) {
    var now = new Date();
    var today = new Date(now.getFullYear(), now.getMonth(), now.getDate()).valueOf();
    var dtDate = new Date(dt);
    var chb = dtDate >= today ? 'checked="checked" ' : '';
    var days = ['Вс', 'Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб'];
    
    return '<div  class="form-group form-check">' +
        (typeof dt !== 'undefined' ? '<input id="Date' + numDateString(dt) + '" type="checkbox" ' + chb + 'class="form-check-input chkDates">' : '') +
           (typeof dt !== 'undefined' ? '<label class="form-check-label label-of-checkbox" for="Date' +
              numDateString(dt) + '"><img class="pic-of-checkbox" src="/images/i/' + getDayOfWeek(dt) + '.png" alt="' + days[dtDate.getDay()] +'"></img>' : '') +
                (typeof dt !== 'undefined' ? formatDateString(dt) : '') + '</label></div></div>';
}

function appendGenreColumn(genre) {
    return '<div class="form-group form-check">' +
        (typeof genre !== 'undefined' ? '<input id="Genre' + genre.GenreId + '" type="checkbox" class="form-check-input chkGenres">' : '') +
        (typeof genre !== 'undefined' ? '<label class="form-check-label label-of-checkbox" for="Genre' +
            genre.GenreId + '"><img class="pic-genre-of-checkbox" src="' + genre.GenrePath + '" title="' + genre.GenreName + '" alt="' + genre.GenreName + '" height="24px" width="24px"></img>' : '') +
        (typeof genre !== 'undefined' ? genre.GenreName : '') + '</label></div></div>';
}

function appendChannelColumn(channel) {
    return '<div class="form-group form-check">' +
        (typeof channel !== 'undefined' ? '<input id="Channel' + channel.ChannelId + '" type="checkbox" checked="checked" class="form-check-input chkChannels">' : '') +
        (typeof channel !== 'undefined' ? '<label class="form-check-label label-of-checkbox" for="Channel' +
        channel.ChannelId + '"><img class="pic-channel-of-checkbox" src="' + channel.FileName25 + '" alt="' + channel.Title + '"></img>' : '') +
        (typeof channel !== 'undefined' ? channel.Title : '') + '</label></div></div>';
}
function getDates(startDate, stopDate) {
    var dateArray = new Array();
    var currentDate = startDate;
    while (currentDate <= stopDate) {
        dateArray.push(new Date(currentDate));
        currentDate = currentDate.addDays(1);
    }
    return dateArray;
}

function getDayOfWeek(dt) {
    var days = ['Sun', 'Mon', 'Tue', 'Wen', 'Ths', 'Fri', 'Sat'];

    return days[dt.getDay()];
}

function fillSearchTab() {
    window.setTimeout(function () {
        $('#searchPanel').show();     
    }, 3000);
}

// Отметить все чекбоксы:
function checkSearchAll() {
    if ($("#calendarTab").hasClass("active")) {
        $(".chkDates").each(function () {
            $(this).prop('checked', true);
        });
    } else if ($("#channelTab").hasClass("active")) {
        $(".chkChannels").each(function () {
            $(this).prop('checked', true);
        });
    } else if ($("#genreTab").hasClass("active")) {
        $(".chkGenres").each(function () {
            $(this).prop('checked', true);
        });
    }
}

// Снять все флажки у чекбоксов:
function uncheckSearchAll() {
    if ($("#calendarTab").hasClass("active")) {
        $(".chkDates").each(function () {
            $(this).prop('checked', false);
        });
    } else if ($("#channelTab").hasClass("active")) {
        $(".chkChannels").each(function () {
            $(this).prop('checked', false);
        });
    } else if ($("#genreTab").hasClass("active")) {
        $(".chkGenres").each(function () {
            $(this).prop('checked', false);
        });
    }
}

// Инвертирование установки флажка:
function invertSearchAll() {
    if ($("#calendarTab").hasClass("active")) {
        $(".chkDates").each(function () {
            $(this).prop('checked', !$(this).is(":checked"));
        });
    } else if ($("#channelTab").hasClass("active")) {
        $(".chkChannels").each(function () {
            $(this).prop('checked', !$(this).is(":checked"));
        });
    } else if ($("#genreTab").hasClass("active")) {
        $(".chkGenres").each(function () {
            $(this).prop('checked', !$(this).is(":checked"));
        });
    }
}