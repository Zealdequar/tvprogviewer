var incSearch = 1;
let chansArr = [];
$(function () {
    $.jgrid.no_legacy_api = true;
    $.jgrid.useJSON = true;

    chansArr = getStorageChannels();
    fillGenresToolbarSearch();
    
    $("#anonsToolbarSearch").on('click', function () {
        $('#anonsDescrGlobalSearch').toggle(100);
    });

    $(".search-button").on('click', function () {
       globalSearchProgramme($('.search-text').val());
    });

    $("#dragToolbarGenreSearch").draggable();
});

// Поиск по всей программе передач
function globalSearchProgramme(findTitle) {
    if (findTitle.length == 0 || findTitle == "" || findTitle == null)
        return;

    if (incSearch == 1) {
        $('#TvProgrammeGlobalSearchGrid').jqGrid(
            {
                url: "Catalog/GlobalSearchProgramme?progType=1&findTitle=" + findTitle + "&category=" + $('#userCategory option:selected').val().split(';')[1] +
                    "&genres=" + GetGenres(".btn-genre-search.active") + "&channels=" + ((chansArr) ? chansArr.map(ch => ch.ChannelId).join(";") : ""),
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
                    $('#TvProgrammeGlobalSearchGrid').jqGrid('resetSelection');
                    return (true);
                },
                loadComplete: function () {
                    $(this).find("tr.jqgrow:odd").addClass("alt-green-background");

                    $("tr.jqgrow td input", "#TvProgrammeGlobalSearchGrid").on('click', function () {
                        if ($(this).closest('tr').find('td:nth-child(6)').find('img').length) {
                            $("#mainToolbarSearch").show(50);
                            $("#anonsToolbarSearch").show(50);
                            $("#anonsDescrGlobalSearch").html($(this).closest('tr').find('td:nth-child(13)').attr('title'));
                        }
                        else {
                            $("#mainToolbarSearch").hide(50);
                            $("#anonsToolbarSearch").hide(50);
                            $("#anonsDescrGlobalSearch").hide(150);
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
                pager: '#TvProgrammeGlobalSearchPager',
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
            }).navGrid('#TvProgrammeGlobalSearchPager',
                {
                    edit: false, add: false, del: false, search: false,
                    searchtext: "Поиск передачи", refresh: true
                },
                {
                    zIndex: 100,
                    caption: "Поиск передачи",
                    sopt: ['cn']
                });
        $("#TvProgrammeGlobalSearchGrid").jqGrid('hideCol', "GenreName");
        $("#TvProgrammeGlobalSearchGrid").jqGrid('hideCol', "RatingName");
        $("#TvProgrammeGlobalSearchGrid").jqGrid('hideCol', "RatingContent");
        $('#TvProgrammeGlobalSearchGrid').jqGrid('setGridWidth', $("#mwSearch").width());
        $("#TvProgrammeGlobalSearchGrid").jqGrid("setGridHeight", 548);
    }
    else {
        $("#TvProgrammeGlobalSearchGrid").setGridParam({
            url: "Catalog/GlobalSearchProgramme?progType=1&findTitle=" + findTitle + "&category=" + $('#userCategory option:selected').val().split(';')[1] +
                "&genres=" + GetGenres(".btn-genre-search.active") + "&channels=" + ((chansArr) ? chansArr.map(ch => ch.ChannelId).join(";") : "")
        });
        $("#TvProgrammeGlobalSearchGrid").trigger("reloadGrid");
    }
    incSearch++;
    $(".drag-tool-genre").width($("#mwSearch").width());
    window.setTimeout(function () { $('#genresToolbarSearch').show(); }, 3000);
}

function fillGenresToolbarSearch() {
    $('#genresToolbarSearch').hide();
    $.ajax({
        url: "Catalog/GetGenres",
        dataType: 'json',
        async: false,
        type: 'Get',
        contentType: 'application/json; charset=utf-8',
        success: function (response) {
            $('#genresToolbarSearch').empty();
            for (var i = 0; i < response.length; i++) {
                var b = $('<button id="' + response[i].GenreId + '" class="btn btn-default btn-genre-global-search">');

                $('#genresToolbarSearch').append(
                    b.html('<img src="' + response[i].GenrePath + '" title="' + response[i].GenreName + '" alt="' + response[i].GenreName + '" height="24px" width="24px">'));


            }
            $('.btn-group-genres-global-search').on('click', '.btn', function (e) {
                e.preventDefault();
                $(this).toggleClass("active");
                $("#TvProgrammeGlobalSearchGrid").setGridParam({
                    url: "Catalog/GlobalSearchProgramme?progType=1&findTitle=" + $('.search-text').val() +
                        "&category=" + $('#userCategory option:selected').val().split(';')[1] +
                        "&genres=" + GetGenres(".btn-genre-global-search.active") + "&channels=" + ((chansArr) ? chansArr.map(ch => ch.ChannelId).join(";") : ""),
                });
                $("#TvProgrammeGlobalSearchGrid").trigger("reloadGrid");
            });
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
    