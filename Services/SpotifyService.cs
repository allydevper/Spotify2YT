using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Serilog;
using Spotify2YouTube.Models;
using Spotify2YT.Models;
using SpotifyAPI.Web;
using System.Net.Http.Headers;
using System.Text;

namespace Spotify2YT.Services
{
    public class SpotifyService
    {
        private readonly IConfiguration _configuration;

        public SpotifyService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

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

            string urlAuthorize = _configuration["AppConfig:SpotifyUrlAuthorize"] ?? "";
            Log.Information(QueryHelpers.AddQueryString(urlAuthorize, queryParams));
            Log.Information("GetUrlAuthorizeAsync End");
            return QueryHelpers.AddQueryString(urlAuthorize, queryParams);
        }

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
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string urlToken = _configuration["AppConfig:SpotifyUrlToken"] ?? "";
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

        public async Task<List<TrackModel>> GetListPlaylistAsync(string token, string playList)
        {
            Log.Information("GetListPlaylistAsync Init");

            List<TrackModel> TrackModel = [];

            var spotify = new SpotifyClient(token);
            var playlist = await spotify.Playlists.Get(playList);

            foreach (PlaylistTrack<IPlayableItem> item in playlist?.Tracks?.Items ?? [])
            {
                if (item.Track is FullTrack track)
                {
                    string cover = track.Album.Images.FirstOrDefault(s => s.Width == 64)?.Url
                                ?? "https://placehold.co/64";
                    TrackModel _TrackModel = new()
                    {
                        Id = track.Id,
                        NameTrack = track.Name,
                        Artist = track.Artists[0]?.Name ?? "",
                        Cover = cover,
                        PlayListName = track.Name
                    };
                    TrackModel.Add(_TrackModel);
                }
            }
            Log.Information("GetListPlaylistAsync End");
            return TrackModel;
        }

        private async Task<SpotifyCredentialsModel?> GetSpotifyCredentialsAsync()
        {
            var filePath = _configuration["AppConfig:SpotifyClientSecret"] ?? "";
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