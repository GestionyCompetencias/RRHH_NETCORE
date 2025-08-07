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
    desde.setDate(hasta.getDate() );
    hasta.setDate(hasta.getDate()+3);
    let fechades = formatFechaGuion1(desde.toLocaleDateString());
    let fechahas = formatFechaGuion1(hasta.toLocaleDateString());
    document.getElementById("txt_des").value = fechades;
    document.getElementById("txt_has").value = fechahas;
    ComboListaPersonas();

    //document.getElementById("esperar").style.display = 'block';
}
function ObtenerDatos() {
    rut = document.getElementById("cbo_persona").value
    des = document.getElementById("txt_des").value;
    has = document.getElementById("txt_has").value;
    if (rut != "") {
        $.get("VerificaSolicitudVacaciones/?rut=" + rut + '&des=' + des + '&has=' + has, function (data) {
            if (data.info.result == 0) {
                alert(data.info.mensaje);
                return;
            }
            MostrarDatos();
       });
    }
}
function MostrarDatos() {
    rut = document.getElementById("cbo_persona").value
    des = document.getElementById("txt_des").value;
    has = document.getElementById("txt_has").value;
    if (rut != "") {
            //document.getElementById("esperar").style.display = 'block';
            $.get("ConsultarSolicitudVacaciones/?rut=" + rut + '&des=' + des + '&has=' + has, function (data) {
                ListaRegistros(["Periodo", "Desde", "Hasta", "Dias"], data, "list_solicitudvacaciones");

                //document.getElementById("esperar").style.display = 'none';

            })
    }
}



function ListaRegistros(cabeceras, data, divId) {
    var contenido = "";
    contenido += "<table id='tabla' class='table datatable-basic dataTable no-footer'>";

    //Las cabeceras
    contenido += "<thead>";
    contenido += "<tr>";
    for (var i = 0; i < cabeceras.length; i++) {
        contenido += "<td>" + cabeceras[i] + "</td>"
    }
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
            var id = '"'+ fila.ruttrabajador + '*' + fila.fecha+ '"';
            for (var j = 0; j < propiedadesObjeto.length; j++) {
                var nombrePropiedad = propiedadesObjeto[j];
                if (nombrePropiedad == "periodo" || nombrePropiedad == "fechainicio" || nombrePropiedad == "fechatermino" || nombrePropiedad == "diastotal")
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";

            }
            contenido += "</tr>";
        }
        contenido += "</tbody>";
    }

    contenido += "</table>";
    document.getElementById(divId).innerHTML = contenido;

}

function Procesar() {
    rut = document.getElementById("cbo_persona").value
    des = document.getElementById("txt_des").value;
    has = document.getElementById("txt_has").value;
    if (rut != "") {
        //document.getElementById("esperar").style.display = 'block';
        $.get("ProcesarSolicitudVacaciones/?rut=" + rut + '&des=' + des + '&has=' + has, function (data) {
            alert(data.info.mensaje);
            //document.getElementById("esperar").style.display = 'none';

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


