namespace ECommerceAPI.Models
{
    public class User
    {
        // Kullanýcý kimlik numarasý
        public int Id { get; set; }
        
        // Kullanýcý adý
        public string Username { get; set; } = string.Empty;
        
        // Kullanýcý e-posta adresi
        public string Email { get; set; } = string.Empty;
        
        // Þifrenin hash deðeri
        public string PasswordHash { get; set; } = string.Empty;
        
        // Kullanýcý rolü (Admin, User vb.)
        public string Role { get; set; } = string.Empty;
    }
}