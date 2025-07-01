using Microsoft.AspNetCore.Mvc;
using ECommerceAPI.Models;
using ECommerceAPI.Services;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdvancedXmlController : ControllerBase
    {
        private readonly XmlProductService _productService;
        private readonly XmlParsingService _parsingService;
        private readonly AdvancedXmlService _advancedXmlService;
        private readonly CustomXmlSerializer _customXmlSerializer;
        private readonly ExternalServiceClient _externalClient;
        
        public AdvancedXmlController(
            XmlProductService productService,
            XmlParsingService parsingService,
            AdvancedXmlService advancedXmlService,
            CustomXmlSerializer customXmlSerializer,
            ExternalServiceClient externalClient)
        {
            _productService = productService;
            _parsingService = parsingService;
            _advancedXmlService = advancedXmlService;
            _customXmlSerializer = customXmlSerializer;
            _externalClient = externalClient;
        }
        
        [HttpGet("parsing/compare")]
        [Produces("application/json")]
        public IActionResult CompareXmlParsingMethods()
        {
            var results = _parsingService.CompareXmlParsingPerformance();
            return Ok(results);
        }
        
        [HttpGet("advanced/{id}")]
        [Produces("application/xml")]
        public IActionResult GetAdvancedXml(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
                return NotFound();
                
            string xml = _advancedXmlService.GenerateAdvancedXml(product);
            return Content(xml, "application/xml");
        }
        
        [HttpGet("compare-formats/{id}")]
        [Produces("application/json")]
        public IActionResult CompareXmlFormats(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
                return NotFound();
                
            var results = _advancedXmlService.CompareDifferentXmlFormats(product);
            return Ok(results);
        }
        
        [HttpGet("custom-serialize")]
        [Produces("application/xml")]
        public IActionResult GetCustomSerialized()
        {
            var products = _productService.GetAllProducts();
            string xml = _customXmlSerializer.SerializeProductCatalog(products, "Örnek Katalog");
            return Content(xml, "application/xml");
        }
        
        [HttpGet("external")]
        [Produces("application/json")]
        public IActionResult GetExternalProducts()
        {
            try
            {
                var products = _externalClient.GetMockExternalProducts();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Dýþ servis hatasý: {ex.Message}");
            }
        }
    }
}