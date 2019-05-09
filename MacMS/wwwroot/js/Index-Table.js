$('#ParentRow > td').hover(function () {
    if ($(this).find("th").length > 0) return;
    $(this).addClass("gridRowHover");
},
    function () { $(this).removeClass("gridRowHover"); }
);

$("table").click(function (event) {
        
    if (window.getSelection().type == "Caret") {

        event.stopPropagation();
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

});

$('document').ready(function () {

    if ('@TempData["Success"]' == false) {

        alert("Error! Form was not submitted successfully");
    }
});
