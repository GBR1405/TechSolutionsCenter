namespace TechSolutionsCenterAPI.Models
{
    public class FacturaModel
    {

        public long ID_Factura { get; set; }

        public DateTime Fecha { get; set; }

        public bool Estado { get; set; }

        public string? Comentario { get; set; }

        public long ID_CasoAtendido { get; set; }

    }
}
