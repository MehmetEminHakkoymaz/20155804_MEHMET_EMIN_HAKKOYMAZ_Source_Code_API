using Microsoft.AspNetCore.Mvc;
using ECommerceAPI.Services.Soap;
using System.Text;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SoapInfoController : ControllerBase
    {
        // SOAP servislerle ilgili bilgi sa�lar
        [HttpGet("explanation")]
        public IActionResult GetSoapExplanation()
        {
            var explanation = SoapClientExample.GetSoapStructureExplanation();
            return Ok(explanation);
        }
        
        // SOAP WSDL belgesi hakk�nda bilgi
        [HttpGet("wsdl-info")]
        public IActionResult GetWsdlInfo()
        {
            var info = new
            {
                Description = "WSDL (Web Services Description Language), SOAP web servislerinin aray�z�n� tan�mlayan XML belgesidir.",
                HowToAccess = "SOAP servisinin WSDL dosyas�na eri�mek i�in: http://localhost:5000/ProductService.asmx?wsdl",
                WsdlPurpose = "WSDL, servisin hangi metodlar� sundu�unu, parametrelerini ve d�n�� de�erlerini tan�mlar."
            };
            
            return Ok(info);
        }
        
        // SOAP test etme k�lavuzu
        [HttpGet("testing-guide")]
        public IActionResult GetSoapTestingGuide()
        {
            var guide = new StringBuilder()
                .AppendLine("SOAP Servisini Test Etme K�lavuzu:")
                .AppendLine("1. SOAP UI arac�n� indirin: https://www.soapui.org/downloads/soapui/")
                .AppendLine("2. Yeni SOAP projesi olu�turun ve WSDL URL'sini girin: http://localhost:5000/ProductService.asmx?wsdl")
                .AppendLine("3. Olu�turulan servis metodlar�ndan birini se�in (�rn. GetAllProducts)")
                .AppendLine("4. �stek parametrelerini d�zenleyin (gerekirse)")
                .AppendLine("5. �ste�i g�nderin ve yan�t� inceleyin")
                .AppendLine("\nAlternatif olarak, Postman arac�n� da kullanabilirsiniz. Ancak SOAP isteklerini manuel olarak olu�turman�z gerekir.");
                
            return Ok(guide.ToString());
        }
    }
}