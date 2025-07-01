using ECommerceAPI.Models;

namespace ECommerceAPI.Services.Soap
{
    public class ProductSoapService : IProductSoapService
    {
        private readonly XmlProductService _xmlProductService;
        
        public ProductSoapService(XmlProductService xmlProductService)
        {
            _xmlProductService = xmlProductService;
        }
        
        public List<Product> GetAllProducts()
        {
            // XML web servisine yönlendirerek tüm ürünleri getir
            return _xmlProductService.GetAllProducts();
        }
        
        public Product GetProductById(int id)
        {
            // XML web servisine yönlendirerek ID'ye göre ürün getir
            return _xmlProductService.GetProductById(id);
        }
        
        public void AddProduct(Product product)
        {
            // XML web servisine yönlendirerek ürün ekle
            _xmlProductService.AddProduct(product);
        }
        
        public void UpdateProduct(Product product)
        {
            // XML web servisine yönlendirerek ürün güncelle
            _xmlProductService.UpdateProduct(product);
        }
        
        public bool DeleteProduct(int id)
        {
            // Ürünün var olup olmadýðýný kontrol et
            var product = _xmlProductService.GetProductById(id);
            if (product == null)
                return false;
                
            // XML web servisine yönlendirerek ürün sil
            _xmlProductService.DeleteProduct(id);
            return true;
        }
    }
}