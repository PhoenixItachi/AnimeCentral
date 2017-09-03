using AnimeCentralWeb.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace AnimeCentralWeb.Domain
{
    public class Announcement
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string AuthorId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public AnnType Type { get; set; }
        public DateTime Date { get; set; }

        public virtual ApplicationUser Author { get; set; }

    }

    public enum AnnType
    {
        [Display(Name = "Anime Nou")]
        Anime = 1,
        [Display(Name = "Sezon Nou")]
        Season = 2,
        [Display(Name = "Anime OVA/Special")]
        Special = 3,
        [Display(Name = "Anunt")]
        Annoucement = 4,
        [Display(Name = "Film")]
        Movie = 5
    }
}
