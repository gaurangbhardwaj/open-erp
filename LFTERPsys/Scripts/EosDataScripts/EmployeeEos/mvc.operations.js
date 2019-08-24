 var i = 0, j = 0;
var element;
var validaterow = 1;

var token = $("input[name=__RequestVerificationToken]").val();
var sessionid = $("#SessionID");
var empid = $("#EmpID");


$(document).ready(function () {

    $("#EnrollNew").click(function () //Onclick function to add new row
    {
        $("#DummyAddWorkpercentrow").clone().appendTo("#workpercent");

        for (var j = 0; j <= 12; j++)
            $('#workpercent').find("tr:eq(" + (($("#workpercent tr").length) - 1) + ")").find("td:eq(" + j + ")").find("input").val(0);

        $("#workpercent").find("tr:eq(" + (($("#workpercent tr").length) - 1) + ")").css('display', '');
    });

    $('#workpercent').on('click', '#RemoveNewEnrollment', function () {
        $(this).closest('tr').remove();
        validate_workpercent();
        validate_projectid();
    });


    $('#workpercent').on('click', '#RemoveEnrolled', function () {

        var _this = this;
        bootbox.confirm({
            title: "<strong>Are You Sure?</strong>",
            message: "Do you really want to delete <strong>" + $(_this).closest('tr').find('td:eq(0)').find('select').val() + "</strong> project enrollment? This process cannot be undone.",
            className: 'rubberBand animated',
            centerVertical: true,
            buttons: {
                cancel: {
                    label: "Cancel",
                    className: "btn-success"
                },
                confirm: {
                    label: "Delete",
                    className: "btn-danger"
                }
            },
            callback: function (result) {
                if (result) {
                    var jsonRemoveValuesObject = { empid: "" + empid.val() + "", projname: "" + $(_this).closest('tr').find("td:eq(0)").find("select").val() + "" };
                    $.ajax({
                        type: "POST",
                        url: "/EosData/EmployeeEosDelete/" + sessionid.val(),
                        data: JSON.stringify(jsonRemoveValuesObject),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        headers: { '__RequestVerificationToken': token },
                        success: function (response) {
                            if (response = "Success") {
                                $(_this).closest('tr').remove();
                                $('#deletesuccess').css('display', '');
                                $('#deletesuccess').fadeOut(5000);
                            }
                        },
                        error: function (jqXHR, textStatus, errorThrown) {
                            $('#deleteerror').css('display', '');
                            $('#deleteerror').fadeOut(5000);
                        }
                    });
                }
            }
        });
    });

    $('#workpercent').on('click', '#RemoveRow', function () {
        $(this).closest('tr').remove();
        validate_workpercent();
        validate_projectid();
    });

    $("#RollBack").click(function () {
        window.location = "/EosData/AllEmployeesEOS/" + sessionid.val();
    });

    $("#ExportEOSbtn").click(function () {
        validate_workpercent();
        var check = 0;

        if (validate_workpercent_row()) // Validation Warning < Danger for each work percent (select) validation.
            check = check + 1;
        else
            $('#workpercentwarning').addClass('alert-danger').removeClass('alert-warning');


        if (validate_projectid()) // Validation Warning < Danger for each projectid (select) validation.
            check = check + 1;
        else
            $('#projectidwarning').addClass('alert-danger').removeClass('alert-warning');



        if (check == 2) {
            $('#workpercentwarning').addClass('alert-danger').removeClass('alert-warning');
            $('#projectidwarning').addClass('alert-danger').removeClass('alert-warning');

            var tab_text = "<table border='2px'>";
            var j = 0;
            tab = document.getElementById('workpercent'); // id of table

            var element;

            //---------------------------------------------For Edited values table-------------------------------------------------------------
            for (var i = 0; i < 2; i++) {
                tab_text = tab_text + "<tr>";
                for (var j = 0; j <= 12; j++) {
                    if (i == 0) //for project id styling
                    {
                        element = $('#workpercent').find("tr:eq(" + i + ")").find("th:eq(" + j + ")").html();
                        tab_text = tab_text + '<th>' + element + "</th>";
                    }
                    else {
                        element = $('#workpercent').find("tr:eq(" + i + ")").find("td:eq(" + j + ")").html();
                        if (j > 0)
                            tab_text = tab_text + '<td style="font-weight:bold; color:green; background-color:yellow;">' + element + "</td>";
                        else
                            tab_text = tab_text + '<td style="font-weight:bold;">' + element + "</td>";
                    }
                }
                tab_text = tab_text + "</tr>";
            }

            for (var i = 2; i < $("#workpercent tr").length; i++) {
                tab_text = tab_text + "<tr>";
                for (var j = 0; j <= 12; j++) {
                    if (j == 0) //for project id styling
                    {
                        element = $('#workpercent').find("tr:eq(" + i + ")").find("td:eq(" + j + ")").find("select").val();
                        tab_text = tab_text + '<td style="color: darkblue; font-weight: bold;" >' + element + "</td>";
                    }
                    else {
                        element = $('#workpercent').find("tr:eq(" + i + ")").find("td:eq(" + j + ")").find("input").val();
                        tab_text = tab_text + '<td>' + element + "</td>";
                    }
                }
                tab_text = tab_text + "</tr>";
            }
            //------------------------------------------------------------------------------------------------------------------------------

            tab_text = tab_text + "</table>";

            //...................For IE...........................
            var ua = window.navigator.userAgent;
            var msie = ua.indexOf("MSIE ");

            if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) {
                txtArea1.document.open("txt/html", "replace");
                txtArea1.document.write(tab_text);
                txtArea1.document.close();
                txtArea1.focus();
                sa = txtArea1.document.execCommand("SaveAs", true, "MyEOS.xls");
            }
            //.......................................................

            else                 //other browser not tested on IE 11
                sa = window.open('data:application/vnd.ms-excel,' + encodeURIComponent(tab_text));

            return (sa);
        }
    });
});


