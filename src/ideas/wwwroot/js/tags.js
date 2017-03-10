var taglist = [];
var removeList = [];
function OnKeyPressTag(e) {
    var self = $(this);
    if (e.keyCode == 13 && !e.shiftKey) {
        e.preventDefault();
        $("#taglistinput").append(getTagsFromString(self.text()));
        return false;
    }
    
    if (e.keyCode == 32 && !e.shiftKey) {
        $("#taglistinput").append(getTagsFromString(self.text()));
    }

    if (e.keyCode == 8 && !e.shiftKey && self.text() == "") {
        removeLast();
    }
}

function initTagEditor() {
    taglist = [];
    removeList = [];
}

function getTagList() {
    return taglist;
}

function refreshHidden() {
    if (taglist.length == 0) {
        $("#JsonTags").val("");
    } else {
        $("#JsonTags").val(JSON.stringify(taglist));
    }
    $("#JsonTagsRemove").val(JSON.stringify(removeList));
}

function getTagsFromString(string) {
    var regex = /[A-z0-9-#+.]*/g;
    var myArray = regex.exec(string.trim());
    if (myArray !== null && myArray[0].length > 0 && myArray[0] !== "") {
        var found = false;
        for (var x = 0; x < taglist.length; x++) {
            if (myArray[0].toUpperCase() == taglist[x].TagValue) {
                found = true;
            }
        }
        if (!found) {
            taglist.push({ "Value": myArray[0].toUpperCase() });
            $('.tagsInput').html("");
            refreshHidden();
            var tag =  createTagInput(myArray[0]);
        } else {
            var tag = "";
        }
        return tag;
    }
}

    function removeTag() {
        var self = $(this);
        var tag = self.attr("data-tag");
        var tmp = [];
        for (var x = 0; x < taglist.length; x++) {
            if (tag == taglist[x].Value) {
                removeList.push(taglist[x]);
            } else {
                tmp.push(taglist[x]);
            }
        }
        taglist = tmp;
        self.parent().remove();
        refreshHidden();
    }

    function removeLast() {
        var tmp = [];
        if (taglist.length > 0) {
            for (var x = 0; x < taglist.length - 1; x++) {
                if (taglist[taglist.length - 1].Value == taglist[x].Value) {
                    removeList.push(taglist[x]);
                } else {
                    tmp.push(taglist[x]);
                }
            }
            $("a.removetag[data-tag=" + taglist[taglist.length - 1].Value + "]").parent().remove();
            removeList.push(taglist[taglist.length - 1]);
        }
        taglist = tmp;
        refreshHidden();
    }

    function CreateTagInputHolder() {
        var holder = $('<div>');
        var tagholder = $("<div>", { "id": "tageditor", "class": "tagholder" });
        var list = $("<div>", { "class": "taglist taglistinput", "id": "taglistinput" });
        var input = $("<div>", { "class": "tagsInput", "contenteditable": "true", "data-text": "Start tagging: Website, Business, Sport, etc ", "data-input": "" });
        var hidden = $("<input>", { "type": "hidden", "name": "JsonTags", "id": "JsonTags" });
        var hiddenremove = $("<input>", { "type": "hidden", "name": "JsonTagsRemove", "id": "JsonTagsRemove" });
        var errorhandeling = $('<span>', { class: "JsonTagsError" });
        tagholder.append(list).append(input).append(hidden).append(hiddenremove);
        holder.append(tagholder).append(errorhandeling);
        input.on("keydown", OnKeyPressTag);
        return holder;
    }

    function createTagInput(text) {
        var holder = $("<div>", { class: "tag" });
        var close = $("<a>", { class: 'removetag', "href": "javascript:;", "data-tag": text.toUpperCase() }).html('<i class="fa fa-times" aria-hidden="true"></i>');
        close.on("click", removeTag);
        var tag = $("<span>", { class: 'tagText' }).text(text.toUpperCase());
        holder.append(close).append(tag);
        return holder;
    }

    function CreateTagHolder() {
        var holder = $("<div>", { "class": "tagholder" });
        var list = $("<div>", { "class": "taglist" });
        holder.append(list);
        return holder;
    }

    function createTag(text) {
        var holder = $("<div>", { class: "tag" });
        var tag = $("<span>", { class: 'tagText' }).text(text.toUpperCase());
        holder.append(tag);
        return holder;
    }