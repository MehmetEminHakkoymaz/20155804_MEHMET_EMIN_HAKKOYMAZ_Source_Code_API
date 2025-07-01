using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using ECommerceAPI.Models;

namespace ECommerceAPI.Services
{
    public class AuthService
    {
        private readonly string _xmlFilePath;
        private readonly IConfiguration _configuration;
        
        public AuthService(IWebHostEnvironment env, IConfiguration configuration)
        {
            // Kullan�c� XML dosyas�n�n yolunu ayarla
            _xmlFilePath = Path.Combine(env.ContentRootPath, "XmlData", "Users.xml");
            _configuration = configuration;
            
            // XML dosyas�n�n var oldu�undan emin ol
            EnsureUsersXmlExists();
        }
        
        public string GenerateJwtToken(User user)
        {
            // JWT token olu�turma
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), 
                    SecurityAlgorithms.HmacSha256Signature)
            };
            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        
        public User AuthenticateUser(string username, string password)
        {
            // Kullan�c� kimlik do�rulama
            if (!File.Exists(_xmlFilePath))
                return null;
                
            var doc = XDocument.Load(_xmlFilePath);
            var userElement = doc.Descendants("User")
                .FirstOrDefault(u => u.Element("Username").Value == username);
                
            if (userElement == null)
                return null;
                
            var storedHash = userElement.Element("PasswordHash").Value;
            
            if (VerifyPassword(password, storedHash))
            {
                return new User
                {
                    Id = int.Parse(userElement.Element("Id").Value),
                    Username = userElement.Element("Username").Value,
                    Email = userElement.Element("Email").Value,
                    PasswordHash = storedHash,
                    Role = userElement.Element("Role").Value
                };
            }
            
            return null;
        }
        
        public User RegisterUser(string username, string email, string password)
        {
            try
            {
                // XML dosyas�n�n var oldu�undan emin ol
                EnsureUsersXmlExists();
                
                // Yeni kullan�c� kayd�
                var doc = XDocument.Load(_xmlFilePath);
                var users = doc.Element("Users");
                
                // Kullan�c�n�n var olup olmad���n� kontrol et
                var existingUser = doc.Descendants("User")
                    .FirstOrDefault(u => u.Element("Username").Value == username || 
                                         u.Element("Email").Value == email);
                                         
                if (existingUser != null)
                    return null;
                    
                // En y�ksek ID'yi bul ve 1 art�r
                var maxId = 0;
                var idElements = doc.Descendants("Id");
                if (idElements.Any())
                    maxId = idElements.Max(e => int.Parse(e.Value));
                    
                var newId = maxId + 1;
                var passwordHash = HashPassword(password);
                
                var newUser = new XElement("User",
                    new XElement("Id", newId),
                    new XElement("Username", username),
                    new XElement("Email", email),
                    new XElement("PasswordHash", passwordHash),
                    new XElement("Role", "User") // Varsay�lan rol
                );
                
                users.Add(newUser);
                doc.Save(_xmlFilePath);
                
                return new User
                {
                    Id = newId,
                    Username = username,
                    Email = email,
                    PasswordHash = passwordHash,
                    Role = "User"
                };
            }
            catch (Exception ex)
            {
                // Hata loglama burada yap�labilir
                throw;
            }
        }
        
        private string HashPassword(string password)
        {
            // �ifre hashleme
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
        
        private bool VerifyPassword(string password, string storedHash)
        {
            // �ifre do�rulama
            var hashOfInput = HashPassword(password);
            return hashOfInput == storedHash;
        }

        private void EnsureUsersXmlExists()
        {
            var directory = Path.GetDirectoryName(_xmlFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            if (!File.Exists(_xmlFilePath))
            {
                // Varsay�lan Users.xml dosyas�n� olu�tur
                var doc = new XDocument(
                    new XDeclaration("1.0", "utf-8", null),
                    new XElement("Users",
                        new XElement("User",
                            new XElement("Id", 1),
                            new XElement("Username", "admin"),
                            new XElement("Email", "admin@example.com"),
                            new XElement("PasswordHash", "jZae727K08KaOmKSgOaGzww/XVqGr/PKEgIMkjrcbJI="),
                            new XElement("Role", "Admin")
                        )
                    )
                );
                
                doc.Save(_xmlFilePath);
            }
        }
    }
}