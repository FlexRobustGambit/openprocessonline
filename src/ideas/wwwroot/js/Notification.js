
function GetNotifications() {
    $(".notification-menu").html(createLoader());
    $.ajax({
        url: '/Home/Notifications',
        method: 'post',
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
            var tmp = [];
            for (var x = 0; x < data.length; x++) {
                if (data[x].isOwner && !data[x].viewed) {
                    tmp.push(data[x]);
                }
            }
            if (tmp.length > 0) {
                $('#notificationcounter').text(tmp.length);
                $('#notificationcounter').fadeIn();
                $("#noticationlink").on("click", viewedNotifications);
            }
            if (data.length > 0) {
                AddNotifications(data);
                $('.notificationImageHolder').keepsquare();
            }
            console.dir(data);
        }
    });
}

function AddNotifications(data) {
    $(".notification-menu").html("");
    for (var x = 0; x < data.length; x++) {
        var li = $('<li>').append(createNotification(data[x]));
        $(".notification-menu").append(li);
    }
}

function viewedNotifications() {
    console.log("test");
    $.ajax({
        url: '/Home/ViewedNotifications',
        method: 'post',
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
            $("#noticationlink").off("click", viewedNotifications);
        }
    });
}

/*
None = 0,
Update = 1, 
NewPost = 2,
NewFollower = 3,
Commented = 4,
Favorited = 5 */

function createNotification(data) {

    var text = "";
    var action = "";
    switch (data.notificationType) {
        case 0:
            text = "There is no reason for this notification but oke!";
            break;
        case 1:
            text = ((data.isOwner) ? '<span class="bold">' + data.initiator.userName + '</span> has updated <span class="bold">' + data.idea.titel + "</span>" : "You updated " + data.idea.titel);
            action = "/p/" + data.idea.id + "/" + data.idea.titel.trim().replace(/\s/g, "_");
            break;
        case 2:
            text = ((data.isOwner) ? '<span class="bold">' + data.initiator.userName + '</span> has posted <span class="bold">' + data.idea.titel + "</span>" : "You posted " + data.idea.titel);
            break;
        case 3:
            text = ((data.isOwner) ? '<span class="bold">' + data.initiator.userName + "</span> started following! " : "You started following " + data.owner.userName);
            break;
        case 4:
            text = ((data.isOwner) ? '<span class="bold">' + data.initiator.userName + '</span> commented on <span class="bold">' + data.idea.titel + "</span>" : "You commented on " + data.idea.titel);
            break;
        case 5:
            text = ((data.isOwner) ? '<span class="bold">' + data.initiator.userName + '</span> has favorited <span class="bold">' + data.idea.titel + "</span>" : "You favorited on " + data.idea.titel);
            break;
        
    }

    var holder = $('<a>', { "href": "javascript:;" });
    var imageHolder = $('<div>', { class: "notificationImageHolder" });
    var image = $('<img>', { "src": "/images/uploads/superthumbnails/" + data.initiator.image.fileName });
    imageHolder.append(image);
    var rightholder = $('<div>', { class: "notificationRightholder" });
    var textHolder = $('<div>', { class: "notificationtextholder" }).html(text);
    var dateHolder = $('<div>', { class: "notificationdateholder" }).append(createDate(data.dateTime));
    rightholder.append(textHolder).append(dateHolder);
    holder.append(imageHolder).append(rightholder);
    return holder;
}