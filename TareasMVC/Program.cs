using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TareasMVC;
using TareasMVC.Servicios;

var builder = WebApplication.CreateBuilder(args);

var politicaUsuariosAutenticados = new AuthorizationPolicyBuilder() //haremos que nuestra aplicacion por defecto acepte usuarios autenticados, n filtro global
            .RequireAuthenticatedUser()
            .Build(); //despuesde terminar le pasamos al siguiente middelware es importante el rodend e donde se delcaran estos
// Add services to the container.
builder.Services.AddControllersWithViews(opciones =>
{
    opciones.Filters.Add(new AuthorizeFilter(politicaUsuariosAutenticados));
}).AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)//para el sufijo pueda mostranos en ingles
.AddDataAnnotationsLocalization(opciones =>
{
    opciones.DataAnnotationLocalizerProvider = (_, factoria) => factoria.Create(typeof(RecursoCompartido));
});//ESTA ES UNA TECNIVA QUE NOS PERMITIRA UTILIZAR  UN UNICO ARCHIVO DE RECURSOS PARA TRADUCIR LAS ANOTACIONES DE DATOS Y SE LLAMARA
    // RECURSOD E DATOS ES DECIR LOS ERRORES DE LOS CAMPOS DE INPUIT YE SAS COSAS


builder.Services.AddDbContext<ApplicationDbContext>(opciones => opciones.UseSqlServer("name=DefaultConnection"));

builder.Services.AddAuthentication(); // servicios apra que el usuario se peude logear de  de autenticacion


builder.Services.AddIdentity<IdentityUser, IdentityRole>(opciones =>
{
    opciones.SignIn.RequireConfirmedAccount = false; //quiere deicr que no requiero de una cuenta confifrmada para que el usuario pueda logearse
}).AddEntityFrameworkStores<ApplicationDbContext>() //necesitamso agregar el servicio para agregar el sistmea idebntty como tal
.AddDefaultTokenProviders();


builder.Services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme,
   opciones =>
   {
       opciones.LoginPath = "/usuarios/login"; //sera nuestra url para logearnos
       opciones.AccessDeniedPath = "/usuarios/login";
   }); //para no trabajar con las vistas de login y esos que nos brina entity por defecto sino queremos usar unas personaldiadas

builder.Services.AddLocalization(opciones =>
{
    opciones.ResourcesPath = "Recursos";  //tenemos quie configurar nuestro archivo de recursos creados y darle el nombre
}); //agregamos el servcio que nos brinda microsoft sobre la localizacion


builder.Services.AddTransient<IServicioUsuarios, ServicioUsuario>();
builder.Services.AddAutoMapper(typeof(Program));


var app = builder.Build();


var culturasUISoportadas = new[] { "es", "en" };        //configuracion para poder soportar Culturas (UI, MONEDAS, ETC)

app.UseRequestLocalization(opciones =>
{
    opciones.DefaultRequestCulture = new RequestCulture("es");     //cultura por defecto que usaremos
    opciones.SupportedUICultures = culturasUISoportadas
    .Select(cultura => new CultureInfo(cultura)).ToList();  //culturas UI Soporrtadas
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); //CON ESTE MIDDLEWARE OBTENEDREMOS LA DATA DEL USUARIO AUTENTICADO

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
