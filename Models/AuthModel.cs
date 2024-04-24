namespace Project.Models
{
    public class AuthModel
    {
        public string Message { get; set; }
        public string UserName { get; set; }
        public bool IsAuthenticated { get; set; }
        public string? ProfilePictureUrl {  get; set; }
        public string Email {  get; set; }
        public string PhoneNumber {  get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public List<string> Roles { get; set; }
        public string Token {  get; set; }
        public DateTime ExpiresOn { get; set; }

    }
}
