var array = [];

function loadComments() {
    self = this;
    var input = { "IdeaId": self.getAttribute("data-idea") };
    if (!jQuery.inArray(self.getAttribute("data-idea"), array)) {
        //collapse or something 
    } else {
        array.push(self.getAttribute("data-idea"));
        var holder = $(self).parent().parent();
        var commentholder = holder.find('div.commentsHolder')[0];
        $(commentholder).append(createLoader());
        $.ajax({
            url: '/Home/LoadComments',
            method: 'POST',
            contentType: "application/json",
            data: JSON.stringify(input),
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
                var json = JSON.parse(e)
                $(commentholder).html("");
                $.each(json[0].reverse(), function (e, comment) {
                    $(commentholder).append(createComment(comment));
                });
            }
        });
    }
}