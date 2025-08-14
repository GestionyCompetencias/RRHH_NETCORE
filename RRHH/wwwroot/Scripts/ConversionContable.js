var tipo = "P";
inicio();

function inicio() {
    ComboConceptos();
    ComboCuentas();
}

function Listar() {
    document.getElementById("nuevo").style.display = 'none';
    tipo = document.getElementById("txt_modulo").value;
    $.get("ListarConversionContable/?tipo=" + tipo, function (data) {
        ListaConversiones(["Id", "Modulo", "Pago", "Concepto", "Cuenta", "Tip.Auxiliar", "Auxiliar", "Debe o haber", "Agrupa"], data, "list_conversion");
    })

}



function ListaConversiones(cabeceras, data, divId) {
    document.getElementById("esperar").style.display = 'block';
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
    if (data.info.result == 1) {
        if (info.length > 0) {
            var propiedadesObjeto = Object.keys(info[0]);

            contenido += "<tbody>";

            var fila;
            for (var i = 0; i < info.length; i++) {
                fila = info[i];
                contenido += "<tr>";

                for (var j = 0; j < propiedadesObjeto.length; j++) {
                    var nombrePropiedad = propiedadesObjeto[j];
                    if (nombrePropiedad == "id" || nombrePropiedad == "modulo" || nombrePropiedad == "pago" || nombrePropiedad == "concepto"
                        || nombrePropiedad == "cuenta" || nombrePropiedad == "tipoauxiliar" || nombrePropiedad == "codigoauxiliar" || nombrePropiedad == "debehaber"
                        || nombrePropiedad == "agrupacion" ) contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                contenido += "<td>";
                contenido += "<button onclick='abrirModal(" + fila.id + ")' class='btn btn-primary rounded-round btn-sm' ";
                contenido += "data-toggle='modal' data-target='#modal_Conversion'><i class='fa fa-pen'></i></button> ";
                contenido += "<button onclick='Eliminar(" + fila.id + ")' class='btn btn-danger rounded-round btn-sm btn'><i class='fa fa-trash-alt'></i></button> ";
                contenido += "</td>";
                contenido += "</tr>";
            }

            contenido += "</tbody>";
        }

    }

    contenido += "</table>";

    document.getElementById(divId).innerHTML = contenido;
    document.getElementById("esperar").style.display = 'none';
    document.getElementById("nuevo").style.display = 'block';

}


function abrirModal(id) {
    if (id != undefined) {
        document.getElementById("txt_Cabecera").innerHTML = "Editando conversion";
        $.get("ConsultaConversionContableId/?id=" + id, function (data) {
            var info = data.info.data;
            console.log(info);
            document.getElementById("txt_id").innerHTML = info.id;
            document.getElementById("cbo_concepto").value = info.concepto;
            document.getElementById("cbo_cuenta").value = info.cuenta;
            document.getElementById("txt_tipaux").value = info.tipoauxiliar;
            document.getElementById("txt_auxiliar").value = info.auxiliar;
            document.getElementById("txt_deshab").value = info.debehaber;
            document.getElementById("txt_tipven").value = info.tipovencimiento;
            document.getElementById("txt_diaven").value = info.diavencimiento;
            document.getElementById("txt_mesven").value = info.mesvencimineto;
            document.getElementById("txt_grupo").value = info.grupo;

            var elemento = document.getElementsByClassName("form-group-float-label");

            for (var i = 0; i < elemento.length; i++) {
                elemento[i].className += " is-visible";
            }
        })
    }
    else {
        document.getElementById("txt_id").value = "0";
        document.getElementById("txt_Cabecera").innerHTML = "Nueva conversion";
        document.getElementById("cbo_concepto").value = "";
        document.getElementById("cbo_cuenta").value = "";
        document.getElementById("txt_tipaux").value = "1";
        document.getElementById("txt_auxiliar").value = "";
        document.getElementById("txt_deshab").value = "D";
        document.getElementById("txt_tipven").value = "1";
        document.getElementById("txt_diaven").value = "0";
        document.getElementById("txt_mesven").value = "0";
        document.getElementById("txt_grupo").value = "2";

        var elemento = document.getElementsByClassName("form-group-float-label");
        for (var i = 0; i < elemento.length; i++) {
            elemento[i].classList.replace('is-visible', 'no-visible');
        }
    }
}


function limpiar() {
    document.getElementById("txt_id").innerHTML = "0";

    var elementosConClaseLimpiar = document.getElementsByClassName("cls_t");
    var nelementos = elementosConClaseLimpiar.length;
    for (var i = 0; i < nelementos; i++) {
        elementosConClaseLimpiar[i].value = "";
    }

}


function Guarda() {
    var id = document.getElementById("txt_id").innerHTML;
        var modulo = document.getElementById("txt_modulo").value;
        var concepto = document.getElementById("cbo_concepto").value;
        var cuenta = document.getElementById("cbo_cuenta").value;
        var tipoauxiliar = document.getElementById("txt_tipaux").value;
        var auxiliar = document.getElementById("txt_auxiliar").value;
        var tipodeshab = document.getElementById("txt_deshab").value;
        var tipovencimiento = document.getElementById("txt_tipven").value;
        var diaven = document.getElementById("txt_diaven").value;
        var mesven = document.getElementById("txt_mesven").value;
        var grupo = document.getElementById("txt_grupo").value;

    var frm = new FormData();
    frm.append("id", id);
    frm.append("modulo", modulo);
    frm.append("pago", "L");
    frm.append("concepto", concepto);
    frm.append("cuenta", cuenta);
    frm.append("tipoauxiliar", tipoauxiliar);
    frm.append("codigoauxiliar", auxiliar);
    frm.append("debehaber", tipodeshab);
    frm.append("tipovencimiento", tipovencimiento);
    frm.append("diavencimiento", diaven);
    frm.append("mesvencimiento", mesven);
    frm.append("grupo", grupo);

    $.ajax({
        type: "POST",
        url: "CrearConversionContable",
        data: frm,
        contentType: false,
        processData: false,

        success: function (data) {
            var info = data.info;
            alert(info.mensaje)
            if (data.result == 2) { document.getElementById("btnCerrare").click(); }
            else { document.getElementById("btnCerrar").click(); }
            limpiar();
            document.getElementById("txt_modulo").value = tipo;
            Listar();
        }
    });
}


function Eliminar(id) {

    if (confirm("Confirma que desea eliminar esta información?") == 1) {

        $.get("InhabilitaConversionContable/?id=" + id, function (data) {
            alert(data.mensaje)
            limpiar();
            document.getElementById("txt_modulo").value = tipo;
            Listar();
        })
    }
}
function ComboConceptos() {
    $.get("ComboConceptos", (data) => {
        var info = data.info.data;
        var contenido = "";
        contenido += "<option value=''>--Seleccione--</option>"
        //Generando las opciones
        for (var i = 0; i < info.length; i++) {
            contenido += "<option value='" + info[i].codigo + "'>" + info[i].descripcion + "</option>"

        }
        document.getElementById("cbo_concepto").innerHTML = contenido;
    })
}
function ComboCuentas() {
    $.get("ComboCuentas", (data) => {
        var info = data.info.data;
        var contenido = "";
        contenido += "<option value=''>--Seleccione--</option>"
        //Generando las opciones
        for (var i = 0; i < info.length; i++) {
            contenido += "<option value='" + info[i].codigo + "'>" + info[i].descripcion + "</option>"

        }
        document.getElementById("cbo_cuenta").innerHTML = contenido;
    })
}

