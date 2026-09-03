namespace GestionAcademica.Models
{
    public class Matricula
    {
        public int MatriculaId { get; set; }
        public int EstudianteId { get; set; }
        public Estudiante? Estudiante { get; set; }
        public int CursoId { get; set; }
        public Curso? Curso { get; set; }
        public DateTime FechaMatricula { get; set; } = DateTime.Now;
        public ICollection<Calificacion> Calificaciones { get; set; } = new List<Calificacion>();
    }
}