$('#ParentRow > td').hover(function () {

    if ($(this).find("th").length > 0) return;
    $(this).addClass("gridRowHover");
},
    function () { $(this).removeClass("gridRowHover"); }
);

$("table").click(function (event) {

    if (window.getSelection().type == "Caret") {

        event.stopPropagation();

        if (event.ctrlKey == false) {
            var $start = $(event.target);

            var $target = $(event.target).closest('tr').next('tr');

            // This checks that only the top row is clickable to avoid hiding reports
            if ($start.closest('tr').attr('id') == "toprow") {

                // Checks to see if the child rows are visible
                if ($start.closest('tr').next('tr').is(":visible")) {

                    $target.slideUp("fast");
                }
                else {

                    $target.slideToggle("fast");
                }
            }
        }
    }
});


function SubmitImport(requestUrl, callbackUrl) {

    var element = document.getElementById('importSelect');
    var importTypeVal = element.options[element.selectedIndex].text;

    var file = document.getElementById('file');
    var formData = new FormData();
    var fileData = file.files[0];
    formData.append('file', fileData);
    formData.append('source', importTypeVal);

    $.ajax({
        url: requestUrl,
        processData: false,
        contentType: false,
        data: formData,
        type: "POST",
        success: function (response) {

            if (response) {
                alert("Import succeded!");

                window.location.href = callbackUrl;
            }
            else if (!response) {
                alert("Import failed!");
            }
            else {
                alert("Error!");
            }
        }
    });
};


function SubmitExport(url) {

    var element = document.getElementById('exportSelect');
    var exportTypeVal = element.options[element.selectedIndex].text;
    var searchVal = document.getElementById("search").value;

    var rowItems = $(".SelectedRow");
    var selection = "";

    for (var i = 0; i < rowItems.length; i++) {

        selection += rowItems[i].cells[2].innerHTML + " ";
    }

    var str = url
        .replace("_exportType_", exportTypeVal)
        .replace("_searchValue_", searchVal)
        .replace("_selection_", selection);

    window.location = str;
};

// Prevents doubleclicking from selecting text in table
function PreventDoubleClick() {

    var table = document.getElementById('table');
    table.addEventListener('mousedown', function (event) {

        if (event.detail > 1 && event.ctrlKey == false) {

            event.preventDefault();
        }
        // Ctrl click selecting rows for delete/export
        else if (event.ctrlKey) {

            event.preventDefault();
            var $target = $(event.target).closest('tr');

            if ($target.hasClass("SelectedRow")) {
                $target.removeClass("SelectedRow");
            }
            else {
                $target.addClass("SelectedRow");
            }

            var rowItems = $('.SelectedRow');

            if (rowItems.length > 0) {

                document.getElementById("selectionButtons").style.display = "";
            }
            else {
                document.getElementById("selectionButtons").style.display = "none";
            }
        }
    }, false);



    $("#selectionExport").click(function () {

        $.getScript("/js/ConfirmationDialogue.js", function () {
            ConfirmClickGeneric(Export, '#exportDialog');
        });
    });


    $("#selectionDelete").click(function (event) {


        var table = document.getElementById("deleteTable");
        var rowItems = $(".SelectedRow");

        for (var i = 0; i < rowItems.length; i++) {

            var model = rowItems[i].cells[1].innerHTML;
            var serial = rowItems[i].cells[2].innerHTML;
            var owner = rowItems[i].cells[3].innerHTML;

            var row = document.createElement("tr");
            var cell = document.createElement("td");

            cell.innerHTML = model;
            row.append(cell);
            cell = document.createElement("td");
            cell.innerHTML = serial;
            row.append(cell);
            cell = document.createElement("td");
            cell.innerHTML = owner;
            row.append(cell);

            table.appendChild(row);
        }

        $.getScript("/js/ConfirmationDialogue.js", function () {
            ConfirmClickGeneric(Delete, '#deleteDialog', 900);
        });

    });
};









