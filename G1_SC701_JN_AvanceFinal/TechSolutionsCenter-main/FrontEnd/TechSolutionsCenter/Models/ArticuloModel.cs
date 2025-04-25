using System.Reflection.Metadata.Ecma335;

namespace TechSolutionsCenterAPI.Models
{
    public class ArticuloModel
    {

        public long ID_Articulo { get; set; }

        public string? Nombre { get; set; }

        public decimal Precio { get; set; }

        public long ID_Marca { get; set; }

        public long ID_Tipo { get; set; }
    }
}
