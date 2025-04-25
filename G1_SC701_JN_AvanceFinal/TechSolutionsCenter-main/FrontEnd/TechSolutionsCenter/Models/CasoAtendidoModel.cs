namespace TechSolutionsCenterAPI.Models
{
    public class CasoAtendidoModel
    {

        public long ID_CasoAtendido { get; set; }

        public DateTime? Fecha_Atencion { get; set; }

        public DateTime? Fecha_Finalizado { get; set; }

        public long ID_Caso { get; set; }

        public long ID_Usuario { get; set; }

        public string? Titulo { get; set; }
        public string? Descripcion { get; set; }
        public char? Estado { get; set; }
        public byte[]? Imagen { get; set; }
        public DateTime? Fecha_Ingreso { get; set; }

        public string? Nombre_Usuario { get; set; }
        public string? Apellidos { get; set; }
        public string? Email { get; set; }

        public string? comentario { get; set; }
    }
}
