let checkedArray = [];
var pageNum = 1;
$(function () {
    setGrid();
    let jsonData = window.localStorage.getItem("optChans");
    let uuid = window.localStorage.getItem("_tv_uuid");
    if (jsonData && jsonData.length != 0) {
        try {
            checkedArray = JSON.parse(jsonData);
        }
        catch {
            checkedArray = [];
            window.localStorage.setItem("optChans", "[]");
        }
    }
    if (!uuid || uuid.length == 0) {
        window.localStorage.setItem("_tv_uuid", crypto.randomUUID());
    }

    if (!jsonData || !checkedArray || checkedArray.length == 0) {
        $("#infoPanel").show(300);
    } else {
        $("#infoPanel").hide(100);
    }
    $("#choicePnl").show();
});

function chAction() {
    let jsonData = window.localStorage.getItem("optChans");
    if (jsonData) {
        for (let i in jsonData) {
            checkedArray.push(jsonData[i]);
        }
        checkedArray = JSON.parse(window.localStorage.getItem("optChans"));
    }
    for (var i = 0; i < checkedArray.length; i++) {
        if (checkedArray[i].pageNum == pageNum) {
            $("#tblSystemChannelsGrid").jqGrid('setSelection', checkedArray[i].rowId, false);
        }
    }
    window.localStorage.setItem("optChans", "[]");
}
// Отправка статистических данных для создания пользователя
function sendStatForChannelRating() {
    let result;
    let chansArr = getStorageChannels();
    $.ajax({
        url: "Home/SendStatForChannelRating?uuid=" + window.localStorage.getItem("_tv_uuid") +
            "&progType=" + $('#userTypeProg option:selected').val().split(';')[1] +
            "&channels=" + ((chansArr) ? chansArr.map(ch => ch.ChannelId).join(";") : ""),
        dataType: 'json',
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        success: function (response) {
            result = "OK";
        },
        error: function (err) {
            result = err.statusText;
        }
    });
    return result;
}


    $("#ChannelTool").on('click', chAction);
    $("#ChannelToolBottom").on('click', chAction);
$("#ApplyTool").on('click', function () {
    sendStatForChannelRating();
    document.location.href = "/";
    });
$("#ApplyToolBottom").on('click', function () {
    sendStatForChannelRating();
    document.location.href = "/";
    });
// Заполнение раскрывающихся списков
/*
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
}*/

