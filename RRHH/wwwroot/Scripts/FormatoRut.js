
function ValidarRut(rut) {

	var logitudRut = rut.toString().length;
	rutParte1 = rut.toString().substring(0, logitudRut - 1);
	rutVerificador = rut.toString().substring(logitudRut - 1, logitudRut);

	rutCompleto = rutParte1 + "-" + rutVerificador;

	rutCompleto = rutCompleto.replace("‐", "-");
	if (!/^[0-9]+[-|‐]{1}[0-9kK]{1}$/.test(rutCompleto))
		return false;
	var tmp = rutCompleto.split('-');
	var digv = tmp[1];
	var rut = tmp[0];
	if (digv == 'K') digv = 'k';

	return (DigVerifi(rut) == digv);
}

function DigVerifi(T) {
	var M = 0, S = 1;
	for (; T; T = Math.floor(T / 10))
		S = (S + T % 10 * (9 - M++ % 6)) % 11;
	return S ? S - 1 : 'k';
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