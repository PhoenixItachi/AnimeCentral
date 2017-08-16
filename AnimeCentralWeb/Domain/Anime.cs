using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AnimeCentralWeb.Domain
{
    public class Anime
    {
        [Key]
        public int Id { get; set; }

        public int PrequelId { get; set; }
        public int SequelId { get; set; }
        public int ParentId { get; set; }

        public string Title { get; set; }
        public string NoOfEpisodes { get; set; }
        public string EpisodeLength { get; set; }
        public string Score { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Image { get; set; }
        public string BigImage { get; set; }
        public string Synonyms { get; set; }
        public string Synopsis { get; set; }
        public string Genres { get; set; }
        public int MalId { get; set; }

        public virtual List<Review> Reviews { get; set; }
        public virtual List<Episode> Episodes { get; set; }
    }
}
