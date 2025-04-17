using System;
using System.ComponentModel.DataAnnotations;

namespace CasoPractico2_PrograAvanzada.Models
{
    public class Categoria
    {
        public int CategoriaId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        public string Estado { get; set; }

        [DataType(DataType.Date)]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}