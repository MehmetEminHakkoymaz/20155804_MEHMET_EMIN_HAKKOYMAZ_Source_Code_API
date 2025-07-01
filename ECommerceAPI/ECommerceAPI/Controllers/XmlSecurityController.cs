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
        
        // XML �mzalama �rne�i
        [HttpGet("sign/{id}")]
        [Produces("application/xml")]
        public IActionResult SignProductXml(int id)
        {
            try
            {
                // �r�n� getir
                var product = _productService.GetProductById(id);
                if (product == null)
                    return NotFound();
                
                // �r�n XML'ini olu�tur
                var doc = new XmlDocument();
                XmlElement root = doc.CreateElement("Product");
                doc.AppendChild(root);
                
                AddElement(doc, root, "Id", product.Id.ToString());
                AddElement(doc, root, "Name", product.Name);
                AddElement(doc, root, "Description", product.Description);
                AddElement(doc, root, "Price", product.Price.ToString());
                AddElement(doc, root, "CategoryId", product.CategoryId.ToString());
                AddElement(doc, root, "StockQuantity", product.StockQuantity.ToString());
                
                // �mzala
                var signedDoc = _securityService.SignXmlDocument(doc);
                
                return Content(signedDoc.OuterXml, "application/xml");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"�mzalama hatas�: {ex.Message}");
            }
        }
        
        // �mza do�rulama �rne�i
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
                return StatusCode(500, $"Do�rulama hatas�: {ex.Message}");
            }
        }
        
        // XML �ifreleme �rne�i
        [HttpGet("encrypt/{id}/{elementName}")]
        [Produces("application/xml")]
        public IActionResult EncryptProductElement(int id, string elementName)
        {
            try
            {
                // �r�n� getir
                var product = _productService.GetProductById(id);
                if (product == null)
                    return NotFound();
                
                // �r�n XML'ini olu�tur
                var doc = new XmlDocument();
                XmlElement root = doc.CreateElement("Product");
                doc.AppendChild(root);
                
                AddElement(doc, root, "Id", product.Id.ToString());
                AddElement(doc, root, "Name", product.Name);
                AddElement(doc, root, "Description", product.Description);
                AddElement(doc, root, "Price", product.Price.ToString());
                AddElement(doc, root, "CategoryId", product.CategoryId.ToString());
                AddElement(doc, root, "StockQuantity", product.StockQuantity.ToString());
                
                // Belirli bir eleman� �ifrele
                var encryptedDoc = _securityService.EncryptXmlElement(doc, elementName);
                
                return Content(encryptedDoc.OuterXml, "application/xml");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"�ifreleme hatas�: {ex.Message}");
            }
        }
        
        // G�venli XML �rne�i (hem �ifreli hem imzal�)
        [HttpGet("secure/{id}")]
        [Produces("application/xml")]
        public IActionResult SecureProduct(int id)
        {
            try
            {
                var product = _productService.GetProductById(id);
                if (product == null)
                    return NotFound();
                
                // �r�n� g�venli XML olarak al
                string secureXml = _securityService.SecureProduct(product);
                
                return Content(secureXml, "application/xml");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"G�venli XML i�leme hatas�: {ex.Message}");
            }
        }
        
        // �mza raporu olu�turma
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
                return StatusCode(500, $"Rapor olu�turma hatas�: {ex.Message}");
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