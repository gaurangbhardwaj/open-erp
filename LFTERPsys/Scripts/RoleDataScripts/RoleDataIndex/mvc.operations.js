var token = $("input[name=__RequestVerificationToken]").val();
var sessionid = $("#SessionID");

$(document).ready(function () {
    $('#Role').on('click', '#RoleDelete', function (e) {
        
        var _this = this;
        
        bootbox.confirm({
            title: "<strong>Are You Sure?</strong>",
            message: "Do you really want to delete <strong>" + $(_this).closest('tr').find('td:eq(0)').text().trim() + "</strong> Role? This process cannot be undone.",
            className: 'rubberBand animated',
            centerVertical: true,
            buttons: {
                cancel: {
                    label: "Cancel",
                    className: "btn-success"
                },
                confirm: {
                    label: 'Delete',
                    className: "btn-danger"
                }
            },
            callback: function (result) {
                if (result) {
                    var jsonValue = { 'rolename': "" + $(_this).closest('tr').find('td:eq(0)').text().trim() + "" };
                    $.ajax
                        ({
                            type: "POST",
                            url: "/RoleData/RoleDataDelete/" + sessionid.val(),
                            dataType: "json",
                            data: JSON.stringify(jsonValue),
                            contentType: "application/json; charset=utf-8",
                            headers: { '__RequestVerificationToken': token },
                            success: function () {
                                $(_this).closest('tr').remove();
                            }
                        });
                }
            }
        });
    });
});