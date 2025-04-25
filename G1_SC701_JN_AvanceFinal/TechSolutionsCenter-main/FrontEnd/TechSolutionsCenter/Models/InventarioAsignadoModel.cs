namespace TechSolutionsCenter.Models
{
    public class InventarioAsignadoModel
    {
        public long ID_InventarioUtilizado { get; set; }
        public long ID_Inventario { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public int Cantidad { get; set; }
        public DateTime Fecha { get; set; }
        public long ID_CasoAtendido { get; set; }
    }
}
