using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using refreshtokenApi.Helpers;
using refreshtokenApi.Models;
using refreshtokenApi.ViewModels;

namespace refreshtokenApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        public LoginController(AppDbContext appDbContext,SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IConfiguration configuration)
        {
            AppDbContext = appDbContext;
            SignInManager = signInManager;
            UserManager = userManager;
            Configuration = configuration;
        }

        public AppDbContext AppDbContext { get; }
        public SignInManager<AppUser> SignInManager { get; }
        public UserManager<AppUser> UserManager { get; }
        public IConfiguration Configuration { get; }

        // POST api/<UsersController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ResponseViewModel() {
                   Message="Model boş.",
                   StatusCode=200
                });
            }

            var signInResult= await SignInManager.PasswordSignInAsync(model.Username, model.Password, false, false);
            if (signInResult.Succeeded)
            {
                var user=await UserManager.FindByEmailAsync(model.Username);
                var roles= await UserManager.GetRolesAsync(user);
                string userRole = roles.Any() ? roles[0] : Roles.Administrator.ToString();
                var tokenData= TokenHelper.GenerateToken(user, userRole, Configuration);
                string refreshToken = TokenHelper.CreateRefreshToken();
                DateTime refreshTokenExpireDate = DateTime.Now.AddDays(7);
                if(user.RefreshTokens==null)
                user.RefreshTokens = new List<RefreshToken>();
                user.RefreshTokens.Add(new RefreshToken
                {
                    Token = refreshToken,
                    ExpireDate = refreshTokenExpireDate,
                    Created = DateTime.Now,
                    CreatedByIp = TokenHelper.ipAddress(Request, HttpContext)
                });
                tokenData.RefreshToken = refreshToken;
                tokenData.RefreshTokenExpireDate = refreshTokenExpireDate;
                await UserManager.UpdateAsync(user);
                return Ok(new LoginReponseViewModel()
                {
                    Message = "Model var.",
                    StatusCode = 200,
                    Fullname =$"{user.Name} {user.Surname}",
                    Role = roles.Any()?Enum.Parse<RoleView>(roles[0]):RoleView.Administrator,
                    TokenResponse=tokenData,
                    Username=user.UserName
                });
            }
            else
            {
                return Ok(new ResponseViewModel()
                {
                    Message = "Kullanıcı bulunamadı.",
                    StatusCode = 404
                });
            }
        }

        private void setTokenCookie(string token)
        {
            // append cookie with refresh token to the http response
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("refreshToken2", token, cookieOptions);
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }
    }

}
