function deleteUser(obj) {
    userId = obj.getAttribute('userId');
    swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: "/Administration/DeleteUser",
                method: "POST",
                headers: {
                    "RequestVerificationToken": "@GetAntiXsrfRequestToken()"
                },
                data: {
                    id: userId
                }

            }).done(function () {
                Swal.fire(
                    'the user has been deleted!',
                    '',
                    'success'
                )
            }).fail(function () {
                Swal.fire(
                    'Error!',
                    'someting went wrong please try again later.',
                    'error'
                )
            });
        }
    })

}


$(document).ready(function () {
    //pagination table
    $('#paginationFullNumbers').DataTable({
        "pagingType": "full_numbers"
    });
    $('.dataTables_length').addClass('bs-select');

    $('#delete').click(function () {
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (result.isConfirmed) {
                $('#deletForm').submit();
            }
        })
    })
})


