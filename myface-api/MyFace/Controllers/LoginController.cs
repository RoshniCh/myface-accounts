using Microsoft.AspNetCore.Mvc;
// using MyFace.Models.Request;
using MyFace.Models.Response;
using MyFace.Repositories;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System; 
// using System.Security.Cryptography;
// using System.Web;
using Microsoft.AspNetCore.Http; 
using System.Text;

namespace MyFace.Controllers
{
  [ApiController]
  [Route("/login")]
  public class LoginController : ControllerBase
  {
    private readonly IUsersRepo _users;

    public LoginController(IUsersRepo users)
    {
        _users = users;
    }
    
    [HttpGet("")]
    public ActionResult Index()
    {
            string authHeader = Request.Headers["Authorization"];
            if (authHeader != null && authHeader.StartsWith("Basic")) 
            {
                string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                Encoding encoding = Encoding.GetEncoding("iso-8859-1");
                string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

                int seperatorIndex = usernamePassword.IndexOf(':');

                var username = usernamePassword.Substring(0, seperatorIndex);
                var password = usernamePassword.Substring(seperatorIndex + 1);
                
                var user = _users.GetByUserName(username);
                var correct_hashed_password=user.Hashed_Password;
        
                string hashed_password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: password,
                    salt: Convert.FromBase64String(user.Salt),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8));

                if (hashed_password == correct_hashed_password)
                {
                  return StatusCode(200);
                }  
                else
                {
                  return StatusCode(401);
                }
            } 
            else
            {
              return StatusCode(401);
            }
    }
  }  
}

