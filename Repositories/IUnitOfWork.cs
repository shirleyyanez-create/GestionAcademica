using GestionAcademica.Models;

namespace GestionAcademica.Repositories
{
    public interface IUnitOfWork
    {
        IRepository<Estudiante> Estudiantes { get; }
        IRepository<Profesor> Profesores { get; }
        IRepository<Curso> Cursos { get; }
        IRepository<Matricula> Matriculas { get; }
        IRepository<Calificacion> Calificaciones { get; }
        Task<int> SaveChangesAsync();
    }
}