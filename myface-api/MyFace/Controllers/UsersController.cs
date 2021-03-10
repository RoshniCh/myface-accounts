using Microsoft.AspNetCore.Mvc;
using MyFace.Models.Request;
using MyFace.Models.Response;
using MyFace.Repositories;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System; 
using System.Security.Cryptography;
using System.Web;
using Microsoft.AspNetCore.Http; 
using System.Text;

namespace MyFace.Controllers
{
 

    [ApiController]
    [Route("/users")]
    public class UsersController : ControllerBase
    {
        public interface Authentication
        {
            bool AuthenticateUser(string authHeader);
        }

        class UserAuth : Authentication
        {  
            public bool AuthenticateUser(string authHeader)
            {
                // code to authernticate
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
                        bool validUser = false;
                    if (hashed_password == correct_hashed_password)
                        {
                        validUser = true;
                        }
                    else 
                    {
                        validUser = false;
                    }
                    return validUser;
                }
                else
                {
                    return (false);
                }
            }
        }
        private readonly IUsersRepo _users;
        

        public UsersController(IUsersRepo users)
        {
            _users = users;
        }
        UserAuth test = new UserAuth();
        [HttpGet("")]
        public ActionResult<UserListResponse> Search([FromQuery] UserSearchRequest searchRequest)
        {
            string authHeader = Request.Headers["Authorization"];
            bool validUser = test.AuthenticateUser(authHeader);

            if (validUser == true)
            {
                var users = _users.Search(searchRequest);
                var userCount = _users.Count(searchRequest);
                return UserListResponse.Create(searchRequest, users, userCount);
            }  
            else
            {
                return Unauthorized("Invalid authorisation");
                    // return BadRequest(ModelState);    
            }
        
            // else
            // {
            //     return Unauthorized("Invalid authorisation");
            //     // return BadRequest(ModelState);
            // }
                  
        }

        [HttpGet("{id}")]
        public ActionResult<UserResponse> GetById([FromRoute] int id)
        {
            var user = _users.GetById(id);
            return new UserResponse(user);
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] CreateUserRequest newUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _users.Create(newUser);
 
            var url = Url.Action("GetById", new { id = user.Id });
            var responseViewModel = new UserResponse(user);
           
            return Created(url, responseViewModel);
        }

        [HttpPatch("{id}/update")]
        public ActionResult<UserResponse> Update([FromRoute] int id, [FromBody] UpdateUserRequest update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _users.Update(id, update);
            return new UserResponse(user);
        }
        
        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            _users.Delete(id);
            return Ok();
        }
    }
}