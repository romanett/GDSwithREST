using GDSwithREST.Data.Models.ApiModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GDSwithREST.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IConfiguration _configuration;
        public LoginController(IConfiguration Configuration)
        {
            _configuration = Configuration;
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Login([FromBody] LoginApiModel login)
        {
            try
            {
                if (string.IsNullOrEmpty(login.UserName) ||
                    string.IsNullOrEmpty(login.Password))
                    return BadRequest("Username and/or Password not specified");
                if (login.UserName.Equals("appadmin") &&
                    login.Password.Equals("demo"))
                {
                    var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                    var tokeOptions = new JwtSecurityToken(
                        issuer: _configuration["Jwt:Issuer"], 
                        audience: _configuration["Jwt:Audience"], 
                        claims: new List<Claim>(), 
                        expires: DateTime.Now.AddMinutes(20), 
                        signingCredentials: signinCredentials);
                    var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                    return Ok(tokenString);
                }
            }
            catch
            {
                return BadRequest
                ("An error occurred in generating the token");
            }
            return Unauthorized();
        }
    }
}
