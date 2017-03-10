function initDropZone() {
    var loader = createLoader();
    $('#dropZone').filedrop({
        url: '/Home/UploadFiles',
        xhrSetup: [],
        paramname: 'files',
        maxfiles: 6,
        maxfilesize: 4,
        allowedfiletypes: ['image/jpeg', 'image/jpg', 'image/png'],
        allowedfileextensions: ['.jpg', '.jpeg', '.png'],
        headers: {
            "Accept": "application/json",
            "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
        },
        dragOver: function () {
            var filterVal = 'blur(1px)';
            $(' #imagepreview img')
              .css('filter', filterVal)
              .css('webkitFilter', filterVal)
              .css('mozFilter', filterVal)
              .css('oFilter', filterVal)
              .css('msFilter', filterVal);
            $('#dropZone').css('border-color', 'green');
        },
        dragLeave: function () {
            var filterVal = 'blur(0px)';
            $('#dropZone , #imagepreview img')
              .css('filter', filterVal)
              .css('webkitFilter', filterVal)
              .css('mozFilter', filterVal)
              .css('oFilter', filterVal)
              .css('msFilter', filterVal);
            $('#dropZone').css('border-color', '#cccccc');
        },
        drop: function () {
            var filterVal = 'blur(0px)';
            $('#dropZone , #imagepreview img')
              .css('filter', filterVal)
              .css('webkitFilter', filterVal)
              .css('mozFilter', filterVal)
              .css('oFilter', filterVal)
              .css('msFilter', filterVal);
            $('#dropZone').css('border-color', '#cccccc');
            $('#dropZone').append(loader);
        },
        afterAll: function () {
            //$('#dropZone').html('The file(s) have been uploaded successfully!');
            loader.remove();
        },
        uploadFinished: function (i, file, response, time) {
           
            if (typeof (Storage) !== "undefined") {
                if (localStorage.getItem("images") !== "") {
                    var list = JSON.parse(localStorage.getItem("images"));
                    if (list == null) {
                        list = response;
                        localStorage.setItem("images", list);
                        list = JSON.stringify(list);
                    } else {
                        list = list.concat(JSON.parse(response));
                        list = JSON.stringify(list);
                        localStorage.setItem("images", list);
                    }
                    $("#JsonImages").val(list);
                }
            }

            if ($("#JsonImagesNew").val() == "") {
                $("#JsonImagesNew").val(response);
            } else {
                var addList = JSON.parse($("#JsonImagesNew").val());
                var added = addList.concat(JSON.parse(response));
                $("#JsonImagesNew").val(JSON.stringify(added));
            }

            $.each(JSON.parse(response), function (i, item) {
                $('#imagepreview').append(createPreviewImage(item));
                $('.niceandsquare').keepsquare();
            });
        }
    });
}



