inicio();
var idEmpre = "";
var estado = "";

var contratoModal = ComponentBuilder.setModal("modal_Estados");

function inicio() {
    document.getElementById("esperar").style.display = 'none';
    mostrarDatos();
}


function mostrarDatos() {

    document.getElementById("esperar").style.display = 'block';
    $.get("EmpresaLog", function (data) {
        idEmpre = data;
        estado= document.getElementById("txt_estado").value
        if (estado != "") {
            obtenerDatosValidos(idEmpre);
        }
    })
}


function obtenerDatosValidos(idEmpre) {

    $.get("ListarEstadoContratos?empresa=" + idEmpre + "&estado=" + estado , function (data) {

        if (data.info.result == 0) {
            alert(data.info.mensaje);
            document.getElementById("esperar").style.display = 'none';
        } else {
            crearListado(["Rut", "Nombres", "Apellidos", "Tipo", "Contrato", "Inicio", "Termino", "Sueldo"], data, "list_estados");

            document.getElementById("esperar").style.display = 'none';
        }


    })

}


function crearListado(cabeceras, data, divId) {

    var z = data.info;

    var contenido = "";
    contenido += "<table id='tabla' class='table nowrap'>";

    //Las cabeceras
    contenido += "<thead>";
    contenido += "<tr>";

    for (var i = 0; i < cabeceras.length; i++) {
        contenido += "<td>" + cabeceras[i] + "</td>"
    }
    contenido += "<td>Operaciones</td>";
    contenido += "</tr>";

    contenido += "</thead>";
    if (z.data.length > 0) {
        var propiedadesObjeto = Object.keys(z.data[0]);

        contenido += "<tbody>";

        var fila;
        for (var i = 0; i < z.data.length; i++) {
            fila = z.data[i];
            contenido += "<tr>";

            for (var j = 0; j < propiedadesObjeto.length; j++) {

                var nombrePropiedad = propiedadesObjeto[j];

                if (nombrePropiedad == 'rut') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'nombres') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'apellidos') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'contrato') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'tipocontrato') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'inicio') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'termino') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'sueldobase') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }

            };

            contenido += "<td style='text-align:right'>";

            contenido += "<button onclick='abrirModal(" + fila.id + ")' class='btn btn-primary rounded-round btn-sm' ";
            contenido += "data-bs-toggle='modal' data-bs-target='#modal_Estados'><i class='fa fa-eye''></i></button> ";

            contenido += "</td>";

            contenido += "</tr>";

        }

        contenido += "</tbody>";

    }

    contenido += "</table>";

    document.getElementById(divId).innerHTML = contenido;
    ComponentBuilder.configurarDataTable("tabla");

}


function abrirModal(id) {
    console.log("id=" + id);
    if (id != undefined) {

        document.getElementById("txt_Cabecera").innerHTML = "Contrato";

        $.get("ConsultaEstadoContratoId?id=" + id + "&empresa=" + idEmpre, function (data) {
            data = data.info.data;
            console.log(data);
            document.getElementById("txt_rut").value = data[0].rut;
            document.getElementById("txt_nombres").value = data[0].nombres;
            document.getElementById("txt_apellidos").value = data[0].apellidos;
            document.getElementById("txt_contrato").value = data[0].contrato;
            document.getElementById("txt_tipocontrato").value = data[0].tipocontrato;
            document.getElementById("txt_inicio").value = data[0].inicio;
            document.getElementById("txt_termino").value = data[0].termino;
            document.getElementById("txt_faena").value = data[0].faena;
            document.getElementById("txt_cargo").value = data[0].cargo;
            document.getElementById("txt_centro").value = data[0].centrocosto;
            document.getElementById("txt_jornada").value = data[0].jornada;
            document.getElementById("txt_sueldo").value = data[0].sueldobase;
            document.getElementById("txt_observacion").value = data[0].observaciones;
            document.getElementById("txt_tipocarga").value = data[0].tipocarga;
            document.getElementById("txt_articulo22").value = data[0].articulo22;

        })
    }
}

function limpiar() {

    var elementosConClaseLimpiar = document.getElementsByClassName("cls_t");
    var nelementos = elementosConClaseLimpiar.length;
    for (var i = 0; i < nelementos; i++) {
        elementosConClaseLimpiar[i].value = "";
    }

}

function existeRut(T) {
    var rut = T;
    if (rut != "") {
        $.get("ExistePersona/?rut=" + rut + "&empresa=" + idEmpre, function (data) {
            var res = data.info;
            if (res.result == 1) {
                document.getElementById("txt_nombres").value = res.data.nombres.toString().toUpperCase();
                document.getElementById("txt_apellidos").value = res.data.apellidos.toString().toUpperCase();
            }
        });
    }
}


