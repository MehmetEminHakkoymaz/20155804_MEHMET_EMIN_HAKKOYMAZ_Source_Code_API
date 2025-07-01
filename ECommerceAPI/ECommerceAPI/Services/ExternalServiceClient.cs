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
        
        // Bu metot �rnek bir d�� XML API'yi t�ketir
        public async Task<List<Product>> GetExternalProductsAsync(string apiUrl)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();
                
                string xmlContent = await response.Content.ReadAsStringAsync();
                
                // XML'i ayr��t�r
                XmlSerializer serializer = new XmlSerializer(typeof(List<Product>), new XmlRootAttribute("Products"));
                using StringReader reader = new StringReader(xmlContent);
                return (List<Product>)serializer.Deserialize(reader);
            }
            catch (HttpRequestException ex)
            {
                // HTTP hata y�netimi
                throw new Exception($"API iste�i ba�ar�s�z: {ex.Message}", ex);
            }
            catch (XmlException ex)
            {
                // XML ayr��t�rma hata y�netimi
                throw new Exception($"XML ayr��t�rma hatas�: {ex.Message}", ex);
            }
        }
        
        // Mock d�� servis �a�r�s� - ger�ek API olmadan test i�in
        public List<Product> GetMockExternalProducts()
        {
            // XML string direkt burada olu�turuldu - normalde API'den gelirdi
            string xmlData = @"<?xml version=""1.0"" encoding=""utf-8""?>
                <Products>
                    <Product>
                        <Id>101</Id>
                        <Name>D�� API �r�n� 1</Name>
                        <Description>D�� servisten al�nan �r�n</Description>
                        <Price>299.99</Price>
                        <CategoryId>5</CategoryId>
                        <ImageUrl>/images/external-1.jpg</ImageUrl>
                        <StockQuantity>20</StockQuantity>
                    </Product>
                    <Product>
                        <Id>102</Id>
                        <Name>D�� API �r�n� 2</Name>
                        <Description>D�� servisten al�nan ba�ka �r�n</Description>
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
                throw new Exception($"Mock XML ayr��t�rma hatas�: {ex.Message}", ex);
            }
        }
    }
}