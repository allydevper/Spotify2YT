namespace Spotify2YT.Models
{
    public class TrackModel
    {
        public required string Id { get; set; }
        public required string Cover { get; set; }
        public required string NameTrack { get; set; }
        public required string Artist { get; set; }
        public bool Active { get; set; } = true;
        public string PlayListName { get; set; } = "";
    }
}
