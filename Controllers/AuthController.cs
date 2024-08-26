using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MosadAPIServer.Models;
using MosadAPIServer.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MosadAPIServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly MosadDbContext _context;

        public AuthController(MosadDbContext context)
        {
            this._context = context;
        }


        private string GenerateToken(string userIP)
        {
            // token handler can create token
            var tokenHandler = new JwtSecurityTokenHandler();

            string secretKey = "1234dyi5fjthgjdndfadsfgdsjfgj464twiyyd5ntyhgkdrue74hsf5ytsusefh55678"; //TODO: remove this from code
            byte[] key = Encoding.ASCII.GetBytes(secretKey);

            // token descriptor describe HOW to create the token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // things to include in the token
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                    new Claim(ClaimTypes.Name, userIP),
                    }
                ),
                // expiration time of the token
                Expires = DateTime.UtcNow.AddMinutes(1),
                // the secret key of the token
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                    )
            };

            // creating the token
            var token = tokenHandler.CreateToken(tokenDescriptor);
            // converting the token to string
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserDetails userDetails)
        {
            int status = StatusCodes.Status200OK;

            await _context.SaveChangesAsync();

            if (CheckeDataInDB(userDetails.Username))
            {

                // getting the user (requester) IP
                string userIP = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();

                return StatusCode(200
                    , new { token = GenerateToken(userIP) }
                    );
            }
            return StatusCode(StatusCodes.Status401Unauthorized,
                    new { error = "invalid credentials" });
        }
        public bool CheckeDataInDB(string username)
        {
            var user = _context.users.FirstOrDefault(u => u.Username == username);
            if (user == null) { return false; }
            return true;
        }
        //[HttpPost]
        //public async Task<IActionResult> CreateUser(UserDetails user)
        //{
        //    int status = StatusCodes.Status200OK;
        //    var allUsers = await this._context.users.ToListAsync();
        //    _context.users.Add(user);
        //    await _context.SaveChangesAsync();

        //    return StatusCode(200
        //        , new { message = user }

        //        );
        //}
    }
}
