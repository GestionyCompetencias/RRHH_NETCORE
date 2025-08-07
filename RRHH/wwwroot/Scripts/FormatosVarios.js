

const formatNumber = (numero) => {
    return new Intl.NumberFormat("es-CL").format(Math.round(numero));
}

const formatFechaGuion1 = (fecha) => {
    // Formato 2001-04-21
    let f = fecha.substring(0, 10);
    let arrF = f.split('-');
    return `${arrF[2]}-${arrF[1]}-${arrF[0]}`;
}

const formatFechaGuion2 = (fecha) => {
    // Formato 21-04-2001
    let f = fecha.substring(0, 10);
    let arrF = f.split('-');
    return `${arrF[0]}-${arrF[1]}-${arrF[2]}`;
}

const formatFechaBarra1 = (fecha) => {
    // Formato 2001/04/21
    let f = fecha.substring(0, 10);
    let arrF = f.split('-');
    return `${arrF[2]}/${arrF[1]}/${arrF[0]}`;
}

const formatFechaBarra2 = (fecha) => {
    // Formato 21/04/2001
    let f = fecha.substring(0, 10);
    let arrF = f.split('-');
    return `${arrF[0]}/${arrF[1]}/${arrF[2]}`;
}