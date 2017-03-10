
function loadComplete() {
    var id = $(this).attr("data-idea");
    $.ajax({
        url: "/Home/ById",
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify({ "IdeaId": id }),
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
            $('#DetailsHolder').append(showComplete(e));
            history.pushState({}, '', window.location.origin + '/Home/Details/' + id);
            $('.niceandsquare').keepsquare();
            connectDots(e.idea.id);
        }
     });
}

function showComplete(e) {
    $("#ScrollHolder").hide();
    $("#loaderholder").hide();
    var holder = createShell(e);
    var ideaId = holder.find('.ideaItem');
    ideaId.prevObject[0].setAttribute('id', e.idea.id);
    holder.find('div.personal').append(createPersonal(e));

    holder.find('section.header').append(createHeadNoClick(e));
    holder.find('div.ideaTitleList').prepend(createHeadInfo(e))
    holder.find('div.ideaTextList')[0].innerHTML = e.idea.text;

    var controlholder = holder.find('section.control')[0];
    $(controlholder).append(createFavorite(e));
    $(controlholder).append(createCommentsLink(e));

    var imageholder = holder.find('div.imageHolder')[0];
    $.each(e.idea.images, function (e, image) {
        $(imageholder).append(createImage(image));
    });
        
    if (e.idea.updates.length > 0) {
        var updatesection = holder.find('section.update')[0];
        $.each(e.idea.updates, function (index, update) {
            $(updatesection).append(addUpdate(index, update, e.idea.id, e));
        });
    }
    var commentholder = holder.find('div.commentsHolder')[0];
    $.each(e.idea.comments, function (e, comment) {
        $(commentholder).append(createComment(comment));
    });
    
    if (e.authorized) {
        holder.find('section.comment').append(createCommentForm(e));
    }
    return holder;
}

function showProcess(e) {
    var holder = createShell(e);
    var ideaId = holder.find('.ideaItem');
    ideaId.prevObject[0].setAttribute('id', e.idea.id);

    holder.find('section.header').append(createHeadNoClick(e));
    holder.find('div.ideaTitleList').prepend(createHeadInfo(e))
    holder.find('div.ideaTextList')[0].innerHTML = e.idea.text;

    var imageholder = holder.find('div.imageHolder')[0];
    $.each(e.idea.images, function (e, image) {
        $(imageholder).append(createImage(image));
    });

    if (e.idea.updates.length > 0) {
        var updatesection = holder.find('section.update')[0];
        $.each(e.idea.updates, function (e, update) {
            $(updatesection).append(addUpdate(e, update));
        });
    }

       
    return holder;
}