function Search(s, test) {

    var filter = s.value.toLowerCase();
    var ul = document.getElementById(test.id);
    var li = ul.getElementsByTagName('li');

    var dropDownMaxDisplay = 10;

    var displayCount = 0;

    for (var i = 0; i < li.length; i++) {

        val = li[i].textContent || li[i].innerText;

        if (displayCount < dropDownMaxDisplay && val.toLowerCase().indexOf(filter) > -1) {

            li[i].style.display = "";
            li[i].hidden = false;
            displayCount++;
        }
        else {

            li[i].style.display = "none";
            li[i].hidden = true;
        }
        if (displayCount == 9) {

            end = document.getElementById('end');
            end.style.display = "";
        }
    }

    ul.style.display = "";
};