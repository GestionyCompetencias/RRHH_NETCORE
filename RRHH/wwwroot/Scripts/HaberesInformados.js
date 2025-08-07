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
var haberesinformadosModal = ComponentBuilder.setModal("modal_Haberesinformados");

var codigo = 100;
var mes = 1;
var anio = 2020;
var pago = "L";
var idEmpre = "";
var fecini = "";
var fecfin = "";

inicio();
ComboHaberes();
ComboTrabajadores();
ComboMes() ;
ComboAnio();
ComboPago();

function inicio() {
    document.getElementById("esperar").style.display = 'none';
    mostrarDatos();
}


function mostrarDatos() {

    document.getElementById("esperar").style.display = 'block';
    $.get("EmpresaLog", function (data) {
        idEmpre = data;
        codigo = document.getElementById("cbo_haberes").value
        if (codigo != "") {
            obtenerDatosValidos();
        }

    })
}

function obtenerDatosValidos() {
    mes = document.getElementById("cbo_mes").value;
    anio = document.getElementById("cbo_anio").value;
    BuscaFechas();
    pago = document.getElementById("cbo_pago").value;
    if (mes=="") {
        alert("Debe ingresar mes.");
        return;
    }
    if (anio == "") {
        alert("Debe ingresar año.");
        return;
    }
    if (pago == "") {
        alert("Debe ingresar pago.");
        return;
    }
    $.get("ListarHaberesInformados?codigo=" + codigo + '&mes=' + mes + '&anio=' + anio + '&pago=' + pago, function (data) {
        document.getElementById("nuevo").style.display = 'block';
        if (data.info.result == 0) {
            alert(data.info.mensaje);
        }
        crearListado(["Rut", "Nombre", "Afecta", "pago", "Calculo", "Monto","Cantidad", "Desde", "Hasta"], data, "list_haberesinformados");
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

                if (nombrePropiedad == 'rutTrabajador') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'nombre') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'afecta') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'pago') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'tipoCalculo') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'monto') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'dias') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'fechaDesde') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'fechaHasta') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
            };

            contenido += "<td style='text-align:right'>";

            contenido += "<button onclick='abrirModal(" + fila.id + ")' class='btn btn-primary rounded-round btn-sm' ";
            contenido += "data-bs-toggle='modal' data-bs-target='#modal_Haberesinformados'><i class='fa fa-pen'></i></button> ";

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


function abrirModal(id) {
    limpiar();
    document.getElementById("cbo_haberes").value = codigo;

    document.getElementById("divErrores").innerHTML = "";

    if (id != undefined) {

        document.getElementById("txt_Cabecera").innerHTML = "Editar haberes informados";

        $.get("ConsultarHaberInformadoId?id=" + id + "&empresa=" + idEmpre, function (data) {
            data = data.info.data;
            document.getElementById("txt_id").innerHTML = data[0].id;
            document.getElementById("cbo_trabajador").value = data[0].rutTrabajador;
            document.getElementById("txt_tipocalculo").value = data[0].tipoCalculo;
            document.getElementById("txt_monto").value = data[0].monto;
            document.getElementById("txt_cantidad").value = data[0].dias;
            document.getElementById("txt_desde").value = data[0].fechaDesde;
            document.getElementById("txt_hasta").value = data[0].fechaHasta;

            var elemento = document.getElementsByClassName("form-group-float-label");
            for (var i = 0; i < elemento.length; i++) {
                elemento[i].className += " is-visible";
            }
        })
    }
    else {
        document.getElementById("txt_id").innerHTML = "";
        document.getElementById("txt_Cabecera").innerHTML = "Nuevo haber informado";
        document.getElementById("txt_desde").value = fecini;
        document.getElementById("txt_hasta").value = fecfin;
        document.getElementById("txt_cantidad").value = "0";

        var elemento = document.getElementsByClassName("form-group-float-label");
        for (var i = 0; i < elemento.length; i++) {
            elemento[i].classList.replace('is-visible', 'no-visible');
        }
    }

}

function limpiar() {

    var elementosConClaseLimpiar = document.getElementsByClassName("cls_t");
    var nelementos = elementosConClaseLimpiar.length;
    for (var i = 0; i < nelementos; i++) {
        elementosConClaseLimpiar[i].value = "";
    }

}



function Eliminar(id) {

    if (confirm("confirma que desea eliminar la información del haber?") == 1) {

        $.get("inhabilitaHaberInformado?Id=" + id + "&empresa=" + idEmpre, function (data) {
            alert(data.info.mensaje);
            inicio();
        })

    }

}

