namespace TechSolutionsCenter.Models
{
    public class UsuarioModel
    {
        public long IdUsuario { get; set; }
        public string? NombreUsuario { get; set; }
        public string? Apellidos { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public string? Email { get; set; }
        public string? Contrasenna { get; set; }
        public long IdGenero { get; set; }
        public long IdRol { get; set; }
        public string? Token { get; set; }
    }
}
