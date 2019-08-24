$(document).ready(function () {
    $('#UserLoginData').DataTable({
        "language": { "lengthMenu": "Show _MENU_ Records per page", },
        'processing': true,
        'columns': [

            { data: 'empid' }, /* index = 0 */

            { data: 'empname' }, /* index = 1 */

            { data: 'role' }, /* index = 2 */

            { data: ' ' } /* index = 3 */

        ],

        'columnDefs': [{

            'targets': [3], /* column index */

            'orderable': false, /* true or false */

        }]

    });

});