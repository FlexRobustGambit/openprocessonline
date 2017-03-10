
function addFav() {
    self = this;
    $(self).html(createLoader());
    var favto = { "IdeaId": self.getAttribute("data-idea") };
    $.ajax({
        url: '/Home/Favorite',
        method: 'post',
        data: JSON.stringify(favto),
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
            self.innerHTML = '<i class="fa fa-star" aria-hidden="true"></i>Added to favorites!';
            $(self).off("click", addFav);
            $(self).on("click", removeFav);
            console.log("succes");
        }
    }); 
}

function removeFav() {
    self = this;
    var favto = { "IdeaId": self.getAttribute("data-idea") };
    $.ajax({
        url: '/Home/RemoveFavorite',
        method: 'post',
        data: JSON.stringify(favto),
        dateType: "json",
        headers: {
            "Accept": "application/json",
            "Content-Type": "application/json",
            "RequestVerificationToken": $('#__AjaxAntiForgeryForm input[name="__RequestVerificationToken"]').val()
        },
        error: function (e) {
            console.log(e);
        },
        success: function (e) {
            self.innerHTML = '<i class="fa fa-star-o" aria-hidden="true"></i>Add to favorites';
            $(self).off("click", removeFav);
            $(self).on("click", addFav);
        }
    });
}