function UploadImage(response, postData) {
    var formData = new FormData();
    formData.append('file', $('#FileName25')[0].files[0]);
    formData.append('userChannelId', postData.UserChannelID);
    $.ajax({
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
    });
    return [true, '', false];
}
// Установка таблички
function setGrid()
{
    let jsonData = window.localStorage.getItem("optChans");
    if (jsonData) {
        for (let i in jsonData) {
            checkedArray.push(jsonData[i]);
        }
        checkedArray = JSON.parse(window.localStorage.getItem("optChans"));
    }
    var lastcell;
    $("#tblSystemChannelsGrid").jqGrid({
        url: "Home/GetSystemChannels?tvProgProvider=" + $('#userProvider option:selected').val().split(';')[1] + "&progType=" + $('#userTypeProg option:selected').val().split(';')[1],
        datatype: "json",
        mtype: "GET",
        success: function () { },
        error: function (e) {
        },
        colNames: ["", "", "Логотип", "Канал", "Номер канала"],
        colModel: [
            {
                key: true, name: "UserChannelID", index: "UserChannelID", width: "0px", hidden: true
            },
            {
                key: true, name: "ChannelId", index: "ChannelId", width: "0px", hidden: true
            },
            {
                key: false, name: "FileName25", index: "FileName25", sortable: false, width: "3%", align: "center", formatter: imgTvChannel,
                editable: true, edittype: 'file', editoptions: { enctype: "multipart/form-data" }, search: false
            },
            {
                key: false, name: "SystemTitle", index: "SystemTitle", sortable: true, width: "40%", editable: false
            },
            {
                key: false, name: "SysOrderCol", index: "SysOrderCol", sortable: true, width: "10%", align: "center", editable: true, hidden: true
            }
        ],
        rowNum: 20,
        loadComplete: function (data) {
            $(this).find("tr.jqgrow:odd").addClass("alt-green-background");
            // Изменение pageNum номера страницы глобальной переменной
            pageNum = data.page;
            // Цикл по массиву чтобы сделать rowId в выбранной странице
            if (checkedArray) {
                for (var i = 0; i < checkedArray.length; i++) {
                    //if (checkedArray[i].pageNum == pageNum) {
                        $("#tblSystemChannelsGrid").jqGrid('setSelection', checkedArray[i].rowId, true);
                   // }
                }
            }
            
            $(window).triggerHandler('resize.jqGrid');
            $("tr.jqgrow td input", "#tblSystemChannelsGrid").on('click', function () {
               // if ($(this).closest('tr').find('td:nth-child(4)').find('img').length) {
                $("#mainToolChannel").show(50);
                $("#ChannelTool").show(100);
                $("#ApplyTool").show(100);
                $("#mainToolChannelBottom").show(50);
                $("#ChannelToolBottom").show(100);
                $("#ApplyToolBottom").show(100);
                /*}
                else {
                    $("#ChannelTool").hide(50);
                    $("#mainToolChannel").hide(50);
                }*/
            });
        },
        rowList: [20],
        height: 'auto',
        width: null,
        autowidth: false,
        rownumbers: true,
        autoResizing: { minColWidth: 80 },
        viewrecords: true,
        multiselect: true,
        onSelectRow: function (rowId, status) {
            // Данные выбранной строки:
            let rowData = $('#tblSystemChannelsGrid').jqGrid('getRowData', rowId);
            let checkedItem = { "pageNum": pageNum, "rowId": rowId, "ChannelId": rowData.ChannelId };
            if (checkedArray) {
                if (status) {
                    // Если строка отмечена, массив будет дополнен если такого нет в массиве:
                    for (let i = 0; i < checkedArray.length; i++) {
                        if (checkedArray[i].rowId == rowId) {
                            return false;
                        }
                    };
                    var sel_id = $('#tblSystemChannelsGrid').getGridParam('selrow');
                    var index = $('#tblSystemChannelsGrid').jqGrid('getInd', sel_id);
                    // Если строка отмечена, массив будет дополнен если такого нет в массиве:
                    checkedArray.splice((pageNum - 1) * 20 + index - 1, 0, checkedItem);  
                } else {
                    for (let i = 0; i < checkedArray.length; i++) {
                        if (checkedArray[i].rowId == rowId) {
                            checkedArray.splice(i, 1);
                            break;
                        }
                    }
                    
                }
                $("#hiddenChannels").val(checkedArray.map(ch => ch.ChannelId).join(";"));
                window.localStorage.setItem("optChans", JSON.stringify(checkedArray));
            }
            $("#mainToolChannel").show(50);
            $("#ChannelTool").show(100);
            $("#ApplyTool").show(100);
            $("#mainToolChannelBottom").show(50);
            $("#ChannelToolBottom").show(100);
            $("#ApplyToolBottom").show(100);
        },
        onSelectAll: function (rowIds, status) {  // Возникает когда всё выбрано на страничке пейджинга
            // Удалить все страницы в массиве checkedArray
            for (var i = 0; i < rowIds.length; i++) {
                for (var j = 0; j < checkedArray.length; j++) {
                    if (checkedArray[j].pageNum == pageNum && checkedArray[j].rowId == rowIds[i]) {
                        checkedArray.splice(j, 1);
                        break;
                    }
                }
            }
            // Если выбрано, добавить все на текущей страничке пейджинга
            if (status) {
                for (var i = 0; i < rowIds.length; i++) {
                    var rowData = $("#tblSystemChannelsGrid").jqGrid('getRowData', rowIds[i]);
                    var checkedItem = { "pageNum": pageNum, "rowId": rowIds[i], "ChannelId": rowData.ChannelId};
                    checkedArray.push(checkedItem)
                }
            }
            $("#hiddenChannels").val(checkedArray.map(ch => ch.ChannelId).join(";"));
            window.localStorage.setItem("optChans", JSON.stringify(checkedArray));
        },
        /*onSelectRow: function (id) {
            if (id && id !== lastcell) {
                jQuery('#tblSystemChannelsGrid').jqGrid('restoreRow', lastcell);
                jQuery('#tblSystemChannelsGrid').jqGrid('editRow', id, true);
                lastcell = id;
            }
        },*/ 
        caption: 'Настройка телеканалов для отображения программы передач | Выберите необходимые телеканалы | Обновите страницу или щёлкните на кнопке «Применить» после выбора',
        emptyrecords: 'Телеканалы не обнаружены',
        /*formEditing: {
            width: 400,
            zIndex: 1000,
            closeOnEscape: true,
            closeAfterEdit: true,
            reloadAfterSubmit: true,
            afterSubmit: function (response, postData) {
                UploadImage(response, postData);
                $(this).jqGrid("setGridParam", { datatype: 'json' });
                return [true];
            }
        },*/
        pager: '#systemChannelsPager',
        //editurl: '/Channel/UpdateChannel?tvProgProviderID=' + $('#TVProgProvider option:selected').val(),
        loadonce: false,
        reloadAfterSubmit: true,
        afterSubmit: UploadImage,
        onInitializeForm: function (formid) {
            $(formid).attr('method', 'POST');
            $(formid).attr('action', '');
            $(formid).attr('enctype', 'multipart/form-data');
        }
    }).navGrid('#systemChannelsPager',
        {
            edit: false, add: false, del: false, search: false,
            searchtext: "Поиск телеканала", refresh: true
        },
        {
            zIndex: 100,
            width: 400,
            caption: "Поиск телеканала",
            sopt: ['cn'],
            reloadAfterSubmit: true,
            closeAfterEdit: true
           // afterSubmit: UploadImage
        });
    jQuery("#tblSystemChannelsGrid").jqGrid('filterToolbar', { stringResult: true, searchOnEnter: false, defaultSearch: "cn" });
    jQuery('#tblSystemChannelsGrid').jqGrid('setGridWidth', $("#mw4").width());
    jQuery("#tblSystemChannelsGrid").jqGrid("setGridHeight", 548);
    $("input#gs_SystemTitle").attr("placeholder", "Начните вводить телеканал...");
    $("input#gs_SystemTitle").height(25);
    $("input#gs_SystemTitle").trigger("focus");
}

// Перезагрузка после подтверждения
function reload() {
    $("#tblSystemChannelsGrid").trigger("reloadGrid");
}

// Пиктограмма для каналов
function imgTvChannel(cellvalue, options, rowObject) {
    if (cellvalue)
        return getImgTag(cellvalue, "Эмблема телеканала " + rowObject.SystemTitle);
    return "<img src='/images/i/satellite_25.png' alt='Эмблема телеканала' height='25px' width='25px' />";
}

//Получение тега пиктограммы
function getImgTag(s, str) {
    return "<img src='" + s + "' alt='" + str + "' height='25px' width='25px' />";
}