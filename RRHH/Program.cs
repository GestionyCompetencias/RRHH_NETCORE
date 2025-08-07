// Imports del sistema contable
using RRHH.BaseDatos;
using RRHH.BaseDatos.NoSQL;
using RRHH.Models.Utilities.Configuration;
using RRHH.Servicios.Sessions;
using RRHH.Servicios.Formularios;
using RRHH.Servicios.Storage;
using RRHH.Servicios.Perfiles;
using RRHH.Servicios.Faenas;
using RRHH.Servicios.Cargos;
using RRHH.Servicios.CentrosCostos;
using RRHH.Servicios.Jornadas;
using RRHH.Servicios.Parametros;
using RRHH.Servicios.CambioClave;
using RRHH.Servicios.Contratos;
using RRHH.Servicios.EstadoContratos;
using RRHH.Servicios.Haberes;
using RRHH.Servicios.Descuentos;
using RRHH.Servicios.HaberesInformados;
using RRHH.Servicios.DescuentosInformados;
using RRHH.Servicios.AsistenciasInformadas;
using RRHH.Servicios.CuadraturaHaberes;
using RRHH.Servicios.LibroRemuneraciones;
using RRHH.Servicios.CuadraturaDescuentos;
using RRHH.Servicios.CuadraturaAsistencias;
using RRHH.Servicios.Liquidacion;
using RRHH.Servicios.ArchivoPrevired;
using RRHH.Servicios.ArchivoDireccionTrabajo;
using RRHH.Servicios.ArchivoDepositos;
using RRHH.Servicios.CierraProceso;
using RRHH.Servicios.ReajusteSueldos;
using RRHH.Servicios.SaldosLiquidos;
using RRHH.Servicios.ConversionContable;
using RRHH.Servicios.CargaAsistencia;
using RRHH.Servicios.ComprobanteContable;
using RRHH.Servicios.Papeletas;
using RRHH.Servicios.ModificaMarcas;
using RRHH.Servicios.AutorizaSobretiempos;
using RRHH.Servicios.ReporteMarcas;
using RRHH.Servicios.ResumenAsistencias;
using RRHH.Servicios.LicenciasMedicas;
using RRHH.Servicios.LicenciasConsulta;
using RRHH.Servicios.PermisoAsistencia;
using RRHH.Servicios.PermisoConsulta;
using RRHH.Servicios.PermisoAutoriza;
using RRHH.Servicios.SolicitudVacaciones;
using RRHH.Servicios.VacacionesPersonal;
using RRHH.Servicios.VacacionesAutorizaJefe;
using RRHH.Servicios.VacacionesAutorizaPersonal;
using RRHH.Servicios.VacacionesCtaCte;
using RRHH.Servicios.CompensacionSolicitud;
using RRHH.Servicios.CompensacionAutorizaPersonal;
using RRHH.Servicios.ApiRRHH;
using RRHH.Data;

//Rollbar Imports
using Rollbar;
using Rollbar.NetCore.AspNet;
using RRHH.Repositorios;
using Rotativa.AspNetCore;

// Main builder
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
string cors = "configurarcors";
builder.Services.AddControllers();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
    options.IdleTimeout = TimeSpan.FromMinutes(1200)
);

// Ambiente
string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var environmentObject = builder.Configuration.GetSection("Environments:" + environment).Get<DeployEnvironment>();

// Infraestrcutra de rollbar
ConfigureRollbarInfrastructure(environmentObject);
builder.Services.AddRollbarLogger(loggerOptions =>
{
    loggerOptions.Filter = (loggerName, loglevel) => loglevel >= LogLevel.Error;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: cors, builder =>
    {
        builder.AllowAnyOrigin();
        builder.AllowAnyOrigin();
        builder.AllowAnyOrigin();
    });
});

builder.Services.AddMvcCore();

// --- Servicios y Managers Proyecto Recursos humanos  ---
// -> Gestión de bases de datos
builder.Services.AddScoped<IDatabaseManager, DatabaseManager>();
builder.Services.AddScoped<IMongoDBContext, MongoDBContext>();
builder.Services.AddScoped<IMongoCollectionManager, MongoCollectionManager>();

// -> Utilidades
builder.Services.AddScoped<IJsonDataReader, JsonDataReader>();

