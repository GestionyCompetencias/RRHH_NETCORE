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
var haberesinformadosModal = ComponentBuilder.setModal("modal_permisos");

var idEmpre = "";
inicio();
ComboTrabajadores();

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
    $.get("ListarPermisoAsistencia?rut=" + rut , function (data) {
        document.getElementById("nuevo").style.display = 'block';
        if (data.info.result == 0) {
            alert(data.info.mensaje);
        }
        crearListado(["Rut", "Nombre", "Inicio","Hora", "Termino","Hora","Gose", "Comentario"], data, "list_permisos");
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
            contenido += "<button onclick='abrirModal(" + fila.id + ")' class='btn btn-primary rounded-round btn-sm' ";
            contenido += "data-bs-toggle='modal' data-bs-target='#modal_permisos'><i class='fa fa-pen'></i></button> ";
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
        document.getElementById("txt_Cabecera").innerHTML = "Editar permiso";

        $.get("ConsultarPermisoAsistenciaId?id=" + id + "&empresa=" + idEmpre, function (data) {
            console.log(data);
            data = data.info.data;
            document.getElementById("txt_id").innerHTML = data[0].id;
            document.getElementById("cbo_trabajador").value = data[0].ruttrabajador;
            document.getElementById("txt_gose").value = data[0].gosesueldo;
            document.getElementById("txt_inicio").value = data[0].fechainicio;
            document.getElementById("txt_termino").value = data[0].fechatermino;
            document.getElementById("txt_hrsini").value = data[0].horainicio;
            document.getElementById("txt_hrster").value = data[0].horatermino;
            document.getElementById("txt_gose").value = data[0].gose;
            document.getElementById("txt_comentario").value = data[0].comentario;
            var elemento = document.getElementsByClassName("form-group-float-label");
            for (var i = 0; i < elemento.length; i++) {
                elemento[i].className += " is-visible";
            }
        })
    }
    else {
        document.getElementById("txt_id").innerHTML = "";
        document.getElementById("txt_Cabecera").innerHTML = "Nuevo permiso";
        document.getElementById("txt_inicio").value = fecini;
        document.getElementById("txt_termino").value = fecini;
        document.getElementById("txt_gose").value = "1";

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
        $.get("inhabilitaPermisoAsistencia?Id=" + id + "&empresa=" + idEmpre, function (data) {
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
    var inicio = document.getElementById("txt_inicio").value;
    var termino = document.getElementById("txt_termino").value;
    var hrsini = document.getElementById("txt_hrsini").value;
    var hrster = document.getElementById("txt_hrster").value;
    var gose = document.getElementById("txt_gose").value;
    var comentario = document.getElementById("txt_comentario").value.toString().toUpperCase();
    var empresa = idEmpre;


    var frm = new FormData();
    frm.append("id", id);
    frm.append("ruttrabajador", rut);
    frm.append("fechainicio", inicio);
    frm.append("fechatermino", termino);
    frm.append("horainicio", hrsini);
    frm.append("horatermino", hrster);
   frm.append("gose", gose);
    frm.append("comentario", comentario);
    frm.append("empresa", empresa);


    if (id == '') {
        $.ajax({
            type: "POST",
            url: "CrearPermisoAsistencia",
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
            url: "EditaPermisoAsistencia",
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