function saveall(e) {
    e.preventDefault();
    validate_workpercent();
    var check = 0;

    if (validate_workpercent_row()) // Validation Warning < Danger for each work percent (select) validation.
        check = check + 1;
    else
        $('#workpercentwarning').addClass('alert-danger').removeClass('alert-warning');


    if (validate_projectid()) // Validation Warning < Danger for each projectid (select) validation.
        check = check + 1;
    else
        $('#projectidwarning').addClass('alert-danger').removeClass('alert-warning');


    if (check == 2) {
        $('#workpercentwarning').addClass('alert-danger').removeClass('alert-warning');
        $('#projectidwarning').addClass('alert-danger').removeClass('alert-warning');

        if ($("#workpercent tr").length > 2)
            saveworkpercent();
        else
            alert("Please Enroll in some project!!");

        //window.location = "/UserProjectData/Index/" + sessionid.val() + "/";
    }

}

function saveworkpercent() {
    jsonEditedValuesObject = [];

    //for fetching edited and new values to JSON string
    var fetched_row = [], values;
    for (var i = 2; i < $("#workpercent tr").length; i++) {
        for (var j = 0; j <= 12; j++) {
            if (j == 0) {
                element = $('#workpercent').find("tr:eq(" + i + ")").find("td:eq(" + j + ")").find("select").val();
                fetched_row.push(element);
            }
            else {
                element = $('#workpercent').find("tr:eq(" + i + ")").find("td:eq(" + j + ")").find("input").val();
                fetched_row.push(parseInt(element));
            }
        }

        values = {
            "empid": "" + empid.val() + "",
            "projname": "" + fetched_row[0] + "",
            "april": "" + fetched_row[1] + "",
            "may": "" + fetched_row[2] + "",
            "june": "" + fetched_row[3] + "",
            "july": "" + fetched_row[4] + "",
            "august": "" + fetched_row[5] + "",
            "september": "" + fetched_row[6] + "",
            "october": "" + fetched_row[7] + "",
            "november": "" + fetched_row[8] + "",
            "december": "" + fetched_row[9] + "",
            "january": "" + fetched_row[10] + "",
            "february": "" + fetched_row[11] + "",
            "march": "" + fetched_row[12] + "",
        };
        jsonEditedValuesObject.push(values);
        fetched_row = [];
    }

    $.ajax({
        type: "POST",
        url: "/EosData/EmployeeEos/" + sessionid.val(),
        data: JSON.stringify(jsonEditedValuesObject),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        headers: { '__RequestVerificationToken': token },
        success: function (response) {
            if (response = "Success") {

                $('#serversuccess').css('display', '');
                $('#serversuccess').fadeOut(5000);
            }
            else {
                $('#servererror').css('display', '');
                $('#servererror').fadeOut(5000);
            }
        },
    });
}

