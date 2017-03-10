var IdeaId;
var editor;
var editing = false;

function update() {
    if (!editing) {
        editing = true;
        var self = $(this);
        IdeaId = self.attr('data-idea');
        $('body').append(createUpdateForm());
        var top = $('#' + IdeaId + ' section.update').offset().top + $('#' + IdeaId + ' section.update').height();
        var width = $(".item").width();
        $('.pop-outbackground').fadeIn(100);
        $('.pop-out').fadeIn(750);
        $('.pop-out').css({ "width": width + "px", "margin-left": "-" + (width / 2) + "px", "top": top + "px" });
        editor = new widgEditor('widgEditor');
        initDropZone();
    }
}

function saveUpdate() {
    var form = $("#editform").serializeObject();
    var edit = {
        "IdeaId": IdeaId,
        "EditUpdateViewModel": {
            "JsonImagesRemove": form.JsonImagesRemove,
            "JsonImagesNew":form.JsonImagesNew,
            "Text": editor.getInput()
        }
   
    };

    $.ajax({
        url: "/Home/PostUpdate",
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify(edit),
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
            console.dir(data);
            if ($.isArray(data)) {
                displayError(data);
            } else {
                $('#' + data.ideaId).replaceWith(show(data.ideaEx));
                initPhotoSwipeFromDOM('.post-gallery');
                $('.niceandsquare').keepsquare();
                $('.personal').keepsquare();
                closePopout();
            }
        }
    });
}




function createUpdateForm() {
    var holder = addPopOut();
    var form = $("<form>", { "action": "Home/EditIdea", "method": "POST", "id": "editform" });
    var editor = createTextEditor("");
    var fd = createFileDrop();
    var control = createSaveEditButton(saveUpdate);
    form.append(fd).append(editor).append(control);
    holder.append(form);
    return holder;
}

