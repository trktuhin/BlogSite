namespace BlogSite.Shared.ViewModels
{
    public class AppUserViewModel
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Role { get; set; }
        public string? Token { get; set; }
        public int ExpiresIn { get; set; }
        public DateTime ExpiryTimeStamp { get; set; }
    }
}
