function saveNewPrucess() {
    var form = $("#editform").serializeObject();
    var newPrucess = {
        "Titel" : form.Titel,
        "JsonImagesRemove": form.JsonImagesRemove,
        "JsonImagesNew": form.JsonImagesNew,
        "Text": editor.getInput(),
        "JsonTags": form.JsonTags
    };
    $.ajax({
        url: "/Home/NewPrucess",
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify(newPrucess),
        dateType: "json",
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json",
            "RequestVerificationToken": $('#__AjaxAntiForgeryForm input[name="__RequestVerificationToken"]').val()
        },
        error: function (e) {
            if (e.status == 401 && e.statusText == "Unauthorized") {
                $(location).attr('href', "/Account/Login");
            }
        },
        success: function (e) {
            var data = JSON.parse(e);
            if ($.isArray(data)) {
                displayError(data);
            } else {
                $(location).attr('href', "/p/" + data.id + "/" + data.name);
                clearStorage();
          }
        }
    });
}