var datatable;
$(document).ready(function () {
    const urlParams = new URLSearchParams(window.location.search);
    const status = urlParams.get('status');
    loadDataTable(status);
});

function loadDataTable(status) {
    datatable = $('#tblBookings').DataTable({
        "ajax": {
            url: '/booking/getall?status='+status
        },
        "columns": [
            { data: 'bookingId', "width": "5%" },
            { data: 'name', "width": "10%" },
            { data: 'phone', "width": "10%" },
            { data: 'email', "width": "5%" },
            { data: 'nights', "width": "5%" },
            { data: 'status', "width": "5%" },
            { data: 'checkInDate', "width": "10%" },
            { data: 'checkOutDate', "width": "10%" },
            { data: 'totalCost', "width": "10%" },





            {
                data: 'bookingId',
                "render": function (data) {
                    return `<div class="w-75 btn-group">
                        <a href="/booking/bookingDetails?bookingId=${data}" class="btn btn-outline-warning mx-2"><i class="bi bi-file-earmark-word"></i>Details</a>
                    </div>`
                }
            }


        ]
    });
}
