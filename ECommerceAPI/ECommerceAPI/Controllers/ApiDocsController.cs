using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Text;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApiDocsController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        
        public ApiDocsController(IWebHostEnvironment env) 
        {
            _env = env;
        }
        
        // API dok�mantasyonunu HTML format�nda sunar
        [HttpGet]
        [Produces("text/html")]
        public IActionResult GetApiDocs()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang='en'>");
            sb.AppendLine("<head>");
            sb.AppendLine("    <meta charset='UTF-8'>");
            sb.AppendLine("    <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
            sb.AppendLine("    <title>ECommerceAPI Dok�mantasyonu</title>");
            sb.AppendLine("    <style>");
            sb.AppendLine("        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; color: #333; line-height: 1.6; }");
            sb.AppendLine("        h1 { color: #0066cc; border-bottom: 1px solid #eee; padding-bottom: 10px; }");
            sb.AppendLine("        h2 { color: #0099ff; margin-top: 30px; }");
            sb.AppendLine("        h3 { color: #444; background-color: #f5f5f5; padding: 8px; border-radius: 4px; }");
            sb.AppendLine("        .method { margin-bottom: 15px; border-left: 4px solid #ddd; padding-left: 15px; }");
            sb.AppendLine("        .get { border-left-color: #61affe; }");
            sb.AppendLine("        .post { border-left-color: #49cc90; }");
            sb.AppendLine("        .put { border-left-color: #fca130; }");
            sb.AppendLine("        .delete { border-left-color: #f93e3e; }");
            sb.AppendLine("        .endpoint { font-family: monospace; background: #f0f0f0; padding: 5px; border-radius: 4px; }");
            sb.AppendLine("        pre { background: #f5f5f5; padding: 10px; border-radius: 4px; overflow-x: auto; }");
            sb.AppendLine("        .desc { margin-top: 5px; }");
            sb.AppendLine("        .auth { color: #ff9800; font-size: 0.9em; }");
            sb.AppendLine("        .param { margin-left: 15px; }");
            sb.AppendLine("        .param-name { font-weight: bold; }");
            sb.AppendLine("    </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("    <h1>ECommerceAPI Dok�mantasyonu</h1>");
            
            // �r�n API'leri
            sb.AppendLine("    <h2>�r�n API'leri</h2>");
            
            // GET /api/products
            sb.AppendLine("    <div class='method get'>");
            sb.AppendLine("        <h3>T�m �r�nleri Getir</h3>");
            sb.AppendLine("        <div class='endpoint'><strong>GET</strong> /api/products</div>");
            sb.AppendLine("        <div class='desc'>Sistemdeki t�m �r�nleri listeler.</div>");
            sb.AppendLine("        <p><strong>D�n��:</strong> �r�n listesi (200 OK)</p>");
            sb.AppendLine("    </div>");
            
            // GET /api/products/{id}
            sb.AppendLine("    <div class='method get'>");
            sb.AppendLine("        <h3>ID'ye G�re �r�n Getir</h3>");
            sb.AppendLine("        <div class='endpoint'><strong>GET</strong> /api/products/{id}</div>");
            sb.AppendLine("        <div class='desc'>Belirtilen ID'ye sahip �r�n� getirir.</div>");
            sb.AppendLine("        <p><strong>Parametreler:</strong></p>");
            sb.AppendLine("        <div class='param'><span class='param-name'>id</span> - �r�n ID'si (integer)</div>");
            sb.AppendLine("        <p><strong>D�n��:</strong> �r�n detaylar� (200 OK) veya 404 Not Found</p>");
            sb.AppendLine("    </div>");
            
            // GET /api/products/html
            sb.AppendLine("    <div class='method get'>");
            sb.AppendLine("        <h3>�r�nleri HTML Format�nda Getir</h3>");
            sb.AppendLine("        <div class='endpoint'><strong>GET</strong> /api/products/html</div>");
            sb.AppendLine("        <div class='desc'>T�m �r�nleri XSLT kullanarak HTML olarak formatlar.</div>");
            sb.AppendLine("        <p><strong>D�n��:</strong> HTML format�nda �r�n listesi (200 OK)</p>");
            sb.AppendLine("    </div>");
            
            // POST /api/products
            sb.AppendLine("    <div class='method post'>");
            sb.AppendLine("        <h3>Yeni �r�n Ekle</h3>");
            sb.AppendLine("        <div class='endpoint'><strong>POST</strong> /api/products</div>");
            sb.AppendLine("        <div class='auth'>Yetkilendirme: Admin rol� gereklidir</div>");
            sb.AppendLine("        <div class='desc'>Sisteme yeni bir �r�n ekler.</div>");
            sb.AppendLine("        <p><strong>�stek G�vdesi:</strong></p>");
            sb.AppendLine("        <pre>{<br>  \"name\": \"�r�n Ad�\",<br>  \"description\": \"�r�n a��klamas�\",<br>  \"price\": 99.99,<br>  \"categoryId\": 1,<br>  \"imageUrl\": \"/images/product.jpg\",<br>  \"stockQuantity\": 10<br>}</pre>");
            sb.AppendLine("        <p><strong>D�n��:</strong> Eklenen �r�n (201 Created)</p>");
            sb.AppendLine("    </div>");
            
            // PUT /api/products/{id}
            sb.AppendLine("    <div class='method put'>");
            sb.AppendLine("        <h3>�r�n G�ncelle</h3>");
            sb.AppendLine("        <div class='endpoint'><strong>PUT</strong> /api/products/{id}</div>");
            sb.AppendLine("        <div class='auth'>Yetkilendirme: Admin rol� gereklidir</div>");
            sb.AppendLine("        <div class='desc'>Belirtilen ID'ye sahip �r�n� g�nceller.</div>");
            sb.AppendLine("        <p><strong>Parametreler:</strong></p>");
            sb.AppendLine("        <div class='param'><span class='param-name'>id</span> - G�ncellenecek �r�n�n ID'si (integer)</div>");
            sb.AppendLine("        <p><strong>�stek G�vdesi:</strong></p>");
            sb.AppendLine("        <pre>{<br>  \"id\": 1,<br>  \"name\": \"G�ncellenmi� �r�n Ad�\",<br>  \"description\": \"G�ncellenmi� a��klama\",<br>  \"price\": 149.99,<br>  \"categoryId\": 2,<br>  \"imageUrl\": \"/images/updated-product.jpg\",<br>  \"stockQuantity\": 15<br>}</pre>");
            sb.AppendLine("        <p><strong>D�n��:</strong> 204 No Content, 404 Not Found veya 400 Bad Request</p>");
            sb.AppendLine("    </div>");
            
            // DELETE /api/products/{id}
            sb.AppendLine("    <div class='method delete'>");
            sb.AppendLine("        <h3>�r�n Sil</h3>");
            sb.AppendLine("        <div class='endpoint'><strong>DELETE</strong> /api/products/{id}</div>");
            sb.AppendLine("        <div class='auth'>Yetkilendirme: Admin rol� gereklidir</div>");
            sb.AppendLine("        <div class='desc'>Belirtilen ID'ye sahip �r�n� siler.</div>");
            sb.AppendLine("        <p><strong>Parametreler:</strong></p>");
            sb.AppendLine("        <div class='param'><span class='param-name'>id</span> - Silinecek �r�n�n ID'si (integer)</div>");
            sb.AppendLine("        <p><strong>D�n��:</strong> 204 No Content veya 404 Not Found</p>");
            sb.AppendLine("    </div>");
            
            // XML G�venli�i API'leri
            sb.AppendLine("    <h2>XML G�venli�i API'leri</h2>");
            
            // GET /api/xmlsecurity/sign/{id}
            sb.AppendLine("    <div class='method get'>");
            sb.AppendLine("        <h3>�r�n XML'ini �mzala</h3>");
            sb.AppendLine("        <div class='endpoint'><strong>GET</strong> /api/xmlsecurity/sign/{id}</div>");
            sb.AppendLine("        <div class='desc'>Belirtilen �r�n i�in dijital imzal� bir XML olu�turur.</div>");
            sb.AppendLine("        <p><strong>Parametreler:</strong></p>");
            sb.AppendLine("        <div class='param'><span class='param-name'>id</span> - �mzalanacak �r�n�n ID'si (integer)</div>");
            sb.AppendLine("        <p><strong>D�n��:</strong> �mzal� XML (200 OK) veya 404 Not Found</p>");
            sb.AppendLine("    </div>");
            
            // POST /api/xmlsecurity/verify
            sb.AppendLine("    <div class='method post'>");
            sb.AppendLine("        <h3>XML �mzas�n� Do�rula</h3>");
            sb.AppendLine("        <div class='endpoint'><strong>POST</strong> /api/xmlsecurity/verify</div>");
            sb.AppendLine("        <div class='desc'>G�nderilen XML'de yer alan dijital imzay� do�rular.</div>");
            sb.AppendLine("        <p><strong>��erik T�r�:</strong> application/xml</p>");
            sb.AppendLine("        <p><strong>�stek G�vdesi:</strong> �mzal� XML belgesi</p>");
            sb.AppendLine("        <p><strong>D�n��:</strong> JSON nesnesi (IsValid: true/false)</p>");
            sb.AppendLine("    </div>");
            
            // GET /api/xmlsecurity/encrypt/{id}/{elementName}
            sb.AppendLine("    <div class='method get'>");
            sb.AppendLine("        <h3>XML Eleman�n� �ifrele</h3>");
            sb.AppendLine("        <div class='endpoint'><strong>GET</strong> /api/xmlsecurity/encrypt/{id}/{elementName}</div>");
            sb.AppendLine("        <div class='desc'>Belirtilen �r�n�n se�ilen XML eleman�n� �ifreler.</div>");
            sb.AppendLine("        <p><strong>Parametreler:</strong></p>");
            sb.AppendLine("        <div class='param'><span class='param-name'>id</span> - �r�n ID'si (integer)</div>");
            sb.AppendLine("        <div class='param'><span class='param-name'>elementName</span> - �ifrelenecek eleman�n ad� (string)</div>");
            sb.AppendLine("        <p><strong>D�n��:</strong> �ifrelenmi� XML belgesi (200 OK)</p>");
            sb.AppendLine("    </div>");
            
            // GET /api/xmlsecurity/secure/{id}
            sb.AppendLine("    <div class='method get'>");
            sb.AppendLine("        <h3>G�venli �r�n XML'i</h3>");
            sb.AppendLine("        <div class='endpoint'><strong>GET</strong> /api/xmlsecurity/secure/{id}</div>");
            sb.AppendLine("        <div class='desc'>Belirtilen �r�n�n hassas alanlar�n� �ifrelenmi� ve belgeyi imzalanm�� �ekilde d�ner.</div>");
            sb.AppendLine("        <p><strong>Parametreler:</strong></p>");
            sb.AppendLine("        <div class='param'><span class='param-name'>id</span> - �r�n ID'si (integer)</div>");
            sb.AppendLine("        <p><strong>D�n��:</strong> Hem �ifrelenmi� hem imzalanm�� XML (200 OK)</p>");
            sb.AppendLine("    </div>");
            
            // Kimlik Do�rulama API'leri
            sb.AppendLine("    <h2>Kimlik Do�rulama API'leri</h2>");
            
            // POST /api/auth/login
            sb.AppendLine("    <div class='method post'>");
            sb.AppendLine("        <h3>Kullan�c� Giri�i</h3>");
            sb.AppendLine("        <div class='endpoint'><strong>POST</strong> /api/auth/login</div>");
            sb.AppendLine("        <div class='desc'>Kullan�c� giri�i yaparak JWT token al�r.</div>");
            sb.AppendLine("        <p><strong>�stek G�vdesi:</strong></p>");
            sb.AppendLine("        <pre>{<br>  \"username\": \"admin\",<br>  \"password\": \"password123\"<br>}</pre>");
            sb.AppendLine("        <p><strong>D�n��:</strong> JWT token (200 OK) veya 401 Unauthorized</p>");
            sb.AppendLine("    </div>");
            
            // Di�er API'ler
            sb.AppendLine("    <h2>Di�er API'ler</h2>");
            
            // GET /api/advancedxml/parsing/compare
            sb.AppendLine("    <div class='method get'>");
            sb.AppendLine("        <h3>XML Ayr��t�rma Y�ntemlerini Kar��la�t�r</h3>");
            sb.AppendLine("        <div class='endpoint'><strong>GET</strong> /api/advancedxml/parsing/compare</div>");
            sb.AppendLine("        <div class='desc'>Farkl� XML ayr��t�rma y�ntemlerinin performanslar�n� kar��la�t�r�r.</div>");
            sb.AppendLine("        <p><strong>D�n��:</strong> Kar��la�t�rma sonu�lar� (200 OK)</p>");
            sb.AppendLine("    </div>");
            
            // SOAP Bilgileri
            sb.AppendLine("    <h2>SOAP Web Servisi</h2>");
            sb.AppendLine("    <div>");
            sb.AppendLine("        <p>SOAP web servisi <code>/ProductService.asmx</code> adresinde yay�nlanmaktad�r.</p>");
            sb.AppendLine("        <p>WSDL belgesine <code>/ProductService.asmx?wsdl</code> adresinden eri�ilebilir.</p>");
            sb.AppendLine("        <p>SOAP servis bilgilerini <code>/api/soapinfo</code> endpoint'lerinden alabilirsiniz.</p>");
            sb.AppendLine("    </div>");
            
            // Test Ara�lar� Ba�lant�s�
            sb.AppendLine("    <h2>Test Ara�lar�</h2>");
            sb.AppendLine("    <div>");
            sb.AppendLine("        <p>API'leri test etmek i�in Postman koleksiyonu: <a href=\"/api/docs/postman\">/api/docs/postman</a></p>");
            sb.AppendLine("    </div>");
            
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            
            return Content(sb.ToString(), "text/html");
        }
        
        // Postman koleksiyonunu JSON format�nda sunar
        [HttpGet("postman")]
        [Produces("application/json")]
        public IActionResult GetPostmanCollection()
        {
            string jsonPath = Path.Combine(_env.ContentRootPath, "Documentation", "ECommerceAPI.postman_collection.json");
            
            if (!System.IO.File.Exists(jsonPath))
            {
                // E�er Postman koleksiyonu dosyas� yoksa, yeni olu�tur
                var postmanJson = CreatePostmanCollection();
                
                // Dizin yoksa olu�tur
                var docDir = Path.GetDirectoryName(jsonPath);
                if (!Directory.Exists(docDir))
                {
                    Directory.CreateDirectory(docDir);
                }
                
                // JSON'� dosyaya yaz
                System.IO.File.WriteAllText(jsonPath, postmanJson);
            }
            
            // Dosyay� oku ve g�nder
            string json = System.IO.File.ReadAllText(jsonPath);
            return Content(json, "application/json");
        }
        
        // Postman koleksiyonu olu�turma yard�mc� metodu
        private string CreatePostmanCollection()
        {
            return @"{
    ""info"": {
        ""_postman_id"": ""c7d78c25-8306-4428-a6e6-1c11b8e78091"",
        ""name"": ""ECommerceAPI"",
        ""description"": ""XML tabanl� e-ticaret API test koleksiyonu"",
        ""schema"": ""https://schema.getpostman.com/json/collection/v2.1.0/collection.json""
    },
    ""item"": [
        {
            ""name"": ""�r�nler"",
            ""item"": [
                {
                    ""name"": ""T�m �r�nleri Getir"",
                    ""request"": {
                        ""method"": ""GET"",
                        ""header"": [
                            {
                                ""key"": ""Accept"",
                                ""value"": ""application/json""
                            }
                        ],
                        ""url"": {
                            ""raw"": ""{{baseUrl}}/api/products"",
                            ""host"": [""{{baseUrl}}""],
                            ""path"": [""api"", ""products""]
                        },
                        ""description"": ""Sistemdeki t�m �r�nleri listeler""
                    },
                    ""response"": []
                },
                {
                    ""name"": ""T�m �r�nleri XML Olarak Getir"",
                    ""request"": {
                        ""method"": ""GET"",
                        ""header"": [
                            {
                                ""key"": ""Accept"",
                                ""value"": ""application/xml""
                            }
                        ],
                        ""url"": {
                            ""raw"": ""{{baseUrl}}/api/products"",
                            ""host"": [""{{baseUrl}}""],
                            ""path"": [""api"", ""products""]
                        },
                        ""description"": ""Sistemdeki t�m �r�nleri XML format�nda listeler""
                    },
                    ""response"": []
                },
                {
                    ""name"": ""ID'ye G�re �r�n Getir"",
                    ""request"": {
                        ""method"": ""GET"",
                        ""header"": [],
                        ""url"": {
                            ""raw"": ""{{baseUrl}}/api/products/1"",
                            ""host"": [""{{baseUrl}}""],
                            ""path"": [""api"", ""products"", ""1""]
                        },
                        ""description"": ""ID'ye g�re tek bir �r�n getirir""
                    },
                    ""response"": []
                },
                {
                    ""name"": ""HTML Format�nda �r�nler"",
                    ""request"": {
                        ""method"": ""GET"",
                        ""header"": [],
                        ""url"": {
                            ""raw"": ""{{baseUrl}}/api/products/html"",
                            ""host"": [""{{baseUrl}}""],
                            ""path"": [""api"", ""products"", ""html""]
                        },
                        ""description"": ""�r�nleri HTML format�nda g�r�nt�ler (XSLT d�n���m� kullan�l�r)""
                    },
                    ""response"": []
                },
                {
                    ""name"": ""Yeni �r�n Ekle"",
                    ""request"": {
                        ""method"": ""POST"",
                        ""header"": [
                            {
                                ""key"": ""Content-Type"",
                                ""value"": ""application/json""
                            },
                            {
                                ""key"": ""Authorization"",
                                ""value"": ""Bearer {{token}}""
                            }
                        ],
                        ""body"": {
                            ""mode"": ""raw"",
                            ""raw"": ""{\n  \""name\"": \""Test �r�n\"",\n  \""description\"": \""Bu bir test �r�n�d�r\"",\n  \""price\"": 299.99,\n  \""categoryId\"": 1,\n  \""imageUrl\"": \""/images/test.jpg\"",\n  \""stockQuantity\"": 25\n}""
                        },
                        ""url"": {
                            ""raw"": ""{{baseUrl}}/api/products"",
                            ""host"": [""{{baseUrl}}""],
                            ""path"": [""api"", ""products""]
                        },
                        ""description"": ""Yeni bir �r�n ekler (Yetkilendirme gerekli)""
                    },
                    ""response"": []
                },
                {
                    ""name"": ""�r�n G�ncelle"",
                    ""request"": {
                        ""method"": ""PUT"",
                        ""header"": [
                            {
                                ""key"": ""Content-Type"",
                                ""value"": ""application/json""
                            },
                            {
                                ""key"": ""Authorization"",
                                ""value"": ""Bearer {{token}}""
                            }
                        ],
                        ""body"": {
                            ""mode"": ""raw"",
                            ""raw"": ""{\n  \""id\"": 1,\n  \""name\"": \""G�ncellenmi� �r�n\"",\n  \""description\"": \""Bu �r�n g�ncellendi\"",\n  \""price\"": 399.99,\n  \""categoryId\"": 2,\n  \""imageUrl\"": \""/images/updated.jpg\"",\n  \""stockQuantity\"": 30\n}""
                        },
                        ""url"": {
                            ""raw"": ""{{baseUrl}}/api/products/1"",
                            ""host"": [""{{baseUrl}}""],
                            ""path"": [""api"", ""products"", ""1""]
                        },
                        ""description"": ""�r�n� g�nceller (Yetkilendirme gerekli)""
                    },
                    ""response"": []
                },
                {
                    ""name"": ""�r�n Sil"",
                    ""request"": {
                        ""method"": ""DELETE"",
                        ""header"": [
                            {
                                ""key"": ""Authorization"",
                                ""value"": ""Bearer {{token}}""
                            }
                        ],
                        ""url"": {
                            ""raw"": ""{{baseUrl}}/api/products/1"",
                            ""host"": [""{{baseUrl}}""],
                            ""path"": [""api"", ""products"", ""1""]
                        },
                        ""description"": ""�r�n� siler (Yetkilendirme gerekli)""
                    },
                    ""response"": []
                }
            ]
        },
        {
            ""name"": ""XML G�venli�i"",
            ""item"": [
                {
                    ""name"": ""XML �mzala"",
                    ""request"": {
                        ""method"": ""GET"",
                        ""header"": [],
                        ""url"": {
                            ""raw"": ""{{baseUrl}}/api/xmlsecurity/sign/1"",
                            ""host"": [""{{baseUrl}}""],
                            ""path"": [""api"", ""xmlsecurity"", ""sign"", ""1""]
                        },
                        ""description"": ""Belirtilen �r�n�n imzal� XML s�r�m�n� al�r""
                    },
                    ""response"": []
                },
                {
                    ""name"": ""XML �mzas�n� Do�rula"",
                    ""request"": {
                        ""method"": ""POST"",
                        ""header"": [
                            {
                                ""key"": ""Content-Type"",
                                ""value"": ""application/xml""
                            }
                        ],
                        ""body"": {
                            ""mode"": ""raw"",
                            ""raw"": ""<!-- Buraya imzalanm�� XML belgesi eklenecek -->""
                        },
                        ""url"": {
                            ""raw"": ""{{baseUrl}}/api/xmlsecurity/verify"",
                            ""host"": [""{{baseUrl}}""],
                            ""path"": [""api"", ""xmlsecurity"", ""verify""]
                        },
                        ""description"": ""Verilen XML belgesinin imzas�n� do�rular""
                    },
                    ""response"": []
                },
                {
                    ""name"": ""XML Eleman� �ifrele"",
                    ""request"": {
                        ""method"": ""GET"",
                        ""header"": [],
                        ""url"": {
                            ""raw"": ""{{baseUrl}}/api/xmlsecurity/encrypt/1/Price"",
                            ""host"": [""{{baseUrl}}""],
                            ""path"": [""api"", ""xmlsecurity"", ""encrypt"", ""1"", ""Price""]
                        },
                        ""description"": ""Belirtilen �r�n�n fiyat alan�n� �ifrelenmi� XML olarak d�nd�r�r""
                    },
                    ""response"": []
                },
                {
                    ""name"": ""G�venli �r�n XML"",
                    ""request"": {
                        ""method"": ""GET"",
                        ""header"": [],
                        ""url"": {
                            ""raw"": ""{{baseUrl}}/api/xmlsecurity/secure/1"",
                            ""host"": [""{{baseUrl}}""],
                            ""path"": [""api"", ""xmlsecurity"", ""secure"", ""1""]
                        },
                        ""description"": ""�r�n bilgilerini hem �ifrelenmi� hem imzalanm�� XML olarak al�r""
                    },
                    ""response"": []
                }
            ]
        },
        {
            ""name"": ""Kimlik Do�rulama"",
            ""item"": [
                {
                    ""name"": ""Login"",
                    ""event"": [
                        {
                            ""listen"": ""test"",
                            ""script"": {
                                ""exec"": [
                                    ""var jsonData = JSON.parse(responseBody);"",
                                    ""postman.setEnvironmentVariable('token', jsonData.token);""
                                ],
                                ""type"": ""text/javascript""
                            }
                        }
                    ],
                    ""request"": {
                        ""method"": ""POST"",
                        ""header"": [
                            {
                                ""key"": ""Content-Type"",
                                ""value"": ""application/json""
                            }
                        ],
                        ""body"": {
                            ""mode"": ""raw"",
                            ""raw"": ""{\n  \""username\"": \""admin\"",\n  \""password\"": \""password123\""\n}""
                        },
                        ""url"": {
                            ""raw"": ""{{baseUrl}}/api/auth/login"",
                            ""host"": [""{{baseUrl}}""],
                            ""path"": [""api"", ""auth"", ""login""]
                        },
                        ""description"": ""Kullan�c� giri�i yapar ve JWT token d�ner""
                    },
                    ""response"": []
                }
            ]
        },
        {
            ""name"": ""SOAP Bilgileri"",
            ""item"": [
                {
                    ""name"": ""SOAP A��klamas�"",
                    ""request"": {
                        ""method"": ""GET"",
                        ""header"": [],
                        ""url"": {
                            ""raw"": ""{{baseUrl}}/api/soapinfo/explanation"",
                            ""host"": [""{{baseUrl}}""],
                            ""path"": [""api"", ""soapinfo"", ""explanation""]
                        },
                        ""description"": ""SOAP yap�s� hakk�nda bilgiler""
                    },
                    ""response"": []
                }
            ]
        },
        {
            ""name"": ""API Belgeleri"",
            ""item"": [
                {
                    ""name"": ""HTML Dok�mantasyon"",
                    ""request"": {
                        ""method"": ""GET"",
                        ""header"": [],
                        ""url"": {
                            ""raw"": ""{{baseUrl}}/api/docs"",
                            ""host"": [""{{baseUrl}}""],
                            ""path"": [""api"", ""docs""]
                        },
                        ""description"": ""API dok�mantasyonunu HTML format�nda g�sterir""
                    },
                    ""response"": []
                }
            ]
        }
    ],
    ""event"": [
        {
            ""listen"": ""prerequest"",
            ""script"": {
                ""type"": ""text/javascript"",
                ""exec"": [
                    """"
                ]
            }
        },
        {
            ""listen"": ""test"",
            ""script"": {
                ""type"": ""text/javascript"",
                ""exec"": [
                    """"
                ]
            }
        }
    ],
    ""variable"": [
        {
            ""key"": ""baseUrl"",
            ""value"": ""http://localhost:5000"",
            ""type"": ""string""
        }
    ]
}";
        }
    }
}