# ECommerceAPI - XML ve Web Servis Uygulamasý

Bu proje, XML teknolojileri ve web servisleri kullanan kapsamlý bir e-ticaret API'sidir. Hem RESTful API hem de SOAP web servisi üzerinden ürün yönetimi saðlar.

## Özellikler

- XML tabanlý veri depolama ve iþleme
- XSD ve DTD þemalarý ile XML doðrulama
- XSLT dönüþümleri (XML'den HTML'e)
- XPath sorgularý ile veri filtreleme
- XML þifreleme ve dijital imzalama (XMLDSig)
- Farklý XML ayrýþtýrma yöntemleri (DOM, SAX, LINQ to XML)
- REST API ve SOAP web servisleri
- JWT tabanlý kimlik doðrulama ve yetkilendirme
- Content Negotiation (XML/JSON)
- API versiyonlama

## API Dokümantasyonu

### Genel Bakýþ

- HTML API Dokümantasyonu: `/api/docs`
- Postman Koleksiyonu: `/api/docs/postman`
- SOAP WSDL Belgesi: `/ProductService.asmx?wsdl`

### Ürün API'leri

| Endpoint | Metod | Açýklama | Yetki |
|----------|-------|----------|-------|
| `/api/products` | GET | Tüm ürünleri listele | - |
| `/api/products/{id}` | GET | ID'ye göre ürün getir | - |
| `/api/products/html` | GET | Ürünleri HTML formatýnda getir | - |
| `/api/products` | POST | Yeni ürün ekle | Admin |
| `/api/products/{id}` | PUT | Ürünü güncelle | Admin |
| `/api/products/{id}` | DELETE | Ürünü sil | Admin |

### XML Güvenliði API'leri

| Endpoint | Metod | Açýklama | Format |
|----------|-------|----------|--------|
| `/api/xmlsecurity/sign/{id}` | GET | Ürün XML'ini imzala | XML |
| `/api/xmlsecurity/verify` | POST | XML imzasýný doðrula | XML->JSON |
| `/api/xmlsecurity/encrypt/{id}/{elementName}` | GET | XML elemanýný þifrele | XML |
| `/api/xmlsecurity/secure/{id}` | GET | Ýmzalý ve þifreli XML | XML |
| `/api/xmlsecurity/report` | POST | Ýmza raporu oluþtur | XML->XML |

### Kimlik Doðrulama API'leri

| Endpoint | Metod | Açýklama |
|----------|-------|----------|
| `/api/auth/login` | POST | JWT token al |
| `/api/auth/register` | POST | Yeni kullanýcý oluþtur |

## Örnek Kullaným

### REST API Örneði (JSON)

# Tüm ürünleri getir (JSON)
curl -X GET http://localhost:5000/api/products -H "Accept: application/json"

# ID'ye göre ürün getir
curl -X GET http://localhost:5000/api/products/1

# Oturum aç ve token al
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"password123"}'

# Yeni ürün ekle (token gerekli)
curl -X POST http://localhost:5000/api/products \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{"name":"Yeni Ürün","description":"Açýklama","price":199.99,"categoryId":1,"imageUrl":"/images/product.jpg","stockQuantity":10}'

### REST API Örneði (XML)

# Tüm ürünleri XML olarak getir
curl -X GET http://localhost:5000/api/products -H "Accept: application/xml"

# Ürün XML'ini imzala
curl -X GET http://localhost:5000/api/xmlsecurity/sign/1 -H "Accept: application/xml"

### SOAP API Kullanýmý

# SOAP UI veya Postman aracýlýðýyla:
1.	WSDL belgesini kullanarak client oluþtur: http://localhost:5000/ProductService.asmx?wsdl
2.	Ýstenen operasyonu seç (örn. GetAllProducts)
3.	Ýsteði gönder ve yanýtý al

### Kurulum

# Gereksinimler
•	.NET 8 SDK
•	Visual Studio 2022 veya baþka bir C# IDE

# Adýmlar
1.	Projeyi klonlayýn:
git clone https://github.com/username/ECommerceAPI.git

2.	Proje dizinine gidin:
cd ECommerceAPI
	
3.	Uygulamayý çalýþtýrýn:
dotnet run

4.	API http://localhost:5000 adresinde çalýþmaya baþlayacaktýr

5.	API dokümantasyonuna http://localhost:5000/api/docs adresinden eriþebilirsiniz

### Teknik Detaylar
•	C# 12
•	.NET 8
•	XML Ýþleme Kütüphaneleri (System.Xml, System.Xml.Linq)
•	XML Güvenlik Kütüphaneleri (System.Security.Cryptography.Xml)
•	ASP.NET Core Web API
•	SoapCore (SOAP hizmetleri için)
•	JWT Bearer Authentication

### XML Özellikleri
XML Validasyon
•	XSD (XML Schema Definition) ile güçlü tip kontrolü
•	DTD (Document Type Definition) ile yapýsal doðrulama
•	Validasyon hata yakalama ve raporlama
XML Güvenliði
•	XMLDSig ile dijital imzalama
•	XML Encryption ile veri þifreleme
•	Ýmza doðrulama ve raporlama
XML Dönüþüm ve Sorgulama
•	XSLT ile XML'den HTML'e dönüþüm
•	XPath sorgularý ile belirli verileri çýkarma
•	Farklý XML ayrýþtýrma tekniklerinin karþýlaþtýrmasý
Gelecek Geliþtirmeler
•	Mikroservis mimarisine geçiþ
•	GraphQL desteði
•	WebSocket desteði
•	Geliþmiþ test senaryolarý ve CI/CD entegrasyonu
Lisans
Bu proje MIT Lisansý altýnda lisanslanmýþtýr. Detaylar için LICENSE dosyasýna bakýnýz.
