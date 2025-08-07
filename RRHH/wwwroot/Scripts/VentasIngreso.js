// Inicialización de elementos utilizando ComponentBuilder
$(document).ready(function () {
    ComponentBuilder.configurarSelect("cbo_tipo_mov");
    ComponentBuilder.configurarSelectEnModal("txt_cta_cont", "modal_docVenta");
    ComponentBuilder.configurarSelectEnModal("txt_ctra_cta", "modal_docVenta");
    ComponentBuilder.configurarSelectEnModal("txt_presup_rela", "modal_docVenta");
});

// Creación de modal utilizando ComponentBuilder
var nuevaDocVenta = ComponentBuilder.setModal("modal_docVenta");


var idEmpresa = 0;

$(function () {

    getEmpresaLogVentas();
    $("#cargando_btn").hide();

    $("#btn-add-venta").on("click", function () {
        nuevaDocVenta.show();
    });

    $(document).on("change", "#txt_presup_rela", function () {
        const id = $(this).val();
        if (id === '') return;
        load_hijos_presupuesto(id);
    });

});

const load_hijos_presupuesto = (id) => {
    document.getElementById("esperar").style.display = 'block';
    
    const dest = `ConsultarPresupuestoId?empresa=${idEmpresa}&idPres=${id}`;

    $.get(dest, resp => {
        console.log(resp)
        const presupuestoInt = resp.info.data[0] || null;
        presupuesto = presupuestoInt;
        if (presupuesto.cant_Cursos > 0) {

            hijosTotales = presupuesto.cant_Cursos;

            if (hijosTotales > 0) {
                document.getElementById("Botones_List_hijos").style.display = 'block';
            }

            let contenidoChk = `<div class='row'>`;

            for (h = 1; h <= hijosTotales; h++) {
                contenidoChk += `<div class="col-md-1 ">
                                        <label class="form-check-label">
                                            <input type="checkbox" id="ckbHijo${h}" class="form-check-input-styled" name="ckbHijos" value='${h}'>${h}
                                        </label>
                                    </div>`;
            }

            contenidoChk += `</div>`;

            document.getElementById("List_hijos").innerHTML = contenidoChk;
        } else {
            document.getElementById("Botones_List_hijos").style.display = 'none';
            document.getElementById("List_hijos").innerHTML = '';
        }

        document.getElementById("esperar").style.display = 'none';
    });

    

}

const agregarVentaDoc = () => {

    let valoresCheck = '';

    $("input:checkbox[class=form-check-input-styled]:checked").each(function () {
        let value = $(this).val()
        valoresCheck += value + "*";
    });

    const datosVenta = {
        RutCli: formatoRutGuardar($('#txt_rut_cli').val()),
        NumDoc: $("#txt_num_doc").val(),
        Proyecto: $("#txt_presup_rela").val(),
        fila: $("#lineaPres_id").val(),
        Hijos: valoresCheck,
        Glosa: $("#txt_doc_glosa").val(),
        Cuenta: $("#txt_cta_cont option:selected").text().split(' - ')[0].trim(),
        Contracuenta: $("#txt_ctra_cta option:selected").text().split(' - ')[0].trim(),
    }

    const dest = `VentaGuardarReal?empresa=${idEmpresa}`;

    $.post(dest, datosVenta, resp => {

        if (resp.info.result === 1) {
            nuevaDocVenta.hide();
            alert(resp.info.mensaje);
        } else {
            alert(resp.info.mensaje);
        }
    });
}

const getEmpresaLogVentas = () => {
    document.getElementById("esperar").style.display = 'block';
    $.get("EmpresaLog", (data) => {
        idEmpresa = data;

        getGlosasVentas();
        loadPresupuestos();
    });
};

