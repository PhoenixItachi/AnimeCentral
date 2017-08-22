using System.ComponentModel.DataAnnotations;

namespace AnimeCentralWeb.Models.DomainViewModels
{
    public class SourceViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Label { get; set; }
        [Required]
        public string Link { get; set; }
    }
}
