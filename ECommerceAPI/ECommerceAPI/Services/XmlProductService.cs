using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.XPath;
using ECommerceAPI.Models;

namespace ECommerceAPI.Services
{
    public class XmlProductService
    {
        private readonly string _xmlFilePath;   
        private readonly string _xsdFilePath;
        private readonly string _dtdFilePath;
        
        public XmlProductService(IWebHostEnvironment env)
        {
            // XML ve şema dosyalarının yollarını ayarla
            _xmlFilePath = Path.Combine(env.ContentRootPath, "XmlData", "Products.xml");
            _xsdFilePath = Path.Combine(env.ContentRootPath, "XmlSchemas", "Products.xsd");
            _dtdFilePath = Path.Combine(env.ContentRootPath, "XmlData", "Products.dtd");
        }
        
        public List<Product> GetAllProducts()
        {
            // XML dosyasını doğrula
            ValidateXml();
            ValidateDtd();
            
            var products = new List<Product>();
            var doc = XDocument.Load(_xmlFilePath);
            
            // Tüm ürünleri dolaş ve listeye ekle
            foreach (var productElement in doc.Descendants("Product"))
            {
                products.Add(new Product
                {
                    Id = int.Parse(productElement.Element("Id").Value),
                    Name = productElement.Element("Name").Value,
                    Description = productElement.Element("Description").Value,
                    Price = decimal.Parse(productElement.Element("Price").Value),
                    CategoryId = int.Parse(productElement.Element("CategoryId").Value),
                    ImageUrl = productElement.Element("ImageUrl").Value,
                    StockQuantity = int.Parse(productElement.Element("StockQuantity").Value)
                });
            }
            
            return products;
        }
        
        public Product GetProductById(int id)
        {
            var doc = XDocument.Load(_xmlFilePath);
            var productElement = doc.Descendants("Product")
                .FirstOrDefault(p => int.Parse(p.Element("Id").Value) == id);
                
            if (productElement == null)
                return null;
            
            var product = new Product
            {
                Id = int.Parse(productElement.Element("Id").Value),
                Name = productElement.Element("Name").Value,
                Description = productElement.Element("Description").Value,
                Price = decimal.Parse(productElement.Element("Price").Value),
                CategoryId = int.Parse(productElement.Element("CategoryId").Value),
                ImageUrl = productElement.Element("ImageUrl").Value,
                StockQuantity = int.Parse(productElement.Element("StockQuantity").Value),
                Tags = new List<string>()
            };
            
            // Discount elementini kontrol et ve varsa al
            var discountElement = productElement.Element("Discount");
            if (discountElement != null)
            {
                product.Discount = decimal.Parse(discountElement.Value);
            }
            
            // Tag elementlerini kontrol et ve varsa ekle
            var tagElements = productElement.Elements("Tag");
            if (tagElements.Any())
            {
                foreach (var tagElement in tagElements)
                {
                    product.Tags.Add(tagElement.Value);
                }
            }
            
            return product;
        }
        
        // YENİ: XPath ile kategoriye göre ürün filtreleme metodu 
        public List<Product> GetProductsByCategory(int categoryId)
        {
            var products = new List<Product>();
            var doc = XDocument.Load(_xmlFilePath);
            
            // XPath kullanarak belirli kategorideki ürünleri sorgula
            string xpathQuery = $"//Product[CategoryId={categoryId}]";
            var productElements = doc.XPathSelectElements(xpathQuery);
            
            foreach (var productElement in productElements)
            {
                products.Add(new Product
                {
                    Id = int.Parse(productElement.Element("Id").Value),
                    Name = productElement.Element("Name").Value,
                    Description = productElement.Element("Description").Value,
                    Price = decimal.Parse(productElement.Element("Price").Value),
                    CategoryId = int.Parse(productElement.Element("CategoryId").Value),
                    ImageUrl = productElement.Element("ImageUrl").Value,
                    StockQuantity = int.Parse(productElement.Element("StockQuantity").Value)
                });
            }
            
            return products;
        }
        
