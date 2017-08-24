using AnimeCentralWeb.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnimeCentralWeb.Domain
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        public int EpisodeId { get; set; }
        public int? ParentCommentId { get; set; }
        public string Content { get; set; }
        public ApplicationUser User { get; set; }

        [ForeignKey("ParentCommentId")]
        public virtual Comment ParentComment { get; set; }
        public virtual Episode Episode { get; set; }

        public virtual List<Comment> Replies { get; set; }
    }
}
