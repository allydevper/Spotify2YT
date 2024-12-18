using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Spotify2YT.Models;

namespace Spotify2YT.Services
{
    public class SpotifyService
    {
        public string urlAuthorize = "https://accounts.spotify.com/authorize";
        public string scope = "user-read-private user-read-email";

        public async Task<string> GetUrlAuthorizeAsync(string uri)
        {
            var spotifyCredentials = await GetSpotifyCredentialsAsync();
            string state = GenerateRandomString();

            var queryParams = new Dictionary<string, string?>
            {
                { "response_type", "code" },
                { "client_id", spotifyCredentials?.ClientId },
                { "scope", scope },
                { "redirect_uri", uri },
                { "state", state }
            };

            return QueryHelpers.AddQueryString(urlAuthorize, queryParams);
            //return $"{urlAuthorize}?response_type=code&client_id={spotifyCredentials?.ClientId}&scope={scope}&redirect_uri={uri}&state={state}";
        }

        public string urlToken = "https://accounts.spotify.com/api/token";
        public string grant_type = "authorization_code";

        public async Task<string> GetUrlTokenAsync(string uri, string code)
        {
            var spotifyCredentials = await GetSpotifyCredentialsAsync();

            var queryParams = new Dictionary<string, string?>
            {
                { "code", code },
                { "redirect_uri", uri },
                { "grant_type", grant_type }
            };

            return QueryHelpers.AddQueryString(urlAuthorize, queryParams);
            //return $"{urlAuthorize}?response_type=code&client_id={spotifyCredentials?.ClientId}&scope={scope}&redirect_uri={uri}&state={state}";
        }

        private static async Task<SpotifyCredentialsModel?> GetSpotifyCredentialsAsync()
        {
            var filePath = "wwwroot/credentials/spotify_credentials.json";
            string jsonString = await File.ReadAllTextAsync(filePath);

            return JsonConvert.DeserializeObject<SpotifyCredentialsModel>(jsonString);
        }

        private static string GenerateRandomString(int length = 16)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var result = new char[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = chars[random.Next(chars.Length)];
            }
            return new string(result);
        }
    }
}