using BookStoreAPIs.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreAPIs.Areas.Identity.Controllers
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Identity")]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IEmailSender emailSender;
        private readonly IReposatory<OTPUser> otpUserRepo;
        private readonly SignInManager<ApplicationUser> signinManager;
        private readonly ITokenService tokenService;

        public AccountsController(UserManager<ApplicationUser> _userManager, IEmailSender _emailSender, IReposatory<OTPUser> _otpUserRepo, SignInManager<ApplicationUser> _signinManager, ITokenService _tokenService)
        {
            userManager = _userManager;
            emailSender = _emailSender;
            otpUserRepo = _otpUserRepo;
            signinManager = _signinManager;
            tokenService = _tokenService;
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
            // Get User Roles
            var userRoles = await userManager.GetRolesAsync(user);
            // Create Claims for Token
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier , user.Id),
                new Claim(ClaimTypes.Email , user.Email!),
                new Claim(ClaimTypes.NameIdentifier , user.UserName!),
                new Claim(ClaimTypes.Role , string.Join(", " , userRoles)),
                new Claim(JwtRegisteredClaimNames.Jti , Guid.NewGuid().ToString()),
            };
            // Generate Access Token
            var accessToken = tokenService.GetAccessToken(claims);
            // Generate Refresh Token
            var refreshToken = tokenService.GetRefreshToken();
            // Save Refresh Token in Database
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await userManager.UpdateAsync(user);
            return Ok(new
            {
                success = "Login Successfully",
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }
        [HttpGet("ResendEmailconfirmation")]
        public async Task<IActionResult> ResendEmailconfirmation(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
                return NotFound();
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(EmailConfirmation), "Accounts", new { area = "Identity", token, userId = user.Id }, Request.Scheme);
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
            if (otpsUser.Count() > 3)
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
            return CreatedAtAction(nameof(ValidateToOTP), new { userId = user.Id });
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
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            return Ok(new
            {
                success = "Password Reset Successfully"
            });
        }
        [HttpPost("Refresh")]
        public async Task<IActionResult> Refresh(TokenApiRequest tokenApiRequest)
        {
            if (tokenApiRequest is null || tokenApiRequest.RefreshToken is null || tokenApiRequest.AccessToken is null)
                return BadRequest(new { error = "invalid client request" });
            var claims = tokenService.ExtractClimFromToken(tokenApiRequest.AccessToken);
            var userName = claims.Identity!.Name;
            var user = userManager.Users.FirstOrDefault(u => u.UserName == userName);
            if (user is null || user.RefreshToken is null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return BadRequest(new { error = "invalid client request" });
            var accessToken = tokenService.GetAccessToken(claims.Claims);
            var refreshToken = tokenService.GetRefreshToken();
            user.RefreshToken = refreshToken;
            await userManager.UpdateAsync(user);
            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }
        [HttpPost, Authorize]
        [Route("Revok")]
        public async Task<IActionResult> Revok()
        {
            var userName = User.Identity.Name;
            var user = userManager.Users.FirstOrDefault(u => u.UserName == userName);
            if (user is null) return BadRequest(new { error = "user not found" });
            user.RefreshToken = null;
            await userManager.UpdateAsync(user);
            return NoContent();
        }
    }
}
