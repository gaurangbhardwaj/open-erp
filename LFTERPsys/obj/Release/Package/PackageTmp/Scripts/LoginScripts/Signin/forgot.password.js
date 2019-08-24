
function ForgotPassword()
{
    var locale = {
        OK: 'null',
        CONFIRM: 'Send Verifiaction',
        CANCEL: 'Cancel'
    };

    bootbox.addLocale('custom', locale);

    bootbox.prompt({
        className: 'pulse animated',
        centerVertical: true,
        title: "Employee ID",
        size: "small",
        locale: 'custom',
        callback: function (result) {
            if (result != null) {
                if (result != '')
                    alert('This was logged in the callback: ' + result);
                else {
                    alert('Employee ID is required')
                    ForgotPassword();
                }
            }
        }
    });
}
