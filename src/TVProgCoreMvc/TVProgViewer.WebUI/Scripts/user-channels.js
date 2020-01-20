$(function() {
    fillSelects();
    setGrid();
    $('#TVProgProvider').change(function () {
        fillSelects();
        $('#tblUserChannels').jqGrid('GridUnload');
        setGrid();
    });
    $('#TVProgType').change(function () {
        $('#tblUserChannels').jqGrid('GridUnload');
        setGrid();
    });
    $("#choicePnl").show();
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
    });
}

function UploadImage(response, postData) {
    var formData = new FormData();
    formData.append('file', $('#FileName25')[0].files[0]);
    formData.append('userChannelId', postData.UserChannelID);
    $.ajax(
        {
            url: '/Channel/UploadFile',
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
     var lastcell;
     $("#tblUserChannels").jqGrid({
        url: "/Channel/GetUserChannels?tvProgProvider=" + $('#TVProgProvider option:selected').val() +"&progType=" + $('#TVProgType option:selected').val(),
        datatype: "json",
        mtype: "Get",
        success: function () { },
        colNames: ["","","Вкл./Выкл.", "Логотип", "Канал", "Пользовательское название", "Номер канала", "Сдвиг по времени", "Действие"],
        colModel: [
            {
                key: true, name: "UserChannelID", index: "UserChannelID", width: "0px", hidden: true, editable: true 
            },
            {
                key: true, name: "ChannelID", index: "ChannelID", width: "0px", hidden: true, editable: true
            },
            {
                key: false, name: "Visible", index: "Visible", sortable: true, width: "3%", align: "center",
                formatter: 'checkbox', formatoptions: { disabled: false }, editable: true
            },
            {
                key: false, name: "FileName25", index: "FileName25", sortable: false, width: "3%", align: "center", formatter: function (s) {
                    if (s) {
                        if (s.indexOf('Логотип') === -1)
                            return "<img src='" + s + "' alt='Логотип канала' />";
                        else return s;
                    } 
                    return "<img src='/imgs/i/satellite_25.png' alt='Логотип канала' />";
                }, editable: true, edittype: 'file', editoptions: { enctype: "multipart/form-data"}, search:false
            },
            {
                key: false, name: "SystemTitle", index: "SystemTitle", sortable: true, width: "20%", editable: false
            },
            {
                key: false, name: "UserTitle", index: "UserTitle", sortable: true, width: "20%", editable: true
            },
            {
                key: false, name: "OrderCol", index: "OrderCol", sortable: true, width: "10%", align: "center", editable: true
            },
            {
                key: false, name: "Diff", index: "Diff", sortable: true, width: "10%", align: "center", editable: true
            },
            {  
                name: 'Actions', index: 'Actions', width: 100, editable: false, sortable: false, align: "center", fixed: true, hidedlg: true,
                resizeable: false, search: false, viewable: false, template: 'actions', formatoptions: {
                    keys: true,
                    editformbutton: true,
                    delbutton: false
                }
            }],
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
        multiselect: false, 
        onSelectRow: function (id) {
            if (id && id !== lastcell) {
                jQuery('#tblUserChannels').jqGrid('restoreRow', lastcell);
                jQuery('#tblUserChannels').jqGrid('editRow', id, true);
                lastcell = id;
            }
        }, 
        caption: 'Настройка телеканалов',
        emptyrecords: 'Телеканалы не обнаружены',
        formEditing: {
            width: 400,
            zIndex: 100,
            closeOnEscape: true,
            closeAfterEdit: true,
            reloadAfterSubmit: true,
            afterSubmit: function (response, postData) {
                UploadImage(response, postData);
                $(this).jqGrid("setGridParam", { datatype: 'json' });
                return [true];
            }
        },
        jsonReader:
            {
                repeatitems: false,
                root: function (obj) { return obj; },
                page: function (obj) { return 1; },
                total: function (obj) { return obj.rowNum / obj.height; },
                records: function (obj) { return obj.length; }
            },
        pager: '#userChannelsPager',
        editurl: '/Channel/UpdateChannel?tvProgProviderID=' + $('#TVProgProvider option:selected').val(),
        loadonce: true,
        reloadAfterSubmit: true,
         afterSubmit: UploadImage,
         onInitializeForm: function (formid) {
             $(formid).attr('method', 'POST');
             $(formid).attr('action', '');
             $(formid).attr('enctype', 'multipart/form-data');
         }
    }).navGrid('#userChannelsPager',
        {
            edit: true, add: false, del: false, search: true,
            searchtext: "Поиск телеканала", refresh: true
        },
        {
            zIndex: 100,
            width: 400,
            caption: "Поиск телеканала",
            sopt: ['cn'],
            reloadAfterSubmit: true,
            closeAfterEdit: true,
            afterSubmit: UploadImage
        });
    
}

// Перезагрузка после подтверждения
function reload(result) {
    $("#tblUserChannels").jqGrid("GridUnload");
}
