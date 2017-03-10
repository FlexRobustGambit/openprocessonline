$.fn.localSave = function (key) {
    if (typeof (Storage) !== "undefined") {
        localStorage.setItem(key, $(this).val().toString());
    }
}

$.fn.checkLocal = function () {
    if (typeof (Storage) !== "undefined") {
        return localStorage.getItem(this.id);
    }
}

function clearStorage() {
    if (typeof (localStorage) !== "undefined") {
        localStorage.clear();
    }
}



$(function () {

    $("input[type=text]").each(function () {
        if (typeof (Storage) !== "undefined") {
            // $(this).val(localStorage.getItem(this.id));
        }
    });

    $("form").on("reset", function () {
        $('.niceandsquare').remove();
        if (typeof (Storage) !== "undefined") {
            if (typeof (Storage) !== "undefined") {
                var list = JSON.parse(localStorage.getItem("images"));
                var postdata = { "Images": list };
                if (list !== null) {
                    $.ajax({
                        url: '/Home/Reset',
                        method: "POST",
                        contentType: "application/json",
                        data: JSON.stringify(postdata),
                        dateType: "json",
                        headers: {
                            "Accept": "application/json",
                            "Content-Type": "application/json",
                            "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
                        },
                        error: function (e) {
                            console.log(e);
                            if (e.status == 401 && e.statusText == "Unauthorized") {
                                $(location).attr('href', "/Account/Login");
                            }
                        },
                        success: function (e) {
                        }
                    });
                }
            }
            localStorage.clear();
        }
    });
});

$("input").on("keyup", function (e) {
    var key = this.id;
    $(this).localSave(key);
});

