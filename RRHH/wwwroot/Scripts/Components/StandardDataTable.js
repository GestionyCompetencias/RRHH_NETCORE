// Esta función debe ser utilizada para todas las tablas de la plataforma
// La idea es que todas las tablas tengan la misma estructura
function StandardDataTable(tableId) {
    let jqueryIdString = "#" + tableId;
    $(jqueryIdString).DataTable({
        responsive: true, lengthChange: true, autoWidth: false,
        dom: '<"datatable-header"fl><"datatable-scroll"t><"datatable-footer"ip>',
        language: {
            paginate:
            {
                previous: 'Atrás',
                next: 'Siguiente',
            },
            info: "Mostrando _START_ de un total de _TOTAL_ registros",
            infoEmpty: "No se tienen registros",
            search: '<span class="me-3">Filtrar:</span> <div class="form-control-feedback form-control-feedback-end flex-fill">_INPUT_<div class="form-control-feedback-icon"><i class="ph-magnifying-glass opacity-50"></i></div></div>',
            zeroRecords: "No se han encontrado registros",
            emptyTable: "No hay datos en esta tabla",
            lengthMenu: '<span class="me-3">Mostrar:</span> _MENU_',
            searchPlaceholder: "Escriba para filtrar"
        }
    });
    $(jqueryIdString).addClass("table table-bordered gyc-dtable");
    $(jqueryIdString).children("thead").addClass("thead-dark")
}