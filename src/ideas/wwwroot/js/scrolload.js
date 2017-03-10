var windowHeight = 0;
var documentHeight = 0;
var step = 100;
var laststep = 0;
var loadAfterPercentage = 80;
var plsload = true;
var endofline = false;
var loaded = 0;
var startPoint;
var appuser;
var scrollinit = false;
var loader;


function initScrollLoad(start, appuser) {
    this.appuser = appuser;
    this.scrollinit = true;
    this.loaded = start;
    this.loader = createLoader();
    var d = new Date();
    d.setTime(d.getTime() - new Date().getTimezoneOffset() * 60 * 1000);
    startPoint = d;
    plsload = false;
    $('#ScrollHolder').after(loader);
    loadnew();
}

$(window).scroll(function () {
    if (scrollinit) {
        if ($(document).scrollTop() - step > laststep) {
            laststep = $(document).scrollTop();
            var thumbPosition = windowHeight + $(document).scrollTop();
            var percentage = thumbPosition / documentHeight * 100;
            if (percentage > loadAfterPercentage && plsload) {
                plsload = false;
                loadnew();
            }
        }
    }
});

function loadnew() {
    this.loader.show();
    var input = { "Start": loaded, "StartPoint": startPoint, "AppUser": this.appuser };
    $.ajax({
        url: Settings.PathLazy,
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
            if (e.length < Settings.scrollload) {
                if (!endofline) {
                    loader.hide();
                    $("#loaderholder").append(createEndOfLine());
                    endofline = true;
                }
            } else {
                plsload = true;
            }
            loaded += e.length;
            windowHeight = $(window).height();
            documentHeight = $(document).height();
            $.each(e, function (i, item) {
                var prucess = show(item);
                prucess.hide();
                $("#ScrollHolder").append(prucess);
                $('.niceandsquare').keepsquare();
                $('.personal').keepsquare()
                connectDots(item.idea.id);
                prucess.fadeIn();
            });
            $('.personal').keepsquare();
            initPhotoSwipeFromDOM('.post-gallery');
        }
    });
}