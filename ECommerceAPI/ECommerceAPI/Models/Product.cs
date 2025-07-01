namespace ECommerceAPI.Models
{
    public class Product
    {
        // Ürün kimlik numarasý
        public int Id { get; set; }
        
        // Ürün adý
        public string Name { get; set; } = string.Empty;
        
        // Ürün açýklamasý
        public string Description { get; set; } = string.Empty;
        
        // Ürün fiyatý
        public decimal Price { get; set; }
        
        // Ürünün baðlý olduðu kategori kimliði
        public int CategoryId { get; set; }
        
        // Ürün resmi için URL
        public string ImageUrl { get; set; } = string.Empty;
        
        // Mevcut stok miktarý
        public int StockQuantity { get; set; }
        
        // Opsiyonel indirim oraný
        public decimal? Discount { get; set; }
        
        // Ürün etiketleri
        public List<string> Tags { get; set; } = new List<string>();
    }
}