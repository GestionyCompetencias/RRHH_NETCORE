inicio();
mostrarPaisesFae();
mostrarRegionesFae();
var idEmpre = "";

var faenaModal = ComponentBuilder.setModal("modal_Faenas");

function inicio() {
    document.getElementById("esperar").style.display = 'none';
    mostrarDatos();
}


function mostrarDatos() {

    document.getElementById("esperar").style.display = 'block';

    $.get("EmpresaLog", function (data) {
        idEmpre = data;
        obtenerDatosValidos(idEmpre);
    })

    $("#cbo_region").hide();
    $("#cbo_comuna").hide();

}


function obtenerDatosValidos(idEmpre) {

    $.get("ListarFaenas?empresa=" + idEmpre, function (data) {

        if (data.info.result == 0) {
            alert(data.info.mensaje);
            document.getElementById("esperar").style.display = 'none';
        } else {
            crearListado(["Contrato", "Descripción", "Inicio", "Termino",  "Dirección", "Región", "Comuna"], data, "list_faenas");

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

                if (nombrePropiedad == 'contrato') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'descripcion') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'inicio') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'termino') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'direccion') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'region') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'comuna') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }

            };

            contenido += "<td style='text-align:right'>";

            contenido += "<button onclick='abrirModal(" + fila.id + ")' class='btn btn-primary rounded-round btn-sm' ";
            contenido += "data-bs-toggle='modal' data-bs-target='#modal_Faenas'><i class='fa fa-pen'></i></button> ";

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

        document.getElementById("txt_Cabecera").innerHTML = "Editar faena";

        $.get("ConsultaFaenaId?id="+ id +"&empresa="+idEmpre, function (data) {
            data = data.info.data;
            document.getElementById("txt_id").innerHTML = data[0].id;
            document.getElementById("txt_contrato").value = data[0].contrato;
            document.getElementById("txt_descripcion").value = data[0].descripcion.toString().toUpperCase();
            document.getElementById("txt_fecini").value = data[0].inicio.toString().toUpperCase();
            document.getElementById("txt_fecter").value = data[0].termino;
            document.getElementById("txt_direccion").value = data[0].direccion;
            $("#cbo_pais").val(data[0].idPais).trigger('change');
            setTimeout(() => { $("#cbo_region").val(data[0].idRegion).trigger('change') }, 700);
            mostrarComunasFae();
            setTimeout(() => { $("#cbo_comuna").val(data[0].idComuna).trigger('change') }, 1000);

            var elemento = document.getElementsByClassName("form-group-float-label");
            for (var i = 0; i < elemento.length; i++) {
                elemento[i].className += " is-visible";
            }
        })
    }
    else {
        document.getElementById("txt_id").innerHTML = "";
        document.getElementById("txt_Cabecera").innerHTML = "Nueva faena";

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

    if (confirm("Confirma que desea eliminar la información de esta faena?") == 1) {

        $.get("InhabilitaFaena?Id=" + id + "&empresa=" + idEmpre, function (data) {
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
        
    var id = document.getElementById("txt_id").innerHTML;
    var contrato = formatoRutGuardar(document.getElementById("txt_contrato").value);
    var descripcion = document.getElementById("txt_descripcion").value.toString().toUpperCase();
    var fecini = document.getElementById("txt_fecini").value.toString().toUpperCase();
    var fecter = document.getElementById("txt_fecter").value;
    var direccion = document.getElementById("txt_direccion").value;
    var idpais = document.getElementById("cbo_pais").value;
    var idregion = document.getElementById("cbo_region").value;
    var idcomuna = document.getElementById("cbo_comuna").value;
    var empresa = idEmpre;


    var frm = new FormData();
    frm.append("id", id);
    frm.append("contrato", contrato);
    frm.append("descripcion", descripcion);
    frm.append("inicio", fecini);
    frm.append("termino", fecter);
    frm.append("direccion", direccion);
    frm.append("idPais", idpais);
    frm.append("idregion", idregion);
    frm.append("idcomuna", idcomuna);
    frm.append("empresa", empresa);


    if (id == '') {
        $.ajax({
            type: "POST",
            url: "CrearFaena",
            data: frm,
            contentType: false,
            processData: false,

            success: function (data) {
                    alert(data.info.mensaje);
                    limpiar();
                    document.getElementById("btnCerrar").click();
                    inicio();
            }
        });
    } else {
        $.ajax({
            type: "POST",
            url: "EditaFaena",
            data: frm,
            contentType: false,
            processData: false,

            success: function (data) {
                    alert(data.info.mensaje);
                limpiar();
                document.getElementById("btnCerrar").click();
                inicio();
            }
        });
    }
        
}
function mostrarPaisesFae() {
    $.get("ConsultarPaises/?empresa=28", function (data) {

        var contenido = "";
        contenido += "<option value=''>--Seleccione--</option>"
        //Generando las opciones
        for (var i = 0; i < data.info.data.length; i++) {
            contenido += "<option value='" + data.info.data[i].idPais + "'>" + data.info.data[i].nombre + "</option>"
        }
        document.getElementById("cbo_pais").innerHTML = contenido;
        ComponentBuilder.configurarSelectEnModal("cbo_pais", "modal_Faenas");
    })
}

function mostrarRegionesFae() {

    var idpais = document.getElementById("cbo_pais").value
    if (idpais == "") idpais = 1;
    //if (idPais == 1) {
    //    $("#cbo_region").show();
    //    $("#cbo_comuna").show();
    //} else {
    //    $("#cbo_region").val('')
    //    $("#cbo_comuna").val('')
    //    $("#cbo_region").hide();
    //    $("#cbo_comuna").hide();
    //}
    $.get("ConsultarRegiones?idpais=" + idpais + "&empresa=28", function (data) {
        var contenido = "";

        contenido += "<option value=''>--Seleccione--</option>"
        //Generando las opciones
        if (data.info.result == 1) {
            for (var i = 0; i < data.info.data.length; i++) {
                contenido += "<option value='" + data.info.data[i].idRegion + "'>" + data.info.data[i].nombre + "</option>"

            }
        }

        document.getElementById("cbo_region").innerHTML = contenido;
        ComponentBuilder.configurarSelectEnModal("cbo_region", "modal_Faenas");

    })
}

function mostrarComunasFae() {

    var idRegion = document.getElementById("cbo_region").value
    $.get("ConsultarComunas?idregion=" + idRegion + "&empresa=28", function (data) {

        var contenido = "";
        contenido += "<option value=''>--Seleccione--</option>"
        //Generando las opciones
        if (data.info.result == 1) {
            for (var i = 0; i < data.info.data.length; i++) {
                contenido += "<option value='" + data.info.data[i].idComuna + "'>" + data.info.data[i].nombre + "</option>"

            }
        }

        document.getElementById("cbo_comuna").innerHTML = contenido;
        ComponentBuilder.configurarSelectEnModal("cbo_comuna", "modal_Faenas");

    })
}


