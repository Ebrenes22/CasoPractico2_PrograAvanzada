using System.ComponentModel.DataAnnotations;

namespace CasoPractico2_PrograAvanzada.Models
{
    public class Usuario
    {
        public int UsuarioId { get; set; }

        [Required]
        [Display(Name = "Nombre de Usuario")]
        public string NombreUsuario { get; set; }

        [Required]
        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; }

        [Required]
        [EmailAddress]
        public string Correo { get; set; }

        [Required]
        [Phone]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; }

        [Required]
        public string Rol { get; set; } // "Administrador", "Organizador", "Usuario"
    }
}