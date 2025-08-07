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
        console.log(month);
        if (month < 1 || month > 12) {
            alert("Debe ingresar mes");
            return;
        }
        if (month > 0) {
            const dest = `ListarCuadraturaAsistencias?mes=${month}&anio=${year}`;

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

                    const cabeceras = ['Rut', 'Nombre', 'Ausencias', 'Sob. 50%', 'Sob. 100%', 'Dias col.', 'Horas col.', 'Dias mov.'];

                    cargarTabla(cabeceras, libros);
                    cargarTablaImpresion(cabeceras, libros, month, year);
                    document.getElementById("print-export").style.display = 'block';
                    document.getElementById("esperar").style.display = 'none';
                } else {
                    document.getElementById("esperar").style.display = 'none';
                    document.getElementById("print-export").style.display = 'none';
                    document.getElementById("list_cuadraturaasistencias").innerHTML = '';
                    let msg = `<div class="alert alert-info alert-styled-left alert-dismissible">`;
                    msg += `<span class="font-weight-semibold">Aviso!</span> No se han encontrado datos relacionados con los parametros de busqueda ingresados.`;
                    msg += `</div>`;

                    $("#list_cuadraturaasistencias").html(msg);
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
            filename: "CuadraturaAsistencia.xls",
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
    let totalDias = 0;
    let totalExtra1 = 0;
    let totalExtra2 = 0;
    let totalHrscol = 0;
    let totalDiacol = 0;
    let totalDiamov = 0;

    libro.forEach(it => {
        let montoDias = parseFloat(it.dias);
        totalDias = totalDias + montoDias;
        let montoExtra1 = parseFloat(it.horasExtras1);
        totalExtra1 = totalExtra1 + montoExtra1;
        let montoExtra2 = parseFloat(it.horasExtras2);
        totalExtra2 = totalExtra2 + montoExtra2;
        let montoHrscol = parseFloat(it.horasColacion);
        totalHrscol = totalHrscol + montoHrscol;
        let montoDiaCol = parseFloat(it.diasColacion);
        totalDiaol = totalDiacol + montoDiaCol;
        let montoDiamov = parseFloat(it.diasMovilizacion);
        totalDiamovs = totalDiamov + montoDiamov;
    });

    const newTotal = {
        totalDias,
        totalExtra1,
        totalExtra2,
        totalHrscol,
        totalDiacol,
        totalDiamov
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
    //comprobantes/Editar/eComprobante?iddet=${numComp}&empresa=${idEmpresa}
    contenido = ``;
    let i = 0;

    data.forEach(lib => {
        contenido += `<table id="table-libro-${i}" class="table table-bordered mb-3">`;
        contenido += `<thead class="bg-dark text-white"><tr>`;
        contenido += `<td width="100">${cabeceras[0]}</td>`;
        contenido += `<td width="300" class="text-right">${cabeceras[1]}</td>`;
        contenido += `<td width="100" class="text-right" align="right">${cabeceras[2]}</td>`;
        contenido += `<td width="100" class="text-right" align="right">${cabeceras[3]}</td>`;
        contenido += `<td width="100" class="text-right" align="right">${cabeceras[4]}</td>`;
        contenido += `<td width="100" class="text-right" align="right">${cabeceras[5]}</td>`;
        contenido += `<td width="100" class="text-right" align="right">${cabeceras[6]}</td>`;
        contenido += `<td width="100" class="text-right" align="right">${cabeceras[7]}</td>`;
       contenido += `</tr>`;
        contenido += `</thead>`;
        contenido += `<tbody>`;
        lib.forEach(itLib => {
            contenido += `<tr>`;
            contenido += `<td>${itLib.rutTrabajador}</td>`;
            contenido += `<td class="text-right">${itLib.nombre}</td>`;
            contenido += `<td class="text-right" align="right">${formatNumber(itLib.dias)}</td>`;
            contenido += `<td class="text-right" align="right">${formatNumber(itLib.horasExtras1)}</td>`;
            contenido += `<td class="text-right" align="right">${formatNumber(itLib.horasExtras2)}</td>`;
            contenido += `<td class="text-right" align="right">${formatNumber(itLib.diasColacion)}</td>`;
            contenido += `<td class="text-right" align="right">${formatNumber(itLib.horasColacion)}</td>`;
            contenido += `<td class="text-right" align="right">${formatNumber(itLib.diasMovilizacion)}</td>`;
            contenido += `</tr>`;
        });
        contenido += `</tbody>`;
        contenido += `<tfoot>`;
        contenido += `<tr>`;
        const result = calcularTotalesLibos(lib);
        contenido += `<td class="bg-light" colspan="2">Totales... </td>`;
        contenido += `<td class="text-right" align="right">${formatNumber(result.totalDias)}</td>`;
        contenido += `<td class="text-right" align="right">${formatNumber(result.totalExtra1)}</td>`;
        contenido += `<td class="text-right" align="right">${formatNumber(result.totalExtra2)}</td>`;
        contenido += `<td class="text-right" align="right">${formatNumber(result.totalDiacol)}</td>`;
        contenido += `<td class="text-right" align="right">${formatNumber(result.totalHrscol)}</td>`;
        contenido += `<td class="text-right" align="right">${formatNumber(result.totalDiamov)}</td>`;
        contenido += `</tr>`;
        contenido += `</tfoot>`;
        contenido += `</table>`;
        i++;
    });

    $("#list_cuadraturaasistencias").html(contenido);
}

const cargarTablaImpresion = (cabeceras, data, mes, anio) => {
    const totalesLibros = new Array();
    const meses = ['ENERO', 'FEBRERO', 'MARZO', 'ABRIL', 'MAYO', 'JUNIO', 'JULIO', 'AGOSTO', 'SEPTIEMBRE', 'OCTUBRE', 'NOVIEMBRE', 'DICIEMBRE'];

    $("#title-impresion").html(`CUADRATURA DE ASISTENCIAS DE ${meses[mes]} DEL A&Ntilde;O ${anio}`);

    data.forEach(libro => {
        const result = calcularTotalesLibos(libro);
        totalesLibros.push(result);
    });

    let contenido;

    contenido = ``;

    contenido += `<table id="table-libroZ" border="1" style="width: 100%; border: 1px #c3c3c3 solid; margin-bottom: 10px;">`;
    contenido += `<thead class="bg-dark text-white">`;
    contenido += `<tr>`;
    contenido += `<th width="120" class="text-right">${cabeceras[0]}</th>`;
    contenido += `<th width="300" class="text-right">${cabeceras[1]}</th>`;
    contenido += `<th width="100" class="text-right" align="right">${cabeceras[2]}</th>`;
    contenido += `<th width="100" class="text-right" align="right">${cabeceras[3]}</th>`;
    contenido += `<th width="100" class="text-right" align="right">${cabeceras[4]}</th>`;
    contenido += `<th width="100" class="text-right" align="right">${cabeceras[5]}</th>`;
    contenido += `<th width="100" class="text-right" align="right">${cabeceras[6]}</th>`;
    contenido += `<th width="100" class="text-right" align="right">${cabeceras[7]}</th>`;
    contenido += `</tr>`;
    contenido += `</thead>`;

    data.forEach(lib => {

        contenido += `<tbody>`;

        lib.forEach(itLib => {
            contenido += `<tr>`;
            contenido += `<td class="text-right">${itLib.haber}</td>`;
            contenido += `<td class="text-right">${itLib.descripcion}</td>`;
            contenido += `<td class="text-right" align="right">${formatNumber(itLib.dias)}</td>`;
            contenido += `<td class="text-right" align="right">${formatNumber(itLib.horasExtra1)}</td>`;
            contenido += `<td class="text-right" align="right">${formatNumber(itLib.horasExtra2)}</td>`;
            contenido += `<td class="text-right" align="right">${formatNumber(itLib.diasColacion)}</td>`;
            contenido += `<td class="text-right" align="right">${formatNumber(itLib.horasColacion)}</td>`;
            contenido += `<td class="text-right" align="right">${formatNumber(itLib.diasMovilizacion)}</td>`;
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


