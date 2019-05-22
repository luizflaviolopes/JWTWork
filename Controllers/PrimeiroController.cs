using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Back.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class PrimeiroController : ControllerBase
    {

        string key = "TRON";

        // GET api/values
        [HttpPost]
        public IActionResult Post([FromForm]string login, [FromForm]string password)
        {
            
            if(login != "LF" && password != "teste")
            return Unauthorized();

            
            var keyBytes = System.Text.Encoding.UTF8.GetBytes(key);

            string header = @"{""typ"":""JWT"",""alg"":""HS256""}";
            string payload = @"{""sub"":""123456789"",""name"":""Luiz Flavio""}";

            var headerBytes = System.Text.Encoding.UTF8.GetBytes(header);
            var payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload);

            var header64 = System.Convert.ToBase64String(headerBytes).Replace('+', '-')
      .Replace('/', '_')
      .Replace("=", "")
      ;
            var payload64 = System.Convert.ToBase64String(payloadBytes).Replace('+', '-')
      .Replace('/', '_')
      .Replace("=", "");

            var messageBytes = System.Text.Encoding.UTF8.GetBytes(header64+"."+payload64);

            System.Security.Cryptography.HMACSHA256 crypto = new System.Security.Cryptography.HMACSHA256(keyBytes);

            var signBytes = crypto.ComputeHash(messageBytes);
            var sign64 = System.Convert.ToBase64String(signBytes).Replace('+', '-')
      .Replace('/', '_')
      .Replace("=", "");

            return Ok(header64+"."+payload64+"."+sign64);

        }

        public IActionResult Get(string token){
            

            string[] parts = token.Split('.');

            var keyBytes = System.Text.Encoding.UTF8.GetBytes(key);
            System.Security.Cryptography.HMACSHA256 crypto = new System.Security.Cryptography.HMACSHA256(keyBytes);


            var messageBytes = System.Text.Encoding.UTF8.GetBytes(parts[0]+"."+parts[1]);
            var signBytes = crypto.ComputeHash(messageBytes);
            var sign64 = System.Convert.ToBase64String(signBytes).Replace('+', '-').Replace('/', '_')
      .Replace("=", "");

            if(sign64 != parts[2])
            return Unauthorized();
            
            

            var headCorrection= parts[0].Replace('-', '+').Replace('_', '/');
                switch(headCorrection.Length % 4) {
                case 2: headCorrection += "=="; break;
                case 3: headCorrection += "="; break;
            }

            var payloadCorrection= parts[1].Replace('-', '+').Replace('_', '/');
                switch(payloadCorrection.Length % 4) {
                case 2: payloadCorrection += "=="; break;
                case 3: payloadCorrection += "="; break;
            }

            var headBytes = System.Convert.FromBase64String(headCorrection);
            var head = System.Text.Encoding.UTF8.GetString(headBytes);

            var payloadBytes = System.Convert.FromBase64String(payloadCorrection);
            var payload = System.Text.Encoding.UTF8.GetString(payloadBytes);


            return Ok(payload);

            

        }
    }
}
