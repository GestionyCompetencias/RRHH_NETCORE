var idEmpresa = 0;

$(document).ready(() => {
    ComponentBuilder.configurarSelect("anio");
    ComponentBuilder.configurarSelect("mes");
});

$(function () {

    loadMonths();
    loadYears();

    $(document).on("click", "#btn-filter", function (e) {
        e.preventDefault();
        document.getElementById("esperar").style.display = 'block';
        const month = $("#mes").val();
        const year = $("#anio").val();
        if (month > 0) {
            const dest = `ListarResumenAsistencias?mes=${month}&anio=${year}`;

            $.get(dest, (resp) => {
                const libro = resp.info.data;
                const libros = new Array();

                if (libro !== null) {
                    const compArray = new Array();
                    libro.forEach((item) => {
                        const result = compArray.find(({ id }) => id == item.id);
                        if (result === undefined) {
                            compArray.push(item);
                        }
                    });

                    compArray.forEach((lib) => {
                        const newLibro = new Array();
                        libro.forEach((libInt) => {
                            if (libInt.id === lib.id) {
                                newLibro.push(libInt);
                            }
                        });
                        libros.push(newLibro);
                    });

                    const cabeceras = ['RUT', 'Nombre', 'Presentes', 'Descansos', 'Fallas', 'Licencias', 'Perm. S/G', 'Permisos', 'Vacaciones', 'Otras', 'Total'];

                    cargarTabla(cabeceras, libros);
                    cargarTablaImpresion(cabeceras, libros, month, year);
                    document.getElementById("print-export").style.display = 'block';
                    document.getElementById("esperar").style.display = 'none';
                } else {
                    document.getElementById("esperar").style.display = 'none';
                    document.getElementById("print-export").style.display = 'none';
                    document.getElementById("list_asistencias").innerHTML = '';
                    let msg = `<div class="alert alert-info alert-styled-left alert-dismissible">`;
                    msg += `<span class="font-weight-semibold">Aviso!</span> No se han encontrado datos relacionados con los parametros de busqueda ingresados.`;
                    msg += `</div>`;

                    $("#list_asistencias").html(msg);
                }

            });
        }
    });

    $(document).on("click", "#impress_diario", function () {
        //$("#zona_imprimir").printArea();
        var printContent = document.getElementById('zona_imprimir');
        var WinPrint = window.open('', '', 'width=900,height=650');
        WinPrint.document.write(printContent.innerHTML);
        WinPrint.document.close();
        WinPrint.focus();
        WinPrint.print();
        WinPrint.close();
    });

    $(document).on("click", "#export", function () {
        $("#table-libroZ").table2excel({
            filename: "libroRem.xls",
        })
    });

});

const loadMonths = () => {
    const meses = ['ENERO', 'FEBRERO', 'MARZO', 'ABRIL', 'MAYO', 'JUNIO', 'JULIO', 'AGOSTO', 'SEPTIEMBRE', 'OCTUBRE', 'NOVIEMBRE', 'DICIEMBRE'];
    let contenido = "";
    let index = 1;
    meses.forEach((mes) => {
        contenido += `<option value="${index}">${mes}</option>`;
        index++;
    });
    $("#mes").html(contenido);
};

const loadYears = () => {
    const dtNow = new Date();
    let year = dtNow.getFullYear();
    let contenido = "";
    for (let i = 0; i < 10; i++) {
        contenido += `<option value="${year}">${year}</option>`;
        year--;
    }
    $("#anio").html(contenido);


}

const calcularTotalesLibos = (libro) => {
    let totalpresentes = 0;
    let totaldescansos = 0;
    let totalfallas = 0;
    let totallicencias = 0;
    let totalpermisossg = 0;
    let totalpermisos = 0;
    let totalvacaciones = 0;
    let totalotros = 0;
    let totaltotal = 0;

    libro.forEach(it => {
        let montoPresente = parseFloat(it.presente);
        totalpresentes = totalpresentes + montoPresente;
        let montoDescanso = parseFloat(it.descanso);
        totaldescansos = totaldescansos + montoDescanso;
        let montoFalla = parseFloat(it.falla);
        totalfallas = totalfallas + montoFalla;
        let montoLicencia = parseFloat(it.licencia);
        totallicencias = totallicencias + montoLicencia;
        let montoPermisosg = parseFloat(it.permisosg);
        totalpermisossg = totalpermisossg + montoPermisosg;
        let montoPermiso = parseFloat(it.permiso);
        totalpermisos = totalpermisos + montoPermiso;
        let montoVacacion = parseFloat(it.vacacion);
        totalvacaciones = totalvacaciones + montoVacacion;
        let montoOtro = parseFloat(it.otro);
        totalotros = totalotros + montoOtro;
        let montoTotal = parseFloat(it.total);
        totaltotal = totaltotal + montoTotal;
    });

    const newTotal = {
        totalpresentes,
        totaldescansos,
        totalfallas,
        totallicencias,
        totalpermisossg,
        totalpermisos,
        totalvacaciones,
        totalotros,
        totaltotal,
    };

    return newTotal;
}

