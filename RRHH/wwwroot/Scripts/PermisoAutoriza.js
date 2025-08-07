const desde = new Date();
const hasta = new Date();
var fecini = new Date();
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

var idEmpre = "";
inicio();
desde.setDate(hasta.getDate() - 30);
let fechades = formatFechaGuion1(desde.toLocaleDateString());
let fechahas = formatFechaGuion1(hasta.toLocaleDateString());
document.getElementById("txt_desde").value = fechades;
document.getElementById("txt_hasta").value = fechahas;

function inicio() {
    document.getElementById("esperar").style.display = 'none';
    if (rut != "") mostrarDatos();
    rut = "0";
}


function mostrarDatos() {
    document.getElementById("esperar").style.display = 'block';
   $.get("EmpresaLog", function (data) {
        idEmpre = data;
        obtenerDatosValidos();
    })
}

function obtenerDatosValidos() {
    des = document.getElementById("txt_desde").value;
    has = document.getElementById("txt_hasta").value;
    $.get("ListarPermisoAutoriza?desde=" + des +"&hasta="+has , function (data) {
        if (data.info.result == 0) {
            alert(data.info.mensaje);
        }
        crearListado(["Rut", "Nombre", "Inicio", "Hora", "Termino", "Hora", "Gose", "Comentario","Autoriza","Rechaza"], data, "list_autorizapermisos");
        document.getElementById("esperar").style.display = 'none';
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
    contenido += "</tr>";

    contenido += "</thead>";
    if (data.info.result != 0) {
        var propiedadesObjeto = Object.keys(z.data[0]);

        contenido += "<tbody>";

        var fila;
        for (var i = 0; i < z.data.length; i++) {
            fila = z.data[i];
            contenido += "<tr>";

            for (var j = 0; j < propiedadesObjeto.length; j++) {

                var nombrePropiedad = propiedadesObjeto[j];

                if (nombrePropiedad == 'ruttrabajador') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'nombre') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'fechainicio') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'fechatermino') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'gosestr') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'comentario') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'horainicio') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'horatermino') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
            };

            contenido += "<td style='text-align:right'>";
            contenido += "<input type='checkbox' id='ckbA" + fila.id + "' name='ckeckesA' value='ckeckes'><label for='ckb" + fila.id + "'></label> ";
           //contenido += "<button onclick='Eliminar(" + fila.id + ")' class='btn btn-danger rounded-round btn-sm btn'><i class='fa ffa-apple-alt'></i></button> ";
            contenido += "</td>";
            contenido += "<td style='text-align:right'>";
            contenido += "<input type='checkbox' id='ckbR" + fila.id + "' name='ckeckesR' value='ckeckes'><label for='ckb" + fila.id + "'></label> ";
            //contenido += "<button onclick='Eliminar(" + fila.id + ")' class='btn btn-danger rounded-round btn-sm btn'><i class='fa fa-trash-alt'></i></button> ";
            contenido += "</td>";

            contenido += "</tr>";
        }
        contenido += "</tbody>";
    }
    contenido += "</table>";
    document.getElementById(divId).innerHTML = contenido;
    ComponentBuilder.configurarDataTable("tabla");

}



function limpiar() {

    var elementosConClaseLimpiar = document.getElementsByClassName("cls_t");
    var nelementos = elementosConClaseLimpiar.length;
    for (var i = 0; i < nelementos; i++) {
        elementosConClaseLimpiar[i].value = "";
    }

}

function Eliminar(id) {
    if (confirm("confirma que desea eliminar el registro?") == 1) {
        $.get("inhabilitaPermisoAsistencia?Id=" + id + "&empresa=" + idEmpre, function (data) {
            alert(data.info.mensaje);
            mostrarDatos();
        })
    }
}
function Procesar() {
    if (confirm("Confirma que desea procesar permisos") == 1) {
        Autoriza();
        Rechaza();
        mostrarDatos();
    }
}
function Autoriza() {
    var ids = "";
    var elementos = document.getElementsByName("ckeckesA");
    var nelementos = elementos.length;
    if (nelementos > 0) {
        for (var i = 0; i < nelementos; i++) {
            if (elementos[i].checked == true) {
                ids += elementos[i].id.replace("ckbA", "");
                ids += "*";
            }
        }
        if (ids != "") {
            $.get("AutorizaPermisos/?ids=" + ids + '&des=' + des + '&has=' + has, function (data) {
                alert(data.info.mensaje);
            })
        }
    }
}
function Rechaza() {
    var ids = "";
    var elementos = document.getElementsByName("ckeckesR");
    var nelementos = elementos.length;
    if (nelementos > 0) {
        for (var i = 0; i < nelementos; i++) {
            if (elementos[i].checked == true) {
                ids += elementos[i].id.replace("ckbR", "");
                ids += "*";
            }
        }
        if (ids != "") {
            $.get("RechazaPermisos/?ids=" + ids + '&des=' + des + '&has=' + has, function (data) {
                alert(data.info.mensaje);
            })
        }
    }
}



