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
                var tokenData= TokenHelper.GenerateToken(user, Configuration);
                return Ok(new LoginReponseViewModel()
                {
                    Message = "Model boş.",
                    StatusCode = 200,
                    Fullname =$"{user.Name} {user.Surname}",
                    Role=Roles.User.ToString(),
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
    }

}
