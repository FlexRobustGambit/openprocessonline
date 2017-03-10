
function handler(e) {
    if (e.target.className === "disableonload") {
        e.stopPropagation();
        e.preventDefault();
    }
}
$(function () {
    $('.niceandsquare').keepsquare();
});

$.fn.serializeObject = function () {
    var o = {};
    var a = this.serializeArray();
    $.each(a, function () {
        if (o[this.name]) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value || '');
        } else {
            o[this.name] = this.value || '';
        }
    });
    return o;
};

$.fn.keepsquare = function () {
    this.height(this.width());
};

$.fn.sameHeight = function (as) {
    this.height(as.height());
}

function createEndOfLine() {
    return $("<div>", { id: "endOfLine" }).html('<i class="fa fa-angle-up" aria-hidden="true"></i>');
}

function createHead(e) {
    var holder = $('<div>', { class: "ideaTitleList" });
    var link = $('<a>', { class: "showidea", href: "/p/" + e.idea.id + "/" + e.idea.titel.trim().replace(/\s/g, "_"), 'data-idea': e.idea.id });
    var titel = $('<h2>').html(e.idea.titel);
    link.on("click", loadComplete)
    link.append(titel);
    holder.append(link);
    return holder;
}

function createPersonal(e) {
    var holder = $('<div>', { class: "personalholder" });
    if (e.idea.owner.image !== null) {
        var img = $('<img>', { class: "personalimage", "src": "/images/uploads/superthumbnails/" + e.idea.owner.image.fileName });
    }
    holder.append(img);
    return holder;
}

function createHeadNoClick(e) {
    var holder = $('<div>', { class: "ideaTitleList" });
    var titel = $('<h2>').text(e.idea.titel);
    holder.append(titel);
    return holder;
}

function createHeadInfo(e) {
    var holder = $('<div>', { class: 'ideaHeadInfo' });
    var projectnumber = $('<span>').text('#' + e.idea.id);
    var namelink = $("<a>", { class: "userlink", "href": "/user/" + e.idea.owner.userName }).text(e.idea.owner.userName);
    var publishedby = $('<span>').text(" Posted by: ");
    publishedby.append(namelink);
    holder.append(projectnumber).append(publishedby).append(createDate(e.idea.dateTime));
    if (e.isOwner) {
        holder.append(createEdit(e, 0, false));
        holder.append(createDelete(e, 0, false));
    }
    return holder;
}

function createFavorite(e) {
    var text = ((e.addedToFavorites) ? '<i class="fa fa-star" aria-hidden="true"></i>' : '<i class="fa fa-star-o" aria-hidden="true"></i>Add to favorites');
    var holder = $('<a>', { class: "addtofavoriteslink", href: "javascript:void(0)", 'data-idea': e.idea.id }).html(text);
    holder.on("click", ((!e.addedToFavorites) ? addFav : removeFav));
    return holder;
}



function createImage(img) {
    var holder = $('<figure>', { "itemprop": " associatedMedia", "itemscope": "", "itemtype": "http://schema.org/ImageObject", class: "niceandsquare imagelibrary1" });
    var link = $('<a>', { "href": '/images/uploads/' + img.fileName, "itemprop": "contentUrl", "data-size": img.width + "x" + img.height });
    var image = $('<img>', { class: 'imagepreview', src: '/images/uploads/thumbnails/' + img.fileName, "itemprop": "thumbnail", "alt": img.OriginName });
    link.append(image);
    holder.append(link);
    return holder;
}

function createShell(e) {
    // var text = ((e.addedToFavorites) ? '<i class="fa fa-star" aria-hidden="true"></i>' : '<i class="fa fa-star-o" aria-hidden="true"></i>');

    var holder = $('<div>', { class: "item" });
    var indicator = $('<div>', { class: "indicator", "data-indicator": e.idea.id }).html();
    holder.append(indicator);

    if (e.reason !== 0) {
        var reasonsection = $("<section>", { class: "reason" });
    }

    holder.append(reasonsection);

    var div = $('<div>', { class: "personal" });
    holder.append(div);

    var section1 = $('<section>', { class: "header" });
    holder.append(section1);
    var section2 = $('<section>', { class: "content" });
    section2.append("<hr>");

    if (e.idea.tags !== null && e.idea.tags.length > 0) {
        var tagholder = CreateTagHolder();
        for (var x = 0; x < e.idea.tags.length; x++) {
            tagholder.append(createTag(e.idea.tags[x].value));
        }
    }

    var textholder = $('<div>', { class: "ideaTextList", "data-idea": e.idea.id });
    var imageholder = $('<div>', { class: "imageHolder post-gallery", "itemscope": "", "itemtype": "http://schema.org/ImageGallery" });
    var commentholder = $('<div>', { class: 'commentsHolder' ,  "id" : "commentholder0"});

    section2.append(imageholder).append(textholder).append(tagholder).append(commentholder);
    

    holder.append(section2);
    var section5 = $('<section>', { class: 'update' });
    holder.append(section5);
    var section3 = $('<section>', { class: 'control' });
    holder.append(section3);
    var section4 = $('<section>', { class: 'comment' });
    section4.append("<hr>");

   // var div2 = $('<div>', { class: 'commentsHolder' });
   // section4.append(div2);
    holder.append(section4);
    return holder;
}

