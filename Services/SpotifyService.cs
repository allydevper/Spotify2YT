using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Spotify2YouTube.Models;
using Spotify2YT.Models;
using System.Net.Http.Headers;
using System.Text;

namespace Spotify2YT.Services
{
    public class SpotifyService
    {
        public string urlAuthorize = "https://accounts.spotify.com/authorize";

        public async Task<string> GetUrlAuthorizeAsync(string uri)
        {
            var spotifyCredentials = await GetSpotifyCredentialsAsync();
            string state = GenerateRandomString();

            var queryParams = new Dictionary<string, string?>
            {
                { "response_type", "code" },
                { "client_id", spotifyCredentials?.ClientId },
                { "scope", "user-read-private user-read-email" },
                { "redirect_uri", uri },
                { "state", state }
            };

            //return $"{urlAuthorize}?response_type=code&client_id={spotifyCredentials?.ClientId}&scope={scope}&redirect_uri={uri}&state={state}";
            return QueryHelpers.AddQueryString(urlAuthorize, queryParams);
        }

        public string urlToken = "https://accounts.spotify.com/api/token";

        public async Task<string?> GetTokenAsync(string uri, string code)
        {
            var spotifyCredentials = await GetSpotifyCredentialsAsync();

            var queryParams = new Dictionary<string, string?>
            {
                { "code", code },
                { "redirect_uri", uri },
                { "grant_type", "authorization_code" }
            };

            var content = new FormUrlEncodedContent(queryParams);

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{spotifyCredentials?.ClientId}:{spotifyCredentials?.ClientSecret}")));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-www-form-urlencoded"));

            HttpResponseMessage response = await httpClient.PostAsync(urlToken, content);

            if (response.IsSuccessStatusCode)
            {
                string readAsStringAsync = (await response.Content.ReadAsStringAsync()) ?? "";
                SpotifyTokenModel? SpotifyTokenModel = JsonConvert.DeserializeObject<SpotifyTokenModel>(readAsStringAsync);
                return SpotifyTokenModel?.AccessToken;
            }
            return "";
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