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
        
        // API dokümantasyonunu HTML formatýnda sunar
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
            sb.AppendLine("    <title>ECommerceAPI Dokümantasyonu</title>");
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
            sb.AppendLine("    <h1>ECommerceAPI Dokümantasyonu</h1>");
            
            // Ürün API'leri
            sb.AppendLine("    <h2>Ürün API'leri</h2>");
            
            // GET /api/products
            sb.AppendLine("    <div class='method get'>");
            sb.AppendLine("        <h3>Tüm Ürünleri Getir</h3>");
            sb.AppendLine("        <div class='endpoint'><strong>GET</strong> /api/products</div>");
            sb.AppendLine("        <div class='desc'>Sistemdeki tüm ürünleri listeler.</div>");
            sb.AppendLine("        <p><strong>Dönüþ:</strong> Ürün listesi (200 OK)</p>");
            sb.AppendLine("    </div>");
            
            // GET /api/products/{id}
            sb.AppendLine("    <div class='method get'>");
            sb.AppendLine("        <h3>ID'ye Göre Ürün Getir</h3>");
            sb.AppendLine("        <div class='endpoint'><strong>GET</strong> /api/products/{id}</div>");
            sb.AppendLine("        <div class='desc'>Belirtilen ID'ye sahip ürünü getirir.</div>");
            sb.AppendLine("        <p><strong>Parametreler:</strong></p>");
            sb.AppendLine("        <div class='param'><span class='param-name'>id</span> - Ürün ID'si (integer)</div>");
            sb.AppendLine("        <p><strong>Dönüþ:</strong> Ürün detaylarý (200 OK) veya 404 Not Found</p>");
            sb.AppendLine("    </div>");
            
            // GET /api/products/html
            sb.AppendLine("    <div class='method get'>");
            sb.AppendLine("        <h3>Ürünleri HTML Formatýnda Getir</h3>");
            sb.AppendLine("        <div class='endpoint'><strong>GET</strong> /api/products/html</div>");
            sb.AppendLine("        <div class='desc'>Tüm ürünleri XSLT kullanarak HTML olarak formatlar.</div>");
            sb.AppendLine("        <p><strong>Dönüþ:</strong> HTML formatýnda ürün listesi (200 OK)</p>");
            sb.AppendLine("    </div>");
            
            // POST /api/products
            sb.AppendLine("    <div class='method post'>");
            sb.AppendLine("        <h3>Yeni Ürün Ekle</h3>");
            sb.AppendLine("        <div class='endpoint'><strong>POST</strong> /api/products</div>");
            sb.AppendLine("        <div class='auth'>Yetkilendirme: Admin rolü gereklidir</div>");
            sb.AppendLine("        <div class='desc'>Sisteme yeni bir ürün ekler.</div>");
            sb.AppendLine("        <p><strong>Ýstek Gövdesi:</strong></p>");
            sb.AppendLine("        <pre>{<br>  \"name\": \"Ürün Adý\",<br>  \"description\": \"Ürün açýklamasý\",<br>  \"price\": 99.99,<br>  \"categoryId\": 1,<br>  \"imageUrl\": \"/images/product.jpg\",<br>  \"stockQuantity\": 10<br>}</pre>");
            sb.AppendLine("        <p><strong>Dönüþ:</strong> Eklenen ürün (201 Created)</p>");
            sb.AppendLine("    </div>");
            
            // PUT /api/products/{id}
            sb.AppendLine("    <div class='method put'>");
            sb.AppendLine("        <h3>Ürün Güncelle</h3>");
            sb.AppendLine("        <div class='endpoint'><strong>PUT</strong> /api/products/{id}</div>");
            sb.AppendLine("        <div class='auth'>Yetkilendirme: Admin rolü gereklidir</div>");
            sb.AppendLine("        <div class='desc'>Belirtilen ID'ye sahip ürünü günceller.</div>");
            sb.AppendLine("        <p><strong>Parametreler:</strong></p>");
            sb.AppendLine("        <div class='param'><span class='param-name'>id</span> - Güncellenecek ürünün ID'si (integer)</div>");
            sb.AppendLine("        <p><strong>Ýstek Gövdesi:</strong></p>");
            sb.AppendLine("        <pre>{<br>  \"id\": 1,<br>  \"name\": \"Güncellenmiþ Ürün Adý\",<br>  \"description\": \"Güncellenmiþ açýklama\",<br>  \"price\": 149.99,<br>  \"categoryId\": 2,<br>  \"imageUrl\": \"/images/updated-product.jpg\",<br>  \"stockQuantity\": 15<br>}</pre>");
            sb.AppendLine("        <p><strong>Dönüþ:</strong> 204 No Content, 404 Not Found veya 400 Bad Request</p>");
            sb.AppendLine("    </div>");
            
            // DELETE /api/products/{id}
            sb.AppendLine("    <div class='method delete'>");
            sb.AppendLine("        <h3>Ürün Sil</h3>");
            sb.AppendLine("        <div class='endpoint'><strong>DELETE</strong> /api/products/{id}</div>");
            sb.AppendLine("        <div class='auth'>Yetkilendirme: Admin rolü gereklidir</div>");
            sb.AppendLine("        <div class='desc'>Belirtilen ID'ye sahip ürünü siler.</div>");
            sb.AppendLine("        <p><strong>Parametreler:</strong></p>");
            sb.AppendLine("        <div class='param'><span class='param-name'>id</span> - Silinecek ürünün ID'si (integer)</div>");
            sb.AppendLine("        <p><strong>Dönüþ:</strong> 204 No Content veya 404 Not Found</p>");
            sb.AppendLine("    </div>");
            
            // XML Güvenliði API'leri
            sb.AppendLine("    <h2>XML Güvenliði API'leri</h2>");
            
            // GET /api/xmlsecurity/sign/{id}
            sb.AppendLine("    <div class='method get'>");
            sb.AppendLine("        <h3>Ürün XML'ini Ýmzala</h3>");
            sb.AppendLine("        <div class='endpoint'><strong>GET</strong> /api/xmlsecurity/sign/{id}</div>");
            sb.AppendLine("        <div class='desc'>Belirtilen ürün için dijital imzalý bir XML oluþturur.</div>");
            sb.AppendLine("        <p><strong>Parametreler:</strong></p>");
            sb.AppendLine("        <div class='param'><span class='param-name'>id</span> - Ýmzalanacak ürünün ID'si (integer)</div>");
            sb.AppendLine("        <p><strong>Dönüþ:</strong> Ýmzalý XML (200 OK) veya 404 Not Found</p>");
            sb.AppendLine("    </div>");
            
            // POST /api/xmlsecurity/verify
            sb.AppendLine("    <div class='method post'>");
            sb.AppendLine("        <h3>XML Ýmzasýný Doðrula</h3>");
            sb.AppendLine("        <div class='endpoint'><strong>POST</strong> /api/xmlsecurity/verify</div>");
            sb.AppendLine("        <div class='desc'>Gönderilen XML'de yer alan dijital imzayý doðrular.</div>");
            sb.AppendLine("        <p><strong>Ýçerik Türü:</strong> application/xml</p>");
            sb.AppendLine("        <p><strong>Ýstek Gövdesi:</strong> Ýmzalý XML belgesi</p>");
            sb.AppendLine("        <p><strong>Dönüþ:</strong> JSON nesnesi (IsValid: true/false)</p>");
            sb.AppendLine("    </div>");
            
            // GET /api/xmlsecurity/encrypt/{id}/{elementName}
            sb.AppendLine("    <div class='method get'>");
            sb.AppendLine("        <h3>XML Elemanýný Þifrele</h3>");
            sb.AppendLine("        <div class='endpoint'><strong>GET</strong> /api/xmlsecurity/encrypt/{id}/{elementName}</div>");
            sb.AppendLine("        <div class='desc'>Belirtilen ürünün seçilen XML elemanýný þifreler.</div>");
            sb.AppendLine("        <p><strong>Parametreler:</strong></p>");
            sb.AppendLine("        <div class='param'><span class='param-name'>id</span> - Ürün ID'si (integer)</div>");
            sb.AppendLine("        <div class='param'><span class='param-name'>elementName</span> - Þifrelenecek elemanýn adý (string)</div>");
            sb.AppendLine("        <p><strong>Dönüþ:</strong> Þifrelenmiþ XML belgesi (200 OK)</p>");
            sb.AppendLine("    </div>");
            
            // GET /api/xmlsecurity/secure/{id}
            sb.AppendLine("    <div class='method get'>");
            sb.AppendLine("        <h3>Güvenli Ürün XML'i</h3>");
            sb.AppendLine("        <div class='endpoint'><strong>GET</strong> /api/xmlsecurity/secure/{id}</div>");
            sb.AppendLine("        <div class='desc'>Belirtilen ürünün hassas alanlarýný þifrelenmiþ ve belgeyi imzalanmýþ þekilde döner.</div>");
            sb.AppendLine("        <p><strong>Parametreler:</strong></p>");
            sb.AppendLine("        <div class='param'><span class='param-name'>id</span> - Ürün ID'si (integer)</div>");
            sb.AppendLine("        <p><strong>Dönüþ:</strong> Hem þifrelenmiþ hem imzalanmýþ XML (200 OK)</p>");
            sb.AppendLine("    </div>");
            
            // Kimlik Doðrulama API'leri
            sb.AppendLine("    <h2>Kimlik Doðrulama API'leri</h2>");
            
            // POST /api/auth/login
            sb.AppendLine("    <div class='method post'>");
            sb.AppendLine("        <h3>Kullanýcý Giriþi</h3>");
            sb.AppendLine("        <div class='endpoint'><strong>POST</strong> /api/auth/login</div>");
            sb.AppendLine("        <div class='desc'>Kullanýcý giriþi yaparak JWT token alýr.</div>");
            sb.AppendLine("        <p><strong>Ýstek Gövdesi:</strong></p>");
            sb.AppendLine("        <pre>{<br>  \"username\": \"admin\",<br>  \"password\": \"password123\"<br>}</pre>");
            sb.AppendLine("        <p><strong>Dönüþ:</strong> JWT token (200 OK) veya 401 Unauthorized</p>");
            sb.AppendLine("    </div>");
            
            // Diðer API'ler
            sb.AppendLine("    <h2>Diðer API'ler</h2>");
            
            // GET /api/advancedxml/parsing/compare
            sb.AppendLine("    <div class='method get'>");
            sb.AppendLine("        <h3>XML Ayrýþtýrma Yöntemlerini Karþýlaþtýr</h3>");
            sb.AppendLine("        <div class='endpoint'><strong>GET</strong> /api/advancedxml/parsing/compare</div>");
            sb.AppendLine("        <div class='desc'>Farklý XML ayrýþtýrma yöntemlerinin performanslarýný karþýlaþtýrýr.</div>");
            sb.AppendLine("        <p><strong>Dönüþ:</strong> Karþýlaþtýrma sonuçlarý (200 OK)</p>");
            sb.AppendLine("    </div>");
            
            // SOAP Bilgileri
            sb.AppendLine("    <h2>SOAP Web Servisi</h2>");
            sb.AppendLine("    <div>");
            sb.AppendLine("        <p>SOAP web servisi <code>/ProductService.asmx</code> adresinde yayýnlanmaktadýr.</p>");
            sb.AppendLine("        <p>WSDL belgesine <code>/ProductService.asmx?wsdl</code> adresinden eriþilebilir.</p>");
            sb.AppendLine("        <p>SOAP servis bilgilerini <code>/api/soapinfo</code> endpoint'lerinden alabilirsiniz.</p>");
            sb.AppendLine("    </div>");
            
            // Test Araçlarý Baðlantýsý
            sb.AppendLine("    <h2>Test Araçlarý</h2>");
            sb.AppendLine("    <div>");
            sb.AppendLine("        <p>API'leri test etmek için Postman koleksiyonu: <a href=\"/api/docs/postman\">/api/docs/postman</a></p>");
            sb.AppendLine("    </div>");
            
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            
            return Content(sb.ToString(), "text/html");
        }
        
        // Postman koleksiyonunu JSON formatýnda sunar
        [HttpGet("postman")]
        [Produces("application/json")]
        public IActionResult GetPostmanCollection()
        {
            string jsonPath = Path.Combine(_env.ContentRootPath, "Documentation", "ECommerceAPI.postman_collection.json");
            
            if (!System.IO.File.Exists(jsonPath))
            {
                // Eðer Postman koleksiyonu dosyasý yoksa, yeni oluþtur
                var postmanJson = CreatePostmanCollection();
                
                // Dizin yoksa oluþtur
                var docDir = Path.GetDirectoryName(jsonPath);
                if (!Directory.Exists(docDir))
                {
                    Directory.CreateDirectory(docDir);
                }
                
                // JSON'ý dosyaya yaz
                System.IO.File.WriteAllText(jsonPath, postmanJson);
            }
            
            // Dosyayý oku ve gönder
            string json = System.IO.File.ReadAllText(jsonPath);
            return Content(json, "application/json");
        }
        
        // Postman koleksiyonu oluþturma yardýmcý metodu
        private string CreatePostmanCollection()
        {
            return @"{
    ""info"": {
        ""_postman_id"": ""c7d78c25-8306-4428-a6e6-1c11b8e78091"",
        ""name"": ""ECommerceAPI"",
        ""description"": ""XML tabanlý e-ticaret API test koleksiyonu"",
        ""schema"": ""https://schema.getpostman.com/json/collection/v2.1.0/collection.json""
    },
    ""item"": [
        {
            ""name"": ""Ürünler"",
            ""item"": [
                {
                    ""name"": ""Tüm Ürünleri Getir"",
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
                        ""description"": ""Sistemdeki tüm ürünleri listeler""
                    },
                    ""response"": []
                },
                {
                    ""name"": ""Tüm Ürünleri XML Olarak Getir"",
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
                        ""description"": ""Sistemdeki tüm ürünleri XML formatýnda listeler""
                    },
                    ""response"": []
                },
                {
                    ""name"": ""ID'ye Göre Ürün Getir"",
                    ""request"": {
                        ""method"": ""GET"",
                        ""header"": [],
                        ""url"": {
                            ""raw"": ""{{baseUrl}}/api/products/1"",
                            ""host"": [""{{baseUrl}}""],
                            ""path"": [""api"", ""products"", ""1""]
                        },
                        ""description"": ""ID'ye göre tek bir ürün getirir""
                    },
                    ""response"": []
                },
                {
                    ""name"": ""HTML Formatýnda Ürünler"",
                    ""request"": {
                        ""method"": ""GET"",
                        ""header"": [],
                        ""url"": {
                            ""raw"": ""{{baseUrl}}/api/products/html"",
                            ""host"": [""{{baseUrl}}""],
                            ""path"": [""api"", ""products"", ""html""]
                        },
                        ""description"": ""Ürünleri HTML formatýnda görüntüler (XSLT dönüþümü kullanýlýr)""
                    },
                    ""response"": []
                },
                {
                    ""name"": ""Yeni Ürün Ekle"",
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
                            ""raw"": ""{\n  \""name\"": \""Test Ürün\"",\n  \""description\"": \""Bu bir test ürünüdür\"",\n  \""price\"": 299.99,\n  \""categoryId\"": 1,\n  \""imageUrl\"": \""/images/test.jpg\"",\n  \""stockQuantity\"": 25\n}""
                        },
                        ""url"": {
                            ""raw"": ""{{baseUrl}}/api/products"",
                            ""host"": [""{{baseUrl}}""],
                            ""path"": [""api"", ""products""]
                        },
                        ""description"": ""Yeni bir ürün ekler (Yetkilendirme gerekli)""
                    },
                    ""response"": []
                },
                {
                    ""name"": ""Ürün Güncelle"",
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
                            ""raw"": ""{\n  \""id\"": 1,\n  \""name\"": \""Güncellenmiþ Ürün\"",\n  \""description\"": \""Bu ürün güncellendi\"",\n  \""price\"": 399.99,\n  \""categoryId\"": 2,\n  \""imageUrl\"": \""/images/updated.jpg\"",\n  \""stockQuantity\"": 30\n}""
                        },
                        ""url"": {
                            ""raw"": ""{{baseUrl}}/api/products/1"",
                            ""host"": [""{{baseUrl}}""],
                            ""path"": [""api"", ""products"", ""1""]
                        },
                        ""description"": ""Ürünü günceller (Yetkilendirme gerekli)""
                    },
                    ""response"": []
                },
                {
                    ""name"": ""Ürün Sil"",
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
                        ""description"": ""Ürünü siler (Yetkilendirme gerekli)""
                    },
                    ""response"": []
                }
            ]
        },
        {
            ""name"": ""XML Güvenliði"",
            ""item"": [
                {
                    ""name"": ""XML Ýmzala"",
                    ""request"": {
                        ""method"": ""GET"",
                        ""header"": [],
                        ""url"": {
                            ""raw"": ""{{baseUrl}}/api/xmlsecurity/sign/1"",
                            ""host"": [""{{baseUrl}}""],
                            ""path"": [""api"", ""xmlsecurity"", ""sign"", ""1""]
                        },
                        ""description"": ""Belirtilen ürünün imzalý XML sürümünü alýr""
                    },
                    ""response"": []
                },
                {
                    ""name"": ""XML Ýmzasýný Doðrula"",
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
                            ""raw"": ""<!-- Buraya imzalanmýþ XML belgesi eklenecek -->""
                        },
                        ""url"": {
                            ""raw"": ""{{baseUrl}}/api/xmlsecurity/verify"",
                            ""host"": [""{{baseUrl}}""],
                            ""path"": [""api"", ""xmlsecurity"", ""verify""]
                        },
                        ""description"": ""Verilen XML belgesinin imzasýný doðrular""
                    },
                    ""response"": []
                },
                {
                    ""name"": ""XML Elemaný Þifrele"",
                    ""request"": {
                        ""method"": ""GET"",
                        ""header"": [],
                        ""url"": {
                            ""raw"": ""{{baseUrl}}/api/xmlsecurity/encrypt/1/Price"",
                            ""host"": [""{{baseUrl}}""],
                            ""path"": [""api"", ""xmlsecurity"", ""encrypt"", ""1"", ""Price""]
                        },
                        ""description"": ""Belirtilen ürünün fiyat alanýný þifrelenmiþ XML olarak döndürür""
                    },
                    ""response"": []
                },
                {
                    ""name"": ""Güvenli Ürün XML"",
                    ""request"": {
                        ""method"": ""GET"",
                        ""header"": [],
                        ""url"": {
                            ""raw"": ""{{baseUrl}}/api/xmlsecurity/secure/1"",
                            ""host"": [""{{baseUrl}}""],
                            ""path"": [""api"", ""xmlsecurity"", ""secure"", ""1""]
                        },
                        ""description"": ""Ürün bilgilerini hem þifrelenmiþ hem imzalanmýþ XML olarak alýr""
                    },
                    ""response"": []
                }
            ]
        },
        {
            ""name"": ""Kimlik Doðrulama"",
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
                        ""description"": ""Kullanýcý giriþi yapar ve JWT token döner""
                    },
                    ""response"": []
                }
            ]
        },
        {
            ""name"": ""SOAP Bilgileri"",
            ""item"": [
                {
                    ""name"": ""SOAP Açýklamasý"",
                    ""request"": {
                        ""method"": ""GET"",
                        ""header"": [],
                        ""url"": {
                            ""raw"": ""{{baseUrl}}/api/soapinfo/explanation"",
                            ""host"": [""{{baseUrl}}""],
                            ""path"": [""api"", ""soapinfo"", ""explanation""]
                        },
                        ""description"": ""SOAP yapýsý hakkýnda bilgiler""
                    },
                    ""response"": []
                }
            ]
        },
        {
            ""name"": ""API Belgeleri"",
            ""item"": [
                {
                    ""name"": ""HTML Dokümantasyon"",
                    ""request"": {
                        ""method"": ""GET"",
                        ""header"": [],
                        ""url"": {
                            ""raw"": ""{{baseUrl}}/api/docs"",
                            ""host"": [""{{baseUrl}}""],
                            ""path"": [""api"", ""docs""]
                        },
                        ""description"": ""API dokümantasyonunu HTML formatýnda gösterir""
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