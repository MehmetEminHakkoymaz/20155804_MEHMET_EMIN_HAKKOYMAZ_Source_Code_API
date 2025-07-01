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
                return BadRequest(new { message = "Kullan�c� ad� ve �ifre gereklidir" });
            }
            
            // Giri� i�lemi
            var user = _authService.AuthenticateUser(model.Username, model.Password);
            
            if (user == null)
                return Unauthorized(new { message = "Ge�ersiz kullan�c� ad� veya �ifre" });
                
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
                return BadRequest(new { message = "Kullan�c� ad�, e-posta ve �ifre gereklidir" });
            }
            
            if (model.Password.Length < 6)
            {
                return BadRequest(new { message = "�ifre en az 6 karakter olmal�d�r" });
            }
            
            // Basit e-posta do�rulama
            if (!model.Email.Contains("@") || !model.Email.Contains("."))
            {
                return BadRequest(new { message = "Ge�erli bir e-posta adresi giriniz" });
            }
            
            // Kay�t i�lemi
            var user = _authService.RegisterUser(model.Username, model.Email, model.Password);
            
            if (user == null)
                return Conflict(new { message = "Kullan�c� ad� veya e-posta zaten kullan�l�yor" });
                
            return Ok(new { 
                message = "Kullan�c� ba�ar�yla kaydedildi",
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