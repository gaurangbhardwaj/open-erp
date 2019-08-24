$(document).ready(function () {
    $('#Project').DataTable({
        "language": { "lengthMenu": "Show _MENU_ Records per page", },
        'processing': true,
        'columns': [

            {data: 'projid'}, /* index = 0 */

            { data: 'projname' }, /* index = 1 */

            { data: 'erpprojname' }, /* index = 2 */

            { data: 'depname' }, /* index = 3 */

            { data: 'projstatus' }, /* index = 4 */

            {data: ''} /* index = 5 */
        ],

        'columnDefs': [{

            'targets': [5], /* column index */

            'orderable': false, /* true or false */

        }]

    });

});