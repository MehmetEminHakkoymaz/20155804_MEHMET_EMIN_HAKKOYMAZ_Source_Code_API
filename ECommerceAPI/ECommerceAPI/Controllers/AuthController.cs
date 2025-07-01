using Microsoft.AspNetCore.Mvc;
using ECommerceAPI.Models;
using ECommerceAPI.Services;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        
        public AuthController(AuthService authService)
        {
            _authService = authService;
        }
        
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest(new { message = "Kullanýcý adý ve þifre gereklidir" });
            }
            
            // Giriþ iþlemi
            var user = _authService.AuthenticateUser(model.Username, model.Password);
            
            if (user == null)
                return Unauthorized(new { message = "Geçersiz kullanýcý adý veya þifre" });
                
            var token = _authService.GenerateJwtToken(user);
            
            return Ok(new { 
                token,
                user = new {
                    id = user.Id,
                    username = user.Username,
                    email = user.Email,
                    role = user.Role
                }
            });
        }
        
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterModel model)
        {
            if (string.IsNullOrEmpty(model.Username) || 
                string.IsNullOrEmpty(model.Email) || 
                string.IsNullOrEmpty(model.Password))
            {
                return BadRequest(new { message = "Kullanýcý adý, e-posta ve þifre gereklidir" });
            }
            
            if (model.Password.Length < 6)
            {
                return BadRequest(new { message = "Þifre en az 6 karakter olmalýdýr" });
            }
            
            // Basit e-posta doðrulama
            if (!model.Email.Contains("@") || !model.Email.Contains("."))
            {
                return BadRequest(new { message = "Geçerli bir e-posta adresi giriniz" });
            }
            
            // Kayýt iþlemi
            var user = _authService.RegisterUser(model.Username, model.Email, model.Password);
            
            if (user == null)
                return Conflict(new { message = "Kullanýcý adý veya e-posta zaten kullanýlýyor" });
                
            return Ok(new { 
                message = "Kullanýcý baþarýyla kaydedildi",
                user = new {
                    id = user.Id,
                    username = user.Username,
                    email = user.Email,
                    role = user.Role
                }
            });
        }
    }
    
    public class LoginModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    
    public class RegisterModel
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}