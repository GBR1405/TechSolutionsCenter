namespace TechSolutionsCenter.Models
{
    public class InventarioDisponibleModel
    {
        public long ID_Inventario { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public int Cantidad { get; set; }
        public string N_Lote { get; set; }
        public long ID_Articulo { get; set; }
    }
}
