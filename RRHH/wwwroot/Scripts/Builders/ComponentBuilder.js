const ComponentBuilder = function () {

    const ConfigureSelect = function (id) {
        $("#" + id).select2({ theme: 'bootstrap-5' });
    }

    // Crear un select2 ascoiado a un modal configurado a bootstrap 5 mediante el ID del elemento HTML
    const ConfigureSelectWithModal = function (id, parent_id) {
        $("#" + id).select2({ theme: 'bootstrap-5', dropdownParent: $("#" + parent_id + " .modal-content") });
    }

    const SetAsModal = function (id) {
        return new bootstrap.Modal("#" + id);
    }

    const ConfigureDataTable = function (id) {
        let jqueryIdString = "#" + id;
        $(jqueryIdString).DataTable({
            responsive: true, lengthChange: true, autoWidth: false,
            lengthMenu: [10, 20, 50, 100, 200, 500, 1000],
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
    }

    const LibroAsDataTable = function (id) {
        let jqueryIdString = "#" + id;
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
            },
            buttons: [
                'copy', 'csv', 'excel', 'pdf', 'print'
            ]
        });
        $(jqueryIdString).addClass("table table-bordered gyc-dtable");
    }

    return {
        // -- SELECT2
        // Crear un select2 configurado a bootstrap 5 mediante el ID del elemento HTML
        configurarSelect: function (id) {
            ConfigureSelect(id);
        },

        configurarSelectEnModal: function (id, parent_id) {
            ConfigureSelectWithModal(id, parent_id);
        },

        // -- MODALS
        // Crea un modal bootstrap 5 en base al ID y lo retorna
        setModal: function (id) {
            return SetAsModal(id);
        },

        // -- TABLAS
        // Configurar una tabla para que sea DataTable
        configurarDataTable: function (id) {
            ConfigureDataTable(id);
        },

        configurarLibroDataTable: function (id) {
            LibroAsDataTable(id);
        }
    }
}();