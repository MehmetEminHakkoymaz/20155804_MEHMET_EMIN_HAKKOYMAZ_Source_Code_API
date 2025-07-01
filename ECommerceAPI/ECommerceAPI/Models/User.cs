namespace ECommerceAPI.Models
{
    public class User
    {
        // Kullan�c� kimlik numaras�
        public int Id { get; set; }
        
        // Kullan�c� ad�
        public string Username { get; set; } = string.Empty;
        
        // Kullan�c� e-posta adresi
        public string Email { get; set; } = string.Empty;
        
        // �ifrenin hash de�eri
        public string PasswordHash { get; set; } = string.Empty;
        
        // Kullan�c� rol� (Admin, User vb.)
        public string Role { get; set; } = string.Empty;
    }
}