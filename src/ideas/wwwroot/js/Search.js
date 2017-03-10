$(function () {
    $("#searchbutton").on("click", search);
});

function search() {
    var s = $(".searchbar").val();
    console.log(s);
    if (s.length > 0) {
        var height = $('.navbar-fixed-top').height();
        var holder = $('#searchOutputContent');
        var loader = createLoader();
        holder.animate({ "padding-top": height + "px", "height": "auto" }, 100).html(loader);
        var searchdata = { "SearchViewModel": { "SearchWord": s } };
        $.ajax({
            url: '/Home/Search',
            method: 'post',
            data: JSON.stringify(searchdata),
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
                e =  highlight(e, s);
                var data = JSON.parse(e);
                for (var x = 0; x < data.length; x++) {
                    holder.append(showSearch(data[x]));
                }
                loader.remove();
            }
        });
    }
}


function showSearch(e) {
    var holder = createShell(e);
    holder.addClass("searchItem");

    var ideaId = holder.find('.ideaItem');
    ideaId.prevObject[0].setAttribute('id', 's'+e.idea.id);
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
    holder.find('section.control')[0].remove();
    holder.find('section.comment')[0].remove();
    return holder;
}


function highlight(e, word) {
    e = Array.from(e);
    var end = e.length;
    var wordend = word.length;
    word = word.toLowerCase();
    var copye = Array.from(e);
    var counter = 0; 
    for (var x = 0; x < end; x++) {
        if (e[x].To == word[0]) {
            var found = true;
            for (var y = 1; y < wordend; y++) {
                if (e[x + y] !== word[y]) {
                    found = false;
                    break;
                }
            }
            if (found) {
                var indexfront = x + counter;
                var indexend = indexfront + wordend + 1 ;
                copye.splice(indexfront, 0, "<hi>");
                copye.splice(indexend, 0, '</hi>');
                console.log("found on " + e[x] + " to " + e[x + wordend - 1]);
                counter = counter + 2;
            }
        }
    }
    return copye.join("");
}



function highlightAndShrink(e, word) {
    var size = 100;
    e = Array.from(e);
    var end = e.length;
    var wordend = word.length;
    var copye = Array.from(e);
    var counter = 0;
    for (var x = 0; x < end; x++) {
        if (e[x] == word[0]) {
            var found = true;
            for (var y = 1; y < wordend; y++) {
                if (e[x + y] !== word[y]) {
                    found = false;
                    break;
                }
            }
            if (found) {
                var indexfront = x + counter;
                var indexend = indexfront + wordend + 1;
                copye.splice(indexfront, 0, "<hi>");
                copye.splice(indexend, 0, '</hi>');
                console.log("found on " + e[x] + " to " + e[x + wordend - 1]);
                counter = counter + 2;
            }
        }
    }
    console.log(copye.join(""));
    return copye.join("");
}








