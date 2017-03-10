function follow(){
    var self = this;
    console.dir(this);
    var follow = { "UserName": self.getAttribute("data-user") };
    $.ajax({
        url: '/Profile/Follow',
        method: 'post',
        data: JSON.stringify(follow),
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
            self.innerHTML = '<i class="fa fa-check" aria-hidden="true"></i>';
            $(self).off("click", follow);
            $(self).on("click", unFollow);
            console.log("succes");
        }
    });
    
}

function unFollow() {
    var self = this;
    var follow = { "UserName": self.getAttribute("data-user") };
    $.ajax({
        url: '/Profile/UnFollow',
        method: 'post',
        data: JSON.stringify(follow),
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
            self.innerHTML = '<i class="fa fa-thumb-tack" aria-hidden="true"></i>';
            $(self).off("click", unFollow);
            $(self).on("click", follow);
            console.log("succes");
        }
    });
}

function createFollowButton(following, user, text, text2) {
    var holder = $('<a>', { class: "topcontrol btn-follow", 'data-user': user , "href" : "javascript:;" });
    holder.html(((following) ? text2 : text));
    holder.on('click', ((following) ? unFollow : follow));
    return holder;
}