using GestionAcademica.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionAcademica.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Estudiante> Estudiantes => Set<Estudiante>();
        public DbSet<Profesor> Profesores => Set<Profesor>();
        public DbSet<Curso> Cursos => Set<Curso>();
        public DbSet<Matricula> Matriculas => Set<Matricula>();
        public DbSet<Calificacion> Calificaciones => Set<Calificacion>();
    }
}