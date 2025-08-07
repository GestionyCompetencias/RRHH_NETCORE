var mes = 0;
var anio = 0;
var pago = "L";
var idEmpre = "";

inicio();
ComboMes();
ComboAnio();
ComboPago();

function inicio() {
    document.getElementById("esperar").style.display = 'none';
    procesacierre();
}

function procesacierre() {
    mes  = document.getElementById("cbo_mes").value;
    anio = document.getElementById("cbo_anio").value;
    pago = document.getElementById("cbo_pago").value;
   if (mes > 0) {
        $.get("ProcesaCierraProceso?mes=" + mes + '&anio=' + anio + '&pago=' + pago, function (data) {
            if (data.info.result == 0) {
                alert(data.info.mensaje);
            }
        })
    }
}
function ComboMes() {
    $.get("CargaMes", (data) => {
        var info = data.info.data;
        var contenido = "";
        contenido += "<option value=''>--Seleccione--</option>"
        //Generando las opciones
        for (var i = 0; i < info.length; i++) {
            contenido += "<option value='" + info[i].codigo + "'>" + info[i].descripcion + "</option>"

        }
        document.getElementById("cbo_mes").innerHTML = contenido;
    })
}

function ComboAnio() {
    $.get("CargaAnio", (data) => {
        var info = data.info.data;
        var contenido = "";
        contenido += "<option value=''>--Seleccione--</option>"
        //Generando las opciones
        for (var i = 0; i < info.length; i++) {
            contenido += "<option value='" + info[i].codigo + "'>" + info[i].descripcion + "</option>"

        }
        document.getElementById("cbo_anio").innerHTML = contenido;
    })
}
function ComboPago() {
    $.get("CargaPago", (data) => {
        var info = data.info.data;
        console.log(data);
        var contenido = "";
        contenido += "<option value=''>--Seleccione--</option>"
        //Generando las opciones
        for (var i = 0; i < info.length; i++) {
            contenido += "<option value='" + info[i].codigo + "'>" + info[i].descripcion + "</option>"

        }
        document.getElementById("cbo_pago").innerHTML = contenido;
    })
}

