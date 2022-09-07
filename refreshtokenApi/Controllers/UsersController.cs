using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using refreshtokenApi.Helpers;
using refreshtokenApi.Models;
using refreshtokenApi.ViewModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace refreshtokenApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public AppDbContext AppDbContext { get; }
        public UserManager<AppUser> UserManager { get; }
        public IConfiguration Configuration { get; }

        public UsersController(AppDbContext appDbContext, UserManager<AppUser> userManager, IConfiguration configuration)
        {
            AppDbContext = appDbContext;
            UserManager = userManager;
            Configuration = configuration;
        }
        [HttpPost("")]
        public void Post([FromBody] string value)
        {
        }
        // GET: api/<UsersController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<UsersController>/5
        [Authorize(Roles ="Administrator")]
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return $"value {id}";
        }

        // POST api/<UsersController>
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest refreshToken)
        {
            //var refreshToken = Request.Cookies["refreshToken"];
            var user= UserManager.Users.FirstOrDefault(i => i.RefreshTokens!=null && i.RefreshTokens.Any(j => j.Token == refreshToken.RefreshToken));
            var roles = await UserManager.GetRolesAsync(user);
            string userRole = roles.Any() ? roles[0] : "Administrator";
            if (user == null)
                throw new Exception("Invalid token");

            var tokenReponse= TokenHelper.GenerateToken(user, userRole, Configuration);
            
            user.TokenData = new TokenModel
            {
                AccessToken =tokenReponse.AccessToken,
                AccessTokenExpireDate=tokenReponse.AccessTokenExpireDate
            };

            await UserManager.UpdateAsync(user);
            return Ok(new LoginReponseViewModel()
            {
                Message = "Model var.",
                StatusCode = 200,
                Fullname = $"{user.Name} {user.Surname}",
                Role = (RoleView) Roles.Administrator,
                TokenResponse = tokenReponse,
                Username = user.UserName
            });

        }

        [HttpPost("revoke")]
        public async Task<IActionResult> RevokeToken(RefreshTokenRequest refreshToken)
        {
            // accept token from request body or cookie
            var token = refreshToken.RefreshToken;

            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required" });

            var user = AppDbContext.Users.Include("RefreshTokens").SingleOrDefault(u => u.RefreshTokens!=null && u.RefreshTokens.Any(t => t.Token == token));

            if(user==null)
                return BadRequest(new { message = "User not found" });

            var foundRefreshToken = user.RefreshTokens.Single(x => x.Token == token);

            // revoke token and save
            foundRefreshToken.Revoked = DateTime.UtcNow;
            foundRefreshToken.RevokedByIp = "";
            await UserManager.UpdateAsync(user);


            if (foundRefreshToken==null)
                return NotFound(new { message = "Token not found" });

            return Ok(new { message = "Token revoked" });
        }


        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            
        }
    }
}
