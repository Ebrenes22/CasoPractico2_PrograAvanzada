using System.ComponentModel.DataAnnotations;

namespace CasoPractico2_PrograAvanzada.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Nombre de Usuario")]
        public string NombreUsuario { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; }
    }
}