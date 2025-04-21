using System.ComponentModel.DataAnnotations;

namespace CasoPractico2_PrograAvanzada.Models
{
    public class Inscripciones
    {
        public int InscripcionesId { get; set; }

        [Required(ErrorMessage = "El usuario a inscribirse es obligatorio")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "El evento es obligatorio")]
        public int EventoId { get; set; }

        [DataType(DataType.Date)]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public Usuario Usuario { get; set; }
        public Evento Evento { get; set; }
    }
}
