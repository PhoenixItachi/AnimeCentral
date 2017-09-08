using AnimeCentralWeb.Domain;
using AnimeCentralWeb.Utils;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AnimeCentralWeb.Models.DomainViewModels
{
    public class AnimeViewModel
    {
        public int Id { get; set; }
        public int PrequelId { get; set; }
        public int SequelId { get; set; }
        public int MalId { get; set; }
        public int ParentId { get; set; }

        [Required(ErrorMessage = AnimeUtils.FormErrorRequiredMessage)]
        [Display(Name = "Titlu")]
        public string Title { get; set; }

        [Display(Name = "Episoade")]
        [Required(ErrorMessage = AnimeUtils.FormErrorRequiredMessage)]
        public int NoOfEpisodes { get; set; }

        public string EpisodeLength { get; set; }

        [Display(Name = "Nota MyAnimeList")]
        [Required(ErrorMessage = AnimeUtils.FormErrorRequiredMessage)]
        public string Score { get; set; }

        [Display(Name = "Tipul")]
        [Required(ErrorMessage = AnimeUtils.FormErrorRequiredMessage)]
        public string Type { get; set; }

        [Display(Name = "Status anime")]
        [Required(ErrorMessage = AnimeUtils.FormErrorRequiredMessage)]
        public string Status { get; set; }

        public TranslateStatus TranslateStatus { get; set; }

        [Display(Name = "Coperta")]
        [Required(ErrorMessage = AnimeUtils.FormErrorRequiredMessage)]
        public string Image { get; set; }

        public string BigImage { get; set; }

        [Display(Name = "Sinonime")]
        public string Synonyms { get; set; }

        [Display(Name = "Rezumat")]
        public string Synopsis { get; set; }

        [Display(Name = "Genuri")]
        [Required(ErrorMessage = AnimeUtils.FormErrorRequiredMessage)]
        public string Genres { get; set; }

        public int AnimeViews { get; set; }
        public int TranslatedEpisodes { get; set; }

        public List<EpisodeViewModel> Episodes { get; set; }
    }
}
