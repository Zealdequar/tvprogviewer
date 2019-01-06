var ratingsForSelect;
var ratings;
var idx;

$(function () {
    $("#btnUp").click(function () {
        var sel_id = $("#tblClassifRatings").getGridParam('selrow');
        var cellValue = $("#tblClassifRatings").jqGrid('getCell', sel_id, 'RatingClassificatorID');
        upElem(cellValue);
    });
    $("#btnDown").click(function () {
        var sel_id = $("#tblClassifRatings").getGridParam('selrow');
        var cellValue = $("#tblClassifRatings").jqGrid('getCell', sel_id, 'RatingClassificatorID');
        downElem(cellValue);
    });
    $("#ratingTabs").tabs({
        activate: function (event, ui) {
            idx = ui.newTab.index()

            if (idx === 1) {
                GetRatingsForSelect();
                $("#tblClassifRatings").jqGrid("GridUnload");
                setGrids();
            }
        }
    });
    GetRatingsForSelect();
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

function GetRatingsForSelect() {
    $.ajax({
        url: "/Rating/GetRatings",
        method: "GET",
        dataType: "json",
        async: false,
        success: function (data) {
            var arr = '{';
            ratings = data;
            for (var i in data) {
                var gid = data[i].RatingID;
                var gn = data[i].RatingName;
                arr += '"' + gid + '": "' + gn + '", ';
            }
            arr = arr.substring(0, arr.length - 2);
            arr += '}';
            ratingsForSelect = $.parseJSON(arr);
        }
    });
}

function upElem(ratingClassificatorId) {
    $.ajax({
        url: '/Rating/UpRatingClassificateElem',
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        data: "{'ratingClassificatorId':'" + ratingClassificatorId + "'}",
        success: function (data, status) {
            reload();
            setGrids();
            setTimeout(function () {
                $("#tblClassifRatings").jqGrid('setSelection', ratingClassificatorId, true);
                var rowHeight = 25;
                var sel_id = $("#tblClassifRatings").getGridParam('selrow');
                var index = $("#tblClassifRatings").getInd(sel_id);
                $("#classifDiv").scrollTop(rowHeight * index);
            }, 500);


        }
    });
}

function downElem(ratingClassificatorId) {
    $.ajax({
        url: '/Rating/DownRatingClassificateElem',
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        data: "{'ratingClassificatorId':'" + ratingClassificatorId + "'}",
        success: function (data, status) {
            reload();
            setGrids();
            setTimeout(function () {
                $("#tblClassifRatings").jqGrid('setSelection', ratingClassificatorId, true);
                var rowHeight = 25;
                var sel_id = $("#tblClassifRatings").getGridParam('selrow');
                var index = $("#tblClassifRatings").getInd(sel_id);
                $("#classifDiv").scrollTop(rowHeight * index);
            }, 500);
        }
    });
}

function UploadImage(options, postData, response) {
    var formData = new FormData();
    formData.append('file', $('#RatingPath')[0].files[0]);
    formData.append('RatingId', postData.id);
    $.ajax(
        {
            url: '/Rating/UploadFile',
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
function setGrids() {
    $('#tblUserRatings').jqGrid({
        url: "/Rating/GetRatings",
        datatype: "json",
        mtype: "Get",
        success: function () { },
        colNames: ["", "", "Вкл./Выкл.", "Пик.", "Название"],
        colModel: [
            {
                key: true, name: "RatingID", index: "RatingID", width: "0px", hidden: true, editable: true
            },
            {
                key: false, name: "UID", index: "UID", width: "0px", hidden: true, editable: false
            },
            {
                key: false, name: "Visible", index: "Visible", sortable: true, width: "3%", align: "center",
                formatter: 'checkbox', formatoptions: { disabled: false }, editable: true
            },
            {
                key: false, name: "RatingPath", index: "RatingPath", sortable: false, width: "3%", align: "center", formatter: function (s) {
                    if (s) {
                        if (s.indexOf('рейтинга') === -1)
                            return "<img src='" + s + "' alt='Картинка рейтинга' />";
                        return s;
                    }
                    return "<img src='/imgs/system/rating/untype.png' alt='Картинка рейтинга' />";
                }, editable: true, edittype: 'file', editoptions: { enctype: "multipart/form-data" }, search: false
            },
            {
                key: false, name: "RatingName", index: "RatingName", sortable: true, width: "94%", editable: true
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
        emptyrecords: 'Рейтинги не обнаружены',
        loadonce: true,
        jsonReader:
            {
                repeatitems: false,
                root: function (obj) { return obj; },
                page: function (obj) { return 1; },
                total: function (obj) { return obj.rowNum / obj.height; },
                records: function (obj) { return obj.length; }
            },
        pager: '#userRatingsPager',
        loadonce: true,
        reloadAfterSubmit: true,
        editurl: "/Rating/UpdateRating",
        afterSubmit: UploadImage
    }).navGrid('#userRatingsPager',
        {
            edit: true, add: true, delete: true, search: true,
            refresh: true,
            searchtext: "Поиск рейтинга",
            addtext: "Добавить",
            edittext: "Изменить",
            deltext: "Удалить",
            refreshtext: "Обновить"
        },
        {
            zIndex: 100,
            width: 400,
            caption: "Поиск рейтинга",
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
            url: "/Rating/AddRating",
            afterSubmit: UploadImage
        },
        {
            url: "/Rating/DeleteRating",
            mtype: "POST",
            reloadAfterSubmit: true,
            serializeDelData: function (postdata) {
                return { ratingId: postdata.id };
            }
        });

    $('#tblClassifRatings').jqGrid({
        url: "/Rating/GetRatingClassificators",
        datatype: "json",
        mtype: "Get",
        success: function () { },
        colNames: ["", "Тип рейтинга", "", "", "Пик.", "Содержит", "Не содержит", "", "Удалить после", "Действия"],
        colModel: [
            {
                key: true, name: "RatingClassificatorID", index: "RatingClassificatorID", width: "0px", hidden: true, editable: true
            },
            {
                key: false, name: "RID", index: "RID", width: "-10px", hidden: false, editable: true,
                formatter: "select",
                edittype: "select",
                editoptions: {
                    value: ratingsForSelect,
                    dataInit: function (elem) {
                        $(elem).iconselectmenu()
                            .iconselectmenu("menuWidget")
                            .addClass("ui-menu-icons customicons");
                        for (var i in ratings)
                            $(elem).find('option').each(function () {
                                if (ratings[i].RatingID == $(this).val()) {
                                    $(this).attr("data-class", "avatar");
                                    $(this).attr("data-style", "background-image: url('" + ratings[i].RatingPath + "')");
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
                key: false, name: "RatingName", index: "RatingName", width: "-10px", hidden: true, editable: true
            },
            {
                key: false, name: "RatingPath", index: "RatingPath", sortable: false, width: "50px", align: "center", formatter: function (s) {
                    if (s) {
                        if (s.indexOf('рейтинга') === -1)
                            return "<img src='" + s + "' alt='Картинка рейтинга' />";
                        return s;
                    }
                    return "<img src='/imgs/system/rating/untype.png' alt='Картинка рейтинга' />"
                }, cellattr: function (rowId, val, rawObject, cm, rdata) {
                    return 'title="' + rawObject.RatingName + '"';
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
                name: 'Actions', index: 'Actions', width: 100,editable: false, sortable: false, align: "center", fixed: true, hidedlg: true,
                resizeable: false, search: false, viewable: false, template: 'actions', formatoptions: {
                    keys: true, 
                    editformbutton: true,
                    delbutton: true,
                    delOptions: {
                        url: "/Rating/DeleteRatingClassificator",
                        mtype: "POST",
                        serializeDelData: function (postdata) {
                            return { ratingClassificatorId: postdata.id };
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
        caption: 'Классификатор (рейтингов) передач',
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
        pager: '#classifRatingsPager',
        closeAfterEdit: true,
        reloadAfterEdit: true,
        reloadAfterSubmit: true,
        editurl: "/Rating/UpdateRatingClassificator",
        loadonce: true,
        ondblClickRow: function (rowid) {
            $(this).jqGrid("editGridRow", rowid);
        },
    }).navGrid('#classifRatingsPager',
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
            url: "/Rating/AddRatingClassificator",
            afterSubmit: function () {
                $(this).jqGrid("setGridParam", { datatype: 'json' });
                return [true];
            },
            afterShowForm: dlgRevisible
        },
        {
            url: "/Rating/DeleteRatingClassificator",
            mtype: "POST",
            serializeDelData: function (postdata) {
                return { ratingClassificatorId: postdata.id };
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
    jQuery("#tblClassifRatings").jqGrid('hideCol', "RatingName");
}


function dlgRevisible($form) {
    var dialog = $form.closest('div.ui-jqdialog'),
        selRowCoordinates = $("#tblUserRatings").find(">tbody>tr.jqgrow").filter(":last").offset();
    dialog.offset(selRowCoordinates);
}
// Перезагрузка после подтверждения
function reload() {
    $("#tblUserRatings").jqGrid("GridUnload");
    $("#tblClassifRatings").jqGrid("GridUnload");
}