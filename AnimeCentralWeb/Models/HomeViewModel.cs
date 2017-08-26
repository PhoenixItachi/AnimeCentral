using AnimeCentralWeb.Models.DomainViewModels;
using System.Collections.Generic;

namespace AnimeCentralWeb.Models
{
    public class HomeViewModel
    {
        public List<AnimeViewModel> LatestAnimeUpdates { set; get; }
        public List<AnimeViewModel> Recommendation { set; get; }
        public List<CommentViewModel> LatestComments { set; get; }
        public List<AnimeViewModel> TopAnime { get; set; }
        public List<EpisodeViewModel> TopEpisodes { get; set; }
    }
}
