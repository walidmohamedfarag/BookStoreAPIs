
using System.Threading.Tasks;

namespace BookStoreAPIs.Areas.Identity.Controllers
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Identity")]
    [Authorize]
    public class ProfilesController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signinManager;

        public ProfilesController(UserManager<ApplicationUser> _userManager, SignInManager<ApplicationUser> _signinManager)
        {
            userManager = _userManager;
            signinManager = _signinManager;
        }

        [HttpPost("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileRequest updateProfile)
        {
            var user = await userManager.GetUserAsync(User);
            if (!updateProfile.FisrtName.IsNullOrEmpty())
                user!.FirstName = updateProfile.FisrtName;
            if (!updateProfile.LastName.IsNullOrEmpty())
                user!.LastName = updateProfile.LastName;
            if (!updateProfile.PhoneNumber.IsNullOrEmpty())
                user!.PhoneNumber = updateProfile.PhoneNumber;
            if (!updateProfile.Address.IsNullOrEmpty())
                user!.Address = updateProfile.Address;
            user!.UserName = $"{updateProfile.FisrtName}{updateProfile.LastName}";
            await userManager.UpdateAsync(user!);
            await signinManager.RefreshSignInAsync(user!);
            return NoContent();
        }
        [HttpPost("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordRequest updatePassword)
        {
            var user = await userManager.GetUserAsync(User);
            if (updatePassword.Password.IsNullOrEmpty() || updatePassword.NewPassword.IsNullOrEmpty())
            {
                return BadRequest(new { error = "Password fields cannot be null." });
            }
            var result = await userManager.ChangePasswordAsync(user!, updatePassword.Password, updatePassword.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return NoContent();
        }
    }
}
