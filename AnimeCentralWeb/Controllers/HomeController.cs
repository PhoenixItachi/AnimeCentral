using AnimeCentralWeb.Data;
using AnimeCentralWeb.Domain;
using AnimeCentralWeb.Models;
using AnimeCentralWeb.Models.DomainViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AnimeCentralWeb.Controllers
{
    public class HomeController : Controller
    {
        private AnimeCentralDbContext Context { get; set; }
        public IMapper AutoMapper;

        public HomeController(AnimeCentralDbContext context)
        {
            Context = context;
            AutoMapper = Mapper.Instance;
        }

        public async Task<IActionResult> Index()
        {
            var comments = await Context.Comments.OrderByDescending(x => x.Date).Take(5).Include(x => x.User).Select(x => AutoMapper.Map<CommentViewModel>(x)).ToListAsync();
            foreach(var comment in comments)
            {
                var episode = await Context.Episodes.Include(x => x.Anime).FirstOrDefaultAsync(x => x.Id == comment.Id);
                comment.Episode = AutoMapper.Map<EpisodeViewModel>(episode);
            }

            var anime = Context.Anime.Include(x => x.Episodes).OrderByDescending(x => x.LatestEpisode)
                .Where(x => x.Episodes.Count != 0).Take(10).ToList()
                .Select(x => { x.Episodes = x.Episodes.Take(2).ToList(); return AutoMapper.Map<AnimeViewModel>(x); }).ToList();

            var topAnime = await Context.Anime.Include(x => x.Episodes).Select(x => new AnimeViewModel()
            {
                Id = x.Id,
                Title = x.Title,
                AnimeViews = x.Episodes.Sum(e => e.ViewCount),
                Image = x.Image
            }).OrderByDescending(x => x.AnimeViews).Take(10).ToListAsync();

            var recommendation = Context.Anime.OrderBy(x => Guid.NewGuid()).Where(x => !string.IsNullOrEmpty(x.BigImage)).Take(5).Select(x => AutoMapper.Map<AnimeViewModel>(x)).ToList();

            var topEpisodes = await Context.Episodes.OrderByDescending(x => x.ViewCount).Take(10).Include(x => x.Anime).Select(x => AutoMapper.Map<EpisodeViewModel>(x)).ToListAsync();

            var model = new HomeViewModel() {
                LatestAnimeUpdates = anime,
                Recommendation = recommendation,
                LatestComments = comments,
                TopAnime = topAnime,
                TopEpisodes = topEpisodes
            };

            return View(model);
        }

        public bool SelectUntilCondition(Episode ep, ref List<int> animeIds)
        {
            return true;
        }

        public async Task<IActionResult> SearchAnime(string searchText)
        {
            var malApiUrl = "https://myanimelist.net/api/anime/search.xml";
            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(malApiUrl + "?q=" + searchText);

            var authHeaderBytes = System.Text.Encoding.UTF8.GetBytes("PhoenixItachi:juventus32");
            var authHeader = Convert.ToBase64String(authHeaderBytes);

            wr.Headers[HttpRequestHeader.Authorization] = "Basic " + authHeader;
            HttpWebResponse response = await wr.GetResponseAsync() as HttpWebResponse;
            List<Anime> animeList = new List<Anime>();
            try
            {
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return new JsonResult(new { more = false });
                }

                var responseStream = response.GetResponseStream();

                var xml = XDocument.Load(responseStream);
                var list = xml.Descendants("entry").ToList()?.Take(5);
                foreach (var anime in list)
                {
                    var animeObj = new Anime()
                    {
                        Title = anime.Element("title").Value,
                        NoOfEpisodes = int.Parse(anime.Element("episodes").Value),
                        Status = anime.Element("status").Value,
                        Type = anime.Element("type").Value,
                        Score = anime.Element("score").Value,
                        Image = anime.Element("image").Value,
                        Synonyms = anime.Element("synonyms").Value,
                        Synopsis = anime.Element("synopsis").Value
                    };
                    animeList.Add(animeObj);
                }
            }
            catch (Exception e)
            {

            }

            return new JsonResult(animeList);
        }
    }
}
