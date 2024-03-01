using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class EditProfileModel
    {
        public string UserId { get; set; }
        [EmailAddress]
        public string NewEmail { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string? ProfilePictureUrl {  get; set; }
    }
}
