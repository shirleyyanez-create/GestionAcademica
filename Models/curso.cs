namespace GestionAcademica.Models
{
    public class Curso
    {
        public int CursoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Creditos { get; set; }
        public int ProfesorId { get; set; }
        public Profesor? Profesor { get; set; }
        public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
    }
}