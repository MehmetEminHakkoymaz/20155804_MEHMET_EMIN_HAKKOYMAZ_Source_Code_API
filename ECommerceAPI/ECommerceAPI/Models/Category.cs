namespace ECommerceAPI.Models
{
    public class Category
    {
        // Kategori kimlik numaras�
        public int Id { get; set; }
        
        // Kategori ad�
        public string Name { get; set; } = string.Empty;
        
        // Kategori a��klamas�
        public string Description { get; set; } = string.Empty;
    }
}