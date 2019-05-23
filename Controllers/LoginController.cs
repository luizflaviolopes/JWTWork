using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Back.Auth;
using System.Security.Claims;
using System.Security.Principal;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using System.Text;

namespace Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post(
            [FromForm]userpost usuario)
        {
            bool credenciaisValidas = false;
            //return new JsonResult("teste");

            //userpost user = usuario.ToObject<userpost>();

            if (usuario.Login != "LF" && usuario.Password != "teste")
                return Unauthorized();

            ClaimsIdentity identity = new ClaimsIdentity(
                new GenericIdentity("Luiz Flavio", "Login"),
                new[] {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new Claim(JwtRegisteredClaimNames.UniqueName, "LFUser")
                }
            );

            DateTime dataCriacao = DateTime.Now;
            DateTime dataExpiracao = dataCriacao + TimeSpan.FromSeconds(120);
            
            var keyAsBytes = Encoding.ASCII.GetBytes("TRON.TRON.TRON.TRON.");
            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = "TesteController",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyAsBytes), SecurityAlgorithms.HmacSha256),
                Subject = identity,
                NotBefore = dataCriacao,
                Expires = dataExpiracao,
            });

            var token = handler.WriteToken(securityToken);

            return new JsonResult(new
            {
                authenticated = true,
                created = dataCriacao.ToString("yyyy-MM-dd HH:mm:ss"),
                expiration = dataExpiracao.ToString("yyyy-MM-dd HH:mm:ss"),
                accessToken = token,
                message = "OK"
            });
        }

        public class userpost
        {
            public string Login { get; set; }
            public string Password { get; set; }

        }

        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("deu certo");
        }

    }
}
