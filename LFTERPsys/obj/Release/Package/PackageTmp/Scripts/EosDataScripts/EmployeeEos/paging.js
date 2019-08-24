$(document).ready(function () {
    $('#employeeEos').DataTable({
        "language": { "lengthMenu": "Show _MENU_ Records per page", },
        'processing': true,
        'columns': [

            { data: 'empid' }, /* index = 0 */

            { data: 'empname' }, /* index = 1 */

            { data: ' ' } /* index = 2 */

        ],

        'columnDefs': [{

            'targets': [2], /* column index */

            'orderable': false, /* true or false */

        }]
    });
});