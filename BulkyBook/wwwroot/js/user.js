var dataTable;

$(document).ready(function () {
    loadDataTable();
});


function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/User/GetAll"
        },
        "columns": [
            { "data": "name", "width": "10%" },
            { "data": "email", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            { "data": "company.name", "width": "10%" },
            { "data": "role", "width": "10%" },
            {
                "data": {
                    id: "id", role: "role"
                },
                "render": function (data) {
                    return `
                            <div class="text-center">
                                <a href="/Admin/User/Upsert/${data.id}" class="btn btn-success text-white" style="cursor:pointer">
                                    <i class="fas fa-edit"></i> 
                                </a>                               
                            </div>
                           `;
                }, "width": "20%"
            }
        ]
    });
}

