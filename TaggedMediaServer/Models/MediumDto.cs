namespace Models
{
    public class MediumDto
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public List<TagDto>? CurrentTags { get; set; }
        public List<TagDto>? DissociatedTags { get; set; }
        public int OriginId { get; set; }
        public string LocalPath { get; set; } = string.Empty;
        public string ExtPath { get; set;} = string.Empty;
        public bool IsArchived { get; set; }
    }
}