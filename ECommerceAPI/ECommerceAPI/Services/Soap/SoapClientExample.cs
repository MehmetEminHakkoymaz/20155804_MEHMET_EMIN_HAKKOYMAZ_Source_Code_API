using System.ServiceModel;
using System.Xml;
using System.Text; // Encoding i�in eklendi
using System.Net.Http; // HttpClient ve StringContent i�in
using System.Net.Http.Headers; // MediaTypeHeaderValue i�in

namespace ECommerceAPI.Services.Soap
{
    public class SoapClientExample
    {
        // SOAP istemci �rne�i (sadece g�sterim ama�l�)
        public static async Task<string> CallSoapServiceExample()
        {
            // SOAP iste�i olu�tur
            var soapRequest = @"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" 
               xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
               xmlns:tns=""http://ecommerceapi.example.com/soap"">
    <soap:Header>
        <!-- �ste�e ba�l� SOAP �stbilgileri buraya gelebilir -->
        <tns:AuthHeader>
            <tns:Username>admin</tns:Username>
            <tns:Password>password123</tns:Password>
        </tns:AuthHeader>
    </soap:Header>
    <soap:Body>
        <tns:GetAllProducts />
    </soap:Body>
</soap:Envelope>";

            // HttpClient ile SOAP �a�r�s� yapma
            using var client = new HttpClient();
            var content = new StringContent(soapRequest, Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");
            
            client.DefaultRequestHeaders.Add("SOAPAction", "http://ecommerceapi.example.com/soap/GetAllProducts");
            
            var response = await client.PostAsync("http://localhost:5000/ProductService.asmx", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            return responseContent;
        }
        
        // SOAP istekleri ve yan�tlar�n�n yap�s�n� a��klayan bilgilendirme metodu
        public static string GetSoapStructureExplanation()
        {
            return @"SOAP �stek Yap�s�:
1. Envelope: T�m SOAP mesaj�n� kapsayan ana element
2. Header: �ste�e ba�l� �stbilgiler (kimlik do�rulama, oturum bilgileri vb.)
3. Body: SOAP mesaj�n�n ana i�eri�i ve veriler

SOAP istek �rne�i:
<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
    <soap:Header>
        <!-- �ste�e ba�l� ba�l�k bilgileri -->
    </soap:Header>
    <soap:Body>
        <GetProductById xmlns=""http://ecommerceapi.example.com/soap"">
            <id>1</id>
        </GetProductById>
    </soap:Body>
</soap:Envelope>

SOAP yan�t �rne�i:
<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
    <soap:Body>
        <GetProductByIdResponse xmlns=""http://ecommerceapi.example.com/soap"">
            <GetProductByIdResult>
                <Id>1</Id>
                <Name>Ak�ll� Telefon X</Name>
                <Description>Geli�mi� �zelliklere sahip en son ak�ll� telefon</Description>
                <Price>999.99</Price>
                <!-- Di�er �r�n �zellikleri -->
            </GetProductByIdResult>
        </GetProductByIdResponse>
    </soap:Body>
</soap:Envelope>

SOAP Hata yan�t� �rne�i:
<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
    <soap:Body>
        <soap:Fault>
            <faultcode>soap:Client</faultcode>
            <faultstring>�r�n bulunamad�</faultstring>
            <detail>
                <error xmlns=""http://ecommerceapi.example.com/soap"">
                    <code>404</code>
                    <message>Belirtilen ID ile �r�n bulunamad�</message>
                </error>
            </detail>
        </soap:Fault>
    </soap:Body>
</soap:Envelope>";
        }
    }
}