function createUpdateShell(id, nr, update) {
    var holder = $('<div>', { class: "updateitem", "data-update": update.id, "data-number": nr });
    var indicator = $('<div>', { class: "indicator", "data-indicator": id }).text("#" + nr);
    holder.append(indicator);
    var header = $('<div>', { class: "updateInfo" });
    var imageholder = $('<div>', { class: "imageHolder post-gallery" });
    var textholder = $('<div>', { class: "textholder" });
    var commentholder = $('<div>', { class: 'commentsHolder', "id": "commentholder"+nr });
    holder.append("<hr>").append(header).append(imageholder).append(textholder).append(commentholder);
    return holder;
}

function createUpdateHeadInfo(nr, e, update) {
    var holder = $('<div>', { class: 'ideaHeadInfo' });
    var updateNummer = $('<span>').text('Update #' + (nr + 1));
    holder.append(updateNummer).append(createDate(update.dateTime));
    if (e.isOwner) {
        holder.append(createEdit(e, update.id, true));
        holder.append(createDelete(e, update.id, true));
    }
    return holder;
}

function list(e) {
    var holder = $('<div>', { class: "listItem" });
    var name = $('<a>', { class: "listname", "href": "/p/" + e.idea.id + "/" + e.idea.titel.trim().replace(/\s/g, "_") }).text(e.idea.titel);
    var labelFavorites = $('<span>', { class: "namelabel" }).text("Favorites");
    var labelComments = $('<span>', { class: "namelabel" }).text("Comments");
    var favorites = $('<span>', { class: "listNumber" }).text(((e.idea.stats === null) ? 0 : e.idea.stats.favorites));
    var comments = $('<span>', { class: "listNumber" }).text(((e.idea.stats === null) ? 0 : e.idea.stats.comments));
    holder.append(name).append(labelFavorites).append(favorites).append(labelComments).append(comments);
    return holder;
}

function updatelist(e) {
    var holder = $('<div>', { class: "listItem" });
    var name = $('<a>', { class: "listname", "href": "/Home/UpdateProcess/" + e.idea.id }).text(e.idea.titel);
    var labelFavorites = $('<span>', { class: "namelabel" }).text("Favorites");
    var labelComments = $('<span>', { class: "namelabel" }).text("Comments");
    var favorites = $('<span>', { class: "listNumber" }).text(((e.idea.stats === null) ? 0 : e.idea.stats.favorites));
    var comments = $('<span>', { class: "listNumber" }).text(((e.idea.stats === null) ? 0 : e.idea.stats.comments));
    holder.append(name).append(labelFavorites).append(favorites).append(labelComments).append(comments);
    return holder;
}

function createDate(e) {
    var date = new Date(e);
    var day = date.getDate();
    var months = ['jan', 'feb', 'mar', 'apr', 'May', 'jun', 'jul', 'aug', 'sep', 'oct', 'nov', 'dec'];
    var month = months[date.getMonth()];
    var year = date.getFullYear().toString().split("");
    var createDate = $('<span>', { class: "date" }).text(" at: " + day + " " + month + " \'" + year[2] + "" + year[3]);
    return createDate;
}

function addUpdate(nr, update, id, e) {
    var holder = createUpdateShell(id, (nr + 1), update);
    var headsection = holder.find('div.updateInfo')[0];
    $(headsection).append(createUpdateHeadInfo(nr, e, update));
    var imageHolder = holder.find('div.imageHolder')[0];
    $.each(update.images, function (nr, image) {
        $(imageHolder).append(createImage(image));
    });
    holder.find('div.textholder')[0].innerHTML = update.text;
    return holder;
}

function createLoader() {
    var holder = $('<div>', { class: 'loaderholder' });
    var img = $('<img>', { 'id': 'loader', 'src': "/images/loader.gif" });
    holder.append(img);
    return holder;
}

function createSmallLoader() {
    var holder = $('<div>', { class: 'smalloaderholder' });
    var img = $('<img>', { 'id': 'loader', 'src': "/images/default.svg" });
    holder.append(img);
    return holder;
}

function createPreviewImage(e) {
    var holder = $("<div>", { class: "niceandsquare imagelibrary1" });
    var remove = $("<a>", { class: "removeimagenewLink", "data-name": e.fileName, "href": "javascript:void(0)" }).html('<i class="fa fa-times-circle" aria-hidden="true"></i>');
    remove.on("click", removeFromUpload);
    var img = $("<img>", { class: "imagepreview", "src": "/images/uploads/thumbnails/" + e.fileName });
    holder.append(img).append(remove);
    return holder;
}

$(window).resize(function () {
    $('.niceandsquare').keepsquare();
    $("#dropZone").sameHeight($("#imagepreview"));
    $('.personal').keepsquare();
});

