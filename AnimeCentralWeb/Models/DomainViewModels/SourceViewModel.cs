using AnimeCentralWeb.Domain;
using System.ComponentModel.DataAnnotations;

namespace AnimeCentralWeb.Models.DomainViewModels
{
    public class SourceViewModel
    {
        public int Id { get; set; }

        public string Label { get; set; }
        public string Link { get; set; }
        public SourceOrigin Origin { get; set; }
    }
}
