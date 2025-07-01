namespace ECommerceAPI.Models
{
    public class Product
    {
        // �r�n kimlik numaras�
        public int Id { get; set; }
        
        // �r�n ad�
        public string Name { get; set; } = string.Empty;
        
        // �r�n a��klamas�
        public string Description { get; set; } = string.Empty;
        
        // �r�n fiyat�
        public decimal Price { get; set; }
        
        // �r�n�n ba�l� oldu�u kategori kimli�i
        public int CategoryId { get; set; }
        
        // �r�n resmi i�in URL
        public string ImageUrl { get; set; } = string.Empty;
        
        // Mevcut stok miktar�
        public int StockQuantity { get; set; }
        
        // Opsiyonel indirim oran�
        public decimal? Discount { get; set; }
        
        // �r�n etiketleri
        public List<string> Tags { get; set; } = new List<string>();
    }
}