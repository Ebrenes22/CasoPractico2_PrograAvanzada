using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CasoPractico2_PrograAvanzada.Models
{
    public class Evento
    {
        public int EventoId { get; set; }

        [Required]
        public string Titulo { get; set; }

        [Required]
        public string Descripcion { get; set; }

        [Required]
        public int CategoriaId { get; set; }

        [ForeignKey("CategoriaId")]
        public required Categoria Categoria { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan Hora { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La duración debe ser positiva.")]
        public int Duracion { get; set; } // En minutos

        [Required]
        public string Ubicacion { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "El cupo debe ser mayor a 0.")]
        public int CupoMaximo { get; set; }

        [DataType(DataType.Date)]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public int UsuarioRegistroId { get; set; }

        [ForeignKey("UsuarioRegistroId")]
        public Usuario UsuarioRegistro { get; set; }

    }
}
