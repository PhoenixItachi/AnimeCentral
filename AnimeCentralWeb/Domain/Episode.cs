using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AnimeCentralWeb.Domain
{
    public class Episode
    {
        [Key]
        public int Id { get; set; }
        public int AnimeId { get; set; }

        public string Title { get; set; }
        public float Order { get; set; }
        public int ViewCount { get; set; }
        public DateTime Date { get; set; }

        public virtual Anime Anime { get; set; }
        public virtual List<Comment> Comments { get; set; }
        public virtual List<Source> Sources { get; set; }
    }
}
