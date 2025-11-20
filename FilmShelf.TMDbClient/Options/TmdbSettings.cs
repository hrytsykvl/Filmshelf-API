namespace FilmShelf.TMDbClient.Options;

public class TmdbSettings
{
    public string BaseUrl { get; set; } = null!;
    public string ApiKey { get; set; } = null!;
    public string SortBy { get; set; } = null!;
    public byte PagesToFetch { get; set; }
    public int IntervalInHours { get; set; }
    public string ScheduledTime { get; set; } = null!;

    public ImageSettings Images { get; set; } = new ImageSettings();
    public int TotalPages { get; set; }
    public int NumberOfActors { get; set; }
    public string CrewJob { get; set; } = null!;

    public class ImageSettings
    {
        public string BaseUrl { get; set; } = null!;
        public List<string> PosterSizes { get; set; } = new();
        public string DefaultPosterSize { get; set; } = "w500";
    }
}
