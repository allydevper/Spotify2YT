namespace Spotify2YT.Models
{
    public class SpotifyPlaylistModel
    {
        public required Tracks Tracks { get; set; }
    }

    public class Album
    {
        public required string Id { get; set; }
        public required List<Image> Images { get; set; }
        public required string Name { get; set; }
        public required List<Artist> Artists { get; set; }
    }

    public class Artist
    {
        public required string Href { get; set; }
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Type { get; set; }
    }

    public class Image
    {
        public required string Url { get; set; }
        public required int Width { get; set; }
        public required int Height { get; set; }
    }

    public class Item
    {
        public required Track Track { get; set; }
    }

    public class Track
    {
        public required Album Album { get; set; }
        public required List<Artist> Artists { get; set; }
        public required string Id { get; set; }
        public required string Name { get; set; }
    }

    public class Tracks
    {
        public required string Href { get; set; }
        public required List<Item> Items { get; set; }
    }
}