const getDocInfo = () => {

    $("#cargando_btn").show();

    const numDoc = $('#txt_num_doc').val();

    const dest = `ConsultarDocsVentas?docum=${numDoc}&empresa=${idEmpresa}`;

    $.get(dest, (resp) => {
        const documento = resp.info.data || null;

        if (documento !== null) {

            console.log(documento);
            $("#txt_rut_cli").val(documento[0].rutContaparte);
            $("#txt_razon_cli").val(documento[0].razonSoc);
            $("#txt_fech_doc").val(formatFechaGuion1(documento[0].fecEmis));

            $("#txt_doc_total").val(Mappers.FormatCurrency(documento[0].montoTotal));
            $("#txt_doc_excento").val(Mappers.FormatCurrency(documento[0].montoExcen));
            $("#txt_doc_iva").val(Mappers.FormatCurrency(documento[0].montoIva));

            document.getElementById("txt_doc_glosa").focus();
        } else {
            consultarSII();
        }

        $("#cargando_btn").hide();
    });
}


async function consultarSII() {

    var year = '2024';

    const { value: mesDoc } = await Swal.fire({
        title: "Documento no encontrado",
        text: "¿Desea buscar en SII?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Sí, buscar!",
        input: "select",
        inputOptions: {
            '01': "Enero",
            '02': "Febrero",
            '03': "Marzo",
            '04': "Abril",
            '05': "Mayo",
            '06': "Junio",
            '07': "Julio",
            '08': "Agosto",
            '09': "Septiembre",
            '10': "Octubre",
            '11': "Noviembre",
            '12': "Diciembre"
        },
        inputPlaceholder: "Indique el mes del documento",
        showCancelButton: true,
        inputValidator: (value) => {
            return new Promise((resolve) => {
                if (value >= '01' && value <= '12') {
                    resolve();
                } else {
                    resolve("Debe indicar un mes");
                }
            });
        }
    });
    if (mesDoc) {
        //Swal.fire(`You selected: ${mesDoc}`);

        document.getElementById("esperar").style.display = 'block';
        dest = `VentasApiHttpClient?mes=${mesDoc}&anio=${year}`;

        $.get(dest, (resp) => {
            document.getElementById("esperar").style.display = 'none';
            alert("Proceso finalizado, puede buscar nuevamente el #Documento")
        });
    }
}

const getGlosasVentas = () => {

    document.getElementById("esperar").style.display = 'block';
    const dest = `listarglosas?empresa=${idEmpresa}`;
    $.get(dest, (resp) => {
        const glosas = resp.info.data;
        contenido = "<option value=''>--Seleccione--</option>";
        if (glosas !== null) {
            glosas.forEach(glosa => {
                contenido += `<option value='${glosa.id}'>${glosa.codglosa} - ${glosa.nombreglosa}</option>`
            });
        }
        $("#txt_cta_cont").html(contenido);
        $("#txt_ctra_cta").html(contenido);
    });
    document.getElementById("esperar").style.display = 'none';
};






const loadPresupuestos = () => {
    const dest = `ConsultarPresupuestosLista?empresa=${idEmpresa}`;
    document.getElementById("esperar").style.display = 'block';

    $.get(dest, (resp) => {
        const presupuestos = resp.info.data;
        console.log(resp);
        if (presupuestos !== null) {
            var contenido = `<option value=''>--Seleccione--</option>`;
            presupuestos.forEach(item => {

                contenido += `<option value='${item.id}'>${item.num_proceso} - ${item.descripcion.toUpperCase() }</option>`

                $("#txt_presup_rela").html(contenido);

            });

        }

        document.getElementById("esperar").style.display = 'none';
    });
}






function marcaAll() {
    const all = document.getElementsByName("ckbHijos")
    all.forEach(item => item.checked = true)
}

function desmarcaAll() {
    const all = document.getElementsByName("ckbHijos")
    all.forEach(item => item.checked = false)
}


const resetFormVentas = () => {

    $("#txt_num_doc").val('');

};

const formatFechaGuion1 = (fecha) => {
    // Formato 2001-04-21
    let f = fecha.substring(0, 10);
    let arrF = f.split('-');
    return `${arrF[0]}-${arrF[1]}-${arrF[2]}`;
}

function formatoRutGuardar(rut) {
    var nuevoRut = rut;
    nuevoRut = nuevoRut.replaceAll('.', '').replaceAll('-', '').replaceAll(' ', '');
    return nuevoRut;
}