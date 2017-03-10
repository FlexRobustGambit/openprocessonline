var IdeaId;
var update;
var editor;
var editing = false;

function edit() {
    if (!editing) {
        editing = true;
        var self = $(this);
        var editid = self.attr("data-update");
        var idea = self.attr("data-idea");
        var editthis = { "IdeaId": idea, "EditId": editid };
        $.ajax({
            url: "/Home/Edit",
            method: "POST",
            contentType: "application/json",
            data: JSON.stringify(editthis),
            dateType: "json",
            headers: {
                "Accept": "application/json",
                "Content-Type": "application/json",
                "RequestVerificationToken": $('#__AjaxAntiForgeryForm input[name="__RequestVerificationToken"]').val()
            },
            error: function (e) {
                if (e.status === 401 && e.statusText === "Unauthorized") {
                    $(location).attr('href', "/Account/Login");
                }
            },
            success: function (e) {
                var data = JSON.parse(e);
                IdeaId = data.ideaEx.idea.id;
                update = data.editId;
                initTagEditor();
                if (data.editId === 0 && data.ideaEx !== null) {
                    $('body').append(createEditForm(data));
                    localStorage.setItem("images", JSON.stringify(data.ideaEx.idea.images));
                    var top = $("#" + data.ideaEx.idea.id).offset().top;
                } else {
                    $('body').append(createEditUpdateForm(data));
                    localStorage.setItem("images", JSON.stringify(data.update.images));
                    var top = $(".updateitem[data-update= '" + data.update.id + "']").offset().top;
                }
                
                editor = new widgEditor('widgEditor');
                initDropZone();

                var width = $(".item").width();
                $('.pop-outbackground').fadeIn(100);
                $('.pop-out').css({ "width": width + "px", "margin-left": "-" + (width / 2) + "px", "top": top + "px" });
                $('.pop-out').fadeIn(750);
                $('.niceandsquare').keepsquare();
                $('.personal').keepsquare();
            }
        }); 
    }
}

function saveEdit() {
    var form = $("#editform").serializeObject();
    if (update === 0 && IdeaId !== null) {
        var edit = {
            "IdeaId": IdeaId,
            "EditId": update,
            "EditIdeaViewModel": {
                "Titel": form.Titel,
                "Text": editor.getInput(),
                "JsonImagesRemove": form.JsonImagesRemove,
                "JsonImagesNew": form.JsonImagesNew,
                "JsonTags": form.JsonTags,
                "JsonTagsRemove": form.JsonTagsRemove
            }
        }
        
    } else {
        var edit = {
            "IdeaId": IdeaId,
            "EditId": update,
            "EditUpdateViewModel": {
                "Text": editor.getInput(),
                "JsonImagesRemove": form.JsonImagesRemove,
                "JsonImagesNew": form.JsonImagesNew,
                "JsonTags": form.JsonTags,
                "JsonTagsRemove": form.JsonTagsRemove
            }
        }
    }

    $.ajax({
        url: "/Home/SaveEdit",
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
            if (e.status === 401 && e.statusText === "Unauthorized") {
                $(location).attr('href', "/Account/Login");
            }
        },
        success: function (e) {
            var data = JSON.parse(e);
            if ($.isArray(data)) {
                displayError(data);
            } else {
                $('#' + data.ideaId).replaceWith(show(data.ideaEx));
                initPhotoSwipeFromDOM('.post-gallery');
                $('.niceandsquare').keepsquare();
                $('.personal').keepsquare();
                closePopout();
                clearStorage();
            }
        }
    });
}

function addPopOut() {
    var bg = $("<div>", { class: "pop-outbackground" });
    $('body').append(bg);
    bg.on("click", closePopout);
    return $("<div>", { class: "pop-out" });
}

function createEditUpdateForm(e) {
    var holder = addPopOut();
    var form = $("<form>", { "action": "Home/EditUpdate", "method": "POST", "id": "editform" });
    var editor = createTextEditor(e.update.text);
    var fd = createFileDrop(e.update.images);
    var control = createSaveEditButton(saveEdit);
    form.append(fd).append(editor).append(control);
    holder.append(form)
    return holder;
}

function createEditForm(e) {
    var holder = addPopOut();
    var form = $("<form>", { "action": "Home/EditIdea", "method": "POST", "id": "editform" });
    var titelinput = createTitelInput(e.ideaEx.idea.titel);
    var editor = createTextEditor(e.ideaEx.idea.text);
    var fd = createFileDrop(e.ideaEx.idea.images);
    var control = createSaveEditButton(saveEdit);
    var tagsholder  = CreateTagInputHolder();
    

    if (e.ideaEx.idea.tags !== null && e.ideaEx.idea.tags.length > 0) {
        for (var x = 0; x < e.ideaEx.idea.tags.length; x++) {
            $(tagsholder).find('#taglistinput').append(getTagsFromString(e.ideaEx.idea.tags[x].value));
        }
        $(tagsholder).find("#JsonTags").val(JSON.stringify(getTagList()));
    }
    form.append(titelinput).append(fd).append(editor).append(tagsholder).append(control);
    holder.append(form)
    return holder;
}

function closePopout() {
    if (localStorage !== "undefined") {
        localStorage.setItem("images", "");
    }
    editing = false;
    $('.pop-outbackground').remove();
    $('.pop-out').remove();
    IdeaId = null;
    update = null;
    editor = null;
}