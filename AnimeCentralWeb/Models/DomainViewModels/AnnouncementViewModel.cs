using AnimeCentralWeb.Domain;
using System;
using System.ComponentModel.DataAnnotations;

namespace AnimeCentralWeb.Models.DomainViewModels
{
    public class AnnouncementViewModel
    {
        public int Id { get; set; }
        public string AuthorId { get; set; }
        [Required]
        [Display(Name = "Titlu")]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        [Display(Name = "Tip")]
        public AnnType Type { get; set; }
        public DateTime Date { get; set; }
        public ApplicationUser Author { get; set; }
    }
}
