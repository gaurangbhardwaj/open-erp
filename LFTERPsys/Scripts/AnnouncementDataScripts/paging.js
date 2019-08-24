$(document).ready(function () {
    $('#Announcement').dataTable({

        "language": { "lengthMenu": "Show _MENU_ Records per page", },

        'processing': true,
        'columns': [

            { data: '' }, /* index = 0 */

            { data: 'subject' }, /* index = 1 */

            { data: 'description' }, /* index = 2 */

            { data: 'date' }, /* index = 3 */

            { data: '' } /* index = 4 */

        ],

        'columnDefs': [{

            'targets': [4], /* column index */

            'orderable': false, /* true or false */

        }]

    });

});