        // YENİ: XPath ile fiyat aralığına göre ürün filtreleme metodu
        public List<Product> GetProductsByPriceRange(decimal minPrice, decimal maxPrice)
        {
            var products = new List<Product>();
            var doc = XDocument.Load(_xmlFilePath);
            
            // XPath ile fiyat aralığındaki ürünleri sorgula
            string xpathQuery = $"//Product[Price >= {minPrice} and Price <= {maxPrice}]";
            var productElements = doc.XPathSelectElements(xpathQuery);
            
            foreach (var productElement in productElements)
            {
                products.Add(new Product
                {
                    Id = int.Parse(productElement.Element("Id").Value),
                    Name = productElement.Element("Name").Value,
                    Description = productElement.Element("Description").Value,
                    Price = decimal.Parse(productElement.Element("Price").Value),
                    CategoryId = int.Parse(productElement.Element("CategoryId").Value),
                    ImageUrl = productElement.Element("ImageUrl").Value,
                    StockQuantity = int.Parse(productElement.Element("StockQuantity").Value)
                });
            }
            
            return products;
        }
        
        public void AddProduct(Product product)
        {
            // Yeni ürün ekle
            var doc = XDocument.Load(_xmlFilePath);
            var products = doc.Element("Products");
            
            // En yüksek mevcut ID'yi bul ve 1 artır
            var maxId = doc.Descendants("Id").Max(e => int.Parse(e.Value));
            product.Id = maxId + 1;
            
            var newProduct = new XElement("Product",
                new XElement("Id", product.Id),
                new XElement("Name", product.Name),
                new XElement("Description", product.Description),
                new XElement("Price", product.Price),
                new XElement("CategoryId", product.CategoryId),
                new XElement("ImageUrl", product.ImageUrl),
                new XElement("StockQuantity", product.StockQuantity)
            );
            
            products.Add(newProduct);
            doc.Save(_xmlFilePath);
        }
        
        public bool UpdateProduct(Product updatedProduct)
        {
            try
            {
                // Debug log
                Console.WriteLine($"Güncelleme başladı: ID={updatedProduct.Id}, Name={updatedProduct.Name}");
                
                var existingProduct = GetProductById(updatedProduct.Id);
                if (existingProduct == null)
                {
                    Console.WriteLine("Hata: Mevcut ürün bulunamadı");
                    return false;
                }

                Console.WriteLine($"Mevcut ürün bulundu: Discount={existingProduct.Discount}, Tags={string.Join(",", existingProduct.Tags ?? new List<string>())}");

                // Gelen veride olmayan (null) özellikleri mevcut üründen al
                if (updatedProduct.Discount == null)
                    updatedProduct.Discount = existingProduct.Discount;

                if (updatedProduct.Tags == null || !updatedProduct.Tags.Any())
                    updatedProduct.Tags = existingProduct.Tags;

                var doc = XDocument.Load(_xmlFilePath);
                var productElement = doc.Descendants("Product")
                    .FirstOrDefault(p => int.Parse(p.Element("Id").Value) == updatedProduct.Id);

                if (productElement == null)
                    return false;

                // Diğer güncelleme kodları...
                productElement.Element("Name").Value = updatedProduct.Name;
                productElement.Element("Description").Value = updatedProduct.Description;
                productElement.Element("Price").Value = updatedProduct.Price.ToString(System.Globalization.CultureInfo.InvariantCulture);
                productElement.Element("CategoryId").Value = updatedProduct.CategoryId.ToString();
                productElement.Element("ImageUrl").Value = updatedProduct.ImageUrl;
                productElement.Element("StockQuantity").Value = updatedProduct.StockQuantity.ToString();

                // Discount ve Tags alanlarını güncellemek için özel mantık
                UpdateDiscountElement(productElement, updatedProduct.Discount);
                UpdateTagElements(productElement, updatedProduct.Tags);

                doc.Save(_xmlFilePath);
                Console.WriteLine("Güncelleme başarılı!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"HATA: Ürün güncelleme sırasında beklenmeyen hata: {ex.Message}");
                return false;
            }
        }

        private void UpdateDiscountElement(XElement productElement, decimal? discount)
        {
            var discountElement = productElement.Element("Discount");
            if (discount.HasValue)
            {
                if (discountElement != null)
                    discountElement.Value = discount.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
                else
                    productElement.Add(new XElement("Discount", discount.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)));
            }
            // Discount null değilse mevcut elementi koruyacak, silmeyecek
        }