function validarDatos() {
    //Todos los campos estan bien validados
    var mensaje = "";
    var exito = true;
    var obligatorios = document.getElementsByClassName("Obl_t");
    var nobligatorios = obligatorios.length;

    for (var i = 0; i < nobligatorios; i++) {
        if (obligatorios[i].value == "") {
            exito = false;
            mensaje += "<li>Debe ingresar " + obligatorios[i].name + "</li>";
            return { exito, mensaje };
        }
    }
    return { exito, mensaje };
}


function Guarda() {
    var objeto = validarDatos();
    var esCorrecto = objeto.exito;
    if (esCorrecto == false) {
        document.getElementById("divErrores").innerHTML = "<ol>" + objeto.mensaje + "</ol>";
        return;
    }

    var id = document.getElementById("txt_id").innerHTML;
    var rut = formatoRutGuardar(document.getElementById("cbo_trabajador").value);
    var afecta = "T";
    var tipocalculo = document.getElementById("txt_tipocalculo").value;
    var monto = document.getElementById("txt_monto").value;
    var dias = document.getElementById("txt_cantidad").value;
    var desde = document.getElementById("txt_desde").value.toString().toUpperCase();
    var hasta = document.getElementById("txt_hasta").value;
    var empresa = idEmpre;


    var frm = new FormData();
    frm.append("id", id);
    frm.append("rutTrabajador", rut);
    frm.append("haber", codigo);
    frm.append("afecta", afecta);
    frm.append("pago", pago);
    frm.append("tipoCalculo", tipocalculo);
    frm.append("monto", monto);
    frm.append("dias", dias);
    frm.append("fechaDesde", desde);
    frm.append("fechaHasta", hasta);
    frm.append("empresa", empresa);


    if (id == '') {
        $.ajax({
            type: "POST",
            url: "CrearHaberInformado",
            data: frm,
            contentType: false,
            processData: false,

            success: function (data) {
                alert(data.info.mensaje);
                limpiar();
                document.getElementById("btnCerrar").click();
                obtenerDatosValidos();
            }
        });
    } else {
        $.ajax({
            type: "POST",
            url: "EditaHaberInformado",
            data: frm,
            contentType: false,
            processData: false,

            success: function (data) {
                alert(data.info.mensaje);
                limpiar();
                document.getElementById("btnCerrar").click();
                obtenerDatosValidos();
            }
        });
    }

}

function ComboHaberes() {
    $.get("CargaHaberes?empresa=" + idEmpre, (data) => {
        var info = data.info.data;
        var contenido = "";
        contenido += "<option value=''>--Seleccione--</option>"
        //Generando las opciones
        for (var i = 0; i < info.length; i++) {
            contenido += "<option value='" + info[i].codigo + "'>" + info[i].descripcion + "</option>"

        }
        document.getElementById("cbo_haberes").innerHTML = contenido;
    })
}
function ComboTrabajadores() {
    $.get("CargaTrabajadoresDct", (data) => {
        var info = data.info.data;
        var contenido = "";
        contenido += "<option value=''>--Seleccione--</option>"
        //Generando las opciones
        for (var i = 0; i < info.length; i++) {
            contenido += "<option value='" + info[i].codigo + "'>" + info[i].descripcion + "</option>"

        }
        document.getElementById("cbo_trabajador").innerHTML = contenido;
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
function BuscaFechas() {
    $.get("CargaFechas?mes=" + mes + '&anio=' + anio, function (data) {
        fecini = data.info.data[0];
        fecfin = data.info.data[1];
    })
}
const loadMonths = () => {
    const meses = ['ENERO', 'FEBRERO', 'MARZO', 'ABRIL', 'MAYO', 'JUNIO', 'JULIO', 'AGOSTO', 'SEPTIEMBRE', 'OCTUBRE', 'NOVIEMBRE', 'DICIEMBRE'];
    let contenido = "";
    let index = 1;
    meses.forEach((mes) => {
        contenido += `<option value="${index}">${mes}</option>`;
        index++;
    });
    $("#mes").html(contenido);
};

const loadYears = () => {
    const dtNow = new Date();
    let year = dtNow.getFullYear();
    let contenido = "";
    for (let i = 0; i < 10; i++) {
        contenido += `<option value="${year}">${year}</option>`;
        year--;
    }
    $("#anio").html(contenido);


}

