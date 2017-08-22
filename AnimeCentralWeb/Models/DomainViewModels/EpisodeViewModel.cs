using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AnimeCentralWeb.Models.DomainViewModels
{
    public class EpisodeViewModel
    {
        public int Id { get; set; }
        [Required]
        public int AnimeId { get; set; }
        public string AnimeTitle { get; set; }
        [Display(Name = "Titlu Episod")]
        public string Title { get; set; }
        [Display(Name = "Episodul")]
        [Required]
        public float Order { get; set; }
        public virtual List<SourceViewModel> Sources { get; set; }
    }
}
