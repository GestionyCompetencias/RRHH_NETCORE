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

    //document.getElementById("esperar").style.display = 'block';
}
function ObtenerDatos() {
    rut = document.getElementById("cbo_persona").value
    des = document.getElementById("txt_des").value;
    has = document.getElementById("txt_has").value;
    if (rut != "") {
        //document.getElementById("esperar").style.display = 'block';
        $.get("ListarMarcasTrabajador/?rut=" + rut + '&des=' + des + '&has=' + has , function (data) {
            ListaMarcas(["Fecha","Dia", "Entrada", "Salida"], data, "list_marcas");

            //document.getElementById("esperar").style.display = 'none';

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
            var id = '"'+ fila.ruttrabajador + '*' + fila.fecha+ '"';
            for (var j = 0; j < propiedadesObjeto.length; j++) {
                var nombrePropiedad = propiedadesObjeto[j];
                if (nombrePropiedad == "fecha" || nombrePropiedad == "entrada" || nombrePropiedad == "salida" || nombrePropiedad == "diasemana")
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
            }
            contenido += "<td>";
            contenido += "<button onclick='abrirModal(" + id + ")' class='btn btn-primary rounded-round btn-sm' ";
            contenido += "data-bs-toggle='modal' data-bs-target='#modal_Marcas'><i class='fa fa-pen'></i></button> ";
            contenido += "</td>";
            contenido += "</tr>";
        }

        contenido += "</tbody>";
    }

    contenido += "</table>";
    document.getElementById(divId).innerHTML = contenido;

}


function abrirModal(id) {
    limpiar();
    document.getElementById("divErrores").innerHTML = "";

    if (id != undefined) {

        document.getElementById("txt_Cabecera").innerHTML = "Editar marcas";

        $.get("ConsultaMarcaTrabajador?id=" + id , function (data) {
            data = data.info.data;
            document.getElementById("txt_fecha").value = data.fecha;
            document.getElementById("txt_entrada").value = data.entrada;
            document.getElementById("txt_salida").value = data.salida;
            document.getElementById("txt_checkent").value = data.checkent;
            document.getElementById("txt_checksal").value = data.checksal;
            document.getElementById("txt_diasem").value = data.diasemana;

            var elemento = document.getElementsByClassName("form-group-float-label");
            for (var i = 0; i < elemento.length; i++) {
                elemento[i].className += " is-visible";
            }
        })
        limpiar();
    }
}


function limpiar() {

    var elementosConClaseLimpiar = document.getElementsByClassName("cls_t");
    var nelementos = elementosConClaseLimpiar.length;
    for (var i = 0; i < nelementos; i++) {
        elementosConClaseLimpiar[i].value = "";
    }

}

function Guarda() {
    var checkent = document.getElementById("txt_checkent").value;
    var checksal = document.getElementById("txt_checksal").value;
    var fecha = document.getElementById("txt_fecha").value;
    var diasem = document.getElementById("txt_diasem").value;
    var entrada = document.getElementById("txt_entrada").value;
    var salida = document.getElementById("txt_salida").value;
    var frm = new FormData();
    frm.append("checkent", checkent);
    frm.append("checksal", checksal);
    frm.append("ruttrabajador", rut);
    frm.append("fecha", fecha);
    frm.append("diasemana", diasem);
    frm.append("entrada", entrada);
    frm.append("salida", salida);

    $.ajax({
        type: "POST",
        url: "ModificaMarcaTrabajador",
        data: frm,
        contentType: false,
        processData: false,
        success: function (data) {
            if (data.info.result == 1) {
                var Confirmar = confirm("Se enviara correo al trabajador para su aceptación o rechazo de acuerdo con ORD 5849-133 de la DT");
            }
            document.getElementById('modal_Marcas').style.display = 'none';
            alert(data.info.mensaje);
        }
    });
    ObtenerDatos();
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


