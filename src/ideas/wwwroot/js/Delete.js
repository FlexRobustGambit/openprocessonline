
var deleteIdeaId; 
var deleteUpdateId;
var isremoving = false;


function Delete(){
    deleteIdeaId = $(this).attr('data-idea');
    deleteUpdateId = $(this).attr('data-update');
    $('body').append(createDeleteConfirm());
    var width = $(".item").width();
    $('.pop-outbackground').fadeIn(100);
    $('.pop-out').css({ "width": width + "px", "margin-left": "-" + (width / 2) + "px", "top": "calc(50vh - " +$('.pop-out').height()+"px)" , "position": "fixed"});
    $('.pop-out').fadeIn(750);
}


function createDeleteConfirm() {
    var holder = addPopOut();
    var textholder = $('<div>', { class: "textholderpopup" });
    textholder.html('<h2><i class="fa fa-exclamation-triangle" aria-hidden="true"></i><i class="fa fa-trash-o" aria-hidden="true"></i> Remove <i class="fa fa-trash-o" aria-hidden="true"></i><i class="fa fa-exclamation-triangle" aria-hidden="true"></i></h2> We don\'t save anything so <b>gone = gone</b>. Are you sure you want to remove this prucess? ');
    holder.append(textholder);
    var div = $('<div>', { class: "linkHolder" });
    var back = $('<a>', { class: "deleteconfirm disableonload", "href": "javascript:;" }).text("Cancel");
    var goAhead = $('<a>', { class: "deleteconfirm disableonload", "href": "javascript:;" }).html('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i>Remove<i class="fa fa-exclamation-triangle" aria-hidden="true"></i>');
    back.on("click", closePopout);
    goAhead.on("click", Remove)
    div.append(back).append(goAhead);
    holder.append(div);
    return holder;
}


function Remove() {
    if (!isremoving) {
        console.log(deleteIdeaId);
        console.log(deleteUpdateId);
        this.isremoving = true;
        $('.linkHolder').append(createLoader());
        var removethis = { "IdeaId": deleteIdeaId, "EditId": deleteUpdateId };
  
        $.ajax({
            url: "/Home/Remove",
            method: "POST",
            contentType: "application/json",
            data: JSON.stringify(removethis),
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
                    if (deleteUpdateId == 0) {
                        $("div#" + deleteIdeaId).remove();
                        closePopout();
                    } else {
                        $('#' + data.ideaId).replaceWith(show(data.ideaEx));
                        initPhotoSwipeFromDOM('.post-gallery');
                        $('.niceandsquare').keepsquare();
                        $('.personal').keepsquare();
                        closePopout();
                        clearStorage();
                    }
                }
            }
        });
    }
}