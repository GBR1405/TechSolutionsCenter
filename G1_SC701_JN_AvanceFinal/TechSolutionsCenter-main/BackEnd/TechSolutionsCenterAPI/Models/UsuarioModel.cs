namespace TechSolutionsCenterAPI.Models
{
    public class UsuarioModel
    {
        public long IdUsuario { get; set; }
        public string? Nombre_Usuario { get; set; }
        public string? Apellidos { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public string? Email { get; set; }
        public string? Contrasenna { get; set; }
        public long ID_Genero { get; set; }
        public long ID_Rol { get; set; }
        public string? Token { get; set; }
    }
}
