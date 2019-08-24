  $(document).ready(function () {
            var events = [];
            $.ajax({
                type: "GET",
                url: "/Home/GetEvents",
                success: function (data) {
                    $.each(data, function (i, v) {
                        events.push({
                            eventID: v.eventid,
                            title: v.subject,
                            description: v.description,
                            start: moment(v.startdate),
                            end: v.enddate != null ? moment(v.enddate) : null,
                            color: v.themecolor,
                        });
                    })

                    GenerateCalender(events);
                },
                error: function (error) {
                    alert('failed');
                }
            })

            function GenerateCalender(events) {
                $('#calender').fullCalendar('destroy');
                $('#calender').fullCalendar({
                    contentHeight: 400,
                    handleWindowResize: true,
                    defaultDate: new Date(),
                    timeFormat: 'h(:mm)a',
                    header: {
                        left: 'prev,next today',
                        center: 'title',
                        right: 'month,basicWeek,list'
                    },
                    displayEventTime: false,
                    showNonCurrentDates: false,
                    fixedWeekCount: false,
                    eventLimit: true,
                    eventColor: '#378006',
                    events: events,
                    eventClick: function (calEvent, jsEvent, view) {

                        $('#myModal #eventTitle').text(calEvent.title);
                        var $description = $('<div/>');
                        $description.append($('<p/>').html('<b>Start:</b>' + calEvent.start.format("DD-MMM-YYYY HH:mm a")));
                        if (calEvent.end != null)
                        {
                          $description.append($('<p/>').html('<b>End:</b>' + calEvent.end.format("DD-MMM-YYYY HH:mm a")));
                        }
                        $description.append($('<p/>').html('<b>Description:</b>' + calEvent.description));
                        $('#myModal #pDetails').empty().html($description);

                        $('#myModal').modal();
                    }
                })
            }
  })