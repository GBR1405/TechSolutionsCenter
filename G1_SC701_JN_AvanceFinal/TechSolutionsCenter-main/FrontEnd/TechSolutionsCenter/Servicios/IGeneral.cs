using JN_ProyectoWeb.Models;
using TechSolutionsCenterAPI.Models;

namespace TechSolutionsCenter.Servicios
{
    public interface IGeneral
    {
        HttpResponseMessage ObtenerCasosPorUsuario(long Id);
        HttpResponseMessage ObtenerCasosPendientes();

        HttpResponseMessage ObtenerCasosDisponibles();
        HttpResponseMessage ObtenerCasosAtendido();

        HttpResponseMessage ObtenerInventario();

        HttpResponseMessage ObtenerInventarioDisponible();

        HttpResponseMessage ObtenerArticulos();

        HttpResponseMessage ObtenerInventarioUtilizadoPorCaso(long casoId);

        HttpResponseMessage GestionarInventarioUtilizado(InventarioUtilizadoModel model);

        HttpResponseMessage ActualizarCantidadInventario(long idInventarioUtilizado, int cantidad);

        HttpResponseMessage EliminarInventarioUtilizado(long idInventarioUtilizado);

        HttpResponseMessage AgregarInventarioUtilizado(InventarioUtilizadoModel model);

        HttpResponseMessage RestarInventarioUtilizado(InventarioUtilizadoModel model);

    }
}
