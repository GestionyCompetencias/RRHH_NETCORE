const desde = new Date();
const hasta = new Date();
var rut = "";
var des = new Date();
var has = new Date();
const formatFechaGuion1 = (fecha) => {
    // Formato 2001-04-21
    let f = fecha.substring(0, 10);
    let arrF = f.split('-');

    if (arrF.length != 3) {
        arrF = f.split('/')
    }
    if (arrF[1].length == 1) {
        arrF[1] = "0" + arrF[1]
    }

    if (arrF[0].length == 1) {
        arrF[0] = "0" + arrF[0]
    }

    return `${arrF[2]}-${arrF[1]}-${arrF[0]}`;
}
inicio();

function inicio() {
    desde.setDate(hasta.getDate() - 30);
    let fechades = formatFechaGuion1(desde.toLocaleDateString());
    let fechahas = formatFechaGuion1(hasta.toLocaleDateString());
    document.getElementById("txt_des").value = fechades;
    document.getElementById("txt_has").value = fechahas;
    ComboListaPersonas();
}
function ObtenerDatos() {
    rut = document.getElementById("cbo_persona").value
    des = document.getElementById("txt_des").value;
    has = document.getElementById("txt_has").value;
    if (rut != "") {
        $.get("ListarMarcas/?rut=" + rut + '&des=' + des + '&has=' + has , function (data) {
            ListaMarcas(["Rut","Nombre","Fecha","Hora","Tipo"], data, "list_marcaciones");
        })
    }
}



function ListaMarcas(cabeceras, data, divId) {
    var contenido = "";
    contenido += "<table id='tabla' class='table datatable-basic dataTable no-footer'>";

    //Las cabeceras
    contenido += "<thead>";
    contenido += "<tr>";
    for (var i = 0; i < cabeceras.length; i++) {
        contenido += "<td>" + cabeceras[i] + "</td>"
    }
    contenido += "<td>Operaciones</td>";
    contenido += "</tr>";

    contenido += "</thead>";
    var info = data.info.data;
    if (data.info.result ==1) {
        var propiedadesObjeto = Object.keys(info[0]);

        contenido += "<tbody>";

        var fila;
        for (var i = 0; i < info.length; i++) {
            fila = info[i];
            contenido += "<tr>";
            var id = '"'+ fila.ruttrabajador+ '*' + fila.fecha+ '*'+fila.checktype+'"';
            for (var j = 0; j < propiedadesObjeto.length; j++) {
                var nombrePropiedad = propiedadesObjeto[j];
                if (nombrePropiedad == "ruttrabajador" || nombrePropiedad == "nombre" || nombrePropiedad == "fecha"
                    || nombrePropiedad == "checktime" || nombrePropiedad == "checktype")
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
            }
            contenido += "<td style='text-align:right'>";
            contenido += "<button onclick='MuestraFoto(" + id + ")' class='btn btn-indigo rounded-round btn-sm btn'><i class='fa fa-dochub'></i></button> ";
            contenido += "<button onclick='MuestraMapa(" + fila.id + ")' class='btn btn-indigo rounded-round btn-sm btn'><i class='fa fa-map'></i></button> ";
            contenido += "</td>";
           contenido += "</tr>";
        }
        contenido += "</tbody>";
    }
    contenido += "</table>";
    document.getElementById(divId).innerHTML = contenido;

}
function MuestraFoto(id) {
    window.open('VerImagen?id=' + id);
    return;
    }


function ComboListaPersonas() {

    var persona = document.getElementById("cbo_persona").value

    $.get("ComboPersonas/", function (data) {
        var contenido = "";
        var info = data.info.data;

        contenido += "<option value=''>--Seleccione--</option>"
        //Generando las opciones
        for (var i = 0; i < info.length; i++) {
            contenido += "<option value='" + info[i].codigo + "'>" + info[i].descripcion + "</option>"
        }
        document.getElementById("cbo_persona").innerHTML = contenido;
    })
}


