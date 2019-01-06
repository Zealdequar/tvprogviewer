var genresForSelect;
var genres;
var idx;

$(function () {
    $("#btnUp").click(function () {
        var sel_id = $("#tblClassifGenres").getGridParam('selrow');
        var cellValue = $("#tblClassifGenres").jqGrid('getCell', sel_id, 'GenreClassificatorID');
        upElem(cellValue);
    });
    $("#btnDown").click(function () {
        var sel_id = $("#tblClassifGenres").getGridParam('selrow');
        var cellValue = $("#tblClassifGenres").jqGrid('getCell', sel_id, 'GenreClassificatorID');
        downElem(cellValue);
    });
    $("#genreTabs").tabs({
        activate: function (event, ui) {
            idx = ui.newTab.index()

            if (idx === 1) {
                GetGenresForSelect();
                $("#tblClassifGenres").jqGrid("GridUnload");
                setGrids();
            }
        }
    });
    GetGenresForSelect();
    $.widget("custom.iconselectmenu", $.ui.selectmenu, {
        _renderItem: function (ul, item) {
            var li = $("<li>"),
                wrapper = $("<div>", { text: item.label });

            if (item.disabled) {
                li.addClass("ui-state-disabled");
            }

            $("<span>", {
                style: item.element.attr("data-style"),
                "class": "ui-icon " + item.element.attr("data-class")
            }).appendTo(wrapper);

            return li.append(wrapper).appendTo(ul);
        }
    });
    setGrids();
});

function GetGenresForSelect() {
    $.ajax({
        url: "/Genre/GetGenres",
        method: "GET",
        dataType: "json",
        async: false,
        success: function (data) {
            var arr = '{';
            genres = data;
            for (var i in data) {
                var gid = data[i].GenreID;
                var gn = data[i].GenreName;
                arr += '"' + gid + '": "' + gn + '", ';
            }
            arr = arr.substring(0, arr.length - 2);
            arr += '}';
            genresForSelect = $.parseJSON(arr);
        }
    });
}

function upElem(genreClassificatorId) {
    $.ajax({
        url: '/Genre/UpGenreClassificateElem',
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        data: "{'genreClassificatorId':'" + genreClassificatorId + "'}",
        success: function (data, status) {
            reload();
            setGrids();
            setTimeout(function () {
                $("#tblClassifGenres").jqGrid('setSelection', genreClassificatorId, true);
                var rowHeight = 25;
                var sel_id = $("#tblClassifGenres").getGridParam('selrow');
                var index = $("#tblClassifGenres").getInd(sel_id);
                $("#classifDiv").scrollTop(rowHeight * index);
            }, 500);

            
        }
    });
}

function downElem(genreClassificatorId) {
    $.ajax({
        url: '/Genre/DownGenreClassificateElem',
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        data: "{'genreClassificatorId':'" + genreClassificatorId + "'}",
        success: function (data, status) {
            reload();
            setGrids();
            setTimeout(function () {
                $("#tblClassifGenres").jqGrid('setSelection', genreClassificatorId, true);
                var rowHeight = 25;
                var sel_id = $("#tblClassifGenres").getGridParam('selrow');
                var index = $("#tblClassifGenres").getInd(sel_id);
                $("#classifDiv").scrollTop(rowHeight * index);
            }, 500);
        }
    });
}

function UploadImage(options, postData, response) {
    var formData = new FormData();
    formData.append('file', $('#GenrePath')[0].files[0]);
    formData.append('GenreId', postData.id);
    $.ajax(
        {
            url: '/Genre/UploadFile',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (data, status) {
                reload();
                setGrids();
            },
            error: function (data, status, e) {
                var int = 0;
            }
        }
    );
    return [true, '', false];
}

