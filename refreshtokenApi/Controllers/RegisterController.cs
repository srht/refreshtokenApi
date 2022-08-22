using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using refreshtokenApi.Models;
using refreshtokenApi.ViewModels;

namespace refreshtokenApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        public RegisterController(AppDbContext appDbContext, UserManager<AppUser> userManager, IConfiguration configuration)
        {
            UserManager = userManager;
        }

        public UserManager<AppUser> UserManager { get; }

        // POST api/<UsersController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ResponseViewModel()
                {
                    Message = "Model boş.",
                    StatusCode = 200
                });
            }

            var userCreated = await UserManager.CreateAsync(new AppUser
            {
                Email = model.Username,
                Name = model.FirstName,
                Surname = model.Surname,
                UserName = model.Username
            }, model.Password);

            if (userCreated.Succeeded)
            {
                return Ok(new ResponseViewModel()
                {
                    Message = "Kullanıcı oluşturuldu",
                    StatusCode = 200
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
