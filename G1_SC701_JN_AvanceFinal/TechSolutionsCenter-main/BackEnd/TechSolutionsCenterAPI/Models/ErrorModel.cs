namespace TechSolutionsCenterAPI.Models
{
    public class ErrorModel
    {
        public long ID_Errror { get; set; }
        public DateTime Fecha { get; set; }
        public string? Mensaje { get; set; }
        public string? StackTrace { get; set; }
        public long ID_Usuario { get; set; }
        public string? Origen { get; set; }
    }
}
