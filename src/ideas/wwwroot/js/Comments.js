function postcomment(form) {
    if ($(form.data.holder[0][1]).val().length > 1) {
        if ($(form.data.holder[0][0]).val() !== null) {
            form.data.holder[0][2].disabled = (true);
            var formdata = $(form.data.holder).serializeObject();
            $.ajax({
                url: '/Home/PostComment',
                method: "POST",
                contentType: "application/json",
                data: JSON.stringify(formdata),
                dateType: "json",
                headers: {
                    "Accept": "application/json",
                    "Content-Type": "application/json",
                    "RequestVerificationToken": $('#__AjaxAntiForgeryForm input[name="__RequestVerificationToken"]').val()
                },
                error: function (e) {
                    console.log(e);
                    if (e.status === 401 && e.statusText === "Unauthorized") {
                        $(location).attr('href', "/Account/Login");
                    }
                },
                success: function (e) {
                    var data = JSON.parse(e);
                    form.data.holder[0][2].disabled = (false);
                    var nr = ((data.idea.updates !== null) ? data.idea.updates.length : 0);
                    $('#' + data.idea.id).find('#commentholder' + (nr)).append(createComment(data.idea.comments[(data.idea.comments.length - 1)]));
                    var noc = $('#' + data.idea.id).find('.numberofcomment');
                    $(noc).text(($(noc).text() * 1) + 1);
                    $(form.data.holder[0][1]).val("");
                }
            });
        }
    }
}

function RemoveComment() {
    var self = this;
    var comment = $(self).attr("data-indentifier");
    var idea = $(self).attr("data-idea"); 
    var data = { "CommentId": comment, "IdeaId": idea };
    $('#c' + comment).html(createLoader());
    $.ajax({
        url: '/Home/RemoveComment',
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify(data),
        dateType: "json",
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json",
            "RequestVerificationToken": $('#__AjaxAntiForgeryForm input[name="__RequestVerificationToken"]').val()
        },
        error: function (e) {
            console.log(e);
            if (e.status === 401 && e.statusText === "Unauthorized") {
                $(location).attr('href', "/Account/Login");
            }
        },
        success: function (e) {
            var data = JSON.parse(e);
            $('#c' + comment).fadeOut();
            var noc = $('#' + idea).find('.numberofcomment');
            $(noc).text(($(noc).text() * 1) - 1);
        }
    });
}


function filterComments(e) {
    var comments = e.idea.comments.reverse();
    var noc = comments.length;
    var list = [];
    if (e.idea.updates.length > 0) {
        var start = new Date(e.idea.dateTime);
        var updates = e.idea.updates;
        var nou = updates.length;
        for (var x = 0; x < noc; x++) {
            var commentdate = new Date(comments[x].dateTime).getTime();
            for (var y = 0; y < nou; y++) {
                var update;
                var updateDate = new Date(updates[y].dateTime).getTime();
                var comment = JSON.stringify(comments[x]);
                if ((y === 0) && (commentdate < updateDate)) {
                    update = '{ "update" : ' + y + ' , "comment" : ' + comment + '}';
                    list.push(update);
                } else if ((y - 1 >= 0) &&
                          (commentdate > new Date(updates[y - 1].dateTime).getTime()) &&
                          (commentdate < updateDate)) {
                    update = '{ "update" : ' + y + ' , "comment" : ' + comment + '}';
                    list.push(update);
                } else if (y === (nou - 1) && commentdate > updateDate) {
                    update = '{ "update" : ' + nou + ' , "comment" : ' + comment + '}';
                    list.push(update);
                }
            }
        }
        return (list);
    } else {
        for (var x = 0; x < noc; x++) {
            var comment = JSON.stringify(comments[x]);
            update = '{ "update" : 0 , "comment" : ' + comment + '}';
            list.push(update);
        }
        return (list);
    }
}

function placeComments(holder, sortedComments) {
    var size = sortedComments.length;
    for (var x = 0 ; x < size ; x++) {
        var parsed = JSON.parse(sortedComments[x]);
        holder.find("#commentholder" + parsed.update).append(createComment(parsed.comment, holder.attr("id")));
    }
}

function createCommentsLink(e) {
    var holder = $('<a>', { class: "readCommentslink", href: "javascript:void(0)", 'data-idea': e.idea.id });
    var span = $('<span>', { class: "numberofcomment" }).text((e.idea.stats === null) ? "0" : e.idea.stats.comments);
    holder.append(span).append(" Comment" + (((e.idea.stats !== null) && (e.idea.stats.comments === 1)) ? "" : "s"));
    if (e.idea.comments !== Settings.defaultComments) {
        holder.on("click", loadComments);
    }
    return holder;
}

