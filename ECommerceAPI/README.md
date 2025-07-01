# ECommerceAPI - XML ve Web Servis Uygulamas�

Bu proje, XML teknolojileri ve web servisleri kullanan kapsaml� bir e-ticaret API'sidir. Hem RESTful API hem de SOAP web servisi �zerinden �r�n y�netimi sa�lar.

## �zellikler

- XML tabanl� veri depolama ve i�leme
- XSD ve DTD �emalar� ile XML do�rulama
- XSLT d�n���mleri (XML'den HTML'e)
- XPath sorgular� ile veri filtreleme
- XML �ifreleme ve dijital imzalama (XMLDSig)
- Farkl� XML ayr��t�rma y�ntemleri (DOM, SAX, LINQ to XML)
- REST API ve SOAP web servisleri
- JWT tabanl� kimlik do�rulama ve yetkilendirme
- Content Negotiation (XML/JSON)
- API versiyonlama

## API Dok�mantasyonu

### Genel Bak��

- HTML API Dok�mantasyonu: `/api/docs`
- Postman Koleksiyonu: `/api/docs/postman`
- SOAP WSDL Belgesi: `/ProductService.asmx?wsdl`

### �r�n API'leri

| Endpoint | Metod | A��klama | Yetki |
|----------|-------|----------|-------|
| `/api/products` | GET | T�m �r�nleri listele | - |
| `/api/products/{id}` | GET | ID'ye g�re �r�n getir | - |
| `/api/products/html` | GET | �r�nleri HTML format�nda getir | - |
| `/api/products` | POST | Yeni �r�n ekle | Admin |
| `/api/products/{id}` | PUT | �r�n� g�ncelle | Admin |
| `/api/products/{id}` | DELETE | �r�n� sil | Admin |

### XML G�venli�i API'leri

| Endpoint | Metod | A��klama | Format |
|----------|-------|----------|--------|
| `/api/xmlsecurity/sign/{id}` | GET | �r�n XML'ini imzala | XML |
| `/api/xmlsecurity/verify` | POST | XML imzas�n� do�rula | XML->JSON |
| `/api/xmlsecurity/encrypt/{id}/{elementName}` | GET | XML eleman�n� �ifrele | XML |
| `/api/xmlsecurity/secure/{id}` | GET | �mzal� ve �ifreli XML | XML |
| `/api/xmlsecurity/report` | POST | �mza raporu olu�tur | XML->XML |

### Kimlik Do�rulama API'leri

| Endpoint | Metod | A��klama |
|----------|-------|----------|
| `/api/auth/login` | POST | JWT token al |
| `/api/auth/register` | POST | Yeni kullan�c� olu�tur |

## �rnek Kullan�m

### REST API �rne�i (JSON)

# T�m �r�nleri getir (JSON)
curl -X GET http://localhost:5000/api/products -H "Accept: application/json"

# ID'ye g�re �r�n getir
curl -X GET http://localhost:5000/api/products/1

# Oturum a� ve token al
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"password123"}'

# Yeni �r�n ekle (token gerekli)
curl -X POST http://localhost:5000/api/products \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{"name":"Yeni �r�n","description":"A��klama","price":199.99,"categoryId":1,"imageUrl":"/images/product.jpg","stockQuantity":10}'

### REST API �rne�i (XML)

# T�m �r�nleri XML olarak getir
curl -X GET http://localhost:5000/api/products -H "Accept: application/xml"

# �r�n XML'ini imzala
curl -X GET http://localhost:5000/api/xmlsecurity/sign/1 -H "Accept: application/xml"

### SOAP API Kullan�m�

# SOAP UI veya Postman arac�l���yla:
1.	WSDL belgesini kullanarak client olu�tur: http://localhost:5000/ProductService.asmx?wsdl
2.	�stenen operasyonu se� (�rn. GetAllProducts)
3.	�ste�i g�nder ve yan�t� al

### Kurulum

# Gereksinimler
�	.NET 8 SDK
�	Visual Studio 2022 veya ba�ka bir C# IDE

# Ad�mlar
1.	Projeyi klonlay�n:
git clone https://github.com/username/ECommerceAPI.git

2.	Proje dizinine gidin:
cd ECommerceAPI
	
3.	Uygulamay� �al��t�r�n:
dotnet run

4.	API http://localhost:5000 adresinde �al��maya ba�layacakt�r

5.	API dok�mantasyonuna http://localhost:5000/api/docs adresinden eri�ebilirsiniz

### Teknik Detaylar
�	C# 12
�	.NET 8
�	XML ��leme K�t�phaneleri (System.Xml, System.Xml.Linq)
�	XML G�venlik K�t�phaneleri (System.Security.Cryptography.Xml)
�	ASP.NET Core Web API
�	SoapCore (SOAP hizmetleri i�in)
�	JWT Bearer Authentication

### XML �zellikleri
XML Validasyon
�	XSD (XML Schema Definition) ile g��l� tip kontrol�
�	DTD (Document Type Definition) ile yap�sal do�rulama
�	Validasyon hata yakalama ve raporlama
XML G�venli�i
�	XMLDSig ile dijital imzalama
�	XML Encryption ile veri �ifreleme
�	�mza do�rulama ve raporlama
XML D�n���m ve Sorgulama
�	XSLT ile XML'den HTML'e d�n���m
�	XPath sorgular� ile belirli verileri ��karma
�	Farkl� XML ayr��t�rma tekniklerinin kar��la�t�rmas�
Gelecek Geli�tirmeler
�	Mikroservis mimarisine ge�i�
�	GraphQL deste�i
�	WebSocket deste�i
�	Geli�mi� test senaryolar� ve CI/CD entegrasyonu
Lisans
Bu proje MIT Lisans� alt�nda lisanslanm��t�r. Detaylar i�in LICENSE dosyas�na bak�n�z.
