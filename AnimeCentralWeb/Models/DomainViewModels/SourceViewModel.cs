using AnimeCentralWeb.Domain;
using AnimeCentralWeb.Utils;
using System.ComponentModel.DataAnnotations;

namespace AnimeCentralWeb.Models.DomainViewModels
{
    public class SourceViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Nume Sursa")]
        [Required(ErrorMessage = AnimeUtils.FormErrorRequiredMessage)]
        public string Label { get; set; }

        [Display(Name = "Link Sursa")]
        [Required(ErrorMessage = AnimeUtils.FormErrorRequiredMessage)]
        public string Link { get; set; }

        [Display(Name = "Origine Sursa")]
        [Required(ErrorMessage = AnimeUtils.FormErrorRequiredMessage)]
        public SourceOrigin Origin { get; set; }
    }
}
