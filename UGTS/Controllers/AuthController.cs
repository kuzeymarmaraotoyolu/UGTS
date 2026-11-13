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
        var expectedClientSecret = jwtSettings["ClientSecret"] ?? "KmoUgts_SecuredApi_Secret_#2026!Key*9X";

        var allowedClients = new[]
        {
            jwtSettings["Clients:Europe"] ?? "ugts_client_avrupa",
            jwtSettings["Clients:Asia"] ?? "ugts_client_asya"
        };

        if (string.IsNullOrEmpty(model.ClientId) || !allowedClients.Contains(model.ClientId) || model.ClientSecret != expectedClientSecret)
        {
            return Unauthorized(new { message = "Geçersiz Client ID veya Client Secret." });
        }

        var secretKey = jwtSettings["Secret"] ?? "x9_#mP$7vL@2pQ!4zX*9wE%1rT&6yU(3sI)5oA#8dF$0gH@2jK!7lZ*4xC%9vB&1nM_k7#2";
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