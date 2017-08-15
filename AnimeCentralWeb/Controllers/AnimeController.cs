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
                        NoOfEpisodes = anime.Element("episodes").Value,
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

        [HttpGet]
        public ActionResult GetAnimePartial()
        {
            return PartialView("Partials/_AddAnimePartial");
        }
    }
}
