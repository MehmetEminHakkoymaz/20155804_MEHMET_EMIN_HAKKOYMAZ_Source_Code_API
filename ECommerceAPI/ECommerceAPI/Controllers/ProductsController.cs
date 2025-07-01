using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using ECommerceAPI.Models;
using ECommerceAPI.Services;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly XmlProductService _productService;
        private readonly XsltService _xsltService;
        
        public ProductsController(XmlProductService productService, XsltService xsltService)
        {
            _productService = productService;
            _xsltService = xsltService;
        }
        
        // GET api/products
        [HttpGet]
        public IActionResult GetProducts()
        {
            try
            {
                // Tüm ürünleri getir
                var products = _productService.GetAllProducts();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Sunucu hatası: {ex.Message}");
            }
        }
        
        // GET api/products/5
        [HttpGet("{id}")]
        public IActionResult GetProduct(int id)
        {
            try
            {
                // ID'ye göre ürün getir
                var product = _productService.GetProductById(id);
                if (product == null)
                    return NotFound();
                    
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Sunucu hatası: {ex.Message}");
            }
        }
        
        // GET api/products/html
        [HttpGet("html")]
        public IActionResult GetProductsAsHtml()
        {
            try
            {
                // XSLT dönüşümü ile HTML çıktısı al
                var doc = XDocument.Load(Path.Combine(
                    Directory.GetCurrentDirectory(), "XmlData", "Products.xml"));
                    
                var htmlContent = _xsltService.TransformToHtml(
                    doc.ToString(), "ProductsToHtml.xslt");
                    
                return Content(htmlContent, "text/html");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Sunucu hatası: {ex.Message}");
            }
        }
        
        // POST api/products
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult AddProduct([FromBody] Product product)
        {
            try
            {
                // Yeni ürün ekle
                _productService.AddProduct(product);
                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Sunucu hatası: {ex.Message}");
            }
        }
        
        // PUT api/products/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateProduct(int id, [FromBody] Product product)
        {
            try
            {
                // Ürün güncelleme
                if (id != product.Id)
                    return BadRequest("ID uyuşmazlığı");
                    
                var existingProduct = _productService.GetProductById(id);
                if (existingProduct == null)
                    return NotFound();
                    
                bool success = _productService.UpdateProduct(product);
                if (!success)
                    return StatusCode(500, "Ürün güncellenirken bir hata oluştu.");
                    
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Sunucu hatası: {ex.Message}");
            }
        }
        
        // DELETE api/products/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteProduct(int id)
        {
            try
            {
                // Ürün silme
                var product = _productService.GetProductById(id);
                if (product == null)
                    return NotFound();
                    
                _productService.DeleteProduct(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Sunucu hatası: {ex.Message}");
            }
        }
    }
}