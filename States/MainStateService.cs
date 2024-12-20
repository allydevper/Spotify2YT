namespace Spotify2YT.States
{
    public class MainStateService<T>
    {
        public bool ActiveSpotify { get; set; } = false;
        public string TokenSpotify { get; set; } = "";
        public string PlayListNameSpotify { get; set; } = "";
        public bool ActiveYT { get; set; } = false;

        public Task SaveStateActiveSpotifyAsync(bool activeSpotify)
        {
            ActiveSpotify = activeSpotify;
            return Task.CompletedTask;
        }

        public Task SaveStateTokenSpotifyAsync(string tokenSpotify)
        {
            TokenSpotify = tokenSpotify;
            return Task.CompletedTask;
        }

        public Task SaveStatePlayListNameSpotifyAsync(string playListNameSpotify)
        {
            PlayListNameSpotify = playListNameSpotify;
            return Task.CompletedTask;
        }

        public Task SaveStateActiveYTAsync(bool activeYT)
        {
            ActiveYT = activeYT;
            return Task.CompletedTask;
        }

        public Task<(bool activeSpotify, bool activeYT, string playListNameSpotify)> GetStateAsync()
        {
            return Task.FromResult((ActiveSpotify, ActiveYT, PlayListNameSpotify));
        }
    }
}