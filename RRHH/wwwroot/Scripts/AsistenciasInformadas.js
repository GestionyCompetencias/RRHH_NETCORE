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
var asistenciasinformadasModal = ComponentBuilder.setModal("modal_Asistenciasinformadas");

var mes = 0;
var anio = 2025;
var idEmpre = "";

inicio();
ComboInasistencias();
ComboTrabajadores();
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
        mes = document.getElementById("cbo_mes").value;
        anio = document.getElementById("cbo_anio").value;
        if (mes != 0) {
            obtenerDatosValidos();
        }
    })
}

function obtenerDatosValidos() {
    $.get("ListarAsistenciasInformadas?mes=" + mes + '&anio=' + anio , function (data) {
        document.getElementById("nuevo").style.display = 'block';
        if (data.info.result == 0) {
            alert(data.info.mensaje);
        }
        crearListado(["Rut", "Nombre", "Fecha", "Ausencia", "Días", "Sobretiempo 50%","Sobretiempo 100%", "Dias Colación", "Movilización"], data, "list_asistenciasinformadas");
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
                else if (nombrePropiedad == 'fechaAsistencia') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'inasistencia') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'dias') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'horasExtras1') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'horasExtras2') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'diasColacion') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'diasMovilizacion') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
            };

            contenido += "<td style='text-align:right'>";
            contenido += "<button onclick='abrirModal(" + fila.id + ")' class='btn btn-primary rounded-round btn-sm' ";
            contenido += "data-bs-toggle='modal' data-bs-target='#modal_Asistenciasinformadas'><i class='fa fa-pen'></i></button> ";
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
    document.getElementById("divErrores").innerHTML = "";

    if (id != undefined) {

        document.getElementById("txt_Cabecera").innerHTML = "Editar asistencia";

        $.get("ConsultarAsistenciaInformadaId?id=" + id + "&empresa=" + idEmpre, function (data) {
            data = data.info.data;
            document.getElementById("txt_id").innerHTML = data[0].id;
            document.getElementById("cbo_trabajador").value = data[0].rutTrabajador;
            document.getElementById("txt_fecha").value = data[0].fechaAsistencia;
            document.getElementById("cbo_inasistencia").value = data[0].codigoInasis;
            document.getElementById("txt_dias").value = data[0].dias;
            document.getElementById("txt_extras1").value = data[0].horasExtras1;
            document.getElementById("txt_extras2").value = data[0].horasExtras2;
            document.getElementById("txt_extras3").value = data[0].horasExtras3;
            document.getElementById("txt_diascolacion").value = data[0].diasColacion;
            document.getElementById("txt_horascolacion").value = data[0].horasColacion;
            document.getElementById("txt_diasmovilizacion").value = data[0].diasMovilizacion;

            var elemento = document.getElementsByClassName("form-group-float-label");
            for (var i = 0; i < elemento.length; i++) {
                elemento[i].className += " is-visible";
            }
        })
    }
    else {
        document.getElementById("txt_id").innerHTML = "";
        document.getElementById("txt_Cabecera").innerHTML = "Nueva asistencia";
        document.getElementById("txt_dias").value = "0";
        document.getElementById("txt_extras1").value = "0";
        document.getElementById("txt_extras2").value = "0";
        document.getElementById("txt_extras3").value = "0";
        document.getElementById("txt_diascolacion").value = "0";
        document.getElementById("txt_horascolacion").value = "0";
        document.getElementById("txt_diasmovilizacion").value = "0";

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

    if (confirm("Confirma que desea eliminar la información de asistencia?") == 1) {

        $.get("inhabilitaAsistenciaInformada?Id=" + id + "&empresa=" + idEmpre, function (data) {
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
    var fechaasistencia = document.getElementById("txt_fecha").value;
    var codigoinasis = document.getElementById("cbo_inasistencia").value;
    var dias = document.getElementById("txt_dias").value;
    var extras1 = document.getElementById("txt_extras1").value;
    var extras2 = document.getElementById("txt_extras2").value;
    var extras3 = document.getElementById("txt_extras3").value;
    var diascolacion = document.getElementById("txt_diascolacion").value;
    var horascolacion = document.getElementById("txt_horascolacion").value;
    var diasmovilizacion = document.getElementById("txt_diasmovilizacion").value;
    var empresa = idEmpre;


    var frm = new FormData();
    frm.append("id", id);
    frm.append("rutTrabajador", rut);
    frm.append("fechaAsistencia", fechaasistencia);
    frm.append("codigoInasis", codigoinasis);
    frm.append("dias", dias);
    frm.append("horasExtras1", extras1);
    frm.append("horasExtras2", extras2);
    frm.append("horasExtras3", extras3);
    frm.append("diasColacion", diascolacion);
    frm.append("horasColacion", horascolacion);
    frm.append("diasMovilizacion", diasmovilizacion);
    frm.append("empresa", empresa);


    if (id == '') {
        $.ajax({
            type: "POST",
            url: "CrearAsistenciaInformada",
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
            url: "EditaAsistenciaInformada",
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

function ComboInasistencias() {
    $.get("CargaInasistencias?empresa=" + idEmpre, (data) => {
        var info = data.info.data;
        var contenido = "";
        contenido += "<option value=''>--Seleccione--</option>"
        //Generando las opciones
        for (var i = 0; i < info.length; i++) {
            contenido += "<option value='" + info[i].codigo + "'>" + info[i].descripcion + "</option>"

        }
        document.getElementById("cbo_inasistencia").innerHTML = contenido;
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

