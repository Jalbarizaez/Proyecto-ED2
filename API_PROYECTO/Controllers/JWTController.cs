using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using API_PROYECTO.Models;

namespace API_PROYECTO.Controllers
{
    [Route("api/JWT")]
    [ApiController]
    public class JWTController : ControllerBase
    {
		private const string ALGO = "HS256";
		private const string Llave = "LlaveSuperSecretaImposibleDeHackear";


		[HttpPost]
		public ActionResult GenerarToken([FromBody]User Json)
		{
			try
			{
				var LlaveSecreta = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Llave));
				var credenciales = new SigningCredentials(LlaveSecreta, ALGO);
				var fechaExp = DateTime.UtcNow.AddHours(24); //El token se vence en 24 horas
				var claims = new[] {
					new Claim("nombre", Json.nombre),
					new Claim("apellido", Json.apellido),
					new Claim("sub", Json.id)};

				JwtSecurityToken Token = new JwtSecurityToken(
					claims: claims,
					expires: fechaExp,
					signingCredentials: credenciales);

				var JWTString = new JwtSecurityTokenHandler().WriteToken(Token);
				return Ok(JWTString);
			}
			catch
			{
				return Unauthorized();
			}
		}
	}
}