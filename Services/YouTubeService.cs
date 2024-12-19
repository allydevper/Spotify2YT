﻿using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.YouTube.v3.Data;
using Serilog;
using Spotify2YT.Models;
using YouTubeV3Service = Google.Apis.YouTube.v3.YouTubeService;

namespace Spotify2YT.Services
{
    public class YouTubeService
    {
        public async Task CreatePlaylistYTAsync(string playlistName, List<TrackModel> trackSpotifyModels)
        {
            YouTubeV3Service youtubeService = await GetYouTubeServiceAsync();

            List<TrackModel> trackYTModel = [];

            foreach (var _trackSpotify in trackSpotifyModels)
            {
                TrackModel trackModel = await GetTrackYTAsync(youtubeService, _trackSpotify);
                trackYTModel.Add(trackModel);
            }

            var playlistId = await CreatePlaylistAsync(youtubeService, playlistName);

            foreach (var _trackYT in trackYTModel.Where(s => s.Active))
            {
                await AddTrackToPlayListAsync(youtubeService, _trackYT, playlistId);
            }
        }

        private static async Task<TrackModel> GetTrackYTAsync(YouTubeV3Service youtubeService, TrackModel trackSpotify)
        {
            Log.Information("GetTrackYTAsync Init");
            var searchRequest = youtubeService.Search.List("snippet");
            // Término de búsqueda, como el nombre de la canción o artista
            searchRequest.Q = $"{trackSpotify.NameTrack} {trackSpotify.Artist}";
            // Limitar solo a videos
            searchRequest.Type = "video";
            // Categoría de música en YouTube
            //searchRequest.VideoCategoryId = "10";
            // Número de resultados a devolver
            searchRequest.MaxResults = 1;
            var searchResponse = await searchRequest.ExecuteAsync();

            if (searchResponse.Items.Count > 0)
            {
                var item = searchResponse.Items[0];
                Log.Information("GetTrackYTAsync End");
                return new TrackModel
                {
                    Id = item.Id.VideoId,
                    NameTrack = item.Snippet.Title,
                    Artist = item.Snippet.ChannelTitle,
                    Cover = item.Snippet.Thumbnails.Standard.Url,
                };
            }
            else
            {
                TrackModel trackYT = trackSpotify;
                trackYT.Active = false;
                Log.Information("GetTrackYTAsync End");
                return trackYT;
            }
        }

        private static async Task<string> CreatePlaylistAsync(YouTubeV3Service youtubeService, string playlistName)
        {
            Log.Information("CreatePlaylistAsync Init");
            var newPlaylist = new Playlist
            {
                Snippet = new PlaylistSnippet
                {
                    Title = playlistName,
                    Description = "Esta es una lista de reproducción creada desde la API de YouTube"
                },
                Status = new PlaylistStatus
                {
                    PrivacyStatus = "public"
                }
            };

            var request = youtubeService.Playlists.Insert(newPlaylist, "snippet,status");
            var response = await request.ExecuteAsync();

            Log.Information($"Lista de reproducción creada con ID: {response.Id}");
            Log.Information("CreatePlaylistAsync End");
            return response.Id;
        }

        private static async Task AddTrackToPlayListAsync(YouTubeV3Service youtubeService, TrackModel trackYT, string playlistId)
        {
            Log.Information("AddTrackToPlayListAsync Init");
            var playlistItem = new PlaylistItem
            {
                Snippet = new PlaylistItemSnippet
                {
                    PlaylistId = playlistId,
                    ResourceId = new ResourceId
                    {
                        Kind = "youtube#video",
                        VideoId = trackYT.Id
                    }
                }
            };

            var insertRequest = youtubeService.PlaylistItems.Insert(playlistItem, "snippet");
            var insertResponse = await insertRequest.ExecuteAsync();

            Console.WriteLine($"Canción agregada a la lista de reproducción: {insertResponse.Snippet.Title}");
        }

        public async Task<YouTubeV3Service> GetYouTubeServiceAsync()
        {
            Log.Information("GetYouTubeServiceAsync Init");
            var filePath = "wwwroot/credentials/google_client_secret.json";

            UserCredential credential;

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    [YouTubeV3Service.Scope.Youtube],
                    "user",
                    CancellationToken.None
                );
            }
            Log.Information("GetYouTubeServiceAsync End");

            return new YouTubeV3Service(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "YouTube Test App"
            });
        }
    }
}