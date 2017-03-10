function loadList(username) {
    var input = { "Offset": 5, "UserName": username };
    console.log("loadlist");
    $.ajax({
        url: '/Profile/ProcessList',
        method: 'POST',
        data: JSON.stringify(input),
        dateType: "json",
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json",
            "RequestVerificationToken": $('#__AjaxAntiForgeryForm input[name="__RequestVerificationToken"]').val()
        },
        error: function (e) {
            console.log(e);
        },
        success: function (e) {
            console.log("list loaded");
            var json = JSON.parse(e)
            json.forEach(function (i) {
                $('#list').append(list(i));
            });
            $("#listloader").hide();
          }
    });
}