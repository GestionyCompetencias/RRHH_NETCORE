
inicio();
crearPaises();
mostrarRegiones();

function inicio() {
    document.getElementById("esperar").style.display = 'none';
    mostrarDatos();
}


function mostrarDatos() {

    document.getElementById("esperar").style.display = 'block';

    var idUser = "";
    $.get("SesionUsuario", function (data) {
        idUser = data;
        obtenerDatosValidos(idUser);
    })

}


function obtenerDatosValidos(idUser) {

    $.get("ConsultarMisEmpresas?idUser=" + idUser, function (data) {

        if (data.info.result == 0) {
            alert(data.info.mensaje);
            document.getElementById("esperar").style.display = 'none';
        } else {
            crearListado(["Id", "Rut", "Razon_Social", "Fantasia", "Email"], data, "list_empresas");

            document.getElementById("esperar").style.display = 'none';
        }


    })

}


function crearListado(cabeceras, data, divId) {

    var z = data.info;

    var contenido = "";
    contenido += "<table id='tabla' class='table '>";

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

                if (nombrePropiedad == 'id') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'rut') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'razonsocial') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'fantasia') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }
                else if (nombrePropiedad == 'email') {
                    contenido += "<td>" + fila[nombrePropiedad] + "</td>";
                }


            };


            contenido += "<td style='text-align:right'>";

            contenido += "<button onclick='abrirModal(" + fila.id + ")' class='btn btn-primary rounded-round btn-sm' ";
            contenido += "data-toggle='modal' data-target='#modal_Empresas'><i class='fa fa-pen'></i></button> ";

            contenido += "<button onclick='Eliminar(" + fila.id + ")' class='btn btn-danger rounded-round btn-sm btn'><i class='fa fa-trash-alt'></i></button> ";

            contenido += "</td>";

            contenido += "</tr>";

        }

        contenido += "</tbody>";
    }

    contenido += "</table>";

    document.getElementById(divId).innerHTML = contenido;
    $('#tabla').DataTable({
        stateSave: true,
        columnDefs: [
            {
                target: 0,
                visible: false,
                searchable: false
            },
            {
                target: 3,
                visible: false,
                searchable: false
            },
            {
                target: 4,
                visible: false,
                searchable: false
            }
        ]
    });

}


