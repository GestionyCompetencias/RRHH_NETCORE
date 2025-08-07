const desde = new Date();
const hasta = new Date();
const formatFechaGuion1 = (fecha) => {
    // Formato 2001-04-21
    let f = fecha.substring(0, 10);
    let arrF = f.split('-');

    if (arrF.length != 3) {
        arrF = f.split('/')
    }
    if (arrF[1].length == 1) {
        arrF[1] = "0" + arrF[1]
    }

    if (arrF[0].length == 1) {
        arrF[0] = "0" + arrF[0]
    }

    return `${arrF[2]}-${arrF[1]}-${arrF[0]}`;
}
var idEmpresa = 0;
$(document).ready(() => {
    desde.setDate(hasta.getDate() - 30);
    let fechades = formatFechaGuion1(desde.toLocaleDateString());
    let fechahas = formatFechaGuion1(hasta.toLocaleDateString());
    document.getElementById("txt_desde").value = fechades;
    document.getElementById("txt_hasta").value = fechahas;
    ComponentBuilder.configurarSelect("anio");
    ComponentBuilder.configurarSelect("mes");

});
 
$(function () {

    loadMonths();
    loadYears();

    $(document).on("click", "#btn-filter", function (e) {
        e.preventDefault();
        document.getElementById("esperar").style.display = 'block';
        var des = document.getElementById("txt_desde").value;
        var has = document.getElementById("txt_hasta").value;
        const dest = `ListarLicenciasConsulta?desde=${des}&hasta=${has}`;

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

                    const cabeceras = ['Rut', 'Nombre', 'Codigo', 'Desde', 'hasta', 'dias','Descripción','Comentario'];

                    cargarTabla(cabeceras, libros);
                    cargarTablaImpresion(cabeceras, libros);
                    //document.getElementById("print-export").style.display = 'block';
                    document.getElementById("esperar").style.display = 'none';
                } else {
                    document.getElementById("esperar").style.display = 'none';
                    //document.getElementById("print-export").style.display = 'none';
                    document.getElementById("list_consultalicencias").innerHTML = '';
                    let msg = `<div class="alert alert-info alert-styled-left alert-dismissible">`;
                    msg += `<span class="font-weight-semibold">Aviso!</span> No se han encontrado datos relacionados con los parametros de busqueda ingresados.`;
                    msg += `</div>`;

                    $("#list_consultalicencias").html(msg);
                }

            });
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
            filename: "LicenciasMedicas.xls",
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

    libro.forEach(it => {
        let montoDias = parseFloat(it.dias);
        totalDias = totalDias + montoDias;
    });

    const newTotal = {
        totalDias
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
            contenido += `<td>${formatoRutMostrar(itLib.ruttrabajador)}</td>`;
            contenido += `<td class="text-right">${itLib.nombre}</td>`;
            contenido += `<td class="text-right" align="right">${itLib.codigolicencia}</td>`;
            contenido += `<td class="text-right" align="right">${itLib.fechainicio}</td>`;
            contenido += `<td class="text-right" align="right">${itLib.fechatermino}</td>`;
            contenido += `<td class="text-right" align="right">${formatNumber(itLib.dias)}</td>`;
            contenido += `<td class="text-right" align="right">${itLib.desclicencia}</td>`;
            contenido += `<td class="text-right" align="right">${itLib.comentario}</td>`;
            contenido += `</tr>`;
        });
        contenido += `</tbody>`;
        contenido += `<tfoot>`;
        contenido += `<tr>`;
        const result = calcularTotalesLibos(lib);
        contenido += `<td class="bg-light" colspan="5">Totales... </td>`;
        contenido += `<td class="text-right" align="right">${formatNumber(result.totalDias)}</td>`;
        contenido += `</tr>`;
        contenido += `</tfoot>`;
        contenido += `</table>`;
        i++;
    });

    $("#list_consultalicencias").html(contenido);
}

const cargarTablaImpresion = (cabeceras, data) => {
    const totalesLibros = new Array();
    const meses = ['ENERO', 'FEBRERO', 'MARZO', 'ABRIL', 'MAYO', 'JUNIO', 'JULIO', 'AGOSTO', 'SEPTIEMBRE', 'OCTUBRE', 'NOVIEMBRE', 'DICIEMBRE'];

    $("#title-impresion").html(`Consulta de licencias ${desde} hasta ${hasta}`);

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
            contenido += `<td>${formatoRutMostrar(itLib.ruttrabajador)}</td>`;
            contenido += `<td class="text-right">${itLib.nombre}</td>`;
            contenido += `<td class="text-right" align="right">${formatNumber(itLib.codigolicencia)}</td>`;
            contenido += `<td class="text-right" align="right">${formatNumber(itLib.fechainicio)}</td>`;
            contenido += `<td class="text-right" align="right">${formatNumber(itLib.fechatermino)}</td>`;
            contenido += `<td class="text-right" align="right">${formatNumber(itLib.dias)}</td>`;
            contenido += `<td class="text-right" align="right">${formatNumber(itLib.desclicencia)}</td>`;
            contenido += `<td class="text-right" align="right">${formatNumber(itLib.comentario)}</td>`;
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
    console.log("rut=" + rut);
    var nuevoRut = rut;
    if (rut != '') {
        var longitudRut = nuevoRut.length;
        nuevoRut = nuevoRut.replaceAll(' ', '');
        var nuevoParte1 = nuevoRut.substring(0, longitudRut - 1);
        var nuevoParte2 = nuevoRut.substring(longitudRut - 1, longitudRut);
        nuevoRut = nuevoParte1 + "-" + nuevoParte2;
    }
    var longitudRut = nuevoRut.length;
    nuevoRut = nuevoRut.replaceAll(' ', '');
    var nuevoParte1 = nuevoRut.substring(0, longitudRut - 1);
    var nuevoParte2 = nuevoRut.substring(longitudRut - 1, longitudRut);
    nuevoRut = nuevoParte1 + "-" + nuevoParte2;
    return nuevoRut;
}


