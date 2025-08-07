const desde = new Date();
const hasta = new Date();
var fecini = new Date();
var rut = "";
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
var haberesinformadosModal = ComponentBuilder.setModal("modal_licencias");

var idEmpre = "";
inicio();
ComboTipoLicencias();
ComboTrabajadores();
ComboTipoMedicos();

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
        rut = document.getElementById("cbo_trabajador").value
        if (rut != "") {
            obtenerDatosValidos();
        }

    })
}

function obtenerDatosValidos() {
    $.get("ListarLicenciasMedicas?rut=" + rut , function (data) {
        document.getElementById("nuevo").style.display = 'block';
        if (data.info.result == 0) {
            alert(data.info.mensaje);
        }
        crearListado(["Rut", "Nombre", "Codigo", "Inicio", "Termino","Dias", "Descripción"], data, "list_licencias");
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

                if (nombrePropiedad == 'ruttrabajador') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'nombre') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'codigolicencia') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'fechainicio') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'fechatermino') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'desclicencia') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'dias') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
            };

            contenido += "<td style='text-align:right'>";
            contenido += "<button onclick='abrirModal(" + fila.id + ")' class='btn btn-primary rounded-round btn-sm' ";
            contenido += "data-bs-toggle='modal' data-bs-target='#modal_licencias'><i class='fa fa-pen'></i></button> ";
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
    document.getElementById("cbo_trabajador").value = rut;
    document.getElementById("divErrores").innerHTML = "";

    if (id != undefined) {
        document.getElementById("txt_Cabecera").innerHTML = "Editar licencia medica";

        $.get("ConsultarLicenciaMedicaId?id=" + id + "&empresa=" + idEmpre, function (data) {
            console.log(data);
            data = data.info.data;
            document.getElementById("cbo_trabajador").value = data[0].ruttrabajador;
            document.getElementById("txt_codigo").value = data[0].codigolicencia;
            document.getElementById("txt_dias").value = data[0].dias;
            document.getElementById("txt_inicio").value = data[0].fechainicio;
            document.getElementById("txt_termino").value = data[0].fechatermino;
            document.getElementById("txt_comentario").value = data[0].comentario;
            document.getElementById("cbo_licencia").value = data[0].tipolicencia;
            document.getElementById("cbo_medico").value = data[0].tipomedico;

            var elemento = document.getElementsByClassName("form-group-float-label");
            for (var i = 0; i < elemento.length; i++) {
                elemento[i].className += " is-visible";
            }
        })
    }
    else {
        document.getElementById("txt_id").innerHTML = "";
        document.getElementById("txt_Cabecera").innerHTML = "Nuev licencia medica";
        document.getElementById("txt_inicio").value = fecini;
        document.getElementById("txt_termino").value = fecini;
        document.getElementById("txt_dias").value = "0";

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

    if (confirm("confirma que desea eliminar el registro?") == 1) {

        $.get("inhabilitaLicenciaMedica?Id=" + id + "&empresa=" + idEmpre, function (data) {
            alert(data.info.mensaje);
            mostrarDatos();
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
    var codigo = document.getElementById("txt_codigo").value;
    var inicio = document.getElementById("txt_inicio").value;
    var termino = document.getElementById("txt_termino").value;
    var dias = document.getElementById("txt_dias").value;
    var comentario = document.getElementById("txt_comentario").value.toString().toUpperCase();
    var licencia = document.getElementById("cbo_licencia").value;
    var medico = document.getElementById("cbo_medico").value;
    var empresa = idEmpre;


    var frm = new FormData();
    frm.append("id", id);
    frm.append("ruttrabajador", rut);
    frm.append("codigolicencia", codigo);
    frm.append("fechainicio", inicio);
    frm.append("fechatermino", termino);
    frm.append("dias", dias);
    frm.append("tipolicencia", licencia);
    frm.append("comentario", comentario);
    frm.append("tipomedico", medico);
    frm.append("fechaHasta", hasta);
    frm.append("empresa", empresa);


    if (id == '') {
        $.ajax({
            type: "POST",
            url: "CrearLicenciaMedica",
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
            url: "EditaLicenciaMedica",
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

function ComboTipoLicencias() {
    $.get("CargaTiposLicencias", (data) => {
        var info = data.info.data;
        var contenido = "";
        contenido += "<option value=''>--Seleccione--</option>"
        //Generando las opciones
        for (var i = 0; i < info.length; i++) {
            contenido += "<option value='" + info[i].codigo + "'>" + info[i].descripcion + "</option>"
        }
        document.getElementById("cbo_licencia").innerHTML = contenido;
    })
}

function ComboTipoMedicos() {
    $.get("CargaTiposMedicos", (data) => {
        var info = data.info.data;
        var contenido = "";
        contenido += "<option value=''>--Seleccione--</option>"
        //Generando las opciones
        for (var i = 0; i < info.length; i++) {
            contenido += "<option value='" + info[i].codigo + "'>" + info[i].descripcion + "</option>"
        }
        document.getElementById("cbo_medico").innerHTML = contenido;
    })
}

function ComboTrabajadores() {
    $.get("CargaTrabajadoresLic", (data) => {
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
