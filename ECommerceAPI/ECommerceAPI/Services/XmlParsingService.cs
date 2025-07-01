using System.Xml;
using System.Xml.Linq;
using ECommerceAPI.Models;

namespace ECommerceAPI.Services
{
    public class XmlParsingService
    {
        private readonly string _xmlFilePath;
        
        public XmlParsingService(IWebHostEnvironment env)
        {
            _xmlFilePath = Path.Combine(env.ContentRootPath, "XmlData", "Products.xml");
        }
        
        // XmlReader kullanarak okuma (pull parsing) - bellek kullanýmý az
        public List<Product> GetProductsUsingXmlReader()
        {
            var products = new List<Product>();
            
            // DTD iþleme ayarlarýný belirleyin
            XmlReaderSettings settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Parse,
                ValidationType = ValidationType.DTD,
                XmlResolver = new XmlUrlResolver() // DTD dosyasýný çözümlemek için gerekli
            };
            
            using (var reader = XmlReader.Create(_xmlFilePath, settings))
            {
                Product currentProduct = null;
                string currentElement = string.Empty;
                
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == "Product")
                            {
                                currentProduct = new Product();
                            }
                            else if (currentProduct != null)
                            {
                                currentElement = reader.Name;
                            }
                            break;
                            
                        case XmlNodeType.Text:
                            if (currentProduct != null)
                            {
                                switch (currentElement)
                                {
                                    case "Id":
                                        currentProduct.Id = int.Parse(reader.Value);
                                        break;
                                    case "Name":
                                        currentProduct.Name = reader.Value;
                                        break;
                                    case "Description":
                                        currentProduct.Description = reader.Value;
                                        break;
                                    case "Price":
                                        currentProduct.Price = decimal.Parse(reader.Value);
                                        break;
                                    case "CategoryId":
                                        currentProduct.CategoryId = int.Parse(reader.Value);
                                        break;
                                    case "ImageUrl":
                                        currentProduct.ImageUrl = reader.Value;
                                        break;
                                    case "StockQuantity":
                                        currentProduct.StockQuantity = int.Parse(reader.Value);
                                        break;
                                }
                            }
                            break;
                            
                        case XmlNodeType.EndElement:
                            if (reader.Name == "Product")
                            {
                                products.Add(currentProduct);
                                currentProduct = null;
                            }
                            break;
                    }
                }
            }
            
            return products;
        }
        
        // XmlDocument kullanarak okuma (DOM tabanlý) - tamamýný RAM'de tutar
        public List<Product> GetProductsUsingXmlDocument()
        {
            var products = new List<Product>();
            var doc = new XmlDocument();
            doc.Load(_xmlFilePath);
            
            XmlNodeList productNodes = doc.SelectNodes("//Product");
            
            foreach (XmlNode productNode in productNodes)
            {
                var product = new Product
                {
                    Id = int.Parse(productNode.SelectSingleNode("Id").InnerText),
                    Name = productNode.SelectSingleNode("Name").InnerText,
                    Description = productNode.SelectSingleNode("Description").InnerText,
                    Price = decimal.Parse(productNode.SelectSingleNode("Price").InnerText),
                    CategoryId = int.Parse(productNode.SelectSingleNode("CategoryId").InnerText),
                    ImageUrl = productNode.SelectSingleNode("ImageUrl").InnerText,
                    StockQuantity = int.Parse(productNode.SelectSingleNode("StockQuantity").InnerText)
                };

                // Ýndirim deðerini kontrol et ve varsa ekle
                var discountNode = productNode.SelectSingleNode("Discount");
                if (discountNode != null)
                {
                    product.Discount = decimal.Parse(discountNode.InnerText);
                }

                // Etiketleri topla
                var tagNodes = productNode.SelectNodes("Tag");
                if (tagNodes != null)
                {
                    foreach (XmlNode tagNode in tagNodes)
                    {
                        product.Tags.Add(tagNode.InnerText);
                    }
                }
                
                products.Add(product);
            }
            
            return products;
        }
        
        // XDocument kullanarak okuma (LINQ to XML) - modern, kullanýmý kolay
        public List<Product> GetProductsUsingXDocument()
        {
            var products = new List<Product>();
            var doc = XDocument.Load(_xmlFilePath);
            
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
        
        // Performans karþýlaþtýrma metodu
        public Dictionary<string, long> CompareXmlParsingPerformance()
        {
            var results = new Dictionary<string, long>();
            
            // XmlReader için DTD iþleme izni
            XmlReaderSettings settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Parse,
                ValidationType = ValidationType.DTD,
                XmlResolver = new XmlUrlResolver()
            };
            
            var stopwatch = new System.Diagnostics.Stopwatch();
            
            // XmlReader
            stopwatch.Start();
            var products1 = GetProductsUsingXmlReader();
            stopwatch.Stop();
            results["XmlReader"] = stopwatch.ElapsedMilliseconds;
            
            // XmlDocument
            stopwatch.Restart();
            var products2 = GetProductsUsingXmlDocument();
            stopwatch.Stop();
            results["XmlDocument"] = stopwatch.ElapsedMilliseconds;
            
            // XDocument
            stopwatch.Restart();
            var products3 = GetProductsUsingXDocument();
            stopwatch.Stop();
            results["XDocument"] = stopwatch.ElapsedMilliseconds;
            
            return results;
        }
    }
}