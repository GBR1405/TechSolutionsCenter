namespace TechSolutionsCenter.Models
{
    public class GuardarInventarioModel
    {
        public int casoId { get; set; }
        public List<InventarioItemModel> items { get; set; }
    }

    public class InventarioItemModel
    {
        public long ID_Inventario { get; set; }
        public long ID_InventarioUtilizado { get; set; }
        public int Cantidad { get; set; }
    }
}