function createCommentForm(e) {
    var holder = $('<form>', { 'asp-antiforgery': "true" });
    var input = $('<input>', { type: "hidden", name: "IdeaId", value: e.idea.id });
    var textarea = $('<textarea>', { name: "Comment" });
    var button = $('<input>', { name: "postcomment", value: "Post", type: "button", class: "postcommentfrontpage btn " });
    holder.append(input).append(textarea).append(button);
    button.on("click", { holder: holder }, postcomment);
    return holder;
}

function createComment(comment, idea) {
    var holder = $('<div>', { class: 'comment' , "id": "c"+comment.id });
    var drop = ((comment.isOwner) ? commentDropDownOwner(comment , idea) : commentDropDown(comment , idea));
    holder.append(drop).append(imageholder).append($('<p>').text(comment.text));
    var details = $('<div>', { class: 'commentDetails' });
    var namelink = $("<a>", { class: "userlink", "href": "/user/" + comment.owner.userName }).text(comment.owner.userName);
    if (comment.owner.image !== null) {
        var imageholder = $('<div>', { class: 'commentImage' });
        var image = $('<img>', { "src": "/images/uploads/megasuperthumbnails/" + comment.owner.image.fileName });
        imageholder.append(image);
        details.append(imageholder);
    }
    details.append($('<span>', { class: "commentBy" }).append(namelink));
    details.append($('<span>', { class: "commentPostedOn" }).text(" posted "));
    details.append(createDate(comment.dateTime));
    holder.append(details);
    return holder;
}


function commentDropDown(comment, idea) {
    var reportlink = $('<a>', { "href": "javascript:;", class: "navlink", "data-indentifier": comment.id, "data-idea": idea }).text("Report");
    reportlink.on('click', test);
    var items = [];
    items.push(reportlink);
    return DropDown(items);
}

function test() {
    console.log("test");
}

function commentDropDownOwner(comment , idea) {
    var reportlink = $('<a>', { "href": "javascript:;", class: "navlink", "data-indentifier": comment.id , "data-idea" : idea }).text("Remove");
    reportlink.on('click', RemoveComment);
    var items = [];
    items.push(reportlink);
    return DropDown(items);
    return DropDown(items);
}



function DropDown(items) {
    var holder = $('<ul>', { class: "nav navbar-nav navbar-right nav-bar-comment" });
    var mainli = $('<li>', { class: "dropdown" });
    var link = $('<a>', { class: "dropdown-toggle comment-toggle", "href": "#", "data-toggle": "dropdown" });
    var dropdown = $('<span>', { class: "caret2" }).html('<i class="fa fa-chevron-down" aria-hidden="true"></i>');
    link.append(dropdown);
    mainli.append(link);

    var dropul = $('<ul>', { class: "dropdown-menu" });
    for (var x = 0 ; x < items.length; x++) {
        var li = $('<li>').append(items[x]);
        dropul.append(li);
    }
    mainli.append(dropul);
    holder.append(mainli);
    return holder;
}


/*
 else if ((y - 1 >= 0) && (updates[y - 1].dateTime >= comments[x].dateTime) && (comments[x].dateTime <= updates[y].dateTime)) {
                    update = "2{ 'update' : " + y + " , 'comment' : " + comments[x] + "}";
                } else {
                    update = "3{ 'update' : " + y + " , 'comment' : " + comments[x] + "}";
                }


 else if ((y - 1 >= 0) && (update[y - 1].dateTime > comments[x].dateTime) && (comments[x].dateTime < update[y].dateTime)) {
                    update = "{ 'update' : " + y + " , 'comment' : " + comments[x] + "}";
                } else {
                    update = "{ 'update' : " + y + " , 'comment' : " + comments[x] + "}";
                }
else if ((y - 1 >= 0) && (new Date(update[y - 1].dateTime) > new Date(comments[x].dateTime)) && (new Date(comments[x].dateTime) < new Date(update[y].dateTime))) {
                    var update = "{ 'update' : " + y + " , 'comment' : " + comments[x] + "}";
                } else {
                    var update = "{ 'update' : " + y + " , 'comment' : " + comments[x] + "}";
                }

*/

function createCommentHolder() {



}