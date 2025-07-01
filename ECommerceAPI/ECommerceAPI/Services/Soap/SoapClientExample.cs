using System.ServiceModel;
using System.Xml;
using System.Text; // Encoding için eklendi
using System.Net.Http; // HttpClient ve StringContent için
using System.Net.Http.Headers; // MediaTypeHeaderValue için

namespace ECommerceAPI.Services.Soap
{
    public class SoapClientExample
    {
        // SOAP istemci örneði (sadece gösterim amaçlý)
        public static async Task<string> CallSoapServiceExample()
        {
            // SOAP isteði oluþtur
            var soapRequest = @"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" 
               xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
               xmlns:tns=""http://ecommerceapi.example.com/soap"">
    <soap:Header>
        <!-- Ýsteðe baðlý SOAP üstbilgileri buraya gelebilir -->
        <tns:AuthHeader>
            <tns:Username>admin</tns:Username>
            <tns:Password>password123</tns:Password>
        </tns:AuthHeader>
    </soap:Header>
    <soap:Body>
        <tns:GetAllProducts />
    </soap:Body>
</soap:Envelope>";

            // HttpClient ile SOAP çaðrýsý yapma
            using var client = new HttpClient();
            var content = new StringContent(soapRequest, Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");
            
            client.DefaultRequestHeaders.Add("SOAPAction", "http://ecommerceapi.example.com/soap/GetAllProducts");
            
            var response = await client.PostAsync("http://localhost:5000/ProductService.asmx", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            return responseContent;
        }
        
        // SOAP istekleri ve yanýtlarýnýn yapýsýný açýklayan bilgilendirme metodu
        public static string GetSoapStructureExplanation()
        {
            return @"SOAP Ýstek Yapýsý:
1. Envelope: Tüm SOAP mesajýný kapsayan ana element
2. Header: Ýsteðe baðlý üstbilgiler (kimlik doðrulama, oturum bilgileri vb.)
3. Body: SOAP mesajýnýn ana içeriði ve veriler

SOAP istek örneði:
<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
    <soap:Header>
        <!-- Ýsteðe baðlý baþlýk bilgileri -->
    </soap:Header>
    <soap:Body>
        <GetProductById xmlns=""http://ecommerceapi.example.com/soap"">
            <id>1</id>
        </GetProductById>
    </soap:Body>
</soap:Envelope>

SOAP yanýt örneði:
<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
    <soap:Body>
        <GetProductByIdResponse xmlns=""http://ecommerceapi.example.com/soap"">
            <GetProductByIdResult>
                <Id>1</Id>
                <Name>Akýllý Telefon X</Name>
                <Description>Geliþmiþ özelliklere sahip en son akýllý telefon</Description>
                <Price>999.99</Price>
                <!-- Diðer ürün özellikleri -->
            </GetProductByIdResult>
        </GetProductByIdResponse>
    </soap:Body>
</soap:Envelope>

SOAP Hata yanýtý örneði:
<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
    <soap:Body>
        <soap:Fault>
            <faultcode>soap:Client</faultcode>
            <faultstring>Ürün bulunamadý</faultstring>
            <detail>
                <error xmlns=""http://ecommerceapi.example.com/soap"">
                    <code>404</code>
                    <message>Belirtilen ID ile ürün bulunamadý</message>
                </error>
            </detail>
        </soap:Fault>
    </soap:Body>
</soap:Envelope>";
        }
    }
}