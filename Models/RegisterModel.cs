using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class RegisterModel
    {
        [Required , StringLength(50)]
        public string UserName { get; set; }
        [Required , StringLength(50)]
        public string Email { get; set; }
        [Required , StringLength(28)]
        public string Password { get; set; }
    }
}
