namespace TechSolutionsCenter.Models
{
    public class CasosModel
    {
        public long ID_Caso { get; set; }
        public string? Titulo { get; set; }
        public string? Descripcion { get; set; }
        public string? Estado { get; set; }
        public byte[]? Imagen { get; set; }
        public DateTime Fecha_Ingreso { get; set; }
        public long ID_Usuario { get; set; }

        public string? Nombre_Usuario { get; set; }
        public string? Apellidos { get; set; }
        public string? Email { get; set; }
        public string? Estado_Atencion { get; set; }

        public byte[] ImagenBytes { get; set; }
    }
}
