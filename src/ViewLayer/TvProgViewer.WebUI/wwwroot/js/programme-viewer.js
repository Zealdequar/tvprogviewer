var idx;
var pairKeys;
var incChannelByDay = 1;
let chansArr = [];

// Установка таблички под дням:
function fillProgrammeViewerByDay(date, channelId) {
    if (incChannelByDay == 1) {
        $("#TVProgrammeViewerGrid").jqGrid(
            {
                url: "/TvChannel/GetUserProgrammeOfDay?progTypeID=" + $('#userTypeProg option:selected').val().split(';')[1] + '&cid=' + channelId + '&tsDate=' +
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
                    $('#TVProgrammeViewerGrid').jqGrid('resetSelection');
                    return (true);
                },
                loadComplete: function () {
                    $(this).find("tr.jqgrow:odd").addClass("alt-green-background");

                    $("tr.jqgrow td input", "#TVProgrammeViewerGrid").on('click', function () {
                        if ($(this).closest('tr').find('td:nth-child(6)').find('img').length) {
                            $("#mainToolProgrammeViewer").show(50);
                            $("#anonsToolProgrammeViewer").show(50);
                            $("#anonsDescrProgrammeViewer").html($(this).closest('tr').find('td:nth-child(10)').attr('title'));
                        }
                        else {
                            $("#mainToolProgrammeViewer").hide(50);
                            $("#anonsToolProgrammeViewer").hide(50);
                            $("#anonsDescrProgrammeViewer").hide(150);
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
                pager: '#TVProgrammeViewerPager',
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
            }).navGrid('#TVProgrammeViewerPager',
                {
                    edit: false, add: false, del: false, search: false,
                    searchtext: "Поиск передачи", refresh: true
                },
                {
                    zIndex: 100,
                    caption: "Поиск передачи",
                    sopt: ['cn']
                });
        $("#TVProgrammeViewerGrid").jqGrid('hideCol', "GenreName");
        $("#TVProgrammeViewerGrid").jqGrid('hideCol', "RatingName");
        jQuery('#TVProgrammeViewerGrid').jqGrid('setGridWidth', $("#mwpv").width());
        jQuery("#TVProgrammeViewerGrid").jqGrid("setGridHeight", 548);
    } else {
        $("#TVProgrammeViewerGrid").setGridParam({
            url: "/TVChannel/GetUserProgrammeOfDay?progTypeID=" + $('#userTypeProg option:selected').val().split(';')[1] + '&cid=' + channelId + '&tsDate=' +
                date + "&category=" + $('#userCategory option:selected').val().split(';')[1]
        });
        $("#TVProgrammeViewerGrid").trigger("reloadGrid");
    }
    incChannelByDay++;
    jQuery("#TVProgrammeViewerGrid").jqGrid('setGridWidth', $("#mwpv").width());
    jQuery("#TVProgrammeViewerGrid").jqGrid("setGridHeight", 548);
    //$(".drag-tool-genre").width($("#mwpv").width());
    /*if (uv == 0) {
        jQuery("#TVProgrammeByDaysGrid").jqGrid('hideCol', "RatingContent");
    }*/
}

function formatDate(date) {
    let formattedDate = date.toLocaleDateString('ru-Ru', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
    });

    return formattedDate;
}