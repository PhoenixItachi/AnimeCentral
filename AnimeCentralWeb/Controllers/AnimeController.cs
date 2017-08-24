using AnimeCentralWeb.Data;
using AnimeCentralWeb.Domain;
using AnimeCentralWeb.Models;
using AnimeCentralWeb.Models.DomainViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
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
    public class AnimeController : Controller
    {
        public AnimeCentralDbContext Context;
        public IMapper AutoMapper;
        public UserManager<ApplicationUser> _userManager { get; set; }
        public AnimeController(AnimeCentralDbContext context, UserManager<ApplicationUser> userManager)
        {
            Context = context;
            AutoMapper = Mapper.Instance;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllAnime()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var model = Context.Anime.Select(x => AutoMapper.Map<Anime, AnimeViewModel>(x)).ToList();
            return PartialView("Partials/_AnimeList", model);
        }

        [HttpPost]
        public async Task<ActionResult> AddAnime(AnimeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var anime = AutoMapper.Map<AnimeViewModel, Anime>(model);

                anime.TranslateStatus = "Ongoing";
                await Context.Anime.AddAsync(anime);
                await Context.SaveChangesAsync();

                return Ok();
            }
            return BadRequest();
        }

        public async Task<IActionResult> SearchAnime(string searchText)
        {
            var animeList = new List<Anime>();
            try
            {
                animeList = await GetAnimeFromMAL(searchText);
            }
            catch (Exception e)
            {

            }

            return new JsonResult(animeList);
        }

        public ActionResult GetAnimeByIdPartial(int id)
        {
            var anime = Context.Anime.Include(x => x.Episodes).FirstOrDefault(x => x.Id == id);
            if (anime == null)
                return BadRequest();

            var model = AutoMapper.Map<Anime, AnimeViewModel>(anime);
            return PartialView("Partials/_AnimePartial", model);
        }

        private async Task<List<Anime>> GetAnimeFromMAL(string searchText)
        {
            var malApiUrl = "https://myanimelist.net/api/anime/search.xml";
            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(malApiUrl + "?q=" + searchText);

            var authHeaderBytes = System.Text.Encoding.UTF8.GetBytes("PhoenixItachi:juventus32");
            var authHeader = Convert.ToBase64String(authHeaderBytes);

            wr.Headers[HttpRequestHeader.Authorization] = "Basic " + authHeader;
            HttpWebResponse response = await wr.GetResponseAsync() as HttpWebResponse;
            List<Anime> animeList = new List<Anime>();

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return animeList;
            }

            var responseStream = response.GetResponseStream();

            var xml = XDocument.Load(responseStream);
            var list = xml.Descendants("entry").ToList();
            foreach (var anime in list)
            {
                var animeObj = new Anime()
                {
                    MalId = Int32.Parse(anime.Element("id").Value),
                    Title = anime.Element("title").Value,
                    NoOfEpisodes = int.Parse(anime.Element("episodes").Value),
                    Status = anime.Element("status").Value,
                    Type = anime.Element("type").Value,
                    Score = anime.Element("score").Value,
                    Image = anime.Element("image").Value,
                    Synonyms = anime.Element("synonyms").Value,
                    Synopsis = WebUtility.HtmlDecode(anime.Element("synopsis").Value)
                };
                animeList.Add(animeObj);
            }

            return animeList;
        }

        [HttpGet]
        public ActionResult GetAddAnimePartial()
        {
            return PartialView("Partials/_AddAnimePartial");
        }

        [HttpGet]
        public async Task<ActionResult> GetAnimeForm(int malId, string title)
        {
            if (string.IsNullOrEmpty(title))
                return BadRequest("Missing Anime Title");

            if (malId == 0)
                return BadRequest("Missing Anime MAL id.");

            if (Context.Anime.Any(x => x.MalId == malId))
                return BadRequest("Anime already exists");

            var animeSearchList = await GetAnimeFromMAL(title);
            var animeMal = animeSearchList.FirstOrDefault(x => x.MalId == malId);

            if (animeMal == null)
                return BadRequest("Anime doesn't exist");

            var model = AutoMapper.Map<Anime, AnimeViewModel>(animeMal);

            return PartialView("Partials/_AddAnimeForm", model);
        }

        public async Task<ActionResult> GetAddEpisodePartial(int animeId)
        {
            var anime = await Context.Anime.Include(x => x.Episodes).FirstOrDefaultAsync(x => x.Id == animeId);
            if (anime == null)
                return BadRequest();

            var model = new EpisodeViewModel()
            {
                AnimeTitle = anime.Title,
                AnimeId = anime.Id,
                Order = anime.Episodes.Count + 1,
            };

            return PartialView("Partials/_AddEpisodePartial", model);
        }

        [HttpPost]
        public async Task<ActionResult> AddEpisode(EpisodeViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var anime = await Context.Anime.Include(x => x.Episodes).FirstOrDefaultAsync(x => x.Id == model.AnimeId);
            if (anime == null)
                return BadRequest();

            if (anime.Episodes.Any(x => x.Order == model.Order))
                return BadRequest();

            var episode = AutoMapper.Map<Episode>(model);
            episode.Sources = model.Sources.Select(x => AutoMapper.Map<Source>(x)).ToList();

            anime.Episodes.Add(episode);
            Context.Anime.Update(anime);
            await Context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult> GetEpisodePartial(int id)
        {
            var episode = await Context.Episodes.Include(x => x.Anime).Include(x => x.Sources).FirstOrDefaultAsync(x => x.Id == id);

            if (episode == null)
                return BadRequest();

            episode.ViewCount++;
            Context.Update(episode);
            await Context.SaveChangesAsync();

            var model = AutoMapper.Map<EpisodeViewModel>(episode);

            return PartialView("Partials/_EpisodePartial", model);
        }


        [HttpGet]
        public async Task<ActionResult> GetComments(int id)
        {
            var model = await Context.Comments.Where(x => x.EpisodeId == id).Include(x => x.Replies).Select(x => AutoMapper.Map<CommentViewModel>(x)).ToListAsync();
            return PartialView("Partials/_CommentsPartial", model);
        }

        [HttpPost]
        public async Task<ActionResult> AddComment(CommentViewModel model)
        {
            var comments = await Context.Comments.Include(x => x.Replies).Where(x => x.EpisodeId == model.EpisodeId).ToListAsync();

            if (comments == null)
                return BadRequest();

            if (model.ParentCommentId != null)
            {
                var comment = comments.FirstOrDefault(x => x.Id == model.ParentCommentId);
                if (comment == null)
                    return BadRequest();

                comment.Replies.Add(AutoMapper.Map<Comment>(model));
            }
            else
                Context.Comments.Add(AutoMapper.Map<Comment>(model));

            await Context.SaveChangesAsync();

            return Ok();
        }
    }
}
