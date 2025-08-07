const desde = new Date();
const hasta = new Date();
var des=new Date()
var has = new Date()
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
    document.getElementById("procesa").style.display = 'none';
}
function ObtenerDatos() {
    des = document.getElementById("txt_des").value;
    has = document.getElementById("txt_has").value;
        //document.getElementById("esperar").style.display = 'block';
        $.get("ListarSobretiempos/?des=" + des + '&has=' + has , function (data) {
            ListaSobretiempo(["Rut","Fecha","Dia", "Entrada", "Salida","Horas","Sobretiempo"], data, "list_sobretiempos");


        })
}



function ListaSobretiempo(cabeceras, data, divId) {
    var contenido = "";
    contenido += "<table id='tabla' class='table datatable-basic dataTable no-footer'>";

    //Las cabeceras
    contenido += "<thead>";
    contenido += "<tr>";
    for (var i = 0; i < cabeceras.length; i++) {
        contenido += "<td>" + cabeceras[i] + "</td>"
    }
    contenido += "<td>Selección</td>";
    contenido += "</tr>";

    contenido += "</thead>";
    var info = data.info.data;
   if (info.length > 0) {
        var propiedadesObjeto = Object.keys(info[0]);

        contenido += "<tbody>";
       console.log(info);
        var fila;
        for (var i = 0; i < info.length; i++) {
            fila = info[i];
            contenido += "<tr>";
            var id =  fila.ruttrabajador + ';' + fila.fecha;
            for (var j = 0; j < propiedadesObjeto.length; j++) {
                var nombrePropiedad = propiedadesObjeto[j];
                if (nombrePropiedad == "fecha" || nombrePropiedad == "entrada" || nombrePropiedad == "salida" || nombrePropiedad == "horasextras"
                    || nombrePropiedad == "ruttrabajador" || nombrePropiedad == "diasem" || nombrePropiedad == "horastrabajadas")
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
            }
            contenido += "<td>";
            contenido += "<input type='checkbox' id='ckb" + id + "' name='ckeckes' value='ckeckes'><label for='ckb" + id + "'></label> ";
            contenido += "</td>";
            contenido += "</tr>";
        }

        contenido += "</tbody>";
    }

    contenido += "</table>";
    document.getElementById(divId).innerHTML = contenido;
    document.getElementById("procesa").style.display = 'block';
    //document.getElementById("esperar").style.display = 'none';

}




function limpiar() {

    var elementosConClaseLimpiar = document.getElementsByClassName("cls_t");
    var nelementos = elementosConClaseLimpiar.length;
    for (var i = 0; i < nelementos; i++) {
        elementosConClaseLimpiar[i].value = "";
    }

}

function Autorizar() {
    var ids = "";
    var elementos = document.getElementsByName("ckeckes");
    var nelementos = elementos.length;
    for (var i = 0; i < nelementos; i++) {
        if (elementos[i].checked == true) {
            ids += elementos[i].id.replace("ckb", "");
            ids += "*";
        }
    }
    if (ids != "") {
        if (confirm("Confirma que desea autorizar estas horas extras") == 1) {
            $.get("AutorizaHorasExtras/?ids=" + ids + '&des=' + des + '&has=' + has, function (data) {
                  alert(data.info.mensaje);
            })
        }
    }
    ObtenerDatos();
}

