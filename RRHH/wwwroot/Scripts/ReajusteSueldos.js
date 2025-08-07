inicio();
var reajuste = 0;

function inicio() {
    document.getElementById("esperar").style.display = 'none';
    mostrarDatos();
}


function mostrarDatos() {

    document.getElementById("esperar").style.display = 'block';

    var idEmpre = "";
    $.get("EmpresaLog", function (data) {
        idEmpre = data;
            obtenerDatosValidos();
    })
}


function obtenerDatosValidos() {
    reajuste = document.getElementById("txt_reajuste").value
    if (reajuste != "") {

        $.get("ProcesaReajusteSueldos?reajuste=" + reajuste, function (data) {
            if (data.info.result == 0) {
                alert(data.info.mensaje);
                document.getElementById("esperar").style.display = 'none';
            } else {
                crearListado(["Rut", "Nombres", "Sueldo", "Reajuste", "Nuevo"], data, "list_reajuste");

                document.getElementById("esperar").style.display = 'none';
            }
        })
    }
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

                if (nombrePropiedad == 'rutTrabajador') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'nombre') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'sueldo') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'reajuste') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'nuevo') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }

            };

            contenido += "<td style='text-align:right'>";

            contenido += "</td>";

            contenido += "</tr>";

        }

        contenido += "</tbody>";

    }

    contenido += "</table>";

    document.getElementById(divId).innerHTML = contenido;
    ComponentBuilder.configurarDataTable("tabla");

}