function createEdit(e, update, isupdate) {
    var holder = $('<a>', { class: "editIcon", href: "javascript:void(0)", 'data-idea': e.idea.id, 'data-update': update }).html('<i class="fa fa-pencil" aria-hidden="true"></i>');
    holder.on("click", edit);
    return holder;
}

function createDelete(e, update, isupdate) {
    var holder = $('<a>', { class: "editIcon", href: "javascript:void(0)", 'data-idea': e.idea.id, 'data-update': update }).html('<i class="fa fa-trash-o" aria-hidden="true"></i>');
    holder.on("click", Delete);
    return holder;
}

function createUpdate(e) {
    var holder = $('<a>', { href: "javascript:;", 'data-idea': e.idea.id }).text('Update');
    holder.on('click', update);
    return holder;
}

function createReason(e) {
    var text = "";
    switch (e.reason) {
        case 0:
            text = "No reason"
            break;
        case 2:
            text = "Commented on";
            break;
        case 3:
            text = "Added to your favorites";
            break;
        case 4:
            text = e.idea.owner.userName + " has posted a new prucess";
            break;
        case 5:
            text = "Posted";
            break;
        case 6:
            text = e.idea.owner.userName + " posted an update";
            break;
        case 7:
            text = "New prucess from " + e.idea.owner.userName;
            break;
    }
    return text;
}
/*
        None = 0,
        Commented = 2,
        Favorited = 3,
        Following = 4,
        Owner = 5,
        Updated = 6,
        PostedNew = 7
*/
function show(e) {
    var holder = createShell(e);
    var ideaId = holder.find('.ideaItem');
    ideaId.prevObject[0].setAttribute('id', e.idea.id);
    holder.find('div.personal').append(createPersonal(e));
    
    if (e.reason !== 0) {
        holder.find('section.reason').append(createReason(e));
    }
    holder.find('section.header').append(createHead(e));
    holder.find('div.ideaTitleList').prepend(createHeadInfo(e))

    var imageholder = holder.find('div.imageHolder')[0];
    $.each(e.idea.images, function (e, image) {
        $(imageholder).append(createImage(image));
    });
     
    holder.find('div.ideaTextList')[0].innerHTML = e.idea.text;
    if (e.idea.updates.length > 0) {
        var updatesection = holder.find('section.update')[0];
        $.each(e.idea.updates, function (index, update) {
            $(updatesection).append(addUpdate(index, update, e.idea.id, e));
        });
    }
    var controlholder = holder.find('section.control')[0];
    $(controlholder).append(createFavorite(e));
    $(controlholder).append(createCommentsLink(e));
    if (e.isOwner) {   
        $(controlholder).append(createUpdate(e));
    }

    var comments = filterComments(e);
    if(typeof comments !== "undefined"){
       placeComments(holder, comments); 
    }
   
    if (e.authorized) {
        holder.find('section.comment').append(createCommentForm(e));
    }
    return holder;
}

function setContentSection(section, item) {
    var title = section.find('.ideaTitleList h2');
    title.innerHTML = item.title;
    return section;
}

function createHiddenId(e) {
    var holder = $("<input>", { class: "edittitel", "id": "id", "name": "Id", 'value': e, "Type": "hidden" });
    return holder;
}

function createTitelInput(e) {
    var holder = $('<div>');
    var input = $("<input>", { class: "edittitel", "id": "Titel", "name": "Titel", 'value': e, "Type": "text", "placeholder": "Create a title" });
    var errorhandeling = $('<span>', { class: "TitelError" });
    holder.append(input).append(errorhandeling);
    return holder;
}

function createTextEditor(input) {
    var holder = $('<div>', { class: "texteditorholder" });
    var textarea = $('<textarea>', { "type": "text", class: "widgEditor form-control", "id": "widgEditor" }).text(input);
    var errorhandeling = $('<span>', { class: "TextError" });
    holder.append(textarea).append(errorhandeling);
    return holder;
}

function createFileDrop(e) {
    var holder = $('<div>', { class: "imageuploadholder" });
    var dropzone = $('<div>', { "id": "dropZone" });
    var preview = $('<div>', { "id": "imagepreview" });
    var remove = $('<input>', { "id": "JsonImagesRemove", "name": "JsonImagesRemove", "type": "hidden" });
    var newInput = $('<input>', { "id": "JsonImagesNew", "name": "JsonImagesNew", "type": "hidden" });
    holder.append(remove).append(newInput);
    dropzone.append(preview);
    holder.append(dropzone);
    if (typeof e !== "undefined") {
        $.each(e, function (index, image) {
            $(preview).append(createPreviewImage(image));
        });
    }
    return holder;
}

function createSaveEditButton(savefunction) {
    var holder = $('<div>', { class: "editControls" });
    var save = $('<input>', { "class": "btn btn-success disableonload", "value": "Save", "contenteditable": "false" , "type" : "button" });
    var edit = $('<input>', { "class": "btn btn-warning disableonload", "value": "Back", "contenteditable": "false", "type": "button" });
    save.on("click", savefunction);
    edit.on("click", closePopout);
    holder.append(edit).append(save);
    return holder;
}
