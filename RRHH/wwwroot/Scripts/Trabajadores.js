inicio();
mostrarPaises();
mostrarRegiones();

var trabajadorModal = ComponentBuilder.setModal("modal_Trabajadores");

function inicio() {
    document.getElementById("esperar").style.display = 'none';
    mostrarDatos();
}


function mostrarDatos() {

    document.getElementById("esperar").style.display = 'block';

    var idEmpre = "";
    $.get("EmpresaLog", function (data) {
        idEmpre = data;
        obtenerDatosValidos(idEmpre);
    })

    $("#cbo_region").hide();
    $("#cbo_comuna").hide();

}


function obtenerDatosValidos(idEmpre) {

    $.get("ConsultarTrabajadores?empresa=" + idEmpre, function (data) {

        if (data.info.result == 0) {
            alert(data.info.mensaje);
            document.getElementById("esperar").style.display = 'none';
        } else {
            crearListado(["Rut", "Nombres", "Apellidos","Pais", "Región", "Comuna", "Teléfono"], data, "list_trabajadores");

            document.getElementById("esperar").style.display = 'none';
        }


    })

}


function crearListado(cabeceras, data, divId) {

    var z = data.info;

    var contenido = "";
    contenido += "<table id='tabla' class='table nowrap'>";

    //Las cabeceras
    contenido += "<thead>";
    contenido += "<tr>";

    for (var i = 0; i < cabeceras.length; i++) {
        contenido += "<td>" + cabeceras[i] + "</td>"
    }
    contenido += "<td>Operaciones</td>";
    contenido += "</tr>";

    contenido += "</thead>";
    if (z.data.length > 0) {
        var propiedadesObjeto = Object.keys(z.data[0]);

        contenido += "<tbody>";

        var fila;
        for (var i = 0; i < z.data.length; i++) {
            fila = z.data[i];
            contenido += "<tr>";

            for (var j = 0; j < propiedadesObjeto.length; j++) {

                var nombrePropiedad = propiedadesObjeto[j];

                if (nombrePropiedad == 'rut') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'nombres') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'apellidos') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'pais') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'region') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'comuna') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'tlf') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }

            };

            contenido += "<td style='text-align:right'>";

            contenido += "<button onclick='abrirModal(" + fila.id + ")' class='btn btn-primary rounded-round btn-sm' ";
            contenido += "data-bs-toggle='modal' data-bs-target='#modal_Trabajadores'><i class='fa fa-pen'></i></button> ";

            contenido += "<button onclick='Eliminar(" + fila.id + ")' class='btn btn-danger rounded-round btn-sm btn'><i class='fa fa-trash-alt'></i></button> ";

            contenido += "</td>";

            contenido += "</tr>";

        }

        contenido += "</tbody>";

    }

    contenido += "</table>";

    document.getElementById(divId).innerHTML = contenido;
    ComponentBuilder.configurarDataTable("tabla");

}


function abrirModal(id) {
    limpiar();
    document.getElementById("divErrores").innerHTML = "";

    if (id != undefined) {

        document.getElementById("txt_Cabecera").innerHTML = "Editar trabajador";
        $.get("ConsultaTrabajadorId?id="+ id +"&empresa=0", function (data) {
            data = data.info.data;
            console.log(data);
            document.getElementById("txt_rut").value = formatoRutMostrar(data[0].rut);
            document.getElementById("txt_nombres").value = data[0].nombres.toString().toUpperCase();
            document.getElementById("txt_apellidos").value = data[0].apellidos.toString().toUpperCase();
            document.getElementById("txt_email").value = data[0].email;
            document.getElementById("txt_tlf").value = data[0].tlf;

            $("#cbo_pais").val(data[0].idPais).trigger('change');
            setTimeout(() => { $("#cbo_region").val(data[0].idRegion).trigger('change') }, 700);
            mostrarComunas();
            setTimeout(() => { $("#cbo_comuna").val(data[0].idComuna).trigger('change') }, 1000);

            document.getElementById("txt_direccion").value = data[0].direccion;
            document.getElementById("txt_fecnac").value = data[0].nacimiento;
            document.getElementById("txt_sexo").value = data[0].sexo;
            document.getElementById("txt_hijos").value = data[0].nrohijos;

            var elemento = document.getElementsByClassName("form-group-float-label");
            for (var i = 0; i < elemento.length; i++) {
                elemento[i].className += " is-visible";
            }

        })
    }
    else {

        document.getElementById("txt_id").innerHTML = "";
        document.getElementById("txt_Cabecera").innerHTML = "Nuevo trabajador";

        var elemento = document.getElementsByClassName("form-group-float-label");
        for (var i = 0; i < elemento.length; i++) {
            elemento[i].classList.replace('is-visible', 'no-visible');
        }
    }

}

function limpiar() {

    var elementosConClaseLimpiar = document.getElementsByClassName("cls_t");
    var nelementos = elementosConClaseLimpiar.length;
    for (var i = 0; i < nelementos; i++) {
        elementosConClaseLimpiar[i].value = "";
    }

}


function mostrarPaises() {
    $.get("ConsultarPaises/?empresa=28", function (data) {

        var contenido = "";

        contenido += "<option value=''>--Seleccione--</option>"
        //Generando las opciones
        for (var i = 0; i < data.info.data.length; i++) {
            contenido += "<option value='" + data.info.data[i].idPais + "'>" + data.info.data[i].nombre + "</option>"

        }

        document.getElementById("cbo_pais").innerHTML = contenido;
        ComponentBuilder.configurarSelectEnModal("cbo_pais", "modal_Trabajadores");
    })
}

