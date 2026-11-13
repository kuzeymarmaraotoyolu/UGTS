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
        if (model == null || model.grant_type != "client_credentials")
        {
            return BadRequest(new { message = "Geçersiz grant_type veya istek gövdesi." });
        }

        var jwtSettings = _configuration.GetSection("JwtSettings");
        var expectedClientSecret = jwtSettings["ClientSecret"];

        var allowedClients = new[]
        {
            jwtSettings["Clients:Europe"] ?? "ugts_client_avrupa",
            jwtSettings["Clients:Asia"] ?? "ugts_client_asya"
        };

        if (string.IsNullOrEmpty(model.client_id) || !allowedClients.Contains(model.client_id) || model.client_secret != expectedClientSecret)
        {
            return Unauthorized(new { message = "Geçersiz Client ID veya Client Secret." });
        }

        var secretKey = jwtSettings["Secret"];
        var key = Encoding.ASCII.GetBytes(secretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, model.client_id)
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
            expires_in = 7200,
            refresh_expires_in = 1800,
            refresh_token = (string?)null,
            token_type = "Bearer",
            not_before_policy = 0,
            session_state = (string?)null,
            scope = "roles email profile"
        });
    }
}