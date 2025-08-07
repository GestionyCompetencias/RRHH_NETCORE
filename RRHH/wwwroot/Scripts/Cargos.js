inicio();
var idEmpre = "";

var cargoModal = ComponentBuilder.setModal("modal_Cargos");

function inicio() {
    document.getElementById("esperar").style.display = 'none';
    mostrarDatos();
}


function mostrarDatos() {

    document.getElementById("esperar").style.display = 'block';

    $.get("EmpresaLog", function (data) {
        idEmpre = data;
        obtenerDatosValidos(idEmpre);
    })
}


function obtenerDatosValidos(idEmpre) {

    $.get("ListarCargos?empresa=" + idEmpre, function (data) {

        if (data.info.result == 0) {
            alert(data.info.mensaje);
            document.getElementById("esperar").style.display = 'none';
        } else {
            crearListado(["Codígo", "Descripción", "Inicio", "Termino",  "Mínimo", "Máximo"], data, "list_cargos");

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

                if (nombrePropiedad == 'codigo') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'descripcion') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'inicio') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'termino') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'sueldoMinimo') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'sueldoMaximo') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
            };

            contenido += "<td style='text-align:right'>";

            contenido += "<button onclick='abrirModal(" + fila.id + ")' class='btn btn-primary rounded-round btn-sm' ";
            contenido += "data-bs-toggle='modal' data-bs-target='#modal_Cargos'><i class='fa fa-pen'></i></button> ";

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

        document.getElementById("txt_Cabecera").innerHTML = "Editar cargo";

        $.get("ConsultaCargoId?id="+ id +"&empresa="+idEmpre, function (data) {
            data = data.info.data;
            console.log(data);
            document.getElementById("txt_id").innerHTML = data[0].id;
            document.getElementById("txt_codigo").value = data[0].codigo;
            document.getElementById("txt_descripcion").value = data[0].descripcion.toString().toUpperCase();
            document.getElementById("txt_fecini").value = data[0].inicio.toString().toUpperCase();
            document.getElementById("txt_fecter").value = data[0].termino;
            document.getElementById("txt_requisitos").value = data[0].requisitos;
            document.getElementById("txt_funciones").value = data[0].funciones;
            document.getElementById("txt_sueldominimo").value = data[0].sueldoMinimo;
            document.getElementById("txt_sueldomaximo").value = data[0].sueldoMaximo;

            var elemento = document.getElementsByClassName("form-group-float-label");
            for (var i = 0; i < elemento.length; i++) {
                elemento[i].className += " is-visible";
            }
        })
        limpiar();
    }
    else {
        document.getElementById("txt_id").innerHTML = "";
        document.getElementById("txt_Cabecera").innerHTML = "Nueva cargo";

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



function Eliminar(id) {

    if (confirm("Confirma que desea eliminar la información de esta cargo?") == 1) {

        $.get("InhabilitaCargo?Id=" + id + "&empresa=" + idEmpre, function (data) {
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


    var objeto = validarDatos();
    var esCorrecto = objeto.exito;
    if (esCorrecto == false) {
        document.getElementById("divErrores").innerHTML = "<ol>" + objeto.mensaje + "</ol>";
        return;
    }
        
    var id = document.getElementById("txt_id").innerHTML;
    var codigo = formatoRutGuardar(document.getElementById("txt_codigo").value);
    var descripcion = document.getElementById("txt_descripcion").value.toString().toUpperCase();
    var fecini = document.getElementById("txt_fecini").value.toString().toUpperCase();
    var fecter = document.getElementById("txt_fecter").value;
    var requisitos = document.getElementById("txt_requisitos").value;
    var funciones = document.getElementById("txt_funciones").value;
    var sueldominimo = document.getElementById("txt_sueldominimo").value;
    var sueldomaximo = document.getElementById("txt_sueldomaximo").value;
    var empresa = idEmpre;



    var frm = new FormData();
    frm.append("id", id);
    frm.append("codigo", codigo);
    frm.append("descripcion", descripcion);
    frm.append("inicio", fecini);
    frm.append("termino", fecter);
    frm.append("requisitos", requisitos);
    frm.append("funciones", funciones);
    frm.append("sueldominimo", sueldominimo);
    frm.append("sueldomaximo", sueldomaximo);
    frm.append("empresa", empresa);


    if (id == '') {
        $.ajax({
            type: "POST",
            url: "CrearCargo",
            data: frm,
            contentType: false,
            processData: false,

            success: function (data) {
                    alert(data.info.mensaje);
                    limpiar();
                    document.getElementById("btnCerrar").click();
                    inicio();
            }
        });
    } else {
        $.ajax({
            type: "POST",
            url: "EditaCargo",
            data: frm,
            contentType: false,
            processData: false,

            success: function (data) {
                    alert(data.info.mensaje);
                limpiar();
                document.getElementById("btnCerrar").click();
                inicio();
            }
        });
    }
        
}
