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
            userpost usuario)
        {
            bool credenciaisValidas = false;
            //return new JsonResult("teste");

            //userpost user = usuario.ToObject<userpost>();

            if ((usuario.Login != "LF" && usuario.Login != "Adm") && usuario.Password != "teste")
                return Unauthorized();

            ClaimsIdentity identity = new ClaimsIdentity(
                new GenericIdentity("Luiz Flavio", "Login"),
                new[] {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new Claim(JwtRegisteredClaimNames.UniqueName, "LFUser"),
                }
            );

            if(usuario.Login == "Adm")
            identity.AddClaim(new Claim(ClaimTypes.Role, "admin"));

            DateTime dataCriacao = DateTime.Now;
            DateTime dataExpiracao = dataCriacao.AddDays(1);
            
            var keyAsBytes = Encoding.ASCII.GetBytes("TRON.TRON.TRON.TRON.");
            var handler = new JwtSecurityTokenHandler();

            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyAsBytes), SecurityAlgorithms.HmacSha256Signature),
                Expires = dataExpiracao,
                Issuer = "TesteJWT",
                Subject = identity,
                

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
            return Ok("Foi");
        }

        [Authorize(Roles="admin")]
        [HttpGet]
        [Route("admin")]
        public IActionResult adm()
        {
            return Ok("Fala Adm!!");
        }

    }
}