        private void UpdateTagElements(XElement productElement, List<string> tags)
        {
            if (tags != null && tags.Any())
            {
                // Mevcut etiketleri kaldır
                productElement.Elements("Tag").Remove();
                
                // Yeni etiketleri ekle
                foreach (var tag in tags)
                    productElement.Add(new XElement("Tag", tag));
            }
            // Tags null veya boşsa, mevcut Tag elementlerini koruyacak
        }
        
        public void DeleteProduct(int id)
        {
            // Ürünü sil
            var doc = XDocument.Load(_xmlFilePath);
            var productElement = doc.Descendants("Product")
                .FirstOrDefault(p => int.Parse(p.Element("Id").Value) == id);
                
            if (productElement != null)
            {
                productElement.Remove();
                doc.Save(_xmlFilePath);
            }
        }
        
        // XML şema doğrulama metodu (mevcut)
        private void ValidateXml()
        {
            try 
            {
                // XML doğrulama işlemi
                var schemas = new XmlSchemaSet();
                schemas.Add("", _xsdFilePath);
                
                var doc = XDocument.Load(_xmlFilePath);
                bool isValid = true;
                string validationError = string.Empty;
                
                doc.Validate(schemas, (o, e) => {
                    isValid = false;
                    validationError = e.Message;
                });
                
                if (!isValid)
                    throw new Exception($"XML belgesi şemaya uygun değil: {validationError}");
            }
            catch (XmlSchemaException ex)
            {
                throw new Exception($"XSD şema hatası: {ex.Message}", ex);
            }
            catch (XmlException ex)
            {
                throw new Exception($"XML ayrıştırma hatası: {ex.Message}", ex);
            }
        }
        
        // YENİ: Geliştirilmiş DTD doğrulama metodu - hata yönetimi eklenmiş
        private void ValidateDtd()
        {
            try 
            {
                // DTD dosya yolunu belirleme
                string dtdPath = Path.Combine(Path.GetDirectoryName(_xmlFilePath), "Products.dtd");
                
                // XML dosyasını okuma ve XML içeriğini değiştirme
                string xmlContent = File.ReadAllText(_xmlFilePath);
                
                // DTD referansını daha güvenli bir şekilde tanımlayan düzeltilmiş bir kopya oluştur
                string tempXmlPath = Path.Combine(Path.GetTempPath(), "temp_products.xml");
                string tempDtdPath = Path.Combine(Path.GetTempPath(), "temp_products.dtd");
                
                // DTD dosyasını geçici konuma kopyala
                File.Copy(_dtdFilePath, tempDtdPath, true);
                
                // XML'in başını değiştir ve geçici DTD referansı ekle
                string modifiedXml = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE Products SYSTEM ""{tempDtdPath}"">
<Products>" + xmlContent.Substring(xmlContent.IndexOf("<Products>") + 10);
                
                // Geçici XML dosyasını oluştur
                File.WriteAllText(tempXmlPath, modifiedXml);
                
                // Doğrulama ayarları oluştur
                XmlReaderSettings settings = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Parse,
                    ValidationType = ValidationType.DTD,
                    XmlResolver = new XmlUrlResolver()
                };
                
                bool hasValidationError = false;
                string validationErrorMessage = string.Empty;
                
                settings.ValidationEventHandler += (sender, e) => 
                {
                    if (!e.Message.Contains("Tag") || !e.Message.Contains("not declared"))
                    {
                        hasValidationError = true;
                        validationErrorMessage = e.Message;
                    }
                };
                
                // Geçici XML dosyasını doğrula
                using (XmlReader reader = XmlReader.Create(tempXmlPath, settings))
                {
                    while (reader.Read()) { }
                    
                    if (hasValidationError)
                    {
                        throw new Exception($"DTD Doğrulama Hatası: {validationErrorMessage}");
                    }
                }
                
                // Geçici dosyaları temizle
                try
                {
                    File.Delete(tempXmlPath);
                    File.Delete(tempDtdPath);
                }
                catch { /* Temizleme hataları önemli değil */ }
            }
            catch (Exception ex) when (ex is FileNotFoundException || ex is DirectoryNotFoundException)
            {
                throw new Exception($"DTD dosyası bulunamadı: {ex.Message}", ex);
            }
            catch (XmlException ex)
            {
                throw new Exception($"DTD Doğrulama sırasında XML ayrıştırma hatası: {ex.Message}", ex);
            }
        }
    }
}