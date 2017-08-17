using System.ComponentModel.DataAnnotations;

namespace AnimeCentralWeb.Models.DomainViewModels
{
    public class AnimeViewModel
    {
        public int PrequelId { get; set; }
        public int SequelId { get; set; }
        public int ParentId { get; set; }

        [Required]
        public string Title { get; set; }
        [Required]
        public string NoOfEpisodes { get; set; }
        public string EpisodeLength { get; set; }
        [Required]
        public string Score { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public string Image { get; set; }
        public string BigImage { get; set; }
        public string Synonyms { get; set; }
        [Required]
        public string Synopsis { get; set; }
        [Required]
        public string Genres { get; set; }
        public int MalId { get; set; }
    }
}
