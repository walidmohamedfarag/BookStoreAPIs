
namespace BookStoreAPIs.Areas.Identity.Controllers
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Identity")]
    public class ProfilesController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signinManager;

        public ProfilesController(UserManager<ApplicationUser> _userManager , SignInManager<ApplicationUser> _signinManager)
        {
            userManager = _userManager;
            signinManager = _signinManager;
        }

        [HttpPost("UpdateProfile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile(UpdateProfileRequest updateProfile)
        {
            var user = await userManager.GetUserAsync(User);
            if(updateProfile.FisrtName != default(string))
                user!.FirstName = updateProfile.FisrtName;
            if (updateProfile.LastName != default(string))
                user!.LastName = updateProfile.LastName;
            if (updateProfile.PhoneNumber != default(string))
                user!.PhoneNumber = updateProfile.PhoneNumber;
            if (updateProfile.Address != default(string))
                user!.Address = updateProfile.Address;
            user!.UserName = $"{updateProfile.FisrtName}{updateProfile.LastName}";
            if (updateProfile.Password != default(string) && updateProfile.NewPassword != default(string))
            {
                var result = await userManager.ChangePasswordAsync(user!, updateProfile.Password, updateProfile.NewPassword);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
            }
            await userManager.UpdateAsync(user!);
            await signinManager.RefreshSignInAsync(user!);
            return NoContent();
        }
    }
}
