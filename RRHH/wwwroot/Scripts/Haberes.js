inicio();
var idEmpre = "";

var haberModal = ComponentBuilder.setModal("modal_Haberes");

function inicio() {
    document.getElementById("esperar").style.display = 'none';
    mostrarDatos();
}


function mostrarDatos() {
    document.getElementById("esperar").style.display = 'block';
    $.get("EmpresaLog", function (data) {
        idEmpre = data;
        setTimeout(() => { mostrarCodigosDT()}, 500);
        //setTimeout(() => { mostrarCodigosPrevired() }, 500);
        obtenerDatosValidos(idEmpre);
    })
}


function obtenerDatosValidos(idEmpre) {
    $.get("ListarHaberes?empresa=" + idEmpre, function (data) {

        if (data.info.result == 0) {
            alert(data.info.mensaje);
            document.getElementById("esperar").style.display = 'none';
        } else {
            crearListado(["Haber", "Descripción", "Imponible", "Tributable", "Garantizado", "Retenible", "Deducible", "Codigo DT"], data, "list_haberes");
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

                if (nombrePropiedad == 'haber') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'descripcion') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'imponible') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'tributable') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'garantizado') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'retenible') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'codigoDT') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'deducible') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
            };
            contenido += "<td style='text-align:right'>";
            contenido += "<button onclick='abrirModal(" + fila.id + ")' class='btn btn-primary rounded-round btn-sm' ";
            contenido += "data-bs-toggle='modal' data-bs-target='#modal_Haberes'><i class='fa fa-pen'></i></button> ";
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

        document.getElementById("txt_Cabecera").innerHTML = "Editar haber";
        document.getElementById('txt_haber').disabled = true;
        $.get("ConsultaHaberId?id="+ id +"&empresa="+idEmpre, function (data) {
            data = data.info.data;
            document.getElementById("txt_id").innerHTML = data[0].id;
            document.getElementById("txt_haber").value = data[0].haber;
            document.getElementById("txt_descripcion").value = data[0].descripcion;
            document.getElementById("txt_imponible").value = data[0].imponible;
            document.getElementById("txt_tributable").value = data[0].tributable;
            document.getElementById("txt_garantizado").value = data[0].garantizado;
            document.getElementById("txt_retenible").value = data[0].retenible;
            document.getElementById("txt_deducible").value = data[0].deducible;
            document.getElementById("txt_licencias").value = data[0].baselicencia;
            document.getElementById("txt_sobretiempo").value = data[0].basesobretiempo;
            document.getElementById("txt_indemnizacion").value = data[0].baseindemnizacion;
            document.getElementById("txt_variable").value = data[0].basevariable;
            $("#cbo_DT").val(data[0].codigoDT).trigger('change');
            //$("#cbo_Previred").val(data[0].codigoprevired).trigger('change');
        })
        limpiar();
   }
    else {
        document.getElementById('txt_haber').disabled = false;
        document.getElementById("txt_id").innerHTML = 0;
        $("#cbo_DT").val("").trigger('change');
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

    if (confirm("Confirma que desea eliminar la información de este haber?") == 1) {

        $.get("InhabilitaHaber?Id=" + id + "&empresa=" + idEmpre, function (data) {
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

    var objeto = validarDatos();
    var esCorrecto = objeto.exito;
    if (esCorrecto == false) {
        document.getElementById("divErrores").innerHTML = "<ol>" + objeto.mensaje + "</ol>";
        return;
    }
    document.getElementById("divErrores").innerHTML = "";
 
    var id = document.getElementById("txt_id").innerHTML;
    var haber = formatoRutGuardar(document.getElementById("txt_haber").value);
    var descripcion = document.getElementById("txt_descripcion").value.toString().toUpperCase();
    var imponible = document.getElementById("txt_imponible").value;
    var tributable = document.getElementById("txt_tributable").value;
    var garantizado = document.getElementById("txt_garantizado").value;
    var retenible = document.getElementById("txt_retenible").value;
    var deducible = document.getElementById("txt_deducible").value;
    var baselicencia = document.getElementById("txt_licencias").value;
    var basesobretiempo = document.getElementById("txt_sobretiempo").value;
    var baseindemnizacion = document.getElementById("txt_indemnizacion").value;
    var basevariable = document.getElementById("txt_variable").value;
    var codigoDT = document.getElementById("cbo_DT").value;
    //var codigoprevired = document.getElementById("cbo_previred").value;
    var empresa = idEmpre;
    if (haber < 11 || haber > 99) {
        document.getElementById("divErrores").innerHTML = "<ol>" + "Codigo de haber fuera de rango" + "</ol>";
        return;
   }

    var frm = new FormData();
    frm.append("id", id);
    frm.append("haber", haber);
    frm.append("descripcion", descripcion);
    frm.append("imponible", imponible);
    frm.append("tributable", tributable);
    frm.append("garantizado", garantizado);
    frm.append("retenible", retenible);
    frm.append("deducible", deducible);
    frm.append("baselicencia", baselicencia);
    frm.append("basesobretiempo", basesobretiempo);
    frm.append("baseindemnizacion", baseindemnizacion);
    frm.append("basevariable", basevariable);
    frm.append("codigoDT", codigoDT);
    //frm.append("codigoprevired", codigoprevired);
    frm.append("empresa", empresa);
    if (id != 0) {
        $.ajax({
            type: "POST",
            url: "EditaHaber",
            data: frm,
            contentType: false,
            processData: false,

            success: function (data) {
                alert(data.info.mensaje);
                limpiar();
                inicio();
                document.getElementById("btnCerrar").click();
                }
        });
    } else {
        $.ajax({
             type: "POST",
            url: "CrearHaber",
            data: frm,
            contentType: false,
            processData: false,

            success: function (data) {
                alert(data.info.mensaje);
                limpiar();
                inicio();
                document.getElementById("btnCerrar").click();
            }
       });
    }
        
}

function mostrarCodigosDT() {
    $.get("ComboCodigosDT/?empresa="+idEmpre, function (data) {
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
        document.getElementById("cbo_DT").innerHTML = contenido;
        ComponentBuilder.configurarSelectEnModal("cbo_DT", "modal_Haberes");
    })
}

//function mostrarCodigosPrevired() {
//    $.get("ComboCodigosPrevired/?empresa=" + idEmpre, function (data) {
//        var contenido = "";
//        contenido += "<option value=''>--Seleccione--</option>"
//        //Generando las opciones
//        if (data.info.result == 0) {
//            alert(data.info.mensaje);
//        }
//        if (data.info.result == 1) {
//            for (var i = 0; i < data.info.data.length; i++) {
//                contenido += "<option value='" + data.info.data[i].id + "'>" + data.info.data[i].descripcion + "</option>"
//            }
//        }
//        document.getElementById("cbo_previred").innerHTML = contenido;
//        ComponentBuilder.configurarSelectEnModal("cbo_previred", "modal_Haberes");
//    })
//}