$(document).ready(function () {
    $('#dropZone2').keepsquare();
    if (typeof (Storage) !== "undefined" && localStorage.getItem("images") !== "") {
        try {
            var list = JSON.parse(localStorage.getItem("images"));
            if (list !== null) {
                $.each(list, function (e, item) {
                    $('#imagepreview').append(createPreviewImage(item));
                    $('.niceandsquare').keepsquare();
                });
                list = JSON.stringify(list);
                $("#JsonImages").val(list);
            }
        }
        catch (err) {
            localStorage.clear();
            console.log("cleared");
        }
      
    }
   /*
    $(function () {
        $('#dropZone').filedrop({
            url: '/Home/UploadFiles',
            xhrSetup: [],
            paramname: 'files',
            maxfiles: 6,
            maxfilesize: 4,
            allowedfiletypes: ['image/jpeg', 'image/jpg', 'image/png'],  
            allowedfileextensions: ['.jpg', '.jpeg', '.png'],
            headers: {
                "Accept": "application/json",
                "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
            },
            dragOver: function () {
                var filterVal = 'blur(1px)';
                $(' #imagepreview img')
                  .css('filter', filterVal)
                  .css('webkitFilter', filterVal)
                  .css('mozFilter', filterVal)
                  .css('oFilter', filterVal)
                  .css('msFilter', filterVal);
                $('#dropZone').css('border-color', 'green');
            },
            dragLeave: function () {
                var filterVal = 'blur(0px)';
                $('#dropZone , #imagepreview img')
                  .css('filter', filterVal)
                  .css('webkitFilter', filterVal)
                  .css('mozFilter', filterVal)
                  .css('oFilter', filterVal)
                  .css('msFilter', filterVal);
                $('#dropZone').css('border-color', '#cccccc');
            },
            drop: function () {
                var filterVal = 'blur(0px)';
                $('#dropZone , #imagepreview img')
                  .css('filter', filterVal)
                  .css('webkitFilter', filterVal)
                  .css('mozFilter', filterVal)
                  .css('oFilter', filterVal)
                  .css('msFilter', filterVal);
                $('#dropZone').css('border-color', '#cccccc');
            },
            afterAll: function () {
                //$('#dropZone').html('The file(s) have been uploaded successfully!');
            },
            uploadFinished: function (i, file, response, time) {
                if (typeof (Storage) !== "undefined") {
                    console.dir(localStorage.getItem("images"));
                    var list;
                    if (localStorage.getItem("images") == "") {
                        list = response;
                        localStorage.setItem("images", list);
                        list = JSON.stringify(list);
                    } else {
                        list = JSON.parse(localStorage.getItem("images"));
                        list = list.concat(JSON.parse(response));
                        list = JSON.stringify(list);
                        localStorage.setItem("images", list);
                    }
                    $("#JsonImages").val(list);
                }

                $.each(JSON.parse(response), function (i, item) {
                    $('#imagepreview').append(createPreviewImage(item));
                    $('.niceandsquare').keepsquare();
                });
            }
        })*/

    $('#dropZone2').filedrop({
        url: '/Account/UploadProfileImage',
        xhrSetup: [],
        paramname: 'files',
        maxfiles: 1,
        maxfilesize: 4,
        allowedfiletypes: ['image/jpeg', 'image/jpg', 'image/png'],
        allowedfileextensions: ['.jpg', '.jpeg', '.png'],
        headers: {
            "Accept": "application/json",
            "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
        },
        dragOver: function () {
            var filterVal = 'blur(1px)';
            $(' #imagepreview2 img')
              .css('filter', filterVal)
              .css('webkitFilter', filterVal)
              .css('mozFilter', filterVal)
              .css('oFilter', filterVal)
              .css('msFilter', filterVal);
            $('#dropZone2').css('border-color', 'green');
        },
        dragLeave: function () {
            var filterVal = 'blur(0px)';
            $('#dropZone2 , #imagepreview2 img')
              .css('filter', filterVal)
              .css('webkitFilter', filterVal)
              .css('mozFilter', filterVal)
              .css('oFilter', filterVal)
              .css('msFilter', filterVal);
            $('#dropZone2').css('border-color', '#cccccc');
        },
        drop: function () {
            var filterVal = 'blur(0px)';
            $('#dropZone2 , #imagepreview2 img')
              .css('filter', filterVal)
              .css('webkitFilter', filterVal)
              .css('mozFilter', filterVal)
              .css('oFilter', filterVal)
              .css('msFilter', filterVal);
            $('#dropZone2').css('border-color', '#cccccc');
        },
        afterAll: function () {
            //$('#dropZone').html('The file(s) have been uploaded successfully!');
        },
        
        uploadFinished: function (i, file, response, time) {
            /*
            if (localStorage.getItem("images") !== "") {
                var list = JSON.parse(localStorage.getItem("images"));
                if (list == null) {
                    list = response;
                    localStorage.setItem("images", list);
                    list = JSON.stringify(list);
                } else {
                    list = list.concat(JSON.parse(response));
                    list = JSON.stringify(list);
                    localStorage.setItem("images", list);
                }
                $("#JsonImages").val(list);
            } else {
                list = JSON.parse($("#JsonImages").val());
                list = list.concat(JSON.parse(response));
                $("#JsonImages").val(JSON.stringify(list));
            } */

            $.each(JSON.parse(response), function (i, item) {
                $('#imagepreview2').html(createPreviewImage(item));
                $("#JsonImages").val(response);
                $('.imagelibrary1').keepsquare();
            });

            /*                //$('.niceandsquare').keepsquare();
            
            if (typeof (Storage) !== "undefined") {
                localStorage.getItem("images")
                var list = JSON.parse(localStorage.getItem("images"));
                if (list == null) {
                    list = response;
                    localStorage.setItem("images", list);
                    list = JSON.stringify(list);
                } else {
                    list = list.concat(JSON.parse(response));
                    list = JSON.stringify(list);
                    localStorage.setItem("images", list);
                }
                $("#JsonImages").val(list);
            } else {
                list = JSON.parse($("#JsonImages").val());
                list = list.concat(JSON.parse(response));
                $("#JsonImages").val(JSON.stringify(list));
            }*/
            /*
            if ($("#JsonImageNew").val() == "") {
                $("#JsonImageNew").val(JSON.stringify(response));
            } else {
                var addList = JSON.parse($("#JsonImageNew").val());
                var added = addList.concat(JSON.parse(response));
                $("#JsonImageNew").val(JSON.stringify(added));
            }*/

        
        }
    });
    
});

function removeFromUpload() {
    var self = this;
    var removeimage = { "FileName":  self.getAttribute("data-name")};
    if ($("#JsonImagesRemove").val() == "") {
        console.log("1 list");
        $("#JsonImagesRemove").val(JSON.stringify([removeimage]));
    } else {
        var removelist = JSON.parse($("#JsonImagesRemove").val());
        removelist.push(removeimage);
        $("#JsonImagesRemove").val(JSON.stringify(removelist));
    }
    $(self).parent().remove();


    /*
      list = null;
    if (localStorage != "undefined") {
        list = JSON.parse(localStorage.getItem("images"));
    }
    $.ajax({
        url: '/Home/RemoveImage',
        method: 'post',
        data: JSON.stringify(removeimage),
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
            if (localStorage != "undefined") {
                var list = JSON.parse(localStorage.getItem("images"));
                if (list !== null) {
                    tmp = [];
                    $('#imagepreview').html("");
                    $.each(list, function (x, item) {
                        if (item.fileName !== removeimage.FileName) {
                            tmp.push(item);
                            $('#imagepreview').append(createPreviewImage(item));
                        }
                    });
                    $('.niceandsquare').keepsquare();
                    list = JSON.stringify(tmp);
                    localStorage.setItem("images" , list);
                    $("JsonImages").val(list);
                }
            } else {
                var list = JSON.parse($("#JsonImages").val());
                list = list.concat(JSON.parse(response));
                $("#JsonImages").val(JSON.stringify(list));
            }
            console.log("succes");
        }
    });*/
}