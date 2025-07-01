using Microsoft.AspNetCore.Mvc;
using ECommerceAPI.Services.Soap;
using System.Text;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SoapInfoController : ControllerBase
    {
        // SOAP servislerle ilgili bilgi saðlar
        [HttpGet("explanation")]
        public IActionResult GetSoapExplanation()
        {
            var explanation = SoapClientExample.GetSoapStructureExplanation();
            return Ok(explanation);
        }
        
        // SOAP WSDL belgesi hakkýnda bilgi
        [HttpGet("wsdl-info")]
        public IActionResult GetWsdlInfo()
        {
            var info = new
            {
                Description = "WSDL (Web Services Description Language), SOAP web servislerinin arayüzünü tanýmlayan XML belgesidir.",
                HowToAccess = "SOAP servisinin WSDL dosyasýna eriþmek için: http://localhost:5000/ProductService.asmx?wsdl",
                WsdlPurpose = "WSDL, servisin hangi metodlarý sunduðunu, parametrelerini ve dönüþ deðerlerini tanýmlar."
            };
            
            return Ok(info);
        }
        
        // SOAP test etme kýlavuzu
        [HttpGet("testing-guide")]
        public IActionResult GetSoapTestingGuide()
        {
            var guide = new StringBuilder()
                .AppendLine("SOAP Servisini Test Etme Kýlavuzu:")
                .AppendLine("1. SOAP UI aracýný indirin: https://www.soapui.org/downloads/soapui/")
                .AppendLine("2. Yeni SOAP projesi oluþturun ve WSDL URL'sini girin: http://localhost:5000/ProductService.asmx?wsdl")
                .AppendLine("3. Oluþturulan servis metodlarýndan birini seçin (örn. GetAllProducts)")
                .AppendLine("4. Ýstek parametrelerini düzenleyin (gerekirse)")
                .AppendLine("5. Ýsteði gönderin ve yanýtý inceleyin")
                .AppendLine("\nAlternatif olarak, Postman aracýný da kullanabilirsiniz. Ancak SOAP isteklerini manuel olarak oluþturmanýz gerekir.");
                
            return Ok(guide.ToString());
        }
    }
}