const cargarTabla = (cabeceras, data) => {
    const totalesLibros = new Array();

    data.forEach(libro => {
        const result = calcularTotalesLibos(libro);
        totalesLibros.push(result);
    });

    let contenido;
    contenido = ``;
    let i = 0;

    data.forEach(lib => {
        contenido += `<table id="table-libro-${i}" class="table table-bordered mb-3">`;
        contenido += `<thead class="bg-dark text-white"><tr>`;
        contenido += `<th width="300">${cabeceras[0]}</th>`;
        contenido += `<th width="300" class="text-right">${cabeceras[1]}</th>`;
        contenido += `<th width="100" class="text-right">${cabeceras[2]}</th>`;
        contenido += `<th width="100" class="text-right">${cabeceras[3]}</th>`;
        contenido += `<th width="100" class="text-right">${cabeceras[4]}</th>`;
        contenido += `<th width="100" class="text-right">${cabeceras[5]}</th>`;
        contenido += `<th width="100" class="text-right">${cabeceras[6]}</th>`;
        contenido += `<th width="100" class="text-right">${cabeceras[7]}</th>`;
        contenido += `<th width="100" class="text-right">${cabeceras[8]}</th>`;
        contenido += `<th width="100" class="text-right">${cabeceras[9]}</th>`;
        contenido += `<th width="100" class="text-right">${cabeceras[10]}</th>`;
        contenido += `</tr>`;
        contenido += `</thead>`;
        contenido += `<tbody>`;
        lib.forEach(itLib => {
            contenido += `<tr>`;
            contenido += `<td class="text-left">${itLib.rut}</td>`;
            contenido += `<td class="text-right">${itLib.nombre}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.presente)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.descanso)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.falla)}</td>`;
            contenido += `<th class="text-right">${formatNumber(itLib.licencia)}</th>`;
            contenido += `<td class="text-right">${formatNumber(itLib.permisosg)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.permiso)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.vacacion)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.otro)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.total)}</td>`;
            contenido += `</tr>`;
        });
        contenido += `</tbody>`;
        contenido += `<tfoot>`;
        contenido += `<tr>`;
        const result = calcularTotalesLibos(lib);
        contenido += `<th class="bg-light" colspan="2">Totales... </th>`;
        contenido += `<th class="text-right">${formatNumber(result.totalpresentes)}</th>`;
        contenido += `<th class="text-right">${formatNumber(result.totaldescansos)}</th>`;
        contenido += `<th class="text-right">${formatNumber(result.totalfallas)}</th>`;
        contenido += `<th class="text-right">${formatNumber(result.totallicencias)}</th>`;
        contenido += `<th class="text-right">${formatNumber(result.totalpermisossg)}</th>`;
        contenido += `<th class="text-right">${formatNumber(result.totalpermisos)}</th>`;
        contenido += `<th class="text-right">${formatNumber(result.totalvacaciones)}</th>`;
        contenido += `<th class="text-right">${formatNumber(result.totalotros)}</th>`;
        contenido += `<th class="text-right">${formatNumber(result.totaltotal)}</th>`;
        contenido += `</tr>`;
        contenido += `</tfoot>`;
        contenido += `</table>`;
        i++;
    });

    $("#list_asistencias").html(contenido);
}

const cargarTablaImpresion = (cabeceras, data, mes, anio) => {
    const totalesLibros = new Array();
    const meses = ['ENERO', 'FEBRERO', 'MARZO', 'ABRIL', 'MAYO', 'JUNIO', 'JULIO', 'AGOSTO', 'SEPTIEMBRE', 'OCTUBRE', 'NOVIEMBRE', 'DICIEMBRE'];

    $("#title-impresion").html(`LIBRO DE REMUNERACIONES DE ${meses[mes]} DEL A&Ntilde;O ${anio}`);

    data.forEach(libro => {
        const result = calcularTotalesLibos(libro);
        totalesLibros.push(result);
    });

    let contenido;

    contenido = ``;

    contenido += `<table id="table-libroZ" border="1" style="width: 100%; border: 1px #c3c3c3 solid; margin-bottom: 10px;">`;
    contenido += `<thead class="bg-dark text-white">`;
    contenido += `<tr>`;
    contenido += `<th width="300" class="text-right">${cabeceras[0]}</th>`;
    contenido += `<th width="300" class="text-right">${cabeceras[1]}</th>`;
    contenido += `<th width="100" class="text-right">${cabeceras[2]}</th>`;
    contenido += `<th width="100" class="text-right">${cabeceras[3]}</th>`;
    contenido += `<th width="100" class="text-right">${cabeceras[4]}</th>`;
    contenido += `<th width="100" class="text-right">${cabeceras[5]}</th>`;
    contenido += `<th width="100" class="text-right">${cabeceras[6]}</th>`;
    contenido += `<th width="100" class="text-right">${cabeceras[7]}</th>`;
    contenido += `<th width="100" class="text-right">${cabeceras[8]}</th>`;
    contenido += `<th width="100" class="text-right">${cabeceras[9]}</th>`;
    contenido += `<th width="100" class="text-right">${cabeceras[10]}</th>`;
    contenido += `</tr>`;
    contenido += `</thead>`;

    data.forEach(lib => {

        contenido += `<tbody>`;

        lib.forEach(itLib => {
            contenido += `<tr>`;
            contenido += `<td>${formatoRutMostrar(itLib.rut)}</td>`;
            contenido += `<td class="text-right">${itLib.nombre}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.presente)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.descanso)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.falla)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.licencia)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.permisosg)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.permiso)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.vacacion)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.otro)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.total)}</td>`;
            contenido += `</tr>`;
        });

        contenido += `</tbody>`;

    });

    contenido += `</table>`;

    $("#content-impresion").html(contenido);
    //$("#zona_imprimir").fadeIn();
}

const formatoFecha = (fecha) => {
    let subFecha = fecha.substring(0, 10);
    let arrF = subFecha.split('-');
    let fd = arrF[2] + "/" + arrF[1] + "/" + arrF[0];
    return fd;
}

const formatNumber = (numero) => {
    return new Intl.NumberFormat("es-CL").format(Math.round(numero));
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

    if (rut != undefined) {
        nuevoRut = nuevoRut.replaceAll('.', '');
        nuevoRut = nuevoRut.replaceAll('-', '');
        nuevoRut = nuevoRut.replaceAll(' ', '');
        return nuevoRut;
    }

}