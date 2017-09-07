using AnimeCentralWeb.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AnimeCentralWeb.Models.DomainViewModels
{
    public class EpisodeViewModel
    {
        public int Id { get; set; }
        public int AnimeId { get; set; }

        public string AnimeTitle { get; set; }

        [Display(Name = "Titlu")]
        [Required(ErrorMessage = AnimeUtils.FormErrorRequiredMessage)]
        public string Title { get; set; }

        public int ViewCount { get; set; }

        public DateTime Date { get; set; }

        [Display(Name = "Episodul")]
        [Required(ErrorMessage = AnimeUtils.FormErrorRequiredMessage)]
        public float Order { get; set; }

        public int Next { get; set; }

        public int Previous { get; set; }

        public AnimeViewModel Anime { get; set; }
        public virtual List<SourceViewModel> Sources { get; set; }
        public virtual List<CommentViewModel> Comments { get; set; }
    }
}
