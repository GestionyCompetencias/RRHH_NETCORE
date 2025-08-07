
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
            const dest = `ProcesarLiquidacion?mes=${month}&anio=${year}`;

            $.get(dest, (resp) => {
                alert(resp.info.mensaje);
                document.getElementById("esperar").style.display = 'none';
           });
        }
    });
}
)
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