// Установка таблички
function setGrids()
{
    $('#tblUserGenres').jqGrid({
        url: "/Genre/GetGenres",
        datatype: "json",
        mtype: "Get",
        success: function () { },
        colNames: ["", "", "Вкл./Выкл.", "Пик.", "Название"],
        colModel: [
            {
                key: true, name: "GenreID", index: "GenreID", width: "0px", hidden: true, editable: true
            },
            {
                key: false, name: "UID", index: "UID", width: "0px", hidden: true, editable: false
            },
            {
                key: false, name: "Visible", index: "Visible", sortable: true, width: "3%", align: "center",
                formatter: 'checkbox', formatoptions: { disabled: false }, editable: true
            },
            {
                key: false, name: "GenrePath", index: "GenrePath", sortable: false, width: "3%", align: "center", formatter: function (s) {
                    if (s) {
                        if (s.indexOf('жанра') === -1)
                            return "<img src='" + s + "' alt='Картинка жанра' />";
                        return s;
                    }
                    return "<img src='/imgs/system/genre/untype.png' alt='Картинка жанра' />";
                }, editable: true, edittype: 'file', editoptions: { enctype: "multipart/form-data" }, search: false
            },
            {
                key: false, name: "GenreName", index: "GenreName", sortable: true, width: "94%", editable: true
            }
        ],
        rowNum: 20,
        loadComplete: function () {
            $("tr.jqgrow:odd").css("background", "#EFFFEF");
            $("tr.jqgrow:odd").css("z-index", -100);
        },
        rowList: [10, 20, 30, 40, 50, 100, 500, 1000],
        height: 'auto',
        width: "100%",
        autowidth: true,
        rownumbers: true,
        viewrecords: true,
        reloadAfterEdit: true,
        caption: 'Типы передач',
        emptyrecords: 'Жанры не обнаружены',
        loadonce: true,
        jsonReader:
            {
                repeatitems: false,
                root: function (obj) { return obj; },
                page: function (obj) { return 1; },
                total: function (obj) { return obj.rowNum / obj.height; },
                records: function (obj) { return obj.length; }
            },
        pager: '#userGenresPager',
        loadonce: true,
        reloadAfterSubmit: true,
        editurl: "/Genre/UpdateGenre",
        afterSubmit: UploadImage
    }).navGrid('#userGenresPager',
        {
            edit: true, add: true, delete: true, search: true,
            refresh: true,
            searchtext: "Поиск жанра",
            addtext: "Добавить",
            edittext: "Изменить",
            deltext: "Удалить",
            refreshtext: "Обновить"
        },
        {
            zIndex: 100,
            width: 400,
            caption: "Поиск жанра",
            sopt: ['cn'],
            reloadAfterSubmit: true,
            closeAfterEdit: true,
            afterSubmit: UploadImage
        },
        {
            zIndex: 100,
            closeAfterAdd: true,
            width: 400,
            reloadAfterSubmit: true,
            url: "/Genre/AddGenre",
            afterSubmit: UploadImage
        },
        {
            url: "/Genre/DeleteGenre",
            mtype: "POST",
            reloadAfterSubmit: true,
            serializeDelData: function (postdata) {
               return { genreId: postdata.id };
            }
        });
    $('#tblClassifGenres').jqGrid({
        url: "/Genre/GetGenreClassificators",
        datatype: "json",
        mtype: "Get",
        success: function () { },
        colNames: ["", "Тип жанра", "", "", "Пик.", "Содержит", "Не содержит", "", "Удалить после", "Действия"],
        colModel: [
            {
                key: true, name: "GenreClassificatorID", index: "GenreClassificatorID", width: "0px", hidden: true, editable: true
            },
            {
                key: false, name: "GID", index: "GID", width: "-10px", hidden: false, editable: true,
                formatter: "select",
                edittype: "select",
                editoptions: {
                    value: genresForSelect,
                    dataInit: function (elem) {
                        $(elem).iconselectmenu()
                            .iconselectmenu("menuWidget")
                            .addClass("ui-menu-icons customicons");
                        for (var i in genres)
                            $(elem).find('option').each(function () {
                                if (genres[i].GenreID == $(this).val()) {
                                    $(this).attr("data-class", "avatar");
                                    $(this).attr("data-style", "background-image: url('" + genres[i].GenrePath + "')");
                                    return false;
                                };
                            });
                    }
                }
            },
            {
                key: false, name: "UID", index: "UID", width: "0px", hidden: true, editable: false
            },
            {
                key: false, name: "GenreName", index: "GenreName", width: "-10px", hidden: true, editable: true
            },
            {
                key: false, name: "GenrePath", index: "GenrePath", sortable: false, width: "50px", align: "center", formatter: function (s) {
                    if (s) {
                        if (s.indexOf('жанра') === -1)
                            return "<img src='" + s + "' alt='Картинка жанра' />";
                        return s;
                    }
                    return "<img src='/imgs/system/genre/untype.png' alt='Картинка жанра' />"
                }, cellattr: function (rowId, val, rawObject, cm, rdata) {
                    return 'title="' + rawObject.GenreName + '"';
                }, editable: false, search: false
            },
            {
                key: false, name: "ContainPhrases", index: "ContainPhrases", sortable: true, width: "200px", editable: true
            },
            {
                key: false, name: "NonContainPhrases", index: "NonContainPhrases", sortable: true, width: "200px", editable: true
            },
            {
                key: false, name: "OrderCol", index: "OrderCol", width: "0%", hidden: true, editable: false
            },
            {
                key: false, name: "DeleteAfterDate", index: "DeleteAfterDate", width: "200px", formatter: "date", hidden: false, editable: true,
                edittype: 'text',
                editoptions: {
                    size: 10,
                    maxlenght: 10,
                    dataInit: function (element) {
                        $(element).datepicker({ dateFormat: 'dd.mm.yy' })
                    }
                }
            },
            {
                name: 'Actions', index: 'Actions', width: 100, editable: false, sortable: false, align: "center", fixed: true, hidedlg: true,
                resizeable: false, search: false, viewable: false, template: 'actions', formatoptions: {
                    keys: true,
                    editformbutton: true,
                    delbutton: true,
                    delOptions: {
                        url: "/Genre/DeleteGenreClassificator",
                        mtype: "POST",
                        serializeDelData: function (postdata) {
                            return { genreClassificatorId: postdata.id };
                        },
                        afterShowForm: dlgRevisible
                    }
                }
            }
        ],         
        afterSubmit: function () {
            $(this).jqGrid("setGridParam", { datatype: 'json' });
            return [true];
        },
        rowNum: 999999999,
        loadComplete: function () {
            $("tr.jqgrow:odd").css("background", "#EFFFEF");
        },
        rowList: [10, 20, 30, 40, 50, 100, 500, 1000],
        height: 'auto',
        autoResizing: { minColWidth: 80 },
        rownumbers: true,
        viewrecords: true,
        caption: 'Классификатор (типов) передач',
        emptyrecords: 'Элементы классификатора не обнаружены',
        formEditing: {
            width: 400,
            zIndex: 100,
            closeOnEscape: true,
            closeAfterEdit: true,
            reloadAfterSubmit: true,
            afterSubmit: function () {
                $(this).jqGrid("setGridParam", { datatype: 'json' });
                return [true];
            },
        },
        jsonReader:
            {
                repeatitems: false,
                root: function (obj) { return obj; },
                page: function (obj) { return 1; },
                total: function (obj) { return obj.rowNum / obj.height; },
                records: function (obj) { return obj.length; }
            },
        pager: '#classifGenresPager',
        closeAfterEdit: true,
        reloadAfterEdit: true,
        reloadAfterSubmit: true,
        editurl: "/Genre/UpdateGenreClassificator",
        afterSubmit: function () {
            $(this).jqGrid("setGridParam", { datatype: 'json' });
            return [true];
        },
        loadonce: true,
        ondblClickRow: function (rowid) {
            $(this).jqGrid("editGridRow", rowid);
        },
    }).navGrid('#classifGenresPager', 
        {
            edit: true, add: true, delete: true, search: true,
            refresh: true,
            searchtext: "Поиск элемента",
            addtext: "Добавить",
            edittext: "Изменить",
            deltext: "Удалить",
            refreshtext: "Обновить",
        },
        {
            zIndex: 100,
            width: 400,
            caption: "Поиск элемента",
            sopt: ['cn'],
            reloadAfterSubmit: true,
            closeAfterEdit: true,
            afterSubmit: function () {
                $(this).jqGrid("setGridParam", { datatype: 'json' });
                return [true];
            },
            afterShowForm: dlgRevisible
        },
        {
            zIndex: 100,
            closeAfterAdd: true,
            width: 400,
            url: "/Genre/AddGenreClassificator",
            afterSubmit: function () {
                $(this).jqGrid("setGridParam", { datatype: 'json' });
                return [true];
            },
            afterShowForm: dlgRevisible
        },
        {
            url: "/Genre/DeleteGenreClassificator",
            mtype: "POST",
            serializeDelData: function (postdata) {
                return { genreClassificatorId: postdata.id };
            },
            afterShowForm: dlgRevisible
        },
        {
            left: "40%",
            top: "30%",
            closeAfterSearch: true,
            afterShowForm: dlgRevisible
        },
        {});
    jQuery("#tblClassifGenres").jqGrid('hideCol', "GenreName");
}


function dlgRevisible($form) {
    var dialog = $form.closest('div.ui-jqdialog'),
    selRowCoordinates = $("#tblUserGenres").find(">tbody>tr.jqgrow").filter(":last").offset();
    dialog.offset(selRowCoordinates);
}
// Перезагрузка после подтверждения
function reload() {
    $("#tblUserGenres").jqGrid("GridUnload");
    $("#tblClassifGenres").jqGrid("GridUnload");
}