function mostrarRegiones() {

    var idPais = document.getElementById("cbo_pais").value
    console.log("Pais  :"+idPais);
    if (idPais == 1) {
        $("#cbo_region").show();
        $("#cbo_comuna").show();
    } else {
        $("#cbo_region").val('')
        $("#cbo_comuna").val('')
        $("#cbo_region").hide();
        $("#cbo_comuna").hide();
    }

    $.get("ConsultarRegiones?idpais=" + idPais + "&empresa=28", function (data) {
        var contenido = "";

        contenido += "<option value=''>--Seleccione--</option>"
        //Generando las opciones
        if (data.info.result == 1) {
            for (var i = 0; i < data.info.data.length; i++) {
                contenido += "<option value='" + data.info.data[i].idRegion + "'>" + data.info.data[i].nombre + "</option>"

            }
        }

        document.getElementById("cbo_region").innerHTML = contenido;
        ComponentBuilder.configurarSelectEnModal("cbo_region", "modal_Trabajadores");

    })
}

function mostrarComunas() {

    var idRegion = document.getElementById("cbo_region").value
    console.log("Region :"+idRegion);
    $.get("ConsultarComunas?idregion=" + idRegion + "&empresa=28", function (data) {

        var contenido = "";

        contenido += "<option value=''>--Seleccione--</option>"
        //Generando las opciones
        if (data.info.result == 1) {
            for (var i = 0; i < data.info.data.length; i++) {
                contenido += "<option value='" + data.info.data[i].idComuna + "'>" + data.info.data[i].nombre + "</option>"

            }
        }

        document.getElementById("cbo_comuna").innerHTML = contenido;
        ComponentBuilder.configurarSelectEnModal("cbo_comuna", "modal_Trabajadores");

    })
}


function Eliminar(id) {

    if (confirm("confirma que desea eliminar la información de este trabajador?") == 1) {

        $.get("dTrabajador?Id=" + id, function (data) {
            if (data.info.result == 0) {
                alert("Ocurrio un error");
            }
            else if (data.info.result == -1) {
                alert("No se pudo completar el proceso");
            }
            else {
                alert("Se elimino correctamente");
                inicio();
            }
        })

    }

}

function validarDatos() {
    //Todos los campos estan bien validados
    var mensaje = "";
    var exito = true;
    var obligatorios = document.getElementsByClassName("Obl_t");
    var nobligatorios = obligatorios.length;

    for (var i = 0; i < nobligatorios; i++) {
        if (obligatorios[i].value == "") {
            exito = false;
            mensaje += "<li>Debe ingresar " + obligatorios[i].name + "</li>";
            return { exito, mensaje };
        }
    }

    return { exito, mensaje };
}


function Guarda() {

    if (!ValidarRut(formatoRutGuardar(document.getElementById("txt_rut").value))) {
        alert("El rut ingresado no es válido");
        return 0;
    }

    var objeto = validarDatos();
    var esCorrecto = objeto.exito;
    if (esCorrecto == false) {
        document.getElementById("divErrores").innerHTML = "<ol>" + objeto.mensaje + "</ol>";
        return;
    }
        
    var id = document.getElementById("txt_id").innerHTML;
    var rut = formatoRutGuardar(document.getElementById("txt_rut").value);
    var nombres = document.getElementById("txt_nombres").value.toString().toUpperCase();
    var apellidos = document.getElementById("txt_apellidos").value.toString().toUpperCase();
    var email = document.getElementById("txt_email").value;
    var tlf = document.getElementById("txt_tlf").value;

    var idPais = document.getElementById("cbo_pais").value;
    var idregion = document.getElementById("cbo_region").value;
    var idcomuna = document.getElementById("cbo_comuna").value;

    var direccion = document.getElementById("txt_direccion").value;
    var nacimiento = document.getElementById("txt_fecnac").value;
    var sexo = document.getElementById("txt_sexo").value;
    var nrohijos = document.getElementById("txt_hijos").value;


    var frm = new FormData();
    frm.append("id", id);
    frm.append("rut", rut);
    frm.append("nombres", nombres);
    frm.append("apellidos", apellidos);
    frm.append("email", email);
    frm.append("tlf", tlf);

    frm.append("idPais", idPais);
    frm.append("idregion", idregion);
    frm.append("idcomuna", idcomuna);

    frm.append("direccion", direccion);
    frm.append("nacimiento", nacimiento);
    frm.append("sexo", sexo);
    frm.append("nrohijos", nrohijos);

    if (id == '') {
        $.ajax({
            type: "POST",
            url: "CrearTrabajador",
            data: frm,
            contentType: false,
            processData: false,

            success: function (data) {
                if (data.info.result == 0) {
                    alert("Ocurrio un error");
                } else if (data.info.result == -1) {
                    alert("No se pudo completar el proceso");
                }
                else {
                    alert("Proceso finalizado correctamente");
                    inicio();
                    limpiar();
                    document.getElementById("btnCerrar").click();
                }
            }
        });
    } else {
        $.ajax({
            type: "POST",
            url: "eTrabajador",
            data: frm,
            contentType: false,
            processData: false,

            success: function (data) {
                if (data.info.result == 0) {
                    alert("Ocurrio un error");
                } else if (data.info.result == -1) {
                    alert("No se pudo completar el proceso");
                }
                else {
                    alert("Proceso finalizado correctamente");
                    inicio();
                    limpiar();
                    document.getElementById("btnCerrar").click();
                }
            }
        });
    }
        
}
