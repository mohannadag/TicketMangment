/// this function to handle adding note in the edit view of the ticket
function addNote(obj) {
    tickid = obj.getAttribute('ticketId');
    Swal.fire({
        title: 'New Note',
        input: 'textarea',
        inputLabel: 'Message',
        inputPlaceholder: 'Type your note here...',
        inputAttributes: {
            'aria-label': 'Type your note here'
        },
        showCancelButton: true
    }).then((result) => {
        console.log(result);
        if (result.isConfirmed) {

            $.ajax({
                url: "/ticket/CreateNote",
                method: "POST",
                headers: {
                    "RequestVerificationToken": "@GetAntiXsrfRequestToken()"
                },
                data: {
                    model: result.value,
                    ticketId: tickid
                }

            }).done(function () {
                Swal.fire(
                    'New note created!',
                    '',
                    'success'
                )
                $('#notesTable').append(
                    '<tr><td>' + result.value + '</tr></td>'
                );
            }).fail(function () {
                Swal.fire(
                    'Error!',
                    'someting went wrong please try again later.',
                    'error'
                )
            });
        }
    });

}