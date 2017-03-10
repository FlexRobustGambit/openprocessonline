function displayError(errors) {
    $('.errorMessageUL').remove();
  
    for (var x = 0; x < errors.length; x++) {
        var message = $("<ul>", { class: "errorMessageUL" });
        for (var y = 0; y < errors[x].errors.length; y++) {
            console.dir(errors[x].errors[y]);
            var item = $("<li>").text(errors[x].errors[y].errorMessage);
            message.append(item);
        }
        var holder = $('<span>', { class: "errorMessage" }).html(message);
        $('.' + errors[x].subKey.value + 'Error').html(holder);
        console.log(x);
        console.dir(errors[x]);
    }
}