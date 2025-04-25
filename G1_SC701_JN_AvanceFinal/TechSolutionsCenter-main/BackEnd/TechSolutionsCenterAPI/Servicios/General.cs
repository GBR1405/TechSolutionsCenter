using System.Security.Claims;

namespace JN_ProyectoApi.Servicios
{
    public class General : IGeneral
    {
        public long ObtenerUsuarioFromToken(IEnumerable<Claim> valores)
        {
            if (valores.Any())
            {
                var IdUsuario = valores.FirstOrDefault(x => x.Type == "IdUsuario")?.Value;
                return long.Parse(IdUsuario!);
            }

            return 0;
        }

        public string ObtenerCorreoFromToken(IEnumerable<Claim> valores)
        {
            if (valores.Any())
            {
                var CorreoUsuario = valores.FirstOrDefault(x => x.Type == "Correo")?.Value;
                return CorreoUsuario!;
            }

            return null;
        }
    }
}
