
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


function ConfirmClickGeneric(callBack, dialogId) {

    $(dialogId).dialog({
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

    $(dialogId).dialog("open");
}

