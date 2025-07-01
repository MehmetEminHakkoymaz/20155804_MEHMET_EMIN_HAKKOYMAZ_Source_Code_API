using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using ECommerceAPI.Models;

namespace ECommerceAPI.Services
{
    public class XmlSecurityService
    {
        private readonly RSA _rsaKey;
        private readonly X509Certificate2 _certificate;
        
        public XmlSecurityService(IWebHostEnvironment env)
        {
            // RSA anahtar çifti oluştur - gerçek senaryoda güvenli bir şekilde saklanmalıdır
            _rsaKey = RSA.Create(2048);
            
            // Örnek sertifika - gerçek senaryoda bir sertifika dosyası kullanılmalıdır
            string certPath = Path.Combine(env.ContentRootPath, "Certificates", "xmlsecurity.pfx");
            if (File.Exists(certPath))
            {
                _certificate = new X509Certificate2(certPath, "password");
            }
            else
            {
                // Geliştirme için geçici bir sertifika
                // NOT: Gerçek ortamda bu yaklaşım kullanılmamalıdır
                using (var ecdsa = ECDsa.Create())
                {
                    var req = new CertificateRequest("cn=XMLSecurity", ecdsa, HashAlgorithmName.SHA256);
                    _certificate = req.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1));
                }
            }
        }
        
        #region XML Digital Signature

        /// <summary>
        /// XML belgesine dijital imza ekler
        /// </summary>
        public XmlDocument SignXmlDocument(XmlDocument xmlDoc)
        {
            // Dokümanın bir kopyasını oluşturun
            XmlDocument signedDoc = xmlDoc.Clone() as XmlDocument;
            
            try
            {
                // XML Dijital İmza oluşturucu
                SignedXml signedXml = new SignedXml(signedDoc);
                
                // İmzalama anahtarını ayarla
                signedXml.SigningKey = _rsaKey;
                
                // İmzalanacak referansı oluştur (tüm belge)
                Reference reference = new Reference();
                reference.Uri = ""; // Boş URI, belgenin tamamını ifade eder
                
                // Dönüşüm algoritmaları - XML normalizasyonu 
                XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
                reference.AddTransform(env);
                
                // İmzaya referans ekle
                signedXml.AddReference(reference);
                
                // Anahtar bilgisi ekle
                KeyInfo keyInfo = new KeyInfo();
                keyInfo.AddClause(new RSAKeyValue(_rsaKey));
                signedXml.KeyInfo = keyInfo;
                
                // XML belgesini imzala
                signedXml.ComputeSignature();
                
                // İmzayı XML belgesine ekle
                XmlElement signatureElement = signedXml.GetXml();
                signedDoc.DocumentElement.AppendChild(signedDoc.ImportNode(signatureElement, true));
                
                return signedDoc;
            }
            catch (Exception ex)
            {
                throw new Exception($"XML imzalama hatası: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// XML belgesindeki dijital imzayı doğrular
        /// </summary>
        public bool VerifyXmlSignature(XmlDocument xmlDoc)
        {
            try
            {
                // İmza düğümünü bul
                XmlNodeList signatureNodes = xmlDoc.GetElementsByTagName("Signature", SignedXml.XmlDsigNamespaceUrl);
                if (signatureNodes.Count == 0)
                    return false; // İmza bulunamadı
                    
                // İmzayı doğrula
                SignedXml signedXml = new SignedXml(xmlDoc);
                signedXml.LoadXml((XmlElement)signatureNodes[0]);
                
                return signedXml.CheckSignature();
            }
            catch (Exception ex)
            {
                throw new Exception($"XML imza doğrulama hatası: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// İmzalı XML'i görünür olarak işaretleyerek rapor şeklinde hazırlar
        /// </summary>
        public string GenerateSignatureReport(XmlDocument signedXml)
        {
            XmlDocument reportDoc = new XmlDocument();
            XmlElement reportRoot = reportDoc.CreateElement("SignatureReport");
            reportDoc.AppendChild(reportRoot);
            
            // İmza bilgisi ekle
            XmlElement statusElem = reportDoc.CreateElement("Status");
            bool isValid = VerifyXmlSignature(signedXml);
            statusElem.InnerText = isValid ? "İmza Geçerli ✅" : "İmza Geçersiz ❌";
            reportRoot.AppendChild(statusElem);
            
            // İmza zamanı
            XmlNodeList signatureNodes = signedXml.GetElementsByTagName("Signature", SignedXml.XmlDsigNamespaceUrl);
            if (signatureNodes.Count > 0)
            {
                XmlElement signatureInfo = reportDoc.CreateElement("SignatureInfo");
                signatureInfo.SetAttribute("algorithm", "RSA-SHA256");
                
                XmlNodeList sigValueNodes = signedXml.GetElementsByTagName("SignatureValue");
                if (sigValueNodes.Count > 0)
                {
                    XmlElement sigValueElem = reportDoc.CreateElement("SignatureValue");
                    string sigValue = sigValueNodes[0].InnerText;
                    sigValueElem.InnerText = sigValue.Length > 20 
                        ? sigValue.Substring(0, 20) + "..." 
                        : sigValue;
                    signatureInfo.AppendChild(sigValueElem);
                }
                
                reportRoot.AppendChild(signatureInfo);
            }
            
            // İmzalı içerik özetini ekle
            XmlElement contentElem = reportDoc.CreateElement("SignedContent");
            contentElem.InnerXml = "<![CDATA[" + signedXml.OuterXml.Substring(0, 
                Math.Min(500, signedXml.OuterXml.Length)) + "...]]>";
            reportRoot.AppendChild(contentElem);
            
            return reportDoc.OuterXml;
        }
        
        #endregion
        
        #region XML Encryption
        
        /// <summary>
        /// XML belgesindeki bir elemanı RSA ile şifreler
        /// </summary>
        public XmlDocument EncryptXmlElement(XmlDocument xmlDoc, string elementName)
        {
            try
            {
                // Şifreleme yapmak için dokümanın bir kopyasını oluşturun
                XmlDocument encryptedDoc = xmlDoc.Clone() as XmlDocument;
                
                // Şifrelenecek nodu bul
                XmlNodeList elementsToEncrypt = encryptedDoc.GetElementsByTagName(elementName);
                if (elementsToEncrypt.Count == 0)
                    throw new Exception($"'{elementName}' adlı eleman bulunamadı.");
                    
                // İlk bulunan elemanı şifrele
                XmlElement elementToEncrypt = (XmlElement)elementsToEncrypt[0];
                
                // Şifreleme kullanarak XML Encryption işlemi
                EncryptedXml encryptedXml = new EncryptedXml();
                
                // Şifreleme için AES kullan
                Aes aes = Aes.Create();
                aes.KeySize = 256;
                
                // Elemanı şifrele
                byte[] encryptedData = encryptedXml.EncryptData(elementToEncrypt, aes, false);
                
                // EncryptedData nesnesini oluştur
                EncryptedData encryptedDataElement = new EncryptedData();
                encryptedDataElement.Type = EncryptedXml.XmlEncElementUrl;
                encryptedDataElement.Id = "ED_" + Guid.NewGuid().ToString().Replace("-", "");
                
                // Şifreleme metodu
                encryptedDataElement.EncryptionMethod = new EncryptionMethod(EncryptedXml.XmlEncAES256Url);
                
                // RSA anahtarıyla AES anahtarını şifrele
                EncryptedKey encryptedKey = new EncryptedKey();
                
                // Anahtar şifreleme algoritması
                encryptedKey.EncryptionMethod = new EncryptionMethod(EncryptedXml.XmlEncRSA15Url);
                
                // AES anahtarını RSA kullanarak şifrele
                byte[] encryptedAesKey = EncryptedXml.EncryptKey(aes.Key, _rsaKey, false);
                encryptedKey.CipherData = new CipherData(encryptedAesKey);
                
                // EncryptedKey'i EncryptedData'ya ekle
                encryptedDataElement.KeyInfo = new KeyInfo();
                
                // Alıcı referansı ekle (isteğe bağlı)
                KeyInfoName keyInfoName = new KeyInfoName();
                keyInfoName.Value = "KeyName";
                encryptedKey.KeyInfo.AddClause(keyInfoName);
                
                // Şifreli anahtar referansı
                DataReference dataReference = new DataReference();
                dataReference.Uri = "#" + encryptedDataElement.Id;
                encryptedKey.AddReference(dataReference);
                
                // EncryptedKey'i KeyInfo'ya ekle
                encryptedDataElement.KeyInfo.AddClause(new KeyInfoEncryptedKey(encryptedKey));
                
                // Şifreli veriyi ayarla
                encryptedDataElement.CipherData = new CipherData(encryptedData);
                
                // XML belgesini güncelle
                EncryptedXml.ReplaceElement(elementToEncrypt, encryptedDataElement, false);
                
                return encryptedDoc;
            }
            catch (Exception ex)
            {
                throw new Exception($"XML şifreleme hatası: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// Şifrelenmiş bir XML belgesindeki elemanı çözer
        /// </summary>
        public XmlDocument DecryptXmlDocument(XmlDocument encryptedDoc)
        {
            try
            {
                // Dokümanın bir kopyasını oluştur
                XmlDocument decryptedDoc = encryptedDoc.Clone() as XmlDocument;
                
                // XML şifre çözme işlemini gerçekleştir
                EncryptedXml encryptedXml = new EncryptedXml(decryptedDoc);
                
                // Özel anahtarı kullanarak şifreyi çöz
                encryptedXml.AddKeyNameMapping("KeyName", _rsaKey);
                
                // Belgedeki tüm şifreli elemanları çöz
                encryptedXml.DecryptDocument();
                
                return decryptedDoc;
            }
            catch (Exception ex)
            {
                throw new Exception($"XML şifre çözme hatası: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// XML belgesindeki hassas bilgileri şifreleyerek korur
        /// </summary>
        public XmlDocument SecureXmlDocument(XmlDocument xmlDoc, string[] sensitiveElements)
        {
            // Dokümanın bir kopyasını oluştur
            XmlDocument secureDoc = xmlDoc.Clone() as XmlDocument;
            
            foreach (string elementName in sensitiveElements)
            {
                secureDoc = EncryptXmlElement(secureDoc, elementName);
            }
            
            return secureDoc;
        }
        
        #endregion
        
        #region Combined Security Operations
        
        /// <summary>
        /// XML belgesini önce şifreler, sonra imzalar (imzalı şifreleme)
        /// </summary>
        public XmlDocument EncryptAndSignXml(XmlDocument xmlDoc, string[] sensitiveElements)
        {
            // Önce hassas elemanları şifrele
            XmlDocument encryptedDoc = SecureXmlDocument(xmlDoc, sensitiveElements);
            
            // Ardından belgeyi imzala
            return SignXmlDocument(encryptedDoc);
        }
        
        /// <summary>
        /// İmzalı ve şifreli bir XML belgesinin imzasını doğrular ve şifresini çözer
        /// </summary>
        public (XmlDocument DecryptedDoc, bool SignatureValid) VerifyAndDecryptXml(XmlDocument secureDoc)
        {
            // Önce imzayı doğrula
            bool signatureValid = VerifyXmlSignature(secureDoc);
            
            // Ardından şifreyi çöz
            XmlDocument decryptedDoc = DecryptXmlDocument(secureDoc);
            
            return (decryptedDoc, signatureValid);
        }
        
        /// <summary>
        /// XML Ürün verisini güvenli bir şekilde paketler
        /// </summary>
        public string SecureProduct(Product product)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("SecureProduct");
            doc.AppendChild(root);
            
            // Ürün bilgilerini ekle
            XmlElement idElement = doc.CreateElement("Id");
            idElement.InnerText = product.Id.ToString();
            root.AppendChild(idElement);
            
            XmlElement nameElement = doc.CreateElement("Name");
            nameElement.InnerText = product.Name;
            root.AppendChild(nameElement);
            
            XmlElement descElement = doc.CreateElement("Description");
            descElement.InnerText = product.Description;
            root.AppendChild(descElement);
            
            XmlElement priceElement = doc.CreateElement("Price");
            priceElement.InnerText = product.Price.ToString();
            root.AppendChild(priceElement);
            
            // Hassas bilgileri şifrele ve tüm belgeyi imzala
            var secureDoc = EncryptAndSignXml(doc, new[] { "Price", "Description" });
            
            return secureDoc.OuterXml;
        }
        
        #endregion
    }
}