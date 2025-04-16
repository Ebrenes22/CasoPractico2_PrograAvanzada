

using Microsoft.EntityFrameworkCore;

namespace CasoPractico2_PrograAvanzada.Models
{
    public class EventCorpDbContext : DbContext
    {
        public EventCorpDbContext(DbContextOptions<EventCorpDbContext> options) : 
            base(options)
        {}
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Evento> Eventos { get; set; }
    }
    
    
}
