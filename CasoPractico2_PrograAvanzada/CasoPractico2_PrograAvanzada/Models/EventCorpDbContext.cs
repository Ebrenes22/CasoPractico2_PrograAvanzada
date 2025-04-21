using Microsoft.EntityFrameworkCore;

namespace CasoPractico2_PrograAvanzada.Models
{
    public class EventCorpDbContext : DbContext
    {
        public EventCorpDbContext(DbContextOptions<EventCorpDbContext> options) :
            base(options)
        { }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Inscripciones> Inscripciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relación entre Usuario y Evento (UsuarioRegistro)
            modelBuilder.Entity<Evento>()
                .HasOne(e => e.UsuarioRegistro)
                .WithMany()
                .HasForeignKey(e => e.UsuarioRegistroId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación entre Evento y Categoria
            modelBuilder.Entity<Evento>()
                .HasOne(e => e.Categoria)
                .WithMany()
                .HasForeignKey(e => e.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Inscripciones>()
                .HasOne(i => i.Evento)
                .WithMany()
                .HasForeignKey(i => i.EventoId);

            modelBuilder.Entity<Inscripciones>()
                .HasOne(i => i.Usuario)
                .WithMany()
                .HasForeignKey(i => i.UsuarioId);
        }


    }


}
