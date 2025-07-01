using System.Net.Http.Headers;
using System.Xml;
using System.Xml.Serialization;
using ECommerceAPI.Models;

namespace ECommerceAPI.Services
{
    public class ExternalServiceClient
    {
        private readonly HttpClient _httpClient;
        
        public ExternalServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
        }
        
        // Bu metot örnek bir dýþ XML API'yi tüketir
        public async Task<List<Product>> GetExternalProductsAsync(string apiUrl)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();
                
                string xmlContent = await response.Content.ReadAsStringAsync();
                
                // XML'i ayrýþtýr
                XmlSerializer serializer = new XmlSerializer(typeof(List<Product>), new XmlRootAttribute("Products"));
                using StringReader reader = new StringReader(xmlContent);
                return (List<Product>)serializer.Deserialize(reader);
            }
            catch (HttpRequestException ex)
            {
                // HTTP hata yönetimi
                throw new Exception($"API isteði baþarýsýz: {ex.Message}", ex);
            }
            catch (XmlException ex)
            {
                // XML ayrýþtýrma hata yönetimi
                throw new Exception($"XML ayrýþtýrma hatasý: {ex.Message}", ex);
            }
        }
        
        // Mock dýþ servis çaðrýsý - gerçek API olmadan test için
        public List<Product> GetMockExternalProducts()
        {
            // XML string direkt burada oluþturuldu - normalde API'den gelirdi
            string xmlData = @"<?xml version=""1.0"" encoding=""utf-8""?>
                <Products>
                    <Product>
                        <Id>101</Id>
                        <Name>Dýþ API Ürünü 1</Name>
                        <Description>Dýþ servisten alýnan ürün</Description>
                        <Price>299.99</Price>
                        <CategoryId>5</CategoryId>
                        <ImageUrl>/images/external-1.jpg</ImageUrl>
                        <StockQuantity>20</StockQuantity>
                    </Product>
                    <Product>
                        <Id>102</Id>
                        <Name>Dýþ API Ürünü 2</Name>
                        <Description>Dýþ servisten alýnan baþka ürün</Description>
                        <Price>199.50</Price>
                        <CategoryId>3</CategoryId>
                        <ImageUrl>/images/external-2.jpg</ImageUrl>
                        <StockQuantity>15</StockQuantity>
                    </Product>
                </Products>";
            
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Product>), new XmlRootAttribute("Products"));
                using StringReader reader = new StringReader(xmlData);
                return (List<Product>)serializer.Deserialize(reader);
            }
            catch (Exception ex)
            {
                throw new Exception($"Mock XML ayrýþtýrma hatasý: {ex.Message}", ex);
            }
        }
    }
}