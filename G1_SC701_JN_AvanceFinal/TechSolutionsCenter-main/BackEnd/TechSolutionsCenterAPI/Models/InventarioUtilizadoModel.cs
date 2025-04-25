namespace TechSolutionsCenterAPI.Models
{
    public class InventarioUtilizadoModel
    {

        public long ID_InventarioUtilizado { get; set; }

        public DateTime Fecha { get; set; }

        public int Cantidad { get; set; }

        public long ID_Inventario { get; set; }

        public long ID_CasoAtendido { get; set; }

    }
}
