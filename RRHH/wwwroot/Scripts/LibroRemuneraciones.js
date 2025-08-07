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
            const dest = `ListarLibroRemuneraciones?mes=${month}&anio=${year}`;

            $.get(dest, (resp) => {
                const libro = resp.info.data;
                const libros = new Array();
                console.log("libro=" + libro);

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

                    const cabeceras = ['RUT', 'Nombre', 'Dias', 'Sueldo', 'Hrs.extras', 'Gratificacion', 'Otros Imp.', 'Total Imp.', 'Familiar', 'Total NoI', 'Pensión', 'Salud',
                        'Cesantia', 'Impuesto', 'Otros Leg.', 'Dsctos Varios', 'Liquido'];

                    cargarTabla(cabeceras, libros);
                    cargarTablaImpresion(cabeceras, libros, month, year);
                    document.getElementById("print-export").style.display = 'block';
                    document.getElementById("esperar").style.display = 'none';
                } else {
                    document.getElementById("esperar").style.display = 'none';
                    document.getElementById("print-export").style.display = 'none';
                    document.getElementById("list_libro").innerHTML = '';
                    let msg = `<div class="alert alert-info alert-styled-left alert-dismissible">`;
                    msg += `<span class="font-weight-semibold">Aviso!</span> No se han encontrado datos relacionados con los parametros de busqueda ingresados.`;
                    msg += `</div>`;

                    $("#list_libro").html(msg);
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
    let totalDiast = 0;
    let totalSbase = 0;
    let totalHrsex = 0;
    let totalGrati = 0;
    let totalOtroi = 0;
    let totalTotim = 0;
    let totalFamil = 0;
    let totalOtron = 0;
    let totalPensi = 0;
    let totalSalud = 0;
    let totalSegce = 0;
    let totalImpto = 0;
    let totalOtrol = 0;
    let totalDctva = 0;
    let totalLiqui = 0;

    libro.forEach(it => {
        let montoDiast = parseFloat(it.diast);
        totalDiast = totalDiast + montoDiast;
        let montoSbase = parseFloat(it.sbase);
        totalSbase = totalSbase + montoSbase;
        let montoHrsex = parseFloat(it.hrsex);
        totalHrsex = totalHrsex + montoHrsex;
        let montoGrati = parseFloat(it.grati);
        totalGrati = totalGrati + montoGrati;
        let montoOtroi = parseFloat(it.otroi);
        totalOtroi = totalOtroi + montoOtroi;
        let montoTotim = parseFloat(it.totim);
        totalTotim = totalTotim + montoTotim;
        let montoFamil = parseFloat(it.famil);
        totalFamil = totalFamil + montoFamil;
        let montoOtron = parseFloat(it.otron);
        totalOtron = totalOtron + montoOtron;
        let montoPensi = parseFloat(it.pensi);
        totalPensi = totalPensi + montoPensi;
        let montoSalud = parseFloat(it.salud);
        totalSalud = totalSalud + montoSalud;
        let montoSegce = parseFloat(it.segce);
        totalSegce = totalSegce + montoSegce;
        let montoImpto = parseFloat(it.impto);
        totalImpto = totalImpto + montoImpto;
        let montoOtrol = parseFloat(it.otrol);
        totalOtrol = totalOtrol + montoOtrol;
        let montoDctva = parseFloat(it.dctva);
        totalDctva = totalDctva + montoDctva;
        let montoLiqui = parseFloat(it.liqui);
        totalLiqui = totalLiqui + montoLiqui;
    });

    const newTotal = {
        totalDiast,
        totalSbase,
        totalHrsex,
        totalGrati,
        totalOtroi,
        totalTotim,
        totalFamil,
        totalOtron,
        totalPensi,
        totalSalud,
        totalSegce,
        totalImpto,
        totalOtrol,
        totalDctva,
        totalLiqui
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
        contenido += `<th width="60" class="text-right">${cabeceras[2]}</th>`;
        contenido += `<th width="100" class="text-right">${cabeceras[3]}</th>`;
        contenido += `<th width="100" class="text-right">${cabeceras[4]}</th>`;
        contenido += `<th width="100" class="text-right">${cabeceras[5]}</th>`;
        contenido += `<th width="100" class="text-right">${cabeceras[6]}</th>`;
        contenido += `<th width="100" class="text-right">${cabeceras[7]}</th>`;
        contenido += `<th width="100" class="text-right">${cabeceras[8]}</th>`;
        contenido += `<th width="100" class="text-right">${cabeceras[9]}</th>`;
        contenido += `<th width="100" class="text-right">${cabeceras[10]}</th>`;
        contenido += `<th width="100" class="text-right">${cabeceras[11]}</th>`;
        contenido += `<th width="100" class="text-right">${cabeceras[12]}</th>`;
        contenido += `<th width="100" class="text-right">${cabeceras[13]}</th>`;
        contenido += `<th width="100" class="text-right">${cabeceras[14]}</th>`;
        contenido += `<th width="100" class="text-right">${cabeceras[15]}</th>`;
        contenido += `<th width="100" class="text-right">${cabeceras[16]}</th>`;
        contenido += `</tr>`;
        contenido += `</thead>`;
        contenido += `<tbody>`;
        lib.forEach(itLib => {
            contenido += `<tr>`;
            contenido += `<td class="text-left">${itLib.rut}</td>`;
            contenido += `<td class="text-right">${itLib.nombre}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.diast)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.sbase)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.hrsex)}</td>`;
            contenido += `<th class="text-right">${formatNumber(itLib.grati)}</th>`;
            contenido += `<td class="text-right">${formatNumber(itLib.otroi)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.totim)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.famil)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.otron)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.pensi)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.salud)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.segce)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.impto)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.otrol)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.dctva)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.liqui)}</td>`;
            contenido += `</tr>`;
        });
        contenido += `</tbody>`;
        contenido += `<tfoot>`;
        contenido += `<tr>`;
        const result = calcularTotalesLibos(lib);
        contenido += `<th class="bg-light" colspan="2">Totales... </th>`;
        contenido += `<th class="text-right">${formatNumber(result.totalDiast)}</th>`;
        contenido += `<th class="text-right">${formatNumber(result.totalSbase)}</th>`;
        contenido += `<th class="text-right">${formatNumber(result.totalHrsex)}</th>`;
        contenido += `<th class="text-right">${formatNumber(result.totalGrati)}</th>`;
        contenido += `<th class="text-right">${formatNumber(result.totalOtroi)}</th>`;
        contenido += `<th class="text-right">${formatNumber(result.totalTotim)}</th>`;
        contenido += `<th class="text-right">${formatNumber(result.totalFamil)}</th>`;
        contenido += `<th class="text-right">${formatNumber(result.totalOtron)}</th>`;
        contenido += `<th class="text-right">${formatNumber(result.totalPensi)}</th>`;
        contenido += `<th class="text-right">${formatNumber(result.totalSalud)}</th>`;
        contenido += `<th class="text-right">${formatNumber(result.totalSegce)}</th>`;
        contenido += `<th class="text-right">${formatNumber(result.totalImpto)}</th>`;
        contenido += `<th class="text-right">${formatNumber(result.totalOtrol)}</th>`;
        contenido += `<th class="text-right">${formatNumber(result.totalDctva)}</th>`;
        contenido += `<th class="text-right">${formatNumber(result.totalLiqui)}</th>`;
        contenido += `</tr>`;
        contenido += `</tfoot>`;
        contenido += `</table>`;
        i++;
    });

    $("#list_libro").html(contenido);
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
    contenido += `<th width="60" class="text-right">${cabeceras[2]}</th>`;
    contenido += `<th width="100" class="text-right">${cabeceras[3]}</th>`;
    contenido += `<th width="100" class="text-right">${cabeceras[4]}</th>`;
    contenido += `<th width="100" class="text-right">${cabeceras[5]}</th>`;
    contenido += `<th width="100" class="text-right">${cabeceras[6]}</th>`;
    contenido += `<th width="100" class="text-right">${cabeceras[7]}</th>`;
    contenido += `<th width="100" class="text-right">${cabeceras[8]}</th>`;
    contenido += `<th width="100" class="text-right">${cabeceras[9]}</th>`;
    contenido += `<th width="100" class="text-right">${cabeceras[10]}</th>`;
    contenido += `<th width="100" class="text-right">${cabeceras[11]}</th>`;
    contenido += `<th width="100" class="text-right">${cabeceras[12]}</th>`;
    contenido += `<th width="100" class="text-right">${cabeceras[13]}</th>`;
    contenido += `<th width="100" class="text-right">${cabeceras[14]}</th>`;
    contenido += `<th width="100" class="text-right">${cabeceras[15]}</th>`;
    contenido += `<th width="100" class="text-right">${cabeceras[15]}</th>`;
    contenido += `</tr>`;
    contenido += `</thead>`;

    data.forEach(lib => {

        contenido += `<tbody>`;

        lib.forEach(itLib => {
            contenido += `<tr>`;
            contenido += `<td>${formatoRutMostrar(itLib.rut)}</td>`;
            contenido += `<td class="text-right">${itLib.nombre}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.diast)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.sbase)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.hrsex)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.grati)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.otroi)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.totim)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.famil)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.otron)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.pensi)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.salud)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.segce)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.impto)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.otrol)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.dctva)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.liqui)}</td>`;
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