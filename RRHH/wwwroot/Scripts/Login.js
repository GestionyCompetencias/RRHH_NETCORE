
inicio();

function inicio() {
    document.getElementById("formEmpre").style.display = 'none';
}

function validarIngreso() {

    var usuari = document.getElementById("txt_usuar").value;
    var contra = document.getElementById("txt_contr").value;
    

    var frm = new FormData();
    frm.append("Usuario", usuari);
    frm.append("Password", contra);

    $.ajax({
        type: "POST",
        url: "Login/Logins",
        data: frm,
        contentType: false,
        processData: false,

        success: function (data) {

            var datas = JSON.parse(data);

            if (datas.info.result == 0) {
                alert("Usuario o Contraseña Invalida");
                document.getElementById("txt_contr").value = "";
            } 
            else {

                var idUsu = datas.info.data.idUsu;
                
                obtenerEmpresas(idUsu);
                                
                document.getElementById("formLogin").style.display = 'none';
                document.getElementById("formEmpre").style.display = 'block';
            }
        }
    });
}

function obtenerEmpresas(idUsu) {
    console.log(idUsu);
    $.get("ConsultarEmpresasUsuario?idUsuario="+ idUsu, function (dataEmp) {

        var dataEmp = JSON.parse(dataEmp);
        crearCombo(dataEmp, "cbo_empresas");

    })

}

function crearCombo(data, comboId) {

    var contenido = "";

    contenido += "<option value=''>--Seleccionar Empresa--</option>"
    //Generando las opciones
    for (var i = 0; i < data.info.data.length; i++) {
        contenido += "<option value='" + data.info.data[i].id + "'>" + data.info.data[i].razonsocial + "</option>"
    }

    document.getElementById(comboId).innerHTML = contenido;

}

function SelectEmpreX() {
    var newEmpre = document.getElementById("cbo_empresas").value;

    if (newEmpre == '') {
        alert("Debe seleccionar una empresa");
        return;
    }

    $.get("SeleccionarEmpresa/" + newEmpre, function (data) {
        var data = JSON.parse(data);
        document.location.href = "Inicio";

    })

}



