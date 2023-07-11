using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using TareasMVC;

var builder = WebApplication.CreateBuilder(args);

var politicaUsuariosAutenticados = new AuthorizationPolicyBuilder() //haremos que nuestra aplicacion por defecto acepte usuarios autenticados, n filtro global
            .RequireAuthenticatedUser()
            .Build(); //despuesde terminar le pasamos al siguiente middelware es importante el rodend e donde se delcaran estos
// Add services to the container.
builder.Services.AddControllersWithViews(opciones =>
{
    opciones.Filters.Add(new AuthorizeFilter(politicaUsuariosAutenticados));
});

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

var app = builder.Build();

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
