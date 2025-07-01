using System.Xml;
using ECommerceAPI.Models;

namespace ECommerceAPI.Services
{
    public class AdvancedXmlService
    {
        // Öznitelikler, CDATA, yorum ve iþleme talimatlarý içeren geliþmiþ XML oluþturur
        public string GenerateAdvancedXml(Product product)
        {
            XmlDocument doc = new XmlDocument();
            
            // Ýþleme talimatý
            XmlProcessingInstruction pi = doc.CreateProcessingInstruction("xml-stylesheet", "type=\"text/xsl\" href=\"ProductView.xslt\"");
            doc.AppendChild(pi);
            
            XmlElement root = doc.CreateElement("Product");
            doc.AppendChild(root);
            
            // Öznitelikler ekleme
            XmlAttribute idAttr = doc.CreateAttribute("id");
            idAttr.Value = product.Id.ToString();
            root.Attributes.Append(idAttr);
            
            // Yorum ekleme
            XmlComment comment = doc.CreateComment("Ürün detaylarý aþaðýdadýr");
            root.AppendChild(comment);
            
            // Normal eleman
            XmlElement nameElement = doc.CreateElement("Name");
            nameElement.InnerText = product.Name;
            root.AppendChild(nameElement);
            
            // CDATA içeren eleman
            XmlElement descElement = doc.CreateElement("Description");
            XmlCDataSection cdata = doc.CreateCDataSection(product.Description);
            descElement.AppendChild(cdata);
            root.AppendChild(descElement);
            
            // Ýç içe elemanlar ve ad alanlarý
            XmlElement priceInfo = doc.CreateElement("PriceInfo");
            priceInfo.SetAttribute("xmlns:cur", "http://example.com/currency");
            root.AppendChild(priceInfo);
            
            XmlElement price = doc.CreateElement("cur:Price");
            price.InnerText = product.Price.ToString();
            priceInfo.AppendChild(price);
            
            return doc.OuterXml;
        }
        
        // Farklý XML biçimlerini gösteren karþýlaþtýrmalý örnek üretir
        public Dictionary<string, string> CompareDifferentXmlFormats(Product product)
        {
            var results = new Dictionary<string, string>();
            
            // Basit XML
            XmlDocument simpleDoc = new XmlDocument();
            XmlElement simpleRoot = simpleDoc.CreateElement("Product");
            simpleDoc.AppendChild(simpleRoot);
            
            XmlElement nameElem = simpleDoc.CreateElement("Name");
            nameElem.InnerText = product.Name;
            simpleRoot.AppendChild(nameElem);
            
            results["Simple"] = simpleDoc.OuterXml;
            
            // Öznitelikli XML
            XmlDocument attrDoc = new XmlDocument();
            XmlElement attrRoot = attrDoc.CreateElement("Product");
            attrDoc.AppendChild(attrRoot);
            
            XmlAttribute idAttr = attrDoc.CreateAttribute("id");
            idAttr.Value = product.Id.ToString();
            attrRoot.Attributes.Append(idAttr);
            
            XmlElement attrName = attrDoc.CreateElement("Name");
            attrName.InnerText = product.Name;
            attrRoot.AppendChild(attrName);
            
            results["Attributes"] = attrDoc.OuterXml;
            
            // Ad alanlý XML
            XmlDocument nsDoc = new XmlDocument();
            XmlElement nsRoot = nsDoc.CreateElement("p", "Product", "http://example.com/products");
            nsDoc.AppendChild(nsRoot);
            
            XmlElement nsName = nsDoc.CreateElement("p", "Name", "http://example.com/products");
            nsName.InnerText = product.Name;
            nsRoot.AppendChild(nsName);
            
            results["Namespaces"] = nsDoc.OuterXml;
            
            return results;
        }
    }
}