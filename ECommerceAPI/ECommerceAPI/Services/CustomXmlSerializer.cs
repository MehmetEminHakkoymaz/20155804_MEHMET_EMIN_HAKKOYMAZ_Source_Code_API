using System.Xml;
using System.Xml.Serialization;
using ECommerceAPI.Models;

namespace ECommerceAPI.Services
{
    public class CustomXmlSerializer
    {
        public string Serialize<T>(T obj, string rootName)
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", ""); // Default namespace'i temizle
            
            XmlSerializer serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(rootName));
            StringWriter writer = new StringWriter();
            serializer.Serialize(writer, obj, ns);
            return writer.ToString();
        }
        
        public T Deserialize<T>(string xml, string rootName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(rootName));
            using StringReader reader = new StringReader(xml);
            return (T)serializer.Deserialize(reader);
        }
        
        // Karmaþýk XML yapýlarý için özel serileþtirme örneði
        public string SerializeProductCatalog(List<Product> products, string catalogName)
        {
            XmlDocument doc = new XmlDocument();
            
            // Kök eleman
            XmlElement root = doc.CreateElement("Catalog");
            doc.AppendChild(root);
            
            // Ad alanlarý
            root.SetAttribute("xmlns:p", "http://example.com/products");
            root.SetAttribute("name", catalogName);
            root.SetAttribute("date", DateTime.Now.ToString("yyyy-MM-dd"));
            
            // Ürünler koleksiyonu
            XmlElement productsElement = doc.CreateElement("Products");
            root.AppendChild(productsElement);
            
            foreach (var product in products)
            {
                XmlElement productElem = doc.CreateElement("p:Product");
                productsElement.AppendChild(productElem);
                
                // ID öznitelik olarak ekle
                XmlAttribute idAttr = doc.CreateAttribute("id");
                idAttr.Value = product.Id.ToString();
                productElem.Attributes.Append(idAttr);
                
                // Temel özellikler
                AddElement(doc, productElem, "Name", product.Name);
                
                // CDATA kullanýmý
                XmlElement descElement = doc.CreateElement("Description");
                XmlCDataSection cdata = doc.CreateCDataSection(product.Description);
                descElement.AppendChild(cdata);
                productElem.AppendChild(descElement);
                
                // Ýç içe elemanlar
                XmlElement priceDetails = doc.CreateElement("PriceDetails");
                AddElement(doc, priceDetails, "Amount", product.Price.ToString());
                AddElement(doc, priceDetails, "Currency", "TRY");
                productElem.AppendChild(priceDetails);
                
                AddElement(doc, productElem, "Category", product.CategoryId.ToString());
                AddElement(doc, productElem, "Stock", product.StockQuantity.ToString());
            }
            
            return doc.OuterXml;
        }
        
        private void AddElement(XmlDocument doc, XmlElement parent, string name, string value)
        {
            XmlElement element = doc.CreateElement(name);
            element.InnerText = value;
            parent.AppendChild(element);
        }
    }
}