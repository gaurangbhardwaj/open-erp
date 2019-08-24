$(document).ready(function () {
    $('#multiselect').multiselect({
    });
});

function getsetpageaccess() {
    $('#pageaccess').val($('#multiselect').val());
}