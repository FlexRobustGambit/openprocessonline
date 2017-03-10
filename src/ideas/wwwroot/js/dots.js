function connectDots(id) {
    $('.indicator[data-indicator="' + id + '"]').each(function (index, indicator) {
        var next = $('.indicator[data-indicator="' + id + '"]').eq(index + 1);
        if (next.length) {
            var height = $(next).offset().top - $(indicator).offset().top;
            if ($(indicator).attr('data-indicator') == $('.indicator[data-indicator="' + id + '"]').eq(index + 1).attr('data-indicator')) {
                var line = $('<div>', { class: "line" }).css({"height": height}); 
                $(indicator).append(line);
            }
        }
    });
}

$(window).resize(function () {
    $('.indicator').each(function (index, indicator) {
        var next = $('.indicator').eq(index + 1);
        if (next.length) {
            var height = $(next).offset().top - $(indicator).offset().top;
            if ($(indicator).attr('data-indicator') == $('.indicator').eq(index + 1).attr('data-indicator')) {
                $(indicator).find('.line').css({ "height": height });
            }
        }
    });
});