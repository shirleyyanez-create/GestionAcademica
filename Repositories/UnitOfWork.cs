using GestionAcademica.Data;
using GestionAcademica.Models;

namespace GestionAcademica.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public IRepository<Estudiante> Estudiantes { get; }
        public IRepository<Profesor> Profesores { get; }
        public IRepository<Curso> Cursos { get; }
        public IRepository<Matricula> Matriculas { get; }
        public IRepository<Calificacion> Calificaciones { get; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Estudiantes = new Repository<Estudiante>(context);
            Profesores = new Repository<Profesor>(context);
            Cursos = new Repository<Curso>(context);
            Matriculas = new Repository<Matricula>(context);
            Calificaciones = new Repository<Calificacion>(context);
        }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}