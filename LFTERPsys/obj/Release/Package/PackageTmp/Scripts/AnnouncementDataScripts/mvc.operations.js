var token = $("input[name=__RequestVerificationToken]").val();
var sessionid = $("#SessionID");

$(document).ready(function () {
    $('#Announcement').on('click', '#AnnouncementDelete', function (e) {

        var _this = this;

        bootbox.confirm({
            title: "<strong>Are You Sure?</strong>",
            message: "Do you really want to delete suject : <strong>" + $(_this).closest('tr').find('td:eq(1)').text().trim() + "</strong> announcement? This process cannot be undone.",
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
                    var jsonValue = { 'id': "" + $(_this).closest('tr').find('td:eq(0)').text().trim() + "" };
                    $.ajax
                        ({
                            type: "POST",
                            url: "/AnnouncementData/AnnouncementDataDelete/" + sessionid.val(),
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