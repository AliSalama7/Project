using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class EditProfileModel
    {
        [EmailAddress]
        public string OldEmail { get; set; }
        [EmailAddress]
        public string NewEmail { get; set; }
        public string UserName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address {  get; set; }
        public DateTime? DateOfBirth {  get; set; }
        public string? ProfilePictureUrl {  get; set; }
    }
}
