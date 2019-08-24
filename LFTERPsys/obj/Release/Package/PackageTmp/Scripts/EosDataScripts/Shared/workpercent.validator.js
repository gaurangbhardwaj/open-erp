
var i = 0, j = 0, sum = 0;
var element;
var validaterow = 1;
var projectidcol = 0;
$(document).ready(function () {
    validate_workpercent();
    validate_workpercent_row();
});

function validate_workpercent() {
    for (var j = 1; j <= 12; j++) //For all months(Jan-Dec)
    {
        for (i = 2; i < $("#workpercent tr").length; i++) // For all Work Percent of each project id (Previous data)
        {
            element = $('#workpercent').find("tr:eq(" + i + ")").find("td:eq(" + j + ")").find("input").val();
            if (element == parseInt(element)) {
                sum = sum + parseInt(element);
            }
        }

        $('#workpercent').find("tr:eq(" + validaterow + ")").find("td:eq(" + j + ")").html(sum + "%");  //Display sum %

        if (sum == 100 || sum == 0) {
            $('#workpercent').find("tr:eq(" + validaterow + ")").find("td:eq(" + j + ")").css('color', 'green');
        }
        else {
            $('#workpercent').find("tr:eq(" + validaterow + ")").find("td:eq(" + j + ")").css('color', 'red');
        }
        sum = 0;
    }
    validate_workpercent_row();
}

function validate_workpercent_row() {
    var cnt = 0;
    for (j = 1; j <= 12; j++) {
        element = $('#workpercent').find("tr:eq(" + validaterow + ")").find("td:eq(" + j + ")").html();
        if (element == "100%" || element == "0%") {
            cnt = cnt + 1;
        }
    }

    if (cnt == 12) {
        $('#workpercentwarning').attr('hidden', true)
        return true;
    }
    else {
        $('#workpercentwarning').addClass('alert-warning').removeClass('alert-danger');
        $('#workpercentwarning').attr('hidden', false)
        return false;
    }
}

function validate_projectid() {
    var project_arr = [];

    for (i = 2; i < $("#workpercent tr").length; i++) // For all project id (Previous data)
    {
        element = $('#workpercent').find("tr:eq(" + i + ")").find("td:eq(" + projectidcol + ")").find("select").val();
        project_arr.push(element);
    }

    var duplicacy_check = 0;

    Projectid_Validator_Loop:
    for (i = 0; i < project_arr.length; i++) {
        for (j = i + 1; j < project_arr.length; j++) {
            if (project_arr[i] == project_arr[j]) {
                duplicacy_check = 1;
                break Projectid_Validator_Loop;
            }
        }
    }

    if (duplicacy_check == 1) {
        $('#projectidwarning').addClass('alert-warning').removeClass('alert-danger');
        $('#projectidwarning').attr('hidden', false);
        return false;
    }
    else {
        $('#projectidwarning').attr('hidden', true);
        return true;
    }
}