const desde = new Date();
const hasta = new Date();
var fecini = new Date();
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

var mes = 1;
var anio = 2020;
var tipo = "L";
var idEmpre = "";

inicio();
ComboMes();
ComboAnio();

function inicio() {
    document.getElementById("esperar").style.display = 'none';
    desde.setDate(hasta.getDate());
    let fechades = formatFechaGuion1(desde.toLocaleDateString());
    fecini = fechades;
    mostrarDatos();
}


function mostrarDatos() {

    document.getElementById("esperar").style.display = 'block';
    $.get("EmpresaLog", function (data) {
        idEmpre = data;
            obtenerDatosValidos(idEmpre);
    })
}

function obtenerDatosValidos(idEmpre) {
    mes = document.getElementById("cbo_mes").value;
    anio = document.getElementById("cbo_anio").value;
    tipo = document.getElementById("txt_tipo").value;
    $.get("ListarComprobanteContable?mes=" + mes + '&anio=' + anio + '&tipo=' + tipo, function (data) {
        document.getElementById("nuevo").style.display = 'block';
        if (data.info.result == 0) {
            alert(data.info.mensaje);
        }
        crearListado(["Concepto", "Descripción", "Debe", "Haber", "Cuenta", "Nombre"], data, "list_comprobantecontable");
        document.getElementById("esperar").style.display = 'none';
    })
}
function crearListado(cabeceras, data, divId) {

    var z = data.info;
    var contenido = "";
    contenido += "<table id='tabla' class='table nowrap'>";
    console.log(data);
    //Las cabeceras
    contenido += "<thead>";
    contenido += "<tr>";

    for (var i = 0; i < cabeceras.length; i++) {
        contenido += "<td>" + cabeceras[i] + "</td>"
    }
    contenido += "<td>Operaciones</td>";
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
                if (nombrePropiedad == "DEBE" || nombrePropiedad == "HABER") contenido += "<td class='text-right'>" + fila[nombrePropiedad] + "</td>";
                if (nombrePropiedad == "GLOSA" || nombrePropiedad == "CONCEPTO" || nombrePropiedad == "CUENTA" || nombrePropiedad == "DESCRIPCION")
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
       };

            contenido += "<td style='text-align:right'>";

            contenido += "<button onclick='abrirModal(" + fila.id + ")' class='btn btn-primary rounded-round btn-sm' ";
            contenido += "data-bs-toggle='modal' data-bs-target='#modal_Descuentosinformados'><i class='fa fa-pen'></i></button> ";

            contenido += "<button onclick='Eliminar(" + fila.id + ")' class='btn btn-danger rounded-round btn-sm btn'><i class='fa fa-trash-alt'></i></button> ";

            contenido += "</td>";

            contenido += "</tr>";

        }
        contenido += "</tbody>";
    }
    contenido += "</table>";
    document.getElementById(divId).innerHTML = contenido;
    ComponentBuilder.configurarDataTable("tabla");
}


function procesar() {
    mes = document.getElementById("cbo_mes").value;
    anio = document.getElementById("cbo_anio").value;
    tipo = document.getElementById("txt_tipo").value;
    $.get("ProcesarComprobanteContable?mes=" + mes + '&anio=' + anio + '&tipo=' + tipo, function (data) {
        if (data.info.result == 0) {
            alert(data.info.mensaje);
        }
        document.getElementById("esperar").style.display = 'none';
    })
}



function ComboMes() {
    $.get("CargaMes", (data) => {
        var info = data.info.data;
        var contenido = "";
        contenido += "<option value=''>--Seleccione--</option>"
        //Generando las opciones
        for (var i = 0; i < info.length; i++) {
            contenido += "<option value='" + info[i].codigo + "'>" + info[i].descripcion + "</option>"

        }
        document.getElementById("cbo_mes").innerHTML = contenido;
    })
}

function ComboAnio() {
    $.get("CargaAnio", (data) => {
        var info = data.info.data;
        var contenido = "";
        contenido += "<option value=''>--Seleccione--</option>"
        //Generando las opciones
        for (var i = 0; i < info.length; i++) {
            contenido += "<option value='" + info[i].codigo + "'>" + info[i].descripcion + "</option>"

        }
        document.getElementById("cbo_anio").innerHTML = contenido;
    })
}
function ComboPago() {
    $.get("CargaPago", (data) => {
        var info = data.info.data;
        var contenido = "";
        contenido += "<option value=''>--Seleccione--</option>"
        //Generando las opciones
        for (var i = 0; i < info.length; i++) {
            contenido += "<option value='" + info[i].codigo + "'>" + info[i].descripcion + "</option>"

        }
        document.getElementById("cbo_pago").innerHTML = contenido;
    })
}

