function GetDropDown(action, document) {

    var list = document.getElementById('ownerList');

    list.style.display = "none";

    $.get(action, function (r) {

        cachedResponse = r;

        for (var key in cachedResponse) {

            var item = document.createElement('li');
            var a = document.createElement('a');

            a.appendChild(document.createTextNode(cachedResponse[key]));
            a.id = key;
            a.href = '#';
            a.className = "clickable";
            a.onclick = function () { ClickName(event) };
            item.appendChild(a);
            list.appendChild(item);
        }

        var li = endOfList = document.createElement('li');
        var endOfList = document.createElement('a');

        endOfList.appendChild(document.createTextNode("..."));
        endOfList.id = "end";
        endOfList.style.color = "grey";

        list.appendChild(endOfList);
    });
};