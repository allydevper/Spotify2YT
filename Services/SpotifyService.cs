using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Serilog;
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
            Log.Information("GetUrlAuthorizeAsync Init");
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

            Log.Information(QueryHelpers.AddQueryString(urlAuthorize, queryParams));
            Log.Information("GetUrlAuthorizeAsync End");
            //return $"{urlAuthorize}?response_type=code&client_id={spotifyCredentials?.ClientId}&scope={scope}&redirect_uri={uri}&state={state}";
            return QueryHelpers.AddQueryString(urlAuthorize, queryParams);
        }

        public string urlToken = "https://accounts.spotify.com/api/token";

        public async Task<string?> GetTokenAsync(string uri, string code)
        {
            Log.Information("GetTokenAsync Init");
            var spotifyCredentials = await GetSpotifyCredentialsAsync();
            var token = "";

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
                Log.Information(readAsStringAsync);
                SpotifyTokenModel? SpotifyTokenModel = JsonConvert.DeserializeObject<SpotifyTokenModel>(readAsStringAsync);
                token = SpotifyTokenModel?.AccessToken;
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                int statusCode = (int)response.StatusCode;
                Log.Error($"Error {statusCode}: {errorContent}");
            }
            Log.Information("GetTokenAsync End");
            return token;
        }

        public string urlPlayList = "https://api.spotify.com/v1/playlists";

        public async Task<List<SpotifyTrackListModel>> GetListPlaylistAsync(string playList, string token)
        {
            Log.Information("GetListPlaylistAsync Init");
            List<SpotifyTrackListModel> SpotifyTrackListModel = [];

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            Log.Information($"{urlPlayList}/{playList}");
            HttpResponseMessage response = await httpClient.GetAsync($"{urlPlayList}/{playList}");

            if (response.IsSuccessStatusCode)
            {
                string content = (await response.Content.ReadAsStringAsync()) ?? "";
                Log.Information(content);
                SpotifyPlaylistModel? SpotifyPlaylistModel = JsonConvert.DeserializeObject<SpotifyPlaylistModel>(content);

                foreach (var _SpotifyPlaylistModel in SpotifyPlaylistModel?.Tracks.Items ?? [])
                {
                    string cover = _SpotifyPlaylistModel.Track.Album.Images.FirstOrDefault(s => s.Width == 64)?.Url
                                ?? "https://placehold.co/64";

                    SpotifyTrackListModel _SpotifyTrackListModel = new()
                    {
                        Id = _SpotifyPlaylistModel.Track.Id,
                        NameTrack = _SpotifyPlaylistModel.Track.Name,
                        Artist = _SpotifyPlaylistModel.Track.Artists.ToList().FirstOrDefault()?.Name ?? "",
                        Cover = cover,
                    };
                }

                return SpotifyTrackListModel;
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                int statusCode = (int)response.StatusCode;
                Log.Error($"Error {statusCode}: {errorContent}");
            }
            Log.Information("GetListPlaylistAsync End");
            return [];
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