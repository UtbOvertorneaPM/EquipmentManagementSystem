
function ConfirmClick(e, callBack) {

    e.preventDefault();
    $("#confirmDialog").dialog({
        autoOpen: false,
        modal: true,
        resizable: false,
        buttons: {
            "Confirm": function () {
                callBack();
            },
            "Cancel": function (e) {
                $(this).dialog("close");
            }
        },
    });

    $('#confirmDialog').dialog("open");
}


function ConfirmClickGeneric(callBack, dialogId, widthVal = 300) {

    $(dialogId).dialog({
        autoOpen: false,
        modal: true,
        resizable: false,
        width: widthVal,
        buttons: {
            "Confirm": function () {
                callBack();
            },
            "Cancel": function (e) {
                $(this).dialog("close");
            }
        },
    });

    $(dialogId).dialog("open");
}

