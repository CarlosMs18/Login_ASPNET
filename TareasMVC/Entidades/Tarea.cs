using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TareasMVC.Entidades
{
    public class Tarea
    {
        public int Id { get; set; }

        [StringLength(250)] //SIN USAR EL APIFLUENTE
        [Required] //campo no null
        public string Titulo { get; set; } 
        public string Descripcion { get; set; } 
        public int Orden { get; set; }  
        public DateTime FechaCreacion { get; set; }     

        public string UsuarioCreacionId { get; set; } //cpolocarems el id del usuario que ha creado la tarea recordar que es un GUI por eso es string

        public IdentityUser UsuarioCreacion { get; set; }//en nuestro proyecto identyuser es la clase que define a un usuario en el sistema, asi que
                                                         // por lo tanto el sistema de navegacion sera d eeste tipo

        public List<Paso> Pasos { get; set; }   

        public List<ArchivoAdjunto> ArchivosAdjuntos { get; set; }  
    }
}
