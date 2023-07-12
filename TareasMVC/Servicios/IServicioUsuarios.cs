using System.Security.Claims;

namespace TareasMVC.Servicios
{
    public interface IServicioUsuarios
    {
        string ObtenerUsuarioId();
    }

    public class ServicioUsuario : IServicioUsuarios
    {
        private HttpContext httpContext;
        public ServicioUsuario(IHttpContextAccessor httpContextAccessor)//como no estamos en un controlador tenemos que usar el http comntext accesor
        {
            httpContext = httpContextAccessor.HttpContext;

        }
        public string ObtenerUsuarioId()
        {
            if(httpContext.User.Identity.IsAuthenticated) //obtenemos el id del usuario de la aplicacion
            {
                var idClaim = httpContext.User.Claims
                    .Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault(); //buscamos el claim que tiene el id usuario
                    //recordar que el claim es simplemente una informacion acerca del usuario, entre los claims del usuario

                return idClaim.Value;
            }
            else
            {
                throw new Exception("El usuario no esta autenticado");
            }
           
        }
    }
}
