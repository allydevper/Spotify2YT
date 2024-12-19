using Newtonsoft.Json;

namespace Spotify2YouTube.Models
{
    public class SpotifyTokenModel
    {
        [JsonProperty("access_token")]
        public required string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public required string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public required string RefreshToken { get; set; }

        public required string Scope { get; set; }
    }
}