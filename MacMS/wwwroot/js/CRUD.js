function RequestCreateOwnerModal(url) {

    var phElement = $('#modal-placeholder');

    $.get(url).done(function (data) {

        phElement.html(data);
        phElement.find('.modal').modal('show');
    });

    var x = document.getElementById("modal-placeholder");
    x.style.display = "block";
};