function abrirModal(id) {
    limpiar();
    if (id != undefined) {

        document.getElementById("txt_Cabecera").innerHTML = "Editar Empresa";

        $.get("ConsultaEmpresa/?id=" + id, function (data) {

            data = data.info.data;

            document.getElementById("txt_id").innerHTML = data[0].id;
            document.getElementById("txt_rut").value = data[0].rut;
            document.getElementById("txt_razon").value = data[0].razonsocial;
            document.getElementById("txt_fantasia").value = data[0].fantasia;
            document.getElementById("txt_giro").value = data[0].giro;
            document.getElementById("txt_direccion").value = data[0].direccion;

            document.getElementById("cbo_pais").value = data[0].idPais;
            setTimeout(() => { mostrarRegiones() }, 50);
            setTimeout(() => { document.getElementById("cbo_region").value = data[0].idregion }, 300);
            setTimeout(() => { mostrarComunas() }, 450);
            setTimeout(() => { document.getElementById("cbo_comuna").value = data[0].idcomuna }, 700);

            $.get("ConsultaPersonaId/?id="+ data[0].idrepresentante +"&empresa="+id, function (data2) {

                document.getElementById("txt_rut_repre").value = data2.info.data[0].rut;
                document.getElementById("txt_nombre_repre").value = data2.info.data[0].nombres;
                document.getElementById("txt_ape_repre").value = data2.info.data[0].apellidos;
                document.getElementById("txt_mail_repre").value = data2.info.data[0].email;

            })

            $.get("ConsultaPersonaId/?id="+ data[0].idencargado + "&empresa=" + id, function (data2) {

                document.getElementById("txt_rut_encar").value = data2.info.data[0].rut;
                document.getElementById("txt_nombre_encar").value = data2.info.data[0].nombres;
                document.getElementById("txt_ape_encar").value = data2.info.data[0].apellidos;
                document.getElementById("txt_mail_encar").value = data2.info.data[0].email;

            })

            document.getElementById("txt_mail_empre").value = data[0].email;
            document.getElementById("txt_obs").value = data[0].obs;

            var elemento = document.getElementsByClassName("form-group-float-label");
            for (var i = 0; i < elemento.length; i++) {
                elemento[i].className += " is-visible";
            }

        })
    }
    else {
        limpiar();
        document.getElementById("txt_id").innerHTML = "";
        document.getElementById("txt_Cabecera").innerHTML = "Nueva Empresa";

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

    if (confirm("confirma que desea eliminar esta información?") == 1) {

        $.get("dEmpresa?empresa=" + id, function (data) {
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



function crearPaises() {
    $.get("ConsultarPaises/?empresa=28", function (data) {

        var contenido = "";

        contenido += "<option value=''>--Seleccione--</option>"
        //Generando las opciones
        for (var i = 0; i < data.info.data.length; i++) {
            contenido += "<option value='" + data.info.data[i].idPais + "'>" + data.info.data[i].nombre + "</option>"
        }

        document.getElementById("cbo_pais").innerHTML = contenido;

    })
}

function mostrarRegiones() {

    var idPais = document.getElementById("cbo_pais").value
    console.log("Pais  :" + idPais);
    $.get("ConsultarRegiones?idpais=" + idPais + "&empresa=28", function (data) {

        var contenido = "";

        contenido += "<option value=''>--Seleccione--</option>"
        //Generando las opciones
        for (var i = 0; i < data.info.data.length; i++) {
            contenido += "<option value='" + data.info.data[i].idRegion + "'>" + data.info.data[i].nombre + "</option>"

        }

        document.getElementById("cbo_region").innerHTML = contenido;

    })
}

function mostrarComunas() {

    var idRegion = document.getElementById("cbo_region").value
    console.log("Region:" + idRegion);

    $.get("ConsultarComunas?idregion=" + idRegion + "&empresa=28", function (data) {

        var contenido = "";

        contenido += "<option value=''>--Seleccione--</option>"
        //Generando las opciones
        for (var i = 0; i < data.info.data.length; i++) {
            contenido += "<option value='" + data.info.data[i].idComuna + "'>" + data.info.data[i].nombre + "</option>"

        }

        document.getElementById("cbo_comuna").innerHTML = contenido;

    })
}


function Guarda() {

    var id = document.getElementById("txt_id").innerHTML;
    var rut = document.getElementById("txt_rut").value;
    var razonsocial = document.getElementById("txt_razon").value;
    var fantasia = document.getElementById("txt_fantasia").value;
    var giro = document.getElementById("txt_giro").value;
    var direccion = document.getElementById("txt_direccion").value;
    var idPais = document.getElementById("cbo_pais").value;
    var idregion = document.getElementById("cbo_region").value;
    var idcomuna = document.getElementById("cbo_comuna").value;
    var rutrepresentante = document.getElementById("txt_rut_repre").value;
    var nombresrepresentante = document.getElementById("txt_nombre_repre").value;
    var apellidosrepresentante = document.getElementById("txt_ape_repre").value;
    var rutencargado = document.getElementById("txt_rut_encar").value;
    var nombresencargado = document.getElementById("txt_nombre_encar").value;
    var apellidosencargado = document.getElementById("txt_ape_encar").value;
    var emailrepresentante = document.getElementById("txt_mail_repre").value;
    var emailencargado = document.getElementById("txt_mail_encar").value;
    var email = document.getElementById("txt_mail_empre").value;
    var obs = document.getElementById("txt_obs").value;


    var frm = new FormData();
    frm.append("id", id);
    frm.append("rut", rut);
    frm.append("razonsocial", razonsocial);
    frm.append("fantasia", fantasia);
    frm.append("giro", giro);
    frm.append("direccion", direccion);
    frm.append("idPais", idPais);
    frm.append("idregion", idregion);
    frm.append("idcomuna", idcomuna);
    frm.append("rutrepresentante", rutrepresentante);
    frm.append("nombresrepresentante", nombresrepresentante);
    frm.append("apellidosrepresentante", apellidosrepresentante);
    frm.append("rutencargado", rutencargado);
    frm.append("nombresencargado", nombresencargado);
    frm.append("apellidosencargado", apellidosencargado);
    frm.append("emailrepresentante", emailrepresentante);
    frm.append("emailencargado", emailencargado);
    frm.append("email", email);
    frm.append("obs", obs);


    $.ajax({
        type: "POST",
        url: "CrearEmpresa",
        data: frm,
        contentType: false,
        processData: false,

        success: function (data) {
            if (data.result == 0) {
                alert(data.mensaje);
            } else if (data == -1) {
                alert(data.mensaje);
            }
            else {
                alert("Proceso finalizado correctamente");
            }
            inicio();
            limpiar();
            document.getElementById("btnCerrar").click();
       }
    });

}
