namespace DaoNuHoangLyLy_2123110414.DTOs
{
    public class AuthResponseDTOs
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime ExpiresAtUtc { get; set; }
    }
}