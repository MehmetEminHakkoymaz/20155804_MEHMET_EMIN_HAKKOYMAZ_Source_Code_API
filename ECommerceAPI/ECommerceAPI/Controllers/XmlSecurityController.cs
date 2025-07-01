using Microsoft.AspNetCore.Mvc;
using System.Xml;
using ECommerceAPI.Models;
using ECommerceAPI.Services;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class XmlSecurityController : ControllerBase
    {
        private readonly XmlProductService _productService;
        private readonly XmlSecurityService _securityService;
        
        public XmlSecurityController(XmlProductService productService, XmlSecurityService securityService)
        {
            _productService = productService;
            _securityService = securityService;
        }
        
        // XML Ýmzalama örneði
        [HttpGet("sign/{id}")]
        [Produces("application/xml")]
        public IActionResult SignProductXml(int id)
        {
            try
            {
                // Ürünü getir
                var product = _productService.GetProductById(id);
                if (product == null)
                    return NotFound();
                
                // Ürün XML'ini oluþtur
                var doc = new XmlDocument();
                XmlElement root = doc.CreateElement("Product");
                doc.AppendChild(root);
                
                AddElement(doc, root, "Id", product.Id.ToString());
                AddElement(doc, root, "Name", product.Name);
                AddElement(doc, root, "Description", product.Description);
                AddElement(doc, root, "Price", product.Price.ToString());
                AddElement(doc, root, "CategoryId", product.CategoryId.ToString());
                AddElement(doc, root, "StockQuantity", product.StockQuantity.ToString());
                
                // Ýmzala
                var signedDoc = _securityService.SignXmlDocument(doc);
                
                return Content(signedDoc.OuterXml, "application/xml");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ýmzalama hatasý: {ex.Message}");
            }
        }
        
        // Ýmza doðrulama örneði
        [HttpPost("verify")]
        [Consumes("application/xml")]
        [Produces("application/json")]
        public IActionResult VerifySignedXml([FromBody] string xmlContent)
        {
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(xmlContent);
                
                bool isValid = _securityService.VerifyXmlSignature(doc);
                
                return Ok(new { IsValid = isValid });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Doðrulama hatasý: {ex.Message}");
            }
        }
        
        // XML Þifreleme örneði
        [HttpGet("encrypt/{id}/{elementName}")]
        [Produces("application/xml")]
        public IActionResult EncryptProductElement(int id, string elementName)
        {
            try
            {
                // Ürünü getir
                var product = _productService.GetProductById(id);
                if (product == null)
                    return NotFound();
                
                // Ürün XML'ini oluþtur
                var doc = new XmlDocument();
                XmlElement root = doc.CreateElement("Product");
                doc.AppendChild(root);
                
                AddElement(doc, root, "Id", product.Id.ToString());
                AddElement(doc, root, "Name", product.Name);
                AddElement(doc, root, "Description", product.Description);
                AddElement(doc, root, "Price", product.Price.ToString());
                AddElement(doc, root, "CategoryId", product.CategoryId.ToString());
                AddElement(doc, root, "StockQuantity", product.StockQuantity.ToString());
                
                // Belirli bir elemaný þifrele
                var encryptedDoc = _securityService.EncryptXmlElement(doc, elementName);
                
                return Content(encryptedDoc.OuterXml, "application/xml");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Þifreleme hatasý: {ex.Message}");
            }
        }
        
        // Güvenli XML örneði (hem þifreli hem imzalý)
        [HttpGet("secure/{id}")]
        [Produces("application/xml")]
        public IActionResult SecureProduct(int id)
        {
            try
            {
                var product = _productService.GetProductById(id);
                if (product == null)
                    return NotFound();
                
                // Ürünü güvenli XML olarak al
                string secureXml = _securityService.SecureProduct(product);
                
                return Content(secureXml, "application/xml");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Güvenli XML iþleme hatasý: {ex.Message}");
            }
        }
        
        // Ýmza raporu oluþturma
        [HttpPost("report")]
        [Consumes("application/xml")]
        [Produces("application/xml")]
        public IActionResult GenerateSignatureReport([FromBody] string xmlContent)
        {
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(xmlContent);
                
                string report = _securityService.GenerateSignatureReport(doc);
                
                return Content(report, "application/xml");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Rapor oluþturma hatasý: {ex.Message}");
            }
        }
        
        private void AddElement(XmlDocument doc, XmlElement parent, string name, string value)
        {
            XmlElement element = doc.CreateElement(name);
            element.InnerText = value;
            parent.AppendChild(element);
        }
    }
}