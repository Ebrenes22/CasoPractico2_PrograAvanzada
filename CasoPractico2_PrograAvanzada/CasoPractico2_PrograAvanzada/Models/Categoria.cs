using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CasoPractico2_PrograAvanzada.Models
{
    public class Categoria
    {
        public int CategoriaId { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Descripcion { get; set; }

        [Required]
        public string Estado { get; set; }

        [DataType(DataType.Date)]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public int UsuarioRegistroId { get; set; }

        [ForeignKey("UsuarioRegistroId")]
        public Usuario UsuarioRegistro { get; set; }
    }
}
