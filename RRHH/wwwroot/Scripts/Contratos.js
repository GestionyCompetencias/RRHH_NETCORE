inicio();
var idEmpre = "";

var contratoModal = ComponentBuilder.setModal("modal_Contratos");

function inicio() {
    document.getElementById("esperar").style.display = 'none';
    mostrarDatos();
}


function mostrarDatos() {

    document.getElementById("esperar").style.display = 'block';
    $.get("EmpresaLog", function (data) {
        idEmpre = data;
        setTimeout(() => { mostrarTiposContratos()}, 500);
        setTimeout(() => { mostrarFaenas() }, 500);
        setTimeout(() => { mostrarCargos() }, 500);
        setTimeout(() => { mostrarCentros() }, 500);
        setTimeout(() => { mostrarJornadas() }, 500);
        setTimeout(() => { mostrarBancos() }, 500);
        setTimeout(() => { mostrarAfps() }, 500);
        setTimeout(() => { mostrarIsapres() }, 500);
        setTimeout(() => { mostrarTiposCuentas() }, 500);
       obtenerDatosValidos(idEmpre);
    })
}


function obtenerDatosValidos(idEmpre) {

    $.get("ListarContratos?empresa=" + idEmpre, function (data) {

        if (data.info.result == 0) {
            alert(data.info.mensaje);
            document.getElementById("esperar").style.display = 'none';
        } else {
            crearListado(["Rut", "Nombres", "Apellidos", "Tipo", "Contrato", "Inicio", "Termino", "Sueldo"], data, "list_contratos");

            document.getElementById("esperar").style.display = 'none';
        }
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
    if (z.data.length > 0) {
        var propiedadesObjeto = Object.keys(z.data[0]);

        contenido += "<tbody>";

        var fila;
        for (var i = 0; i < z.data.length; i++) {
            fila = z.data[i];
            contenido += "<tr>";

            for (var j = 0; j < propiedadesObjeto.length; j++) {

                var nombrePropiedad = propiedadesObjeto[j];

                if (nombrePropiedad == 'rut') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'nombres') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'apellidos') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'contrato') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'tipocontrato') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'inicio') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'termino') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'sueldobase') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }

            };

            contenido += "<td style='text-align:right'>";

            contenido += "<button onclick='abrirModal(" + fila.id + ")' class='btn btn-primary rounded-round btn-sm' ";
            contenido += "data-bs-toggle='modal' data-bs-target='#modal_Contratos'><i class='fa fa-pen'></i></button> ";

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
    document.getElementById("divErrores").innerHTML = "";
    if (id != undefined) {

        document.getElementById("txt_Cabecera").innerHTML = "Editar contrato";

        $.get("ConsultaContratoId?id="+ id +"&empresa="+idEmpre, function (data) {
            data = data.info.data;
            console.log(data);
            document.getElementById("txt_id").innerHTML = data.id;
            $("#txt_rut").val(data.rut).trigger('change');
            document.getElementById("camporut").style.visibility = 'hidden';
            document.getElementById("txt_nombres").value = data.nombres;
            document.getElementById("txt_apellidos").value = data.apellidos;
            document.getElementById("txt_contrato").value = data.contrato;
            document.getElementById("txt_inicio").value = data.inicio;
            document.getElementById("txt_termino").value = data.termino;
            document.getElementById("txt_sueldo").value = data.sueldobase;
            document.getElementById("txt_observacion").value = data.observaciones;
            document.getElementById("txt_tipocarga").value = data.tipocarga;
            document.getElementById("txt_articulo22").value = data.articulo22;
            document.getElementById("txt_cuenta").value = data.numerocuenta;
            document.getElementById("txt_tipoapv").value = data.tipoapv;
            document.getElementById("txt_apv").value = data.apv;
            document.getElementById("txt_ufs").value = data.ufs;
            $("#cbo_tipocontrato").val(data.idtipocontrato).trigger('change');
            $("#cbo_faena").val(data.idfaena).trigger('change');
            $("#cbo_cargo").val(data.idcargo).trigger('change');
            $("#cbo_centro").val(data.idcentrocosto).trigger('change');
            $("#cbo_jornada").val(data.idjornada).trigger('change');
            $("#cbo_banco").val(data.idbanco).trigger('change');
            $("#cbo_afp").val(data.codigoafp).trigger('change');
            $("#cbo_isapre").val(data.codigoisapre).trigger('change');
            $("#cbo_tipocuenta").val(data.idtipocuenta).trigger('change');

            var elemento = document.getElementsByClassName("form-group-float-label");
            for (var i = 0; i < elemento.length; i++) {
                elemento[i].className += " is-visible";
            }
        })
    }
    else {

        document.getElementById("txt_id").innerHTML = "";
        document.getElementById("txt_Cabecera").innerHTML = "Nuevo contrato";

        var elemento = document.getElementsByClassName("form-group-float-label");
        for (var i = 0; i < elemento.length; i++) {
            elemento[i].classList.replace('is-visible', 'no-visible');
        }
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



function Eliminar(id) {

    if (confirm("Confirma que desea eliminar la información de este contrato?") == 1) {

        $.get("InhabilitaContrato?Id=" + id + "&empresa=" + idEmpre, function (data) {
            if (data.info.result == 0) {
                alert("Ocurrio un error");
            }
            else if (data.info.result == -1) {
                alert("No se pudo completar el proceso");
            }
            else {
                alert("Se elimino correctamente");
                inicio();
            }
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

    if (!ValidarRut(formatoRutGuardar(document.getElementById("txt_rut").value))) {
        alert("El rut ingresado no es válido");
        return 0;
    }

    var objeto = validarDatos();
    var esCorrecto = objeto.exito;
    if (esCorrecto == false) {
        document.getElementById("divErrores").innerHTML = "<ol>" + objeto.mensaje + "</ol>";
        return;
    }
        
    var id = document.getElementById("txt_id").innerHTML;
    var rut = formatoRutGuardar(document.getElementById("txt_rut").value);
    var nombres = document.getElementById("txt_nombres").value.toString().toUpperCase();
    var apellidos = document.getElementById("txt_apellidos").value.toString().toUpperCase();
    var idtipocontrato = document.getElementById("cbo_tipocontrato").value;
    var contrato = document.getElementById("txt_contrato").value;
    var finicio = document.getElementById("txt_inicio").value;
    var ftermino = document.getElementById("txt_termino").value;
    var sueldo = document.getElementById("txt_sueldo").value;
    var idfaena = document.getElementById("cbo_faena").value;
    var idcargo = document.getElementById("cbo_cargo").value;
    var idcentro = document.getElementById("cbo_centro").value;
    var idjornada = document.getElementById("cbo_jornada").value;
    var idbanco = document.getElementById("cbo_banco").value;
    var idafp = document.getElementById("cbo_afp").value;
    var idisapre = document.getElementById("cbo_isapre").value;
    var observacion = document.getElementById("txt_observacion").value;
    var tipocarga = document.getElementById("txt_tipocarga").value;
    var articulo22 = document.getElementById("txt_articulo22").value;
    var idtipocuenta = document.getElementById("cbo_tipocuenta").value;
    var numerocuenta = document.getElementById("txt_cuenta").value;
    var tipoapv = document.getElementById("txt_tipoapv").value;
    var apv = document.getElementById("txt_apv").value;
    var ufs = document.getElementById("txt_ufs").value;
    var empresa = idEmpre;


    var frm = new FormData();
    frm.append("id", id);
    frm.append("rut", rut);
    frm.append("nombres", nombres);
    frm.append("apellidos", apellidos);
    frm.append("idtipocontrato", idtipocontrato);
    frm.append("contrato", contrato);
    frm.append("inicio", finicio);
    frm.append("termino", ftermino);
    frm.append("sueldobase", sueldo);
    frm.append("idfaena", idfaena);
    frm.append("idcargo", idcargo);
    frm.append("idcentrocosto", idcentro);
    frm.append("idjornada", idjornada);
    frm.append("idbancotrab", idbanco);
    frm.append("idafptrab", idafp);
    frm.append("idisapretrab", idisapre);
    frm.append("observaciones", observacion);
    frm.append("tipocarga", tipocarga);
    frm.append("articulo22", articulo22);
    frm.append("idtipocuenta", idtipocuenta);
    frm.append("numerocuenta", numerocuenta);
    frm.append("tipoapv", tipoapv);
    frm.append("apv", apv);
    frm.append("ufs", ufs);
    frm.append("empresa", empresa);

    if (id == '') {
        $.ajax({
            type: "POST",
            url: "CrearContrato",
            data: frm,
            contentType: false,
            processData: false,

            success: function (data) {
                if (data.info.result == 0) {
                    alert("Ocurrio un error");
                } else if (data.info.result == -1) {
                    alert("No se pudo completar el proceso");
                }
                else {
                    alert("Proceso finalizado correctamente");
                    inicio();
                    limpiar();
                    document.getElementById("btnCerrar").click();
                }
            }
        });
    } else {
        $.ajax({
            type: "POST",
            url: "EditaContrato",
            data: frm,
            contentType: false,
            processData: false,

            success: function (data) {
                if (data.info.result == 0) {
                    alert("Ocurrio un error");
                } else if (data.info.result == -1) {
                    alert("No se pudo completar el proceso");
                }
                else {
                    alert("Proceso finalizado correctamente");
                    inicio();
                    limpiar();
                    document.getElementById("btnCerrar").click();
                }
            }
        });
    }
        
}

function mostrarTiposContratos() {
    $.get("ComboTiposContrato/?empresa="+idEmpre, function (data) {
        var contenido = "";
        contenido += "<option value=''>--Seleccione--</option>"
        //Generando las opciones
        if (data.info.result == 0) {
            alert(data.info.mensaje);
        }
        if (data.info.result == 1) {
            for (var i = 0; i < data.info.data.length; i++) {
                contenido += "<option value='" + data.info.data[i].id + "'>" + data.info.data[i].descripcion + "</option>"
            }
        }
        document.getElementById("cbo_tipocontrato").innerHTML = contenido;
        ComponentBuilder.configurarSelectEnModal("cbo_tipocontrato", "modal_Contratos");
    })
}

function mostrarFaenas() {
    $.get("ComboFaenas/?empresa=" + idEmpre, function (data) {
        var contenido = "";
        contenido += "<option value=''>--Seleccione--</option>"
        //Generando las opciones
        if (data.info.result == 0) {
            alert(data.info.mensaje);
        }
        if (data.info.result == 1) {
            for (var i = 0; i < data.info.data.length; i++) {
                contenido += "<option value='" + data.info.data[i].id + "'>" + data.info.data[i].descripcion + "</option>"
            }
        }
        document.getElementById("cbo_faena").innerHTML = contenido;
        ComponentBuilder.configurarSelectEnModal("cbo_faena", "modal_Contratos");
    })
}
function mostrarCargos() {
    $.get("ComboCargos/?empresa=" + idEmpre, function (data) {
        var contenido = "";
        contenido += "<option value=''>--Seleccione--</option>"
        //Generando las opciones
        if (data.info.result == 0) {
            alert(data.info.mensaje);
        }
        if (data.info.result == 1) {
            for (var i = 0; i < data.info.data.length; i++) {
                contenido += "<option value='" + data.info.data[i].id + "'>" + data.info.data[i].descripcion + "</option>"
            }
        }
        document.getElementById("cbo_cargo").innerHTML = contenido;
        ComponentBuilder.configurarSelectEnModal("cbo_cargo", "modal_Contratos");
    })
}
function mostrarCentros() {
    $.get("ComboCentros/?empresa=" + idEmpre, function (data) {
        var contenido = "";
        contenido += "<option value=''>--Seleccione--</option>"
        //Generando las opciones
        if (data.info.result == 0) {
            alert(data.info.mensaje);
        }
        if (data.info.result == 1) {
            for (var i = 0; i < data.info.data.length; i++) {
                contenido += "<option value='" + data.info.data[i].id + "'>" + data.info.data[i].descripcion + "</option>"
            }
        }
        document.getElementById("cbo_centro").innerHTML = contenido;
        ComponentBuilder.configurarSelectEnModal("cbo_centro", "modal_Contratos");
    })
}
function mostrarJornadas() {
    $.get("ComboJornadas/?empresa=" + idEmpre, function (data) {
        var contenido = "";
        contenido += "<option value=''>--Seleccione--</option>"
        //Generando las opciones
        if (data.info.result == 0) {
            alert(data.info.mensaje);
        }
        if (data.info.result == 1) {
            for (var i = 0; i < data.info.data.length; i++) {
                contenido += "<option value='" + data.info.data[i].id + "'>" + data.info.data[i].descripcion + "</option>"
            }
        }
        document.getElementById("cbo_jornada").innerHTML = contenido;
        ComponentBuilder.configurarSelectEnModal("cbo_jornada", "modal_Contratos");
    })
}
function mostrarBancos() {
    $.get("ComboBancos/?empresa=" + idEmpre, function (data) {
        var contenido = "";
        contenido += "<option value=''>--Seleccione--</option>"
        //Generando las opciones
        if (data.info.result == 0) {
            alert(data.info.mensaje);
        }
        if (data.info.result == 1) {
            for (var i = 0; i < data.info.data.length; i++) {
                contenido += "<option value='" + data.info.data[i].id + "'>" + data.info.data[i].descripcion + "</option>"
            }
        }
        document.getElementById("cbo_banco").innerHTML = contenido;
        ComponentBuilder.configurarSelectEnModal("cbo_banco", "modal_Contratos");
    })
}
function mostrarAfps() {
    $.get("ComboAfps/?empresa=" + idEmpre, function (data) {
        var contenido = "";
        contenido += "<option value=''>--Seleccione--</option>"
        //Generando las opciones
        if (data.info.result == 0) {
            alert(data.info.mensaje);
        }
        if (data.info.result == 1) {
            for (var i = 0; i < data.info.data.length; i++) {
                contenido += "<option value='" + data.info.data[i].id + "'>" + data.info.data[i].descripcion + "</option>"
            }
        }
        document.getElementById("cbo_afp").innerHTML = contenido;
        ComponentBuilder.configurarSelectEnModal("cbo_afp", "modal_Contratos");
    })
}
function mostrarIsapres() {
    $.get("ComboIsapres/?empresa=" + idEmpre, function (data) {
        var contenido = "";
        contenido += "<option value=''>--Seleccione--</option>"
        //Generando las opciones
        if (data.info.result == 0) {
            alert(data.info.mensaje);
        }
        if (data.info.result == 1) {
            for (var i = 0; i < data.info.data.length; i++) {
                contenido += "<option value='" + data.info.data[i].id + "'>" + data.info.data[i].descripcion + "</option>"
            }
        }
        document.getElementById("cbo_isapre").innerHTML = contenido;
        ComponentBuilder.configurarSelectEnModal("cbo_isapre", "modal_Contratos");
    })
}
function mostrarTiposCuentas() {
    $.get("ComboTiposCuentas/?empresa=" + idEmpre, function (data) {
        var contenido = "";
        contenido += "<option value=''>--Seleccione--</option>"
        //Generando las opciones
        if (data.info.result == 0) {
            alert(data.info.mensaje);
        }
        if (data.info.result == 1) {
            for (var i = 0; i < data.info.data.length; i++) {
                contenido += "<option value='" + data.info.data[i].id + "'>" + data.info.data[i].descripcion + "</option>"
            }
        }
        document.getElementById("cbo_tipocuenta").innerHTML = contenido;
        ComponentBuilder.configurarSelectEnModal("cbo_tipocuenta", "modal_Contratos");
    })
}
function existeRut(T) {
    var rut = T;
    if (rut != "") {
        $.get("ExistePersona/?rut=" + rut + "&empresa=" + idEmpre, function (data) {
            var res = data.info;
            if (res.result == 1) {
                document.getElementById("txt_nombres").value = res.data.nombres.toString().toUpperCase();
                document.getElementById("txt_apellidos").value = res.data.apellidos.toString().toUpperCase();
            }
        });
    }
}
