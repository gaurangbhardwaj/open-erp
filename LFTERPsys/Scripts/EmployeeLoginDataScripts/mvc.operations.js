var token = $("input[name=__RequestVerificationToken]").val();
var sessionid = $("#SessionID");

$(document).ready(function () {
    $('#UserLoginData').on('click', '#ResetPassword', function (e) {

        var _this = this;
        bootbox.confirm({
            title: "<strong>Are You Sure?</strong>",
            message: "Do you really want to reset empid : <strong>" + $(_this).closest('tr').find('td:eq(1)').text().trim() + "'s</strong> Password? This process cannot be undone.",
            className: 'rubberBand animated',
            centerVertical: true,
            buttons: {
                cancel: {
                    label: "Cancel",
                    className: "btn-success"
                },
                confirm: {
                    label: 'Reset',
                    className: "btn-danger"
                }
            },
            callback: function (result) {
                if (result) {
                    var jsonValue = { 'empid': "" + $(_this).closest('tr').find('td:eq(1)').text().trim() + "" };
                    $.ajax
                        ({
                            type: "POST",
                            url: "/EmployeeLoginData/EmployeeLoginDataPasswordReset/" + sessionid.val(),
                            dataType: "json",
                            data: JSON.stringify(jsonValue),
                            contentType: "application/json; charset=utf-8",
                            headers: { '__RequestVerificationToken': token },
                            success: function () {
                                bootbox.alert({
                                    message: "Password has been reset successfully for empid : <strong>" + $(_this).closest('tr').find('td:eq(1)').text().trim() +"</strong>.",
                                    centerVertical: true
                                });
                            }
                        });
                }
            }
        });
    });
});