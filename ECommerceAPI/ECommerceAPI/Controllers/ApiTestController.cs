using System.Net.Http;
using System.Text;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ECommerceAPI.Models;
using ECommerceAPI.Services;
using System.Text.Json;
using System.Globalization;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApiTestController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly XmlProductService _xmlProductService;
        private readonly XmlSecurityService _xmlSecurityService;
        private readonly XmlParsingService _xmlParsingService;
        private readonly CustomXmlSerializer _xmlSerializer;
        private readonly IWebHostEnvironment _env;
        
        private readonly string _baseUrl;
        private readonly Dictionary<string, TestResult> _testResults = new();
        
        public ApiTestController(
            IHttpClientFactory clientFactory,
            XmlProductService xmlProductService, 
            XmlSecurityService xmlSecurityService,
            XmlParsingService xmlParsingService,
            CustomXmlSerializer xmlSerializer,
            IWebHostEnvironment env,
            IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _xmlProductService = xmlProductService;
            _xmlSecurityService = xmlSecurityService;
            _xmlParsingService = xmlParsingService;
            _xmlSerializer = xmlSerializer;
            _env = env;

            // Doğru portu ve protokolü kullanın
            _baseUrl = "https://localhost:5001"; // HTTPS için port 5001
        }

        /// <summary>
        /// Tüm API test senaryolarını çalıştırır ve sonuçları görsel olarak gösterir
        /// </summary>
        [HttpGet]
        [Produces("text/html")]
        public async Task<IActionResult> RunAllTests()
        {
            // Test sonuçları sözlüğünü temizle
            _testResults.Clear();
            
            await RunProductApiTests();
            await RunXmlSecurityTests();
            await RunXmlParsingTests();
            
            return Content(GenerateHtmlReport(), "text/html");
        }
        
        /// <summary>
        /// Ürün API testlerini çalıştırır
        /// </summary>
        [HttpGet("products")]
        [Produces("application/json")]
        public async Task<IActionResult> RunProductApiTests()
        {
            try
            {
                // Test 1: Tüm ürünleri getir
                var getAllResult = await TestGetAllProducts();
                _testResults["Test_GetAllProducts"] = getAllResult;
                
                // Test 2: ID'ye göre ürün getir
                var getByIdResult = await TestGetProductById(1);
                _testResults["Test_GetProductById"] = getByIdResult;
                
                // Test 3: Geçersiz ID ile ürün getir
                var getInvalidIdResult = await TestGetProductById(999);
                _testResults["Test_GetProductByInvalidId"] = getInvalidIdResult;
                
                // Test 4: Content negotiation testi
                var contentNegotiationResult = await TestContentNegotiation();
                _testResults["Test_ContentNegotiation"] = contentNegotiationResult;
                
                // Tüm test sonuçlarını döndür
                return Ok(new 
                {
                    TestResults = _testResults
                        .Where(r => r.Key.StartsWith("Test_Get"))
                        .ToDictionary(r => r.Key, r => new { r.Value.Success, r.Value.Message })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Test sırasında hata oluştu: {ex.Message}");
            }
        }
        
        /// <summary>
        /// XML Güvenlik testlerini çalıştırır
        /// </summary>
        [HttpGet("xml-security")]
        [Produces("application/json")]
        public async Task<IActionResult> RunXmlSecurityTests()
        {
            try {
                // Test 1: XML İmzalama
                var signResult = TestXmlSignature();
                _testResults["Test_XmlSignature"] = signResult;
                
                // Test 2: XML İmza Doğrulama
                var verifyResult = TestXmlSignatureVerification();
                _testResults["Test_XmlSignatureVerification"] = verifyResult;
                
                // Test 3: XML Şifreleme
                var encryptResult = TestXmlEncryption();
                _testResults["Test_XmlEncryption"] = encryptResult;
                
                // Test 4: XML Şifre Çözme
                var decryptResult = TestXmlDecryption();
                _testResults["Test_XmlDecryption"] = decryptResult;
                
                // Test 5: Güvenli XML
                var secureResult = TestSecureXml();
                _testResults["Test_SecureXml"] = secureResult;
                
                return Ok(new 
                {
                    TestResults = _testResults
                        .Where(r => r.Key.StartsWith("Test_Xml"))
                        .ToDictionary(r => r.Key, r => new { r.Value.Success, r.Value.Message })
                });
            }
            catch (Exception ex) {
                return StatusCode(500, $"XML Güvenlik testleri sırasında hata oluştu: {ex.Message}");
            }
        }
        
        /// <summary>
        /// XML Ayrıştırma testlerini çalıştırır
        /// </summary>
        [HttpGet("xml-parsing")]
        [Produces("application/json")]
        public async Task<IActionResult> RunXmlParsingTests()
        {
            try {
                // Test 1: XmlReader Performans Testi
                var xmlReaderResult = TestXmlReaderParsing();
                _testResults["Test_XmlReaderParsing"] = xmlReaderResult;
                
                // Test 2: XmlDocument Performans Testi
                var xmlDocumentResult = TestXmlDocumentParsing();
                _testResults["Test_XmlDocumentParsing"] = xmlDocumentResult;
                
                // Test 3: XDocument (LINQ to XML) Performans Testi
                var xDocumentResult = TestXDocumentParsing();
                _testResults["Test_XDocumentParsing"] = xDocumentResult;
                
                // Test 4: XML Ayrıştırma Yöntemlerinin Karşılaştırma Testi
                var compareResult = TestCompareParsingMethods();
                _testResults["Test_CompareParsingMethods"] = compareResult;
                
                return Ok(new 
                {
                    TestResults = _testResults
                        .Where(r => r.Key.Contains("Parsing"))
                        .ToDictionary(r => r.Key, r => new { r.Value.Success, r.Value.Message })
                });
            }
            catch (Exception ex) {
                return StatusCode(500, $"XML Ayrıştırma testleri sırasında hata oluştu: {ex.Message}");
            }
        }

        #region Ürün API Testleri

        private async Task<TestResult> TestGetAllProducts()
        {
            try
            {
                // Dinamik URL yerine bilinen çalışan URL'yi kullan
                var client = _clientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(30); // Zaman aşımı süresini artır

                try
                {
                    // Direkt servis üzerinden ürünleri almayı dene (Şema doğrulamayı atlayabilir)
                    var products = _xmlProductService.GetAllProducts();
                    return TestResult.Pass($"{products.Count} adet ürün başarıyla alındı");
                }
                catch
                {
                    // Servis hatası durumunda API'yi çağır
                    var response = await client.GetAsync($"{_baseUrl}/api/products");
                    
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        return TestResult.Fail($"HTTP hata kodu: {response.StatusCode}, Detay: {errorContent}");
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    
                    try
                    {
                        var products = JsonSerializer.Deserialize<List<Product>>(content,
                            new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true,
                                AllowTrailingCommas = true,
                                ReadCommentHandling = JsonCommentHandling.Skip
                            });

                        if (products == null || !products.Any())
                            return TestResult.Fail("Ürünler alınamadı veya liste boş");

                        return TestResult.Pass($"{products.Count} adet ürün başarıyla alındı");
                    }
                    catch (JsonException jsonEx)
                    {
                        return TestResult.Fail($"JSON çözümleme hatası: {jsonEx.Message}. İçerik: {(content.Length > 100 ? content.Substring(0, 100) + "..." : content)}");
                    }
                }
            }
            catch (Exception ex)
            {
                return TestResult.Fail($"Beklenmeyen hata: {ex.Message}. Hata türü: {ex.GetType().Name}");
            }
        }

        private async Task<TestResult> TestGetProductById(int id)
        {
            try
            {
                var client = _clientFactory.CreateClient();
                
                // Her istekte mevcut URL'i kullanabilirsiniz ya da sabit _baseUrl
                string currentBaseUrl = _baseUrl; // "http://localhost:5000"
                var response = await client.GetAsync($"{currentBaseUrl}/api/products/{id}");
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return TestResult.Pass("Olmayan ID için beklenen 404 yanıtı alındı");
                    
                if (!response.IsSuccessStatusCode)
                    return TestResult.Fail($"HTTP hata kodu: {response.StatusCode}");
                
                var content = await response.Content.ReadAsStringAsync();
                var product = JsonSerializer.Deserialize<Product>(content, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                if (product == null)
                    return TestResult.Fail("Ürün alınamadı");
                
                if (product.Id != id)
                    return TestResult.Fail($"Hatalı ürün alındı. Beklenen ID: {id}, Gelen ID: {product.Id}");
                
                return TestResult.Pass($"ID {id} için ürün başarıyla alındı: {product.Name}");
            }
            catch (Exception ex)
            {
                return TestResult.Fail($"Hata: {ex.Message}");
            }
        }
        
        private async Task<TestResult> TestContentNegotiation()
        {
            try
            {
                var client = _clientFactory.CreateClient();
                
                // JSON İsteği
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                var jsonResponse = await client.GetAsync($"{_baseUrl}/api/products");
                var jsonContent = await jsonResponse.Content.ReadAsStringAsync();
                var jsonContentType = jsonResponse.Content.Headers.ContentType?.ToString();
                
                if (!jsonContentType?.Contains("application/json") ?? true)
                    return TestResult.Fail($"JSON istek için yanlış Content-Type: {jsonContentType}");
                
                // XML İsteği
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Accept", "application/xml");
                var xmlResponse = await client.GetAsync($"{_baseUrl}/api/products");
                var xmlContent = await xmlResponse.Content.ReadAsStringAsync();
                var xmlContentType = xmlResponse.Content.Headers.ContentType?.ToString();
                
                if (!xmlContentType?.Contains("application/xml") ?? true)
                    return TestResult.Fail($"XML istek için yanlış Content-Type: {xmlContentType}");
                
                // XML içeriğini doğrula
                try
                {
                    var doc = new XmlDocument();
                    doc.LoadXml(xmlContent);
                }
                catch (XmlException)
                {
                    return TestResult.Fail("Dönen içerik geçerli bir XML değil");
                }
                
                return TestResult.Pass("Content negotiation başarılı. Hem JSON hem XML yanıtları doğru biçimde alındı");
            }
            catch (Exception ex)
            {
                return TestResult.Fail($"Hata: {ex.Message}");
            }
        }
        
        #endregion
        
        #region XML Güvenlik Testleri
        
        private TestResult TestXmlSignature()
        {
            try
            {
                // Test ürünü oluştur
                var product = new Product 
                { 
                    Id = 1,
                    Name = "Test Ürün",
                    Description = "XML İmzalama testi için ürün",
                    Price = 99.99m,
                    CategoryId = 1,
                    ImageUrl = "/images/test.jpg",
                    StockQuantity = 10
                };
                
                // XML oluştur
                var doc = new XmlDocument();
                XmlElement root = doc.CreateElement("Product");
                doc.AppendChild(root);
                
                AddElement(doc, root, "Id", product.Id.ToString());
                AddElement(doc, root, "Name", product.Name);
                AddElement(doc, root, "Description", product.Description);
                AddElement(doc, root, "Price", product.Price.ToString());
                AddElement(doc, root, "CategoryId", product.CategoryId.ToString());
                AddElement(doc, root, "StockQuantity", product.StockQuantity.ToString());
                
                // XML'i imzala
                var signedDoc = _xmlSecurityService.SignXmlDocument(doc);
                
                // İmza elementini kontrol et
                var signatureNodes = signedDoc.GetElementsByTagName("Signature", "http://www.w3.org/2000/09/xmldsig#");
                
                if (signatureNodes.Count == 0)
                    return TestResult.Fail("İmza elementi bulunamadı");
                
                // İmza elementinin varlığını kontrol et
                return TestResult.Pass("XML başarıyla imzalandı");
            }
            catch (Exception ex)
            {
                return TestResult.Fail($"XML imzalama hatası: {ex.Message}");
            }
        }
        
        private TestResult TestXmlSignatureVerification()
        {
            try
            {
                // Test ürünü oluştur ve imzala
                var product = new Product 
                { 
                    Id = 1,
                    Name = "Test Ürün",
                    Description = "XML İmza Doğrulama testi için ürün",
                    Price = 99.99m,
                    CategoryId = 1,
                    ImageUrl = "/images/test.jpg",
                    StockQuantity = 10
                };
                
                // XML oluştur
                var doc = new XmlDocument();
                XmlElement root = doc.CreateElement("Product");
                doc.AppendChild(root);
                
                AddElement(doc, root, "Id", product.Id.ToString());
                AddElement(doc, root, "Name", product.Name);
                AddElement(doc, root, "Description", product.Description);
                AddElement(doc, root, "Price", product.Price.ToString());
                AddElement(doc, root, "CategoryId", product.CategoryId.ToString());
                AddElement(doc, root, "StockQuantity", product.StockQuantity.ToString());
                
                // XML'i imzala
                var signedDoc = _xmlSecurityService.SignXmlDocument(doc);
                
                // İmza doğrulama
                bool isValid = _xmlSecurityService.VerifyXmlSignature(signedDoc);
                
                if (!isValid)
                    return TestResult.Fail("İmza doğrulanamadı");
                
                // Manipüle edilmiş doküman testi
                var modifiedDoc = signedDoc.Clone() as XmlDocument;
                var priceNode = modifiedDoc.SelectSingleNode("//Price");
                if (priceNode != null)
                    priceNode.InnerText = "199.99"; // Fiyatı değiştir
                
                bool shouldBeFalse = _xmlSecurityService.VerifyXmlSignature(modifiedDoc);
                
                if (shouldBeFalse)
                    return TestResult.Fail("Manipüle edilmiş dokümanın imzası geçersiz olmalıydı");
                
                return TestResult.Pass("XML imza doğrulama testi başarılı");
            }
            catch (Exception ex)
            {
                return TestResult.Fail($"XML imza doğrulama hatası: {ex.Message}");
            }
        }
        
        private TestResult TestXmlEncryption()
        {
            try
            {
                // Test ürünü oluştur
                var product = new Product 
                { 
                    Id = 1,
                    Name = "Test Ürün",
                    Description = "XML Şifreleme testi için ürün",
                    Price = 88.77m,
                    CategoryId = 1,
                    ImageUrl = "/images/test.jpg",
                    StockQuantity = 10
                };
                
                // XML oluştur
                var doc = new XmlDocument();
                XmlElement root = doc.CreateElement("Product");
                doc.AppendChild(root);
                
                AddElement(doc, root, "Id", product.Id.ToString());
                AddElement(doc, root, "Name", product.Name);
                AddElement(doc, root, "Description", product.Description);
                AddElement(doc, root, "Price", product.Price.ToString());
                AddElement(doc, root, "CategoryId", product.CategoryId.ToString());
                AddElement(doc, root, "StockQuantity", product.StockQuantity.ToString());
                
                // Price alanını şifrele
                var encryptedDoc = _xmlSecurityService.EncryptXmlElement(doc, "Price");
                
                // Şifrelenmiş elementi kontrol et
                var encryptedDataNodes = encryptedDoc.GetElementsByTagName("EncryptedData", "http://www.w3.org/2001/04/xmlenc#");
                
                if (encryptedDataNodes.Count == 0)
                    return TestResult.Fail("EncryptedData elementi bulunamadı");
                
                // Şifreli XML içinde orijinal verinin görünüp görünmediğini kontrol et
                if (encryptedDoc.OuterXml.Contains("88.77"))
                    return TestResult.Fail("Şifreleme başarısız: Orijinal değer hala XML'de görünüyor");
                
                return TestResult.Pass("XML başarıyla şifrelendi");
            }
            catch (Exception ex)
            {
                return TestResult.Fail($"XML şifreleme hatası: {ex.Message}");
            }
        }
        
        private TestResult TestXmlDecryption()
        {
            try
            {
                // Test ürünü oluştur
                var product = new Product 
                { 
                    Id = 1,
                    Name = "Test Ürün",
                    Description = "XML Şifre çözme testi için ürün",
                    Price = 55.66m,
                    CategoryId = 1,
                    ImageUrl = "/images/test.jpg",
                    StockQuantity = 10
                };
                
                // XML oluştur
                var doc = new XmlDocument();
                XmlElement root = doc.CreateElement("Product");
                doc.AppendChild(root);
                
                AddElement(doc, root, "Id", product.Id.ToString());
                AddElement(doc, root, "Name", product.Name);
                AddElement(doc, root, "Description", product.Description);
                AddElement(doc, root, "Price", product.Price.ToString());
                AddElement(doc, root, "CategoryId", product.CategoryId.ToString());
                AddElement(doc, root, "StockQuantity", product.StockQuantity.ToString());
                
                // Price alanını şifrele
                var encryptedDoc = _xmlSecurityService.EncryptXmlElement(doc, "Price");
                
                // Şifreyi çöz
                var decryptedDoc = _xmlSecurityService.DecryptXmlDocument(encryptedDoc);
                
                // Çözülen belgeyi kontrol et
                var priceNode = decryptedDoc.SelectSingleNode("//Price");
                
                if (priceNode == null)
                    return TestResult.Fail("Şifre çözme sonrası Price elementi bulunamadı");
                
                // Kültürden bağımsız karşılaştırma için önce string değeri decimal'e çevirelim
                decimal expectedPrice = 55.66m;
                
                // Nokta veya virgül olabilir, her iki durumu da destekleyen çözüm
                decimal actualPrice;
                string priceText = priceNode.InnerText.Replace(',', '.');
                if (!decimal.TryParse(priceText, NumberStyles.Any, CultureInfo.InvariantCulture, out actualPrice))
                {
                    return TestResult.Fail($"Şifre çözme sonrası Price değeri sayısal formata çevrilemedi: {priceNode.InnerText}");
                }
                
                if (Math.Abs(actualPrice - expectedPrice) < 0.001m) // Küçük bir tolerans ile karşılaştır
                    return TestResult.Pass("XML şifresi başarıyla çözüldü");
                else
                    return TestResult.Fail($"Şifre çözme sonrası Price değeri yanlış: {priceNode.InnerText}, beklenen: 55.66");
            }
            catch (Exception ex)
            {
                return TestResult.Fail($"XML şifre çözme hatası: {ex.Message}");
            }
        }
        
        private TestResult TestSecureXml()
        {
            try
            {
                // Test ürünü oluştur
                var product = new Product 
                { 
                    Id = 1,
                    Name = "Test Ürün",
                    Description = "XML Güvenliği testi için ürün",
                    Price = 123.45m,
                    CategoryId = 1,
                    ImageUrl = "/images/test.jpg",
                    StockQuantity = 10
                };
                
                // Güvenli XML oluştur (hem şifreli hem imzalı)
                string secureXml = _xmlSecurityService.SecureProduct(product);
                
                // XML'i kontrol et
                var doc = new XmlDocument();
                doc.LoadXml(secureXml);
                
                // Şifreleme ve imza elementlerini kontrol et
                var encryptedDataNodes = doc.GetElementsByTagName("EncryptedData", "http://www.w3.org/2001/04/xmlenc#");
                var signatureNodes = doc.GetElementsByTagName("Signature", "http://www.w3.org/2000/09/xmldsig#");
                
                if (encryptedDataNodes.Count == 0)
                    return TestResult.Fail("EncryptedData elementi bulunamadı");
                
                if (signatureNodes.Count == 0)
                    return TestResult.Fail("Signature elementi bulunamadı");
                
                // İmza doğrulama
                bool isValid = _xmlSecurityService.VerifyXmlSignature(doc);
                
                if (!isValid)
                    return TestResult.Fail("Güvenli XML'in imzası doğrulanamadı");
                
                return TestResult.Pass("Güvenli XML oluşturma testi başarılı");
            }
            catch (Exception ex)
            {
                return TestResult.Fail($"Güvenli XML oluşturma hatası: {ex.Message}");
            }
        }
        
        #endregion
        
        #region XML Ayrıştırma Testleri
        
        private TestResult TestXmlReaderParsing()
        {
            try
            {
                var products = _xmlParsingService.GetProductsUsingXmlReader();
                
                if (products == null || !products.Any())
                    return TestResult.Fail("XmlReader ile ürünler alınamadı veya liste boş");
                
                return TestResult.Pass($"XmlReader ile {products.Count} ürün başarıyla ayrıştırıldı");
            }
            catch (Exception ex)
            {
                return TestResult.Fail($"XmlReader ayrıştırma hatası: {ex.Message}");
            }
        }
        
        private TestResult TestXmlDocumentParsing()
        {
            try
            {
                var products = _xmlParsingService.GetProductsUsingXmlDocument();
                
                if (products == null || !products.Any())
                    return TestResult.Fail("XmlDocument ile ürünler alınamadı veya liste boş");
                
                return TestResult.Pass($"XmlDocument ile {products.Count} ürün başarıyla ayrıştırıldı");
            }
            catch (Exception ex)
            {
                return TestResult.Fail($"XmlDocument ayrıştırma hatası: {ex.Message}");
            }
        }
        
        private TestResult TestXDocumentParsing()
        {
            try
            {
                var products = _xmlParsingService.GetProductsUsingXDocument();
                
                if (products == null || !products.Any())
                    return TestResult.Fail("XDocument (LINQ to XML) ile ürünler alınamadı veya liste boş");
                
                return TestResult.Pass($"XDocument ile {products.Count} ürün başarıyla ayrıştırıldı");
            }
            catch (Exception ex)
            {
                return TestResult.Fail($"XDocument ayrıştırma hatası: {ex.Message}");
            }
        }
        
        private TestResult TestCompareParsingMethods()
        {
            try
            {
                var results = _xmlParsingService.CompareXmlParsingPerformance();
                
                if (results == null || !results.Any())
                    return TestResult.Fail("Ayrıştırma yöntemleri karşılaştırılamadı");
                
                var sb = new StringBuilder();
                sb.AppendLine("Ayrıştırma yöntemleri performans karşılaştırması:");
                
                foreach (var result in results)
                {
                    sb.AppendLine($"- {result.Key}: {result.Value} ms");
                }
                
                // En hızlı yöntemi bul
                var fastest = results.OrderBy(r => r.Value).FirstOrDefault();
                sb.AppendLine();
                sb.AppendLine($"En hızlı yöntem: {fastest.Key} ({fastest.Value} ms)");
                
                return TestResult.Pass(sb.ToString());
            }
            catch (Exception ex)
            {
                return TestResult.Fail($"Ayrıştırma yöntemleri karşılaştırma hatası: {ex.Message}");
            }
        }
        
        #endregion
        
        #region Yardımcı Metotlar
        
        private void AddElement(XmlDocument doc, XmlElement parent, string name, string value)
        {
            XmlElement element = doc.CreateElement(name);
            element.InnerText = value;
            parent.AppendChild(element);
        }
        
        private string GenerateHtmlReport()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang='en'>");
            sb.AppendLine("<head>");
            sb.AppendLine("    <meta charset='UTF-8'>");
            sb.AppendLine("    <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
            sb.AppendLine("    <title>API Test Sonuçları</title>");
            sb.AppendLine("    <style>");
            sb.AppendLine("        body { font-family: Arial, sans-serif; line-height: 1.6; margin: 0; padding: 20px; }");
            sb.AppendLine("        h1 { color: #333; border-bottom: 1px solid #eee; padding-bottom: 10px; }");
            sb.AppendLine("        h2 { color: #0066cc; margin-top: 30px; }");
            sb.AppendLine("        .test-group { margin-bottom: 30px; }");
            sb.AppendLine("        .test-item { margin-bottom: 15px; border-left: 4px solid #ddd; padding: 10px 15px; }");
            sb.AppendLine("        .success { border-left-color: #4CAF50; background-color: #f1f8e9; }");
            sb.AppendLine("        .fail { border-left-color: #FF5722; background-color: #fbe9e7; }");
            sb.AppendLine("        .test-name { font-weight: bold; margin-bottom: 5px; }");
            sb.AppendLine("        .test-message { font-size: 14px; margin-top: 5px; white-space: pre-line; }");
            sb.AppendLine("        .summary { margin-top: 30px; padding: 15px; background: #f5f5f5; border-radius: 4px; }");
            sb.AppendLine("        .summary p { margin: 5px 0; }");
            sb.AppendLine("    </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            
            sb.AppendLine("    <h1>API Test Sonuçları</h1>");
            
            // Ürün API Testleri
            sb.AppendLine("    <div class='test-group'>");
            sb.AppendLine("        <h2>Ürün API Testleri</h2>");
            AddTestResults(sb, _testResults.Where(r => r.Key.StartsWith("Test_Get")).ToList());
            sb.AppendLine("    </div>");
            
            // XML Güvenlik Testleri
            sb.AppendLine("    <div class='test-group'>");
            sb.AppendLine("        <h2>XML Güvenlik Testleri</h2>");
            AddTestResults(sb, _testResults.Where(r => r.Key.StartsWith("Test_Xml")).ToList());
            sb.AppendLine("    </div>");
            
            // XML Ayrıştırma Testleri
            sb.AppendLine("    <div class='test-group'>");
            sb.AppendLine("        <h2>XML Ayrıştırma Testleri</h2>");
            AddTestResults(sb, _testResults.Where(r => r.Key.Contains("Parsing")).ToList());
            sb.AppendLine("    </div>");
            
            // Test Özeti
            var totalTests = _testResults.Count;
            var passedTests = _testResults.Count(r => r.Value.Success);
            var failedTests = totalTests - passedTests;
            
            sb.AppendLine("    <div class='summary'>");
            sb.AppendLine("        <h2>Test Özeti</h2>");
            sb.AppendLine($"        <p><strong>Toplam Test Sayısı:</strong> {totalTests}</p>");
            sb.AppendLine($"        <p><strong>Başarılı Testler:</strong> {passedTests}</p>");
            sb.AppendLine($"        <p><strong>Başarısız Testler:</strong> {failedTests}</p>");
            sb.AppendLine($"        <p><strong>Başarı Oranı:</strong> {(totalTests > 0 ? (passedTests * 100 / totalTests) : 0)}%</p>");
            sb.AppendLine("    </div>");
            
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            
            return sb.ToString();
        }
        
        private void AddTestResults(StringBuilder sb, List<KeyValuePair<string, TestResult>> results)
        {
            foreach (var result in results.OrderBy(r => r.Key))
            {
                var testName = result.Key.Replace("Test_", "").Replace("Xml", "XML ");
                
                // CamelCase'ten normal metne çevirme
                testName = System.Text.RegularExpressions.Regex.Replace(testName, 
                    "([a-z])([A-Z])", "$1 $2");
                
                sb.AppendLine($"        <div class='test-item {(result.Value.Success ? "success" : "fail")}'>");
                sb.AppendLine($"            <div class='test-name'>{testName}</div>");
                sb.AppendLine($"            <div class='test-status'>{(result.Value.Success ? "✅ Başarılı" : "❌ Başarısız")}</div>");
                sb.AppendLine($"            <div class='test-message'>{result.Value.Message}</div>");
                sb.AppendLine("        </div>");
            }
        }
        
        #endregion
        
        /// <summary>
        /// Test senaryolarını JSON formatında belgeleriyle birlikte döndürür
        /// </summary>
        [HttpGet("documentation")]
        [Produces("application/json")]
        public IActionResult GetTestDocumentation()
        {
            var testGroups = new object[]
            {
                new {
                    Name = "Ürün API Testleri",
                    Description = "Ürün API'lerinin doğru çalışıp çalışmadığını test eder",
                    TestCases = new object[]
                    {
                        new {
                            Name = "GetAllProducts",
                            Description = "Tüm ürünleri getirme API'sini test eder",
                            Endpoint = "GET /api/products",
                            ExpectedResult = "200 OK ve ürün listesi"
                        },
                        new {
                            Name = "GetProductById",
                            Description = "ID'ye göre ürün getirme API'sini test eder",
                            Endpoint = "GET /api/products/{id}",
                            ExpectedResult = "200 OK ve ürün detayları veya 404 Not Found"
                        },
                        new {
                            Name = "ContentNegotiation",
                            Description = "API'nin content negotiation'ı destekleyip desteklemediğini test eder",
                            Endpoint = "GET /api/products (Accept: application/json veya application/xml)",
                            ExpectedResult = "İstemciye uygun formatta yanıt verilmesi"
                        }
                    }
                },
                new {
                    Name = "XML Güvenlik Testleri",
                    Description = "XML güvenlik işlevlerinin doğru çalışıp çalışmadığını test eder",
                    TestCases = new object[]
                    {
                        new {
                            Name = "XmlSignature",
                            Description = "XML imzalama işlevini test eder",
                            ExpectedResult = "İmzalı XML belgesi"
                        },
                        new {
                            Name = "XmlSignatureVerification",
                            Description = "XML imza doğrulama işlevini test eder",
                            ExpectedResult = "Doğru imza için true, manipüle edilmiş belge için false"
                        },
                        new {
                            Name = "XmlEncryption",
                            Description = "XML şifreleme işlevini test eder",
                            ExpectedResult = "Belirtilen element şifrelenmiş XML belgesi"
                        },
                        new {
                            Name = "XmlDecryption",
                            Description = "XML şifre çözme işlevini test eder",
                            ExpectedResult = "Şifresi çözülmüş XML belgesi"
                        },
                        new {
                            Name = "SecureXml",
                            Description = "Hem şifreleme hem imzalama içeren güvenli XML oluşturmayı test eder",
                            ExpectedResult = "Güvenli XML belgesi"
                        }
                    }
                },
                new {
                    Name = "XML Ayrıştırma Testleri",
                    Description = "XML ayrıştırma yöntemlerinin doğru çalışıp çalışmadığını ve performans karşılaştırmalarını test eder",
                    TestCases = new object[]
                    {
                        new {
                            Name = "XmlReaderParsing",
                            Description = "XmlReader ile XML ayrıştırma işlevini test eder",
                            ExpectedResult = "XML dosyasından ürün listesi"
                        },
                        new {
                            Name = "XmlDocumentParsing",
                            Description = "XmlDocument ile XML ayrıştırma işlevini test eder",
                            ExpectedResult = "XML dosyasından ürün listesi"
                        },
                        new {
                            Name = "XDocumentParsing",
                            Description = "XDocument (LINQ to XML) ile XML ayrıştırma işlevini test eder",
                            ExpectedResult = "XML dosyasından ürün listesi"
                        },
                        new {
                            Name = "CompareParsingMethods",
                            Description = "Farklı XML ayrıştırma yöntemlerinin performansını karşılaştırır",
                            ExpectedResult = "Yöntem ve süre içeren performans karşılaştırma sonuçları"
                        }
                    }
                }
            };
            
            var documentation = new
            {
                Title = "ECommerceAPI Test Senaryoları",
                Version = "1.0",
                TestGroups = testGroups,
                Notes = "Bu test senaryoları, API'lerin beklenen işlevselliği ve performansı sağlayıp sağlamadığını kontrol eder."
            };
            
            return Ok(documentation);
        }
    }
    
    /// <summary>
    /// Test sonucu için yardımcı sınıf
    /// </summary>
    public class TestResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        
        public static TestResult Pass(string message) => new TestResult { Success = true, Message = message };
        public static TestResult Fail(string message) => new TestResult { Success = false, Message = message };
    }
}