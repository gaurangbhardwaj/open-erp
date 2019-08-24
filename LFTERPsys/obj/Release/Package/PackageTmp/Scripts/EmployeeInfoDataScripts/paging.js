$(document).ready(function () {

   $('#myTable').DataTable({
    
        "language": {
            lengthMenu: "Show _MENU_ Records",
            search: "_INPUT_",
            searchPlaceholder: "Search..."},
            
        'processing': true,
        'columns': [

            { data: 'empphoto' }, /* index = 0 */

            { data: 'empid' }, /* index = 1 */

            { data: 'empname' }, /* index = 2 */

            { data: 'divname' }, /* index = 3 */

           

            { data: 'desgname' }, /* index = 4 */

            { data: 'empreporting' }, /* index = 5 */

            { data: ''} /* index = 6 */

        ],

        'columnDefs': [{

            'targets': [0,3,4,5,6], /* column index */

            'orderable': false, /* true or false */
        }]
    });

});