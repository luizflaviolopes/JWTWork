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

namespace Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post(
            userpost usuario,
            [FromServices]SigningConfigurations signingConfigurations,
            [FromServices]TokenConfigurations tokenConfigurations)
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
            DateTime dataExpiracao = dataCriacao +
                TimeSpan.FromSeconds(tokenConfigurations.Seconds);

            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = tokenConfigurations.Issuer,
                Audience = tokenConfigurations.Audience,
                SigningCredentials = signingConfigurations.SigningCredentials,
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

        public IActionResult Get()
        {
            return Ok("deu certo");
        }

    }
}
