
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Models;
using Project.Services;

namespace Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authService.RegisterAsync(model);
            if (!result.IsAuthenticated)
                return BadRequest(result.Message);
            return Ok("Check Your Email For Confirmation");
        }

        [HttpPost("Login")]
        public async Task<IActionResult> GetTokenAsync([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authService.LoginAsync(model);
            if (!result.IsAuthenticated)
                return BadRequest(result.Message);
            return Ok(new { token = result.Token, expireson = result.ExpiresOn , UserName = result.UserName
                ,Email = result.Email , PhoneNumber = result.PhoneNumber,ProfilePicture = result.ProfilePictureUrl,
                Address = result.Address, DateOfBirth = result.DateOfBirth,Roles = result.Roles });
        }
        [HttpGet("Confirm-Email")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ConfirmEmail(string Email)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authService.ConfirmEmailAsync(Email);
            if (!result.IsAuthenticated)
                return BadRequest(result.Message);
            return Ok(result.Message);
        }
        [Authorize(Roles = "Master")]
        [HttpPost("AddRole")]
        public async Task<IActionResult> AddRoleAsync(AddRoleModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authService.AddRoleAsync(model);
            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);
            return Ok(model);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("SendInvitation")]
        public async Task<IActionResult> SendInvitationAsync(string ToEmail)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authService.SendInvitationAsync(ToEmail);
            return Ok("Invitation Sent Successfully");
        }
        [Authorize(Roles = "User")]
        [HttpPost("EditProfile")]
        public async Task<IActionResult> EditProfileAsync(EditProfileModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authService.EditProfileAsync(model);
            return Ok(new { Message = result.Message });
        }
        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPasswordAsync(string email)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authService.ForgetPasswordAsync(email);
            return Ok(result);
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authService.ResetPasswordAsync(model);
            return Ok(new { Message = result.Message });
        }

    }
}
