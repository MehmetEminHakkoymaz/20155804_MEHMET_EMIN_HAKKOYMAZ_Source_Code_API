namespace ECommerceAPI.Models
{
    public class Category
    {
        // Kategori kimlik numarasý
        public int Id { get; set; }
        
        // Kategori adý
        public string Name { get; set; } = string.Empty;
        
        // Kategori açýklamasý
        public string Description { get; set; } = string.Empty;
    }
}