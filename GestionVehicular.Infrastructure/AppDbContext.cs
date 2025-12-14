using GestionVehicular.Core;
using GestionVehicular.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestionVehicular.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSets alineados con tu script SQL
        public DbSet<Vehiculo> Vehiculos { get; set; }
        public DbSet<Conductor> Conductores { get; set; }
        public DbSet<Asignacion> Asignaciones { get; set; }
        public DbSet<Log> Logs { get; set; } // opcional, si usas la tabla Logs

        public DbSet<AsignacionView> AsignacionesView { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapear nombres de tablas en singular
            modelBuilder.Entity<Vehiculo>().ToTable("Vehiculo");
            modelBuilder.Entity<Conductor>().ToTable("Conductor");
            modelBuilder.Entity<Asignacion>().ToTable("Asignacion");
            modelBuilder.Entity<Log>().ToTable("Logs");
            modelBuilder.Entity<AsignacionView>().HasNoKey();


            // Vehiculo
            modelBuilder.Entity<Vehiculo>(entity =>
            {
                entity.HasKey(v => v.Id);
                entity.Property(v => v.Matricula).HasMaxLength(20).IsRequired();
                entity.Property(v => v.Marca).HasMaxLength(50).IsRequired();
                entity.Property(v => v.Modelo).HasMaxLength(50).IsRequired();
                entity.Property(v => v.Anio).IsRequired();
                entity.Property(v => v.Tipo).HasMaxLength(30).IsRequired();
                entity.Property(v => v.Estado).HasMaxLength(20).IsRequired();

                entity.HasIndex(v => v.Matricula).IsUnique()
                      .HasDatabaseName("UX_Vehiculo_Matricula");
            });

            // Conductor
            modelBuilder.Entity<Conductor>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.NombreCompleto).HasMaxLength(100).IsRequired();
                entity.Property(c => c.NumeroLicencia).HasMaxLength(30).IsRequired();
                entity.Property(c => c.Contacto).HasMaxLength(100);

                entity.HasIndex(c => c.NumeroLicencia).IsUnique()
                      .HasDatabaseName("UX_Conductor_NumeroLicencia");
            });

            // Asignacion
            modelBuilder.Entity<Asignacion>(entity =>
            {
                entity.HasKey(a => a.Id);

                entity.HasOne(a => a.Vehiculo)
                      .WithMany(v => v.Asignaciones)
                      .HasForeignKey(a => a.VehiculoId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Conductor)
                      .WithMany(c => c.Asignaciones)
                      .HasForeignKey(a => a.ConductorId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(a => a.FechaInicio).IsRequired();
                entity.Property(a => a.FechaFin).IsRequired();

                entity.HasIndex(a => new { a.VehiculoId, a.ConductorId, a.FechaInicio, a.FechaFin })
                      .IsUnique()
                      .HasDatabaseName("UX_Asignacion_Unica");
            });

            // Logs (opcional)
            modelBuilder.Entity<Log>(entity =>
            {
                entity.HasKey(l => l.Id);
                entity.Property(l => l.Accion).HasMaxLength(30).IsRequired();
                entity.Property(l => l.Fecha).IsRequired();
                entity.Property(l => l.RealizadoPor).HasMaxLength(60);
                entity.Property(l => l.Cambios).HasMaxLength(500);
            });
        }
    }
}