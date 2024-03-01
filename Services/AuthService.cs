
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Project.Helpers;
using Project.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;


namespace Project.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMailingService _mailingService;
        private readonly JWT _jwt;
        private static Dictionary<string, string> _randomStringDictionary = new Dictionary<string, string>();
        public AuthService(UserManager<ApplicationUser> userManager, IOptions<JWT> jwt, IMailingService mailingService
            , RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _jwt = jwt.Value;
            _mailingService = mailingService;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public async Task<AuthModel> RegisterAsync(RegisterModel model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthModel { Message = "Email Is Already Registered !" };
            var User = new ApplicationUser
            {
                Email = model.Email,
                UserName = model.Email.Substring(0, model.Email.IndexOf('@')),
                PhoneNumber = model.phoneNumber
            };
            var result = await _userManager.CreateAsync(User, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description} ,";
                }
                return new AuthModel { Message = errors };
            }
            var jwtsecuritytoken = await CreateJwtToken(User, "register");
            var confirmationLink = GenerateConfirmationLink(model.Email);
            var Body = $"<p>Please confirm your email address by clicking the following link:</p><p><a href='{confirmationLink}'>Confirm Email</a></p>";
            await _mailingService.SendEmailAsync(model.Email, "Confirm Your Email", Body);
            return new AuthModel
            {
                Email = User.Email,
                PhoneNumber = User.PhoneNumber,
                ExpiresOn = jwtsecuritytoken.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { "User" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtsecuritytoken),
            };
        }
        public async Task<AuthModel> LoginAsync(LoginModel model)
        {
            var authModel = new AuthModel();
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authModel.Message = "Email or Password Is Incorrect!";
                return authModel;
            }
            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                var rolesList = await _userManager.GetRolesAsync(user);
                var JwtSecurityToken = await CreateJwtToken(user, "login");
                authModel.IsAuthenticated = true;
                authModel.ExpiresOn = JwtSecurityToken.ValidTo;
                authModel.Email = user.Email;
                authModel.Token = new JwtSecurityTokenHandler().WriteToken(JwtSecurityToken);
                authModel.Roles = rolesList.ToList();
            }
            else
                authModel.Message = "This Email Is Not Confirmed ! ";

            return authModel;
        }
        public async Task<string> ForgetPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return  "User not found." ;
            _randomStringDictionary.Add(email, GenerateRandomString(8));
            await _mailingService.SendEmailAsync(email, "Password reset request", $"<html><body><p style=\"font-size: 20px;\">Reset your password?\r\n</p>"+
                $"<p>If you requested a password reset for {user.UserName}," +
                $"<br>use the confirmation code below to complete the process." +
                $"<br>If you didn't make this request, ignore this email.</p>" +
                $"<h4>\r\n {_randomStringDictionary[email]}</h4>");
            return "Your Reset Password Code Sent to Your Email"; 
        }
        public async Task<AuthModel> ResetPasswordAsync(ResetPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (model.Code.Equals(_randomStringDictionary[model.Email]))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, model.Password);
                if (result.Succeeded)
                {
                    user.Address = string.Empty; 
                    return new AuthModel { Message = "Password Reset Successfully." };
                }
                else
                {
                    return new AuthModel { Message = "Error resetting password." };
                }
            }
            else
            {
                return new AuthModel { Message = "Invalid code !" };
            }
        }
        public async Task<string> AddRoleAsync(AddRoleModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user is null || !await _roleManager.RoleExistsAsync(model.Role))
                return "Invalid UserId Or Role";
            if (await _userManager.IsInRoleAsync(user, model.Role))
                return "User already Assigned to this Role";
            var result = await _userManager.AddToRoleAsync(user, model.Role);
            return result.Succeeded ? string.Empty : "Something Went Wrong";
        }
        public async Task<AuthModel> SendInvitationAsync(string email)
        {
            if (await _userManager.FindByEmailAsync(email) is not null)
                return new AuthModel { Message = "Email Is Already Registered !" };
            var User = new ApplicationUser
            {
                Email = email,
                UserName = email.Substring(0, email.IndexOf('@'))
            };
            var result = await _userManager.CreateAsync(User, email);
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description} ,";
                }
                return new AuthModel { Message = errors };
            }
            await _userManager.AddToRoleAsync(User, "User");
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(User);
            await _userManager.ConfirmEmailAsync(User, token);
            var JwtSecurityToken = await CreateJwtToken(User, "register");
            await _mailingService.SendEmailAsync(email, "Invitation For You", $"Your Email : {User.Email} And Your Password : {User.Email}");
            return new AuthModel
            {
                Email = User.Email,
                ExpiresOn = JwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { "User" },
                Token = new JwtSecurityTokenHandler().WriteToken(JwtSecurityToken),
            };
        }
        public async Task<AuthModel> ConfirmEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return new AuthModel { Message = "User not found." };
            if (user.EmailConfirmed)
                return new AuthModel { Message = "User Is Already Confirmed." };
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
                return new AuthModel { Message = "Error Confirming Email." };
            return new AuthModel { Message = "Email Confirmed." };
        }
        public async Task<AuthModel> EditProfileAsync(EditProfileModel model)
        {
            if (model.UserId == null)
                return new AuthModel { Message = "User Is Not Correct" };

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return new AuthModel { Message = $"Unable to load user with Id : {model.UserId}'." };
            user.ProfilePictureUrl = model.ProfilePictureUrl;
            var token = await _userManager.GenerateChangeEmailTokenAsync(user, model.NewEmail);
            var result = await _userManager.ChangeEmailAsync(user, model.NewEmail, token);
            var token1 = await _userManager.GenerateChangePhoneNumberTokenAsync(user, model.PhoneNumber);
            var result1 = await _userManager.ChangePhoneNumberAsync(user, model.PhoneNumber, token1);
            if (!result.Succeeded || !result1.Succeeded)
                return new AuthModel { Message = "Error changing email." };

            if (!string.IsNullOrWhiteSpace(model.UserName))
            {
                var setUserNameResult = await _userManager.SetUserNameAsync(user, model.UserName);
                if (!setUserNameResult.Succeeded)
                    return new AuthModel { Message = "Error changing user name." };
            }
            await _signInManager.RefreshSignInAsync(user);

            return new AuthModel { Message = "Thank you for confirming your email change." };
        }
        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user, string value)
        {
            double DurationInDays;
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            if (value.Equals("register") || string.IsNullOrEmpty(value))
            {
                DurationInDays = 14;
            }
            else if (value.Equals("login"))
            {
                DurationInDays = .3;
            }
            else
            {
                DurationInDays = 90;
            }
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(DurationInDays),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
        private string GenerateConfirmationLink(string email)
        {
            string ConfirmationLink = $"https://localhost:44382/api/auth/confirm-email?email={HttpUtility.UrlEncode(email)}";
            return ConfirmationLink;
        }
        private static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            StringBuilder result = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }
            return result.ToString();
        }
    }
}
