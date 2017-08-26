using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AnimeCentralWeb.Models.DomainViewModels
{
    public class CommentViewModel
    {
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        public int EpisodeId { get; set; }
        public int? ParentCommentId { get; set; }
        public string Content { get; set; }
        public string UserName { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual EpisodeViewModel Episode { get; set; }
        public virtual List<CommentViewModel> Replies { get; set; }
    }
}
