namespace GestionAcademica.Models
{
    public class Profesor
    {
        public int ProfesorId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Especialidad { get; set; }
        public ICollection<Curso> Cursos { get; set; } = new List<Curso>();
    }
}