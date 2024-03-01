using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class ResetPasswordModel
    {
        [Required]
        public string Code { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required, StringLength(28)]
        public string Password { get; set; }

        [Required, StringLength(28)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
