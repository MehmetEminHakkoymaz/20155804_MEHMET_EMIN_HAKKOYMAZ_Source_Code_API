using System.Xml;
using System.Xml.Xsl;

namespace ECommerceAPI.Services
{
    public class XsltService
    {
        private readonly IWebHostEnvironment _env;
        
        public XsltService(IWebHostEnvironment env)
        {
            _env = env;
        }
        
        public string TransformToHtml(string xmlData, string xsltFileName)
        {
            // XML verisini XSLT þablonu kullanarak HTML'e dönüþtür
            var xsltPath = Path.Combine(_env.ContentRootPath, "XsltTemplates", xsltFileName);
            
            var xslt = new XslCompiledTransform();
            xslt.Load(xsltPath);
            
            using var reader = XmlReader.Create(new StringReader(xmlData));
            using var writer = new StringWriter();
            xslt.Transform(reader, null, writer);
            
            return writer.ToString();
        }
    }
}