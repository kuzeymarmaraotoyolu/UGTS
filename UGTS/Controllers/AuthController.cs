using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UGTS.Models;

[Route("api/v1/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("token")]
    public IActionResult GenerateToken([FromBody] TokenRequestModel model)
    {
        if (model == null || model.GrantType != "client_credentials")
        {
            return BadRequest(new { message = "Geçersiz grant_type veya istek gövdesi." });
        }

        var jwtSettings = _configuration.GetSection("JwtSettings");
        var expectedClientId = jwtSettings["ClientId"] ?? "ugts_client_id";
        var expectedClientSecret = jwtSettings["ClientSecret"] ?? "ugts_client_secret_xyz";

        if (model.ClientId != expectedClientId || model.ClientSecret != expectedClientSecret)
        {
            return Unauthorized(new { message = "Geçersiz Client ID veya Client Secret." });
        }

        var secretKey = jwtSettings["Secret"] ?? "gizli-anahtar-varsayilani-en-az-32-karakter-olmalidir";
        var key = Encoding.ASCII.GetBytes(secretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, model.ClientId)
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return Ok(new
        {
            access_token = tokenString,
            token_type = "Bearer",
            expires_in = 7200
        });
    }
}