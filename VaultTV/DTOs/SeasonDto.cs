namespace VaultTV.DTOs
{
    public class SeasonDto
    {
        public int Id { get; set; }
        public int SeasonNumber { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? PosterUrl { get; set; }
        public List<EpisodeDto> Episodes { get; set; } = new();
    }

    public class CreateSeasonDto
    {
        public int SeasonNumber { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
    }
}
