using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
using TareasMVC.Migrations;
using TareasMVC.Models;
using TareasMVC.Servicios;

namespace TareasMVC.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly ApplicationDbContext context;

        public UsuariosController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext context


            ) { // userManager utilziamos para trabajar con nuestros usuarios 
            //aplicationdbcontext es la pirza central de entityframeworkcore y que para hacer queries a las tablas tenemos que usar una instancia de esta
            //recordar que el aplicationdbcontext lo configuramos comos ervicio e la clase program
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.context = context;
            //identityuser es la entidad configurada en al clase program que representa a un usuario
        }
        [AllowAnonymous] // nos permite indicar que cualquier usuario este identificado o no puede invocar esta accion 
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Registro(RegistroViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);    
            }

            var usuario = new IdentityUser()
            {
                Email = modelo.Email,
                UserName = modelo.Email
            };

            var resultado = await userManager.CreateAsync(usuario , password : modelo.Password);
            if (resultado.Succeeded)
            {
                await signInManager.SignInAsync(usuario, isPersistent: true);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach(var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description); // porque es un error que no se relacione con un campo especifico sino que es a nivel de modelo
                }
                return View(modelo);
            }
        }

        [AllowAnonymous]
        public IActionResult Login(string mensaje = null)
        {
            if(mensaje is not null)
            {
                ViewData["mensaje"] = mensaje;  //pasando el viewdata hacia la vistra
            }
            return View();  
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel modelo)
        {
            if(!ModelState.IsValid)
            {
                return View(modelo);
            }

            var resultado
                = await signInManager.PasswordSignInAsync(modelo.Email, modelo.Password, modelo.Recuerdame, lockoutOnFailure: false);
            //lockoutOnFailure : si el usuario se equivoca mcuhas veces en iniciar sesion en esa cuenta la cerraremos, ponemosen false

            if(resultado.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Nombre del usuario o password incorrectos");
                return View(modelo);
            }

        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Home"); 
        }

        [AllowAnonymous]
        [HttpGet]
        public ChallengeResult LoginExterno(string proveedor, string urlRetorno = null)//challengeresult significa que vamos aredirigir al usuario donde va a poder logerase , el retorno opcional
        {
            var urlRedireccion = Url.Action("RegistrarUsuarioExterno", values: new { urlRetorno }); //nombre de la accion que va a recibir la data del usuario
            var propiedades = signInManager.ConfigureExternalAuthenticationProperties(proveedor, urlRedireccion); //configura las propiedades de autenticacion externa 
            return new ChallengeResult(proveedor, propiedades); 
        }

        [AllowAnonymous]
        public async Task<IActionResult> RegistrarUsuario(string urlRetorno = null, string remoteError = null) //EL REM,OTE ERROR HARA REFERENCIA AL URL QE NOS PUIEDE DEVOLVER EL PROVEEDOR EN CASOD E CUALQUIER ERROR
        {
            urlRetorno = urlRetorno ?? Url.Content("~/"); //SI ES NULO SE LE ENVIARA AL ROOT
            var mensaje = "";
            if (remoteError is not null) {
                mensaje = $"Error del proveedor externo : { remoteError}";
                return RedirectToAction("login", routeValues: new { mensaje }); //si hay algun error redireciono al llogin con ese mensaje
            }

            var info = await signInManager.GetExternalLoginInfoAsync();
            if(info is null)
            {
                mensaje = "Error, cargando la data de login externo";
                return RedirectToAction("login", routeValues: new { mensaje });
            }

            var resultadoLoginExterno
                = await signInManager.ExternalLoginSignInAsync(info.LoginProvider,info.ProviderKey, isPersistent : true, bypassTwoFactor : true);//usamos un login pero suando la informacion del login externo

            //YA LA CUENTA EXISTE DEL PROVEEDOR INTERNO Y NNOI TENEMSI QUE HACER NADA
            //HEMOS LOGEADO CON LOS DATOS PROVENIENTES Y FUE TODO OK, EL SUUARIO TIENE CUENA
            if(resultadoLoginExterno.Succeeded)
            {
                return LocalRedirect(urlRetorno);
            }
            string email = "";

            //usuaio no tiene una cuenta co nosotros y teen,so que crearlla                     
            if(info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                email = info.Principal.FindFirstValue(ClaimTypes.Email);
            }
            else
            {
                mensaje = "Error leyendo el email del usuario proveedor";
                return RedirectToAction("login", routeValues: new { mensaje });
            }

            var usuario = new IdentityUser { Email = email, UserName = email };
            var resultadoCrearUsuario = await userManager.CreateAsync(usuario); 

            if(!resultadoCrearUsuario.Succeeded)
            {
                mensaje = resultadoCrearUsuario.Errors.First().Description;
                return RedirectToAction("login", routeValues: new { mensaje });
            }

            //si fue exitoso vamos a agregar ese login externo a la tabla de logins
            var resultadoAgregarLogin = await userManager.AddLoginAsync(usuario, info);
            if (resultadoAgregarLogin.Succeeded)
            {
                await signInManager.SignInAsync(usuario, isPersistent: true, info.LoginProvider);//ahora si todo esta OK logearemos al usuario
                return LocalRedirect(urlRetorno);
            }

            mensaje = "Ha ocurrido un error al agreegar el login";
            return RedirectToAction("login", routeValues: new { mensaje });


        }

        [HttpGet]
        [Authorize(Roles = Constantes.RolAdmin)] //para evitar el ingreso mediante el link a un recurso no autorizado
        public async Task<IActionResult> Listado(string mensaje = null)
        {
            var usuarios = await context.Users.Select(u => new UsuarioViewModel
            {
                Email = u.Email,
            }).ToListAsync();
            //EN ESTE CASO QUEREMOS TRAER SOLO LA CO,UMNA DEL EMAIL POR ESO MEDIANTE EL SELECT SELECIONAMOS CUAL Y NO TRA
            //EMOS TODAS LA COLUMNAS POR DEFECTO
            var modelo = new UsuariosListadoViewModel();
            modelo.Usuarios = usuarios;
            modelo.Mensaje = mensaje;
            return View(modelo);    
        }

        [HttpPost]
        [Authorize(Roles = Constantes.RolAdmin)]
        public async Task<IActionResult> HacerAdmin(string email)
        {
            var usuario = await context.Users.Where( u => u.Email == email).FirstOrDefaultAsync();
            //firstordefault significa el primer valor ol el valor por defecto de usuario que es null
            if(usuario is null)
            {
                return NotFound();
            }
            await userManager.AddToRoleAsync(usuario, Constantes.RolAdmin);
            return RedirectToAction("Listado",routeValues:new {mensaje = "Rol asignado correctamente a" + email});

        }

        [HttpPost]
        [Authorize(Roles = Constantes.RolAdmin)]
        public async Task<IActionResult> RemoverAdmin(string email)
        {
            var usuario = await context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
            //firstordefault significa el primer valor ol el valor por defecto de usuario que es null
            if (usuario is null)
            {
                return NotFound();
            }
            await userManager.RemoveFromRoleAsync(usuario, Constantes.RolAdmin);
            return RedirectToAction("Listado", routeValues: new { mensaje = "Rol removido correctamente a" + email });

        }
    }
 }
