using System.Text.Json.Serialization;

namespace UGTS.Models
{
    public class TokenRequestModel
    {
        public string grant_type { get; set; } = "client_credentials";
        public string client_id { get; set; }
        public string client_secret { get; set; }
    }

    public class TokenResponseModel
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; } = 300;

        [JsonPropertyName("refresh_expires_in")]
        public int RefreshExpiresIn { get; set; } = 1800;

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = "Bearer";

        [JsonPropertyName("not-before-policy")]
        public int NotBeforePolicy { get; set; } = 0;

        [JsonPropertyName("session_state")]
        public string SessionState { get; set; }

        [JsonPropertyName("scope")]
        public string Scope { get; set; } = "roles email profile";
    }
}