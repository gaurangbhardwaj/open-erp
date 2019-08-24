var i = 0, j = 0;
var element;
var sum = 0;
let validaterow = 0;

$(document).ready(function () {

    for (i = 3; i < $("#report tr").length; i++) {
        for (j = 2; j < document.getElementById("report").rows[0].cells.length; j++) {
            var element = $('#report').find("tr:eq(" + i + ")").find("td:eq(" + j + ")");
            if (element.html().trim() == "-")
                element.css({ 'color': 'red'});
            else
                element.css({ 'color': 'green'});

        }
    }


    for (var j = 2; j < document.getElementById('report').rows[0].cells.length; j++)
    {
        for (i = 3; i < $("#report tr").length; i++)
        {
            element = $('#report').find("tr:eq(" + i + ")").find("td:eq(" + j + ")").text().trim();
            if (element == parseInt(element)) {
                sum = sum + parseInt(element);
            }
        }

        $('#report').find("tr:eq(" + validaterow + ")").find("td:eq(" + j + ")").html(sum + "%");

        if (sum == 100) {
            $('#report').find("tr:eq(" + validaterow + ")").find("td:eq(" + j + ")").css('color', 'green');
        }
        else {
            $('#report').find("tr:eq(" + validaterow + ")").find("td:eq(" + j + ")").css('color', 'red');
        }
        sum = 0;
    }

    $("#report").attr('border', '1');
    $("input[name='GridHtml']").val($("#reportGrid").html());
    $("#report").attr('border', '0');
});