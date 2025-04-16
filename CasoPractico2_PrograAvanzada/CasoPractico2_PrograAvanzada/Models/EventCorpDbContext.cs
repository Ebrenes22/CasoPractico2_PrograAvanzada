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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relación entre Usuario y Categoria (UsuarioRegistro)
            modelBuilder.Entity<Categoria>()
                .HasOne(c => c.UsuarioRegistro)
                .WithMany()
                .HasForeignKey(c => c.UsuarioRegistroId)
                .OnDelete(DeleteBehavior.Restrict); 

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
        }


    }


    }
