﻿using System.ComponentModel.DataAnnotations;

namespace AnimeCentralWeb.Domain
{
    public class Source
    {
        [Key]
        public int Id { get; set; }
        public int EpisodeId { get; set; }

        public string Label { get; set; }
        public string Link { get; set; }

        public virtual Episode Episode { get; set; }
    }
}
