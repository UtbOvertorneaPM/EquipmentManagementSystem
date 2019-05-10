function PostForm(form, callBack) {

    $.ajax({
        dataType: 'JSON',
        type: 'POST',
        url: form.attr('action'),
        data: form.serialize(),
        success: function (data) {

            switch (data) {
                case true:
                    callBack(data);
                    break;
                case false:
                    callBack(data);
                    break;
                case null:
                    alert("Model invalid");
                    break;
                default:
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(errorThrown);
            return false;
        }
    });
};

function Post(url, value) {

    alert(value);

    $.ajax({
        dataType: 'JSON',
        type: 'POST',
        url: url,
        data: value,
        success: function (data) {
            switch (data) {
                case true:
                    alert("Succes");
                    break;
                case false:
                    alert("Failed");
                    break;
                default:
            }
        }

    });
};
