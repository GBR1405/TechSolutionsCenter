
namespace TechSolutionsCenterAPI.Models;

public class InventarioArticulosViewModel
{
    public List<InventarioModel> Inventario { get; set; }
    public List<InventarioModel> Articulos { get; set; } // Usamos el mismo modelo pero solo para artículos
    public InventarioModel NuevoInventario { get; set; } // Para el formulario de agregar
    public InventarioModel NuevoArticulo { get; set; } // Para el formulario de agregar artículo
}