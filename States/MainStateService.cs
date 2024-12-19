namespace Spotify2YT.States
{
    public class MainStateService<T>
    {
        public bool ActiveSpotify { get; set; } = false;
        public string TokenSpotify { get; set; } = "";
        public bool ActiveYT { get; set; } = false;
        public List<T> ItemsSpotify { get; set; } = [];
        public List<T> ItemsYT { get; set; } = [];

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

        public Task SaveStateActiveYTAsync(bool activeYT)
        {
            ActiveYT = activeYT;
            return Task.CompletedTask;
        }

        public Task SaveStateItemsSpotifyAsync(List<T> itemsSpotify)
        {
            ItemsSpotify = itemsSpotify;
            return Task.CompletedTask;
        }

        public Task SaveStateItemsYTAsync(List<T> itemsYT)
        {
            ItemsYT = itemsYT;
            return Task.CompletedTask;
        }

        public Task<(bool activeSpotify, bool activeYT, List<T> itemsSpotify, List<T> itemsYT)> GetStateAsync()
        {
            return Task.FromResult((ActiveSpotify, ActiveYT, ItemsSpotify, ItemsYT));
        }
    }
}