// -> Servicios 
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFormService, FormService>();
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IFaenasService, FaenasService>();
builder.Services.AddScoped<ICargosService, CargosService>();
builder.Services.AddScoped<ICentrosCostoService, CentrosCostosService>();
builder.Services.AddScoped<IJornadasService, JornadasService>();
builder.Services.AddScoped<IParametrosService, ParametrosService>();
builder.Services.AddScoped<ICambioClaveService, CambioClaveService>();
builder.Services.AddScoped<IContratosService, ContratosService>();
builder.Services.AddScoped<IEstadoContratosService, EstadoContratosService>();
builder.Services.AddScoped<IHaberesService, HaberesService>();
builder.Services.AddScoped<IDescuentosService, DescuentosService>();
builder.Services.AddScoped<IHaberesInformadosService, HaberesInformadosService>();
builder.Services.AddScoped<IDescuentosInformadosService, DescuentosInformadosService>();
builder.Services.AddScoped<IAsistenciasInformadasService, AsistenciasInformadasService>();
builder.Services.AddScoped<ICuadraturaHaberesService, CuadraturaHaberesService>();
builder.Services.AddScoped<ILibroRemuneracionesService, LibroRemuneracionesService>();
builder.Services.AddScoped<ICuadraturaDescuentosService, CuadraturaDescuentosService>();
builder.Services.AddScoped<ICuadraturaAsistenciasService, CuadraturaAsistenciasService>();
builder.Services.AddScoped<ILiquidacionService, LiquidacionService>();
builder.Services.AddScoped<IArchivoPreviredService, ArchivoPreviredService>();
builder.Services.AddScoped<IArchivoDireccionTrabajoService, ArchivoDireccionTrabajoService>();
builder.Services.AddScoped<IArchivoDepositosService, ArchivoDepositosService>();
builder.Services.AddScoped<ICierraProcesoService, CierraProcesoService>();
builder.Services.AddScoped<IReajusteSueldosService, ReajusteSueldosService>();
builder.Services.AddScoped<ISaldosLiquidosService, SaldosLiquidosService>();
builder.Services.AddScoped<IConversionContableService, ConversionContableService>();
builder.Services.AddScoped<ICargaAsistenciaService, CargaAsistenciaService>();
builder.Services.AddScoped<IComprobanteContableService, ComprobanteContableService>();
builder.Services.AddScoped<IPapeletasService, PapeletasService>();
builder.Services.AddScoped<IModificaMarcasService, ModificaMarcasService>();
builder.Services.AddScoped<IAutorizaSobretiemposService, AutorizaSobretiemposService>();
builder.Services.AddScoped<IReporteMarcasService, ReporteMarcasService>();
builder.Services.AddScoped<IResumenAsistenciasService, ResumenAsistenciasService>();
builder.Services.AddScoped<ILicenciasMedicasService, LicenciasMedicasService>();
builder.Services.AddScoped<ILicenciasConsultaService, LicenciasConsultaService>();
builder.Services.AddScoped<IPermisoAsistenciaService, PermisoAsistenciaService>();
builder.Services.AddScoped<IPermisoConsultaService, PermisoConsultaService>();
builder.Services.AddScoped<IPermisoAutorizaService, PermisoAutorizaService>();
builder.Services.AddScoped<ISolicitudVacacionesService, SolicitudVacacionesService>();
builder.Services.AddScoped<IVacacionesPersonalService, VacacionesPersonalService>();
builder.Services.AddScoped<IVacacionesAutorizaJefeService, VacacionesAutorizaJefeService>();
builder.Services.AddScoped<IVacacionesAutorizaPersonalService, VacacionesAutorizaPersonalService>();
builder.Services.AddScoped<IVacacionesCtaCteService, VacacionesCtaCteService>();
builder.Services.AddScoped<ICompensacionSolicitudService, CompensacionSolicitudService>();
builder.Services.AddScoped<ICompensacionAutorizaPersonalService, CompensacionAutorizaPersonalService>();
builder.Services.AddScoped<IApiRRHHService, ApiRRHHService>();


// Repositorios
builder.Services.AddScoped<IEmpresaRepository, EmpresaRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Inicio/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Implementacion de rollbar en la app
app.UseRollbarMiddleware();

//Enable Session.
app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors(cors);

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Inicio}/{action=Index}/{id?}");

app.Run();
//void Configure(IApplicationBuilder app, IWebHostEnvironment env)
//{
//    // Configuración básica (busca en wwwroot/Rotativa)
//    RotativaConfiguration.Setup(env.ToString());

//}
 void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    // ... otras configuraciones (como app.UseRouting(), etc.)

    // Configura Rotativa con la ruta correcta
    RotativaConfiguration.Setup(env.WebRootPath, "Rotativa"); // Busca en /wwwroot/Rotativa/
}
void ConfigureRollbarInfrastructure(DeployEnvironment env)
{
    // Si por alguna razón se falla en obtener el ambiente, asumimos dev
    string environmentName = env != null ? env.Name : "Development";

    RollbarInfrastructureConfig config = new RollbarInfrastructureConfig(
      "ddb54e92944544e49d49eaefff04629c",
      environmentName
    );
    RollbarDataSecurityOptions dataSecurityOptions = new RollbarDataSecurityOptions();
    dataSecurityOptions.ScrubFields = new string[]
    {
      "url",
      "method",
    };
    config.RollbarLoggerConfig.RollbarDataSecurityOptions.Reconfigure(dataSecurityOptions);

    RollbarInfrastructure.Instance.Init(config);
}
