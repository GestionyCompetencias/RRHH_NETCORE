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
            const dest = `ListarArchivoDepositos?mes=${month}&anio=${year}`;

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

                    const cabeceras = ['RUT', 'Nombre', 'Monto','Banco', 'Tipo cuenta', 'Numero cuenta'];

                    cargarTabla(cabeceras, libros);
                    cargarTablaImpresion(cabeceras, libros, month, year);
                    document.getElementById("print-export").style.display = 'block';
                    document.getElementById("esperar").style.display = 'none';
                } else {
                    document.getElementById("esperar").style.display = 'none';
                    document.getElementById("print-export").style.display = 'none';
                    document.getElementById("list_depositos").innerHTML = '';
                    let msg = `<div class="alert alert-info alert-styled-left alert-dismissible">`;
                    msg += `<span class="font-weight-semibold">Aviso!</span> No se han encontrado datos relacionados con los parametros de busqueda ingresados.`;
                    msg += `</div>`;

                    $("#list_depositos").html(msg);
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
    let totalMonto = 0;

    libro.forEach(it => {
        let montomonto = parseFloat(it.monto);
        totalMonto = totalMonto + montomonto;
    });

    const newTotal = {
        totalMonto
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
        contenido += `<th width="100">${cabeceras[0]}</th>`;
        contenido += `<th width="300" class="text-right">${cabeceras[1]}</th>`;
        contenido += `<th width="80" class="text-right">${cabeceras[2]}</th>`;
        contenido += `<th width="100" class="text-right">${cabeceras[3]}</th>`;
        contenido += `<th width="100" class="text-right">${cabeceras[4]}</th>`;
        contenido += `<th width="100" class="text-right">${cabeceras[5]}</th>`;
        contenido += `</tr>`;
        contenido += `</thead>`;
        contenido += `<tbody>`;
        lib.forEach(itLib => {
            contenido += `<tr>`;
            contenido += `<td>${itLib.rut}</td>`;
            contenido += `<td class="text-right">${itLib.nombre}</td>`;
            contenido += `<td class="text-right" align="right">${formatNumber(itLib.monto)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.banco)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.tipocuenta)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.numerocuenta)}</td>`;
            contenido += `</tr>`;
        });
        contenido += `</tbody>`;
        contenido += `<tfoot>`;
        contenido += `<tr>`;
        const result = calcularTotalesLibos(lib);
        contenido += `<th class="bg-light" colspan="2">Totales... </th>`;
        contenido += `<td class="text-right" align="right">${formatNumber(result.totalMonto)}</td>`;
        contenido += `</tr>`;
        contenido += `</tfoot>`;
        contenido += `</table>`;
        i++;
    });

    $("#list_depositos").html(contenido);
}

const cargarTablaImpresion = (cabeceras, data, mes, anio) => {
    const totalesLibros = new Array();
    const meses = ['ENERO', 'FEBRERO', 'MARZO', 'ABRIL', 'MAYO', 'JUNIO', 'JULIO', 'AGOSTO', 'SEPTIEMBRE', 'OCTUBRE', 'NOVIEMBRE', 'DICIEMBRE'];

    $("#title-impresion").html(`ARCHIVO DE DEPOSITOS BANCARIOS DE ${meses[mes]} DEL A&Ntilde;O ${anio}`);

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
    contenido += `<th width="80" class="text-right">${cabeceras[2]}</th>`;
    contenido += `<th width="100" class="text-right">${cabeceras[3]}</th>`;
    contenido += `<th width="100" class="text-right">${cabeceras[4]}</th>`;
    contenido += `<th width="100" class="text-right">${cabeceras[5]}</th>`;
    contenido += `</tr>`;
    contenido += `</thead>`;

    data.forEach(lib => {

        contenido += `<tbody>`;

        lib.forEach(itLib => {
            contenido += `<tr>`;
            contenido += `<td>${formatoRutMostrar(itLib.rut)}</td>`;
            contenido += `<td class="text-right">${itLib.nombre}</td>`;
            contenido += `<td class="text-right" align="right">${formatNumber(itLib.monto)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.banco)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.tipocuenta)}</td>`;
            contenido += `<td class="text-right">${formatNumber(itLib.numerocuenta)}</td>`;
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