namespace UGTS.Models
{
    public class TokenRequestModel
    {
        public string GrantType { get; set; } = "client_credentials";
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}