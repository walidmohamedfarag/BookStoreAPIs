using System.Threading.Tasks;

namespace BookStoreAPIs.Areas.Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Area("Identity")]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IEmailSender emailSender;
        private readonly IReposatory<OTPUser> otpUserRepo;
        private readonly SignInManager<ApplicationUser> signinManager;

        public AccountsController(UserManager<ApplicationUser> _userManager, IEmailSender _emailSender, IReposatory<OTPUser> _otpUserRepo , SignInManager<ApplicationUser> _signinManager)
        {
            userManager = _userManager;
            emailSender = _emailSender;
            otpUserRepo = _otpUserRepo;
            signinManager = _signinManager;
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            var ifEmailExsite = await userManager.FindByEmailAsync(registerRequest.Email);
            if (ifEmailExsite is not null)
                return BadRequest(new
                {
                    Error = $"{registerRequest.Email} is already exsite"
                });
            var user = new ApplicationUser
            {
                UserName = $"{registerRequest.FirstName}{registerRequest.LastName}",
                FirstName = registerRequest.FirstName,
                LastName = registerRequest.LastName,
                Email = registerRequest.Email,
            };
            var result = await userManager.CreateAsync(user, registerRequest.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(EmailConfirmation), "Accounts", new { area = "Identity", token, userId = user.Id }, Request.Scheme);
            await emailSender.SendEmailAsync(registerRequest.Email, " Email Confirmation From Book Shope", $"<h1> To Confirm Email Click <a href= '{link}'> Here </a></h1>");
            return Ok(new
            {
                Success = "Regisrter Successfully"
            });
        }
        [HttpGet("EmailConfirmation")]
        public async Task<IActionResult> EmailConfirmation(string token, string userId)
        {

            var user = await userManager.FindByIdAsync(userId);
            var result = await userManager.ConfirmEmailAsync(user!, token);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            return Ok(new
            {
                success = "Email confirmed successfully"
            });
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var user = await userManager.FindByEmailAsync(loginRequest.Email);
            if (user is null)
                return BadRequest(new
                {
                    error = "Invalid password or email"
                });
            var isPasswordValid = await userManager.CheckPasswordAsync(user, loginRequest.Password);
            if (!isPasswordValid)
                return BadRequest(new
                {
                    error = "Invalid password or email"
                });
            return Ok(new
            {
                success = "Login Successfully"
            });
        }
        [HttpGet("ResendEmailconfirmation")]
        public async Task<IActionResult> ResendEmailconfirmation(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
                return NotFound();
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(EmailConfirmation), "Accounts", new { area = "Identity", token, userId = user.Id } , Request.Scheme);
            await emailSender.SendEmailAsync(email, "Resend Email Confirmation from Book Shope", $"<h1>To Confirmation Email Click <a href='{link}'> Here </a></h1>");
            return Ok(new
            {
                Success = "Email Sent Successfully"
            });
        }
        [HttpGet("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
                return NotFound();
            var otpsUser = await otpUserRepo.GetAllAsync(o => o.UserId == user.Id && o.CreatedAt.Day == DateTime.Now.Day);
            if(otpsUser.Count() > 3)
                return BadRequest(new
                {
                    error = "You have exceeded the maximum number of OTP requests. Please try again later."
                });
            var otp = new OTPUser
            {
                OTP = new Random().Next(10000, 99999).ToString(),
                UserId = user.Id,
            };
            await otpUserRepo.AddAsync(otp);
            await otpUserRepo.CommitAsync();
            await emailSender.SendEmailAsync(email, "Send OTP For Reset Password", $"<h1>Your OTP: {otp.OTP}</h1>");
            return CreatedAtAction(nameof(ValidateToOTP) , new {userId = user.Id});
        }
        [HttpGet("ValidateToOTP")]
        public async Task<IActionResult> ValidateToOTP(string userId, string otp)
        {
            var otpInUser = await otpUserRepo.GetOneAsync(o => o.UserId == userId && o.OTP == otp);
            if (otpInUser is null)
                return BadRequest(new
                {
                    error = "Invalid OTP"
                });
            return Ok(new
            {
                success = "OTP Validated Successfully , Now You Can Reset Password"
            });
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPassword resetPassword)
        {
            var user = await userManager.FindByIdAsync(resetPassword.UserId);
            var token = await userManager.GeneratePasswordResetTokenAsync(user!);
            var result = await userManager.ResetPasswordAsync(user!, token, resetPassword.Password);
            if(!result.Succeeded)
                return BadRequest(result.Errors);
            return Ok(new
            {
                success = "Password Reset Successfully"
            });
        }
        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            await signinManager.SignOutAsync();
            return NoContent();
        }
    }
}
