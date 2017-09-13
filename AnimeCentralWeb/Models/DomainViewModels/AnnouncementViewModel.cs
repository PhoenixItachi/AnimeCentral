using AnimeCentralWeb.Domain;
using AnimeCentralWeb.Utils;
using System;
using System.ComponentModel.DataAnnotations;

namespace AnimeCentralWeb.Models.DomainViewModels
{
    public class AnnouncementViewModel
    {
        public int Id { get; set; }
        public string AuthorId { get; set; }

        [Display(Name = "Titlu", Prompt = "Titlu")]
        [Required(ErrorMessage = AnimeUtils.FormErrorRequiredMessage)]
        public string Title { get; set; }

        [Display(Name = "Continut", Prompt = "Continut")]
        [Required(ErrorMessage = AnimeUtils.FormErrorRequiredMessage)]
        public string Content { get; set; }

        [Display(Name = "Tip Anunt")]
        [Required(ErrorMessage = AnimeUtils.FormErrorRequiredMessage)]
        public AnnType Type { get; set; }

        public DateTime Date { get; set; }
        public ApplicationUser Author { get; set; }
    }
}
