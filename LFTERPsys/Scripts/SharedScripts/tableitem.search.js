$(document).ready(function () {
    $("#SearchBox").on("keyup", function () {
        var value = $(this).val().toLowerCase();
        $("#items tr").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        }); 
    });
});