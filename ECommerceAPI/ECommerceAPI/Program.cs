using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SoapCore;
using System.Text;
using System.ServiceModel;
using ECommerceAPI.Services;
using ECommerceAPI.Services.Soap;
using ECommerceAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Mevcut REST API servisleri
builder.Services.AddControllers(options =>
{
    options.RespectBrowserAcceptHeader = true;
    options.ReturnHttpNotAcceptable = true;
})
.AddXmlSerializerFormatters()
.AddJsonOptions(options => 
{
    options.JsonSerializerOptions.WriteIndented = true;
});

// API versiyonlamay� ekle
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

// JWT kimlik do�rulamas�n� yap�land�r
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"] ?? "YourSecureKeyHere1234567890123456"); 
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// Servisleri kaydet
builder.Services.AddSingleton<XmlProductService>();
builder.Services.AddSingleton<XsltService>();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<XmlParsingService>();
builder.Services.AddSingleton<CustomXmlSerializer>();
builder.Services.AddSingleton<AdvancedXmlService>();
builder.Services.AddSingleton<XmlSecurityService>();

// SOAP servisini ekle
builder.Services.AddSingleton<IProductSoapService, ProductSoapService>();

// HttpClient ekle
builder.Services.AddHttpClient<ExternalServiceClient>();
builder.Services.AddHttpClient();

// CORS politikas�
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// URL'leri ayarla
app.Urls.Add("http://localhost:5000");
app.Urls.Add("https://localhost:5001");

// HTTP istek boru hatt�n� yap�land�r
if (app.Environment.IsDevelopment())
{
    // Swagger konfig�rasyonu kald�r�ld�
}

// Middleware s�ras�n� d�zelt
app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseErrorHandler();  // T�m di�er middleware'lerden �nce

// Routing �nce olmal�
app.UseRouting();

// Sonra Authentication ve Authorization
app.UseAuthentication();
app.UseAuthorization(); 

// Ard�ndan endpoint y�nlendirme
app.UseEndpoints(endpoints =>
{
    endpoints.UseSoapEndpoint<IProductSoapService>("/ProductService.asmx", new SoapEncoderOptions(), 
        SoapSerializer.XmlSerializer);
});

app.MapControllers();

// Gerekli dizinleri olu�tur
var xmlDataDir = Path.Combine(app.Environment.ContentRootPath, "XmlData");
var xmlSchemasDir = Path.Combine(app.Environment.ContentRootPath, "XmlSchemas");
var xsltDir = Path.Combine(app.Environment.ContentRootPath, "XsltTemplates");
var docDir = Path.Combine(app.Environment.ContentRootPath, "Documentation");

if (!Directory.Exists(xmlDataDir))
    Directory.CreateDirectory(xmlDataDir);
if (!Directory.Exists(xmlSchemasDir))
    Directory.CreateDirectory(xmlSchemasDir);
if (!Directory.Exists(xsltDir))
    Directory.CreateDirectory(xsltDir);
if (!Directory.Exists(docDir))
    Directory.CreateDirectory(docDir);

// Sertifika dizini olu�tur
var certDir = Path.Combine(app.Environment.ContentRootPath, "Certificates");
if (!Directory.Exists(certDir))
    Directory.CreateDirectory(certDir);

app.Run();
