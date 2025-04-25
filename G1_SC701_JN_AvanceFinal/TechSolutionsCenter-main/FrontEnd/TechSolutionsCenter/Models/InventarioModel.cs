public class InventarioModel
{
    public long ID_Inventario { get; set; }
    public string? N_Lote { get; set; }
    public int Cantidad { get; set; }
    public long ID_Articulo { get; set; }
    public string? NombreArticulo { get; set; }
    public decimal Precio { get; set; }

    // Cambiar a strings simples
    public string? Marca { get; set; }
    public string? Tipo { get; set; }
}



