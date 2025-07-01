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
            // XML web servisine y�nlendirerek t�m �r�nleri getir
            return _xmlProductService.GetAllProducts();
        }
        
        public Product GetProductById(int id)
        {
            // XML web servisine y�nlendirerek ID'ye g�re �r�n getir
            return _xmlProductService.GetProductById(id);
        }
        
        public void AddProduct(Product product)
        {
            // XML web servisine y�nlendirerek �r�n ekle
            _xmlProductService.AddProduct(product);
        }
        
        public void UpdateProduct(Product product)
        {
            // XML web servisine y�nlendirerek �r�n g�ncelle
            _xmlProductService.UpdateProduct(product);
        }
        
        public bool DeleteProduct(int id)
        {
            // �r�n�n var olup olmad���n� kontrol et
            var product = _xmlProductService.GetProductById(id);
            if (product == null)
                return false;
                
            // XML web servisine y�nlendirerek �r�n sil
            _xmlProductService.DeleteProduct(id);
            return true;
        }
    }
}