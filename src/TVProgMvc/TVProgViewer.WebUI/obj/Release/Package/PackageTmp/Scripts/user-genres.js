$(function () {
    setGrid();
});

function UploadImage(options, postData, response) {
    var formData = new FormData();
    formData.append('file', $('#GenrePath')[0].files[0]);
    formData.append('GenreId', postData.GenreID);
    $.ajax(
        {
            url: '/Genre/UploadFile',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (data, status) {
                reload();
                setGrid();
            },
            error: function (data, status, e) {

            }
        }
    );
    return [true, '', false];
}

// Установка таблички
function setGrid()
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
                key: false, name: "UID", index: "UID", width: "0px", hidden: true, editable: true
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
        },
        rowList: [10, 20, 30, 40, 50, 100, 500, 1000],
        height: 'auto',
        width: "100%",
        autowidth: true,
        rownumbers: true,
        viewrecords: true,
        reloadAfterEdit: true,
        caption: 'Настройка жанров',
        emptyrecords: 'Жанры не обнаружены',
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
        afterSubmit: UploadImage
    }).navGrid('#userGenresPager',
        {
            edit: true, add: true, delete: true, search: true,
            searchtext: "Поиск жанра", refresh: true
        },
        {
            zIndex: 100,
            caption: "Поиск жанра",
            sopt: ['cn'],
            reloadAfterSubmit: true,
            closeAfterEdit: true,
           afterSubmit: UploadImage
        });
}