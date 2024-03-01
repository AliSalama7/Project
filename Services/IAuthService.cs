using Project.Models;

namespace Project.Services
{
    public interface IAuthService
    {
        Task<AuthModel> RegisterAsync(RegisterModel model);
        Task<AuthModel> LoginAsync(LoginModel model);
        Task<AuthModel> ConfirmEmailAsync(string email);
        Task<AuthModel> ResetPasswordAsync(ResetPasswordModel model);
        Task<string> ForgetPasswordAsync(string email);
        Task<AuthModel> SendInvitationAsync(string email);
        Task<AuthModel> EditProfileAsync(EditProfileModel model);
        Task<string> AddRoleAsync(AddRoleModel model);
    }
}

