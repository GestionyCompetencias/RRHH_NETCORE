
var idVentAcept = '';

$(document).ready(function () {
    ComponentBuilder.configurarSelectEnModal("txt_cta_cont", "modal_VentaView");
    ComponentBuilder.configurarSelectEnModal("txt_ctra_cta", "modal_VentaView");
    ComponentBuilder.configurarSelectEnModal("txt_presup_rela", "modal_VentaView");
});

// Creación de modal utilizando ComponentBuilder
var DocVenta = ComponentBuilder.setModal("modal_VentaView");


$(function () {

    getEmpresaLog();

    $(document).on("change", "#txt_presup_rela", function () {
        const id = $(this).val();
        if (id === '') return;
        load_hijos_presupuesto(id);
    });

});

const getEmpresaLog = () => {
    document.getElementById("esperar").style.display = 'block';
    $.get("EmpresaLog", (data) => {
        idEmpresa = data;

        getVentasPresup();
        getGlosas();
        loadPresupuestos();
    });
};


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


const getVentasPresup = () => {
    const dest = `ConsultarVentasPendientes?empresa=${idEmpresa}`;

    $.get(dest, (resp) => {
        const ventas = resp.info.data || null;

        if (ventas !== null) {

            console.log(ventas);

            var contenido = ``;
            ventas.forEach(item => {
                contenido += `<tr>`;
                contenido += `<td>${item.presupuestoCod}</td>`;
                contenido += `<td>${item.descripPresup}</td>`;

                contenido += `<td>${item.folio}</td>`;
                contenido += `<td>${formatFecha(item.fechaFact)}</td>`;
                contenido += `<td>${(item.rutCli)}</td>`;
                contenido += `<td>${item.razonSocial}</td>`;
                contenido += `<td>${item.monto}</td>`;
                contenido += `<td>${item.cuenta}</td>`;
                contenido += `<td>${item.contracuenta}</td>`;

                contenido += `<td class='text-center'>`;
                contenido += `<button onclick='verVenta(${item.idVP})' class='text-primary  border-0' data-toggle='modal' data-target='#modal_usuarios'><i class='fa fa-eye'></i></button>`;
                contenido += `<button onclick='aprobarVenta(${item.idVP})' class='text-success  border-0' data-toggle='modal' data-target='#modal_usuarios'><i class='fa fa-check'></i></button>`;
                contenido += ` <button onclick='rechazarVenta(${item.idVP})' data-toggle='modal' data-target='#modal_delete_user' class='text-danger  border-0'><i class='fa fa-trash'></i></button>`;
                contenido += `</td>`;
                contenido += `</tr>`;
            });
            $("#list_documentos").html(contenido);
            $('#tabla-ventas').DataTable({
                autoWidth: true,
            });
            

        } else {

        }

        document.getElementById("esperar").style.display = 'none';
    });
}


function agregarVentaDoc() {

    if (idVentAcept != '') {
        aprobarVenta(idVentAcept)
    } else {
        alert('Ocurrió un problema, por favor recargue la página e intente de nuevo');
    }    

}


const getGlosas = () => {

    document.getElementById("esperar").style.display = 'block';
    const dest = `listarglosas?empresa=${idEmpresa}`;
    $.get(dest, (resp) => {
        const glosas = resp.info.data;
        contenido = "<option value=''>--Seleccione--</option>";
        if (glosas !== null) {
            glosas.forEach(glosa => {
                contenido += `<option value='${glosa.codglosa}'>${glosa.codglosa} - ${glosa.nombreglosa}</option>`
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

                contenido += `<option value='${item.id}'>${item.num_proceso} - ${item.descripcion.toUpperCase()}</option>`

                $("#txt_presup_rela").html(contenido);

            });

        }

        document.getElementById("esperar").style.display = 'none';
    });
}



