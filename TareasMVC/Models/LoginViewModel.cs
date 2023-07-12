using System.ComponentModel.DataAnnotations;

namespace TareasMVC.Models
{
    public class LoginViewModel
    {

        //[Required(ErrorMessage = "El campo {0} es requerido")]
        [Required(ErrorMessage = "Error.Requerido")] //mediante el antotacion locale provider
        //[EmailAddress(ErrorMessage = "El campo debe de ser un correo electronico válido")]
        [EmailAddress(ErrorMessage = "Error.Email")]
        public string Email { get; set; }


        //[Required(ErrorMessage = "El campo {0} es requerido")]
        [Required(ErrorMessage = "Error.Requerido")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Display(Name = "Recuérdame")]
        public bool Recuerdame { get; set; }    
    }
}
