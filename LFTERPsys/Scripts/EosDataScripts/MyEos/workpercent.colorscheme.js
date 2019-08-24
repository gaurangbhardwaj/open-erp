
var i = 0, j = 0;
var element;

$(document).ready(function () {

    var currentdate = new Date();
    for (i = 2; i < $("#workpercent tr").length; i++) {
        if (currentdate.getMonth() == 0 || currentdate.getMonth() == 1 || currentdate.getMonth() == 2) // Month for Jan, Feb and Mar
        {
            for (j = 1; j <= (currentdate.getMonth() + 9); j++) //Work Percent(Jan-Mar) change CSS
            {
                var element = $('#workpercent').find("tr:eq(" + i + ")").find("td:eq(" + j + ")").find("input");
                element.attr('disabled', true);
                element.css({ 'color': 'green', 'font-weight': 'bold' });
            }
        }
        else
        {
            for (j = 1; j <= (currentdate.getMonth() - 3); j++) //Work Percent(Apr-Dec) change CSS
            {
                var element = $('#workpercent').find("tr:eq(" + i + ")").find("td:eq(" + j + ")").find("input");
                element.attr('disabled', true);
                element.css({ 'color': 'green', 'font-weight': 'bold' });
            }
        }
    }


    // **********Dummy Row modification***************

    if (currentdate.getMonth() == 0 || currentdate.getMonth() == 1 || currentdate.getMonth() == 2) // Month for Jan, Feb and Mar
    {
        for (j = 1; j <= (currentdate.getMonth() + 9); j++) //Work Percent(Jan-Mar) change CSS
        {
            var element = $('#AddWorkpercent').find("tr:eq(" + 0 + ")").find("td:eq(" + j + ")").find("input");
            element.attr('disabled', true);
            element.css({ 'color': 'green', 'font-weight': 'bold' });
        }
    }
    else
    {
        for (j = 1; j <= (currentdate.getMonth() - 3); j++) //Work Percent(Apr-Dec) change CSS
        {
            var element = $('#AddWorkpercent').find("tr:eq(" + 0 + ")").find("td:eq(" + j + ")").find("input");
            element.attr('disabled', true);
            element.css({ 'color': 'green', 'font-weight': 'bold' });
        }
    }
});
