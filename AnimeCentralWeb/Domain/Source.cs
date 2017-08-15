using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
