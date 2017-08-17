using AnimeCentralWeb.Data;
using AnimeCentralWeb.Domain;
using AnimeCentralWeb.Models.DomainViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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
        public AnimeController(AnimeCentralDbContext context)
        {
            Context = context;
            AutoMapper = Mapper.Instance;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllAnime()
        {
            var model = Context.Anime.Select(x => AutoMapper.Map<Anime, AnimeViewModel>(x)).ToList();
            return PartialView("Partials/_AnimeList", model);
        }

        [HttpPost]
        public async Task<ActionResult> AddAnime(AnimeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var anime = AutoMapper.Map<AnimeViewModel, Anime>(model);

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
                    NoOfEpisodes = anime.Element("episodes").Value,
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
        public ActionResult GetAnimePartial()
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
    }
}
