inicio();
var idEmpre = "";

var descuentoModal = ComponentBuilder.setModal("modal_Descuentos");

function inicio() {
    document.getElementById("esperar").style.display = 'none';
    mostrarDatos();
}


function mostrarDatos() {

    document.getElementById("esperar").style.display = 'block';
    $.get("EmpresaLog", function (data) {
        idEmpre = data;
        mostrarCodigosDT();
        obtenerDatosValidos(idEmpre);
    })
}


function obtenerDatosValidos(idEmpre) {

    $.get("ListarDescuentos?empresa=" + idEmpre, function (data) {

        if (data.info.result == 0) {
            alert(data.info.mensaje);
            document.getElementById("esperar").style.display = 'none';
        } else {
            crearListado(["Descuento", "Descripción", "Prioridad", "Mínimo", "Máximo", "Codigo DT"], data, "list_descuentos");

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

                if (nombrePropiedad == 'descuento') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'descripcion') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'prioridad') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'minimo') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'maximo') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'codigoDT') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }

            };

            contenido += "<td style='text-align:right'>";

            contenido += "<button onclick='abrirModal(" + fila.id + ")' class='btn btn-primary rounded-round btn-sm' ";
            contenido += "data-bs-toggle='modal' data-bs-target='#modal_Descuentos'><i class='fa fa-pen'></i></button> ";

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

        document.getElementById("txt_Cabecera").innerHTML = "Editar descuento";
        document.getElementById('txt_descuento').disabled = true;
        $.get("ConsultaDescuentoId?id="+ id +"&empresa="+idEmpre, function (data) {
            data = data.info.data;
            document.getElementById("txt_id").innerHTML = data[0].id;
            document.getElementById("txt_descuento").value = data[0].descuento;
            document.getElementById("txt_descripcion").value = data[0].descripcion;
            document.getElementById("txt_prioridad").value = data[0].prioridad;
            document.getElementById("txt_minimo").value = data[0].minimo;
            document.getElementById("txt_maximo").value = data[0].maximo;
            $("#cbo_DT").val(data[0].codigoDT).trigger('change');
            //$("#cbo_Previred").val(data[0].codigoprevired).trigger('change');
        })
        limpiar();
   }
    else {
        document.getElementById('txt_descuento').disabled = false;
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

    if (confirm("Confirma que desea eliminar la información de este descuento?") == 1) {

        $.get("InhabilitaDescuento?Id=" + id + "&empresa=" + idEmpre, function (data) {
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
    var descuento = formatoRutGuardar(document.getElementById("txt_descuento").value);
    var descripcion = document.getElementById("txt_descripcion").value.toString().toUpperCase();
    var prioridad = document.getElementById("txt_prioridad").value;
    var minimo = document.getElementById("txt_minimo").value;
    var maximo = document.getElementById("txt_maximo").value;
    var codigoDT = document.getElementById("cbo_DT").value;
    //var codigoprevired = document.getElementById("cbo_previred").value;
    var empresa = idEmpre;
    if (descuento < 100 || descuento > 900) {
        document.getElementById("divErrores").innerHTML = "<ol>" + "Codigo de descuento fuera de rango" + "</ol>";
        return;
    }


    var frm = new FormData();
    frm.append("id", id);
    frm.append("descuento", descuento);
    frm.append("descripcion", descripcion);
    frm.append("prioridad", prioridad);
    frm.append("minimo", minimo);
    frm.append("maximo", maximo);
    frm.append("codigoDT", codigoDT);
    //frm.append("codigoprevired", codigoprevired);
    frm.append("empresa", empresa);
    if (id != 0) {
        $.ajax({
            type: "POST",
            url: "EditaDescuento",
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
            url: "CrearDescuento",
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
    $.get("ComboDescuentosDT/?empresa="+idEmpre, function (data) {
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
        ComponentBuilder.configurarSelectEnModal("cbo_DT", "modal_Descuentos");
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
//        ComponentBuilder.configurarSelectEnModal("cbo_previred", "modal_Descuentos");
//    })
//}
