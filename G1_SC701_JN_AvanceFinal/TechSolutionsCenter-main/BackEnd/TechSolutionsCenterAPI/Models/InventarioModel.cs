namespace TechSolutionsCenterAPI.Models
{
    public class InventarioModel
    {
        public long ID_Inventario { get; set; }
        public string? N_Lote { get; set; }
        public int Cantidad { get; set; }
        public long ID_Articulo { get; set; }
        public string? NombreArticulo { get; set; }
        public decimal Precio { get; set; }

        // Cambia estos de string a objetos complejos
        public string? Marca { get; set; }
        public string? Tipo { get; set; }
    }

    public class MarcaModel
    {
        public long Id { get; set; }
        public string? Nombre { get; set; }
    }

    public class TipoModel
    {
        public long Id { get; set; }
        public string? Nombre { get; set; }
    }
}