const verVenta = (id) => {

    idVentAcept = id;
    document.getElementById("esperar").style.display = 'block';
    const dest = `ConsultarVentaIdVp?IdVp=${id}&empresa=${idEmpresa}`;

    $.get(dest, (data) => {

        const documento = data.info.data || null;

        if (documento !== null) {

            $("#txt_num_doc").val(documento[0].folio);
            $("#txt_rut_cli").val(documento[0].rutCli);
            $("#txt_razon_cli").val(documento[0].razonSocial);
            $("#txt_fech_doc").val(formatFechaGuion1(documento[0].fechaFact));

            $("#txt_doc_total").val(Mappers.FormatCurrency(documento[0].monto));
            //$("#txt_doc_excento").val(Mappers.FormatCurrency(documento[0].montoExcen));
            $("#txt_doc_iva").val(Mappers.FormatCurrency(documento[0].montoIva));
            $("#txt_doc_glosa").val(documento[0].glosa);
            $("#txt_cta_cont").val(documento[0].cuenta).trigger('change');
            $("#txt_ctra_cta").val(documento[0].contracuenta).trigger('change');
            $("#txt_presup_rela").val(documento[0].presupuestoCod).trigger('change');

            setTimeout(() => {
                var hijos = "";
                documento.forEach(item => {
                    if (hijos == '') {
                        hijos += item.hijo
                    } else {
                        hijos += '*' + item.hijo;
                    }

                    document.getElementById("ckbHijo" + item.hijo).checked = true;
                });
            }, 1000);
            

            $("#cargando_btn").hide();
            document.getElementById("txt_doc_glosa").focus();
        } 
                
        DocVenta.show();
        document.getElementById("esperar").style.display = 'none';
    });
       
}


const aprobarVenta = (id) => {

    var EnviarMail = document.getElementById("ckbMail").checked;

    const datosVenta = {
        idVentPresup: id,
        mail: EnviarMail
    }

    const dest = `AprobarVentaPresup?empresa=${idEmpresa}`;

    $.post(dest, datosVenta, resp => {

        if (resp.info.result === 1) {
            alert(resp.info.mensaje);
        } else {
            alert(resp.info.mensaje);
        }
    });

}



const rechazarVenta = (id) => {

    Swal.fire({
        title: "Rechazar este proceso",
        text: "¿Confirma que desea rechazar este proceso?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Si, RechazarYes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {

            const datosVenta = {
                idVentPresup: id
            }

            const dest = `RechazarVentaPresup?empresa=${idEmpresa}`;

            $.post(dest, datosVenta, resp => {

                if (resp.info.result === 1) {
                    Swal.fire({
                        title: "Deleted!",
                        text: "Your file has been deleted.",
                        icon: "success"
                    });
                } else {
                    alert(resp.info.mensaje);
                }
            });


            
        }
    });
       
}










const formatFecha = (fecha) => {
    let f = fecha.substring(0, 10);
    let arrF = f.split('-');
    return `${arrF[2]}-${arrF[1]}-${arrF[0]}`;
}


function formatoRutMostrar(rut) {
    var nuevoRut = rut;
    var longitudRut = nuevoRut.length;
    nuevoRut = nuevoRut.replaceAll(' ', '');
    var nuevoParte1 = nuevoRut.substring(0, longitudRut - 1);
    var nuevoParte2 = nuevoRut.substring(longitudRut - 1, longitudRut);
    nuevoRut = nuevoParte1 + "-" + nuevoParte2;
    return nuevoRut;
}

function formatoRutGuardar(rut) {
    var nuevoRut = rut;
    nuevoRut = nuevoRut.replaceAll('.', '').replaceAll('-', '').replaceAll(' ', '');
    return nuevoRut;
}

const formatFechaGuion1 = (fecha) => {
    // Formato 2001-04-21
    let f = fecha.substring(0, 10);
    let arrF = f.split('-');
    return `${arrF[0]}-${arrF[1]}-${arrF[2]}`;
}