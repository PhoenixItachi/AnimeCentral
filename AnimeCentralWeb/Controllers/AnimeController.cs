using AnimeCentralWeb.Data;
using AnimeCentralWeb.Domain;
using AnimeCentralWeb.Models;
using AnimeCentralWeb.Models.DomainViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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

        #region Anime
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

                anime.TranslateStatus = TranslateStatus.InCursDeTraducere;
                await Context.Anime.AddAsync(anime);
                await Context.SaveChangesAsync();

                return Ok();
            }
            return BadRequest();
        }

        [HttpGet]
        public async Task<ActionResult> GetEditAnimePartial(int id)
        {
            var anime = await Context.Anime.FirstOrDefaultAsync(x => x.Id == id);
            if (anime == null)
                return BadRequest();

            var model = AutoMapper.Map<AnimeViewModel>(anime);
            return PartialView("Partials/_EditAnimePartial", model);
        }

        [HttpPost]
        public async Task<ActionResult> EditAnime(AnimeViewModel model)
        {

            if (ModelState.IsValid)
            {
                var anime = await Context.Anime.FirstOrDefaultAsync(x => x.Id == model.Id);
                if (anime == null)
                    return BadRequest();

                anime.BigImage = model.BigImage;
                anime.EpisodeLength = model.EpisodeLength;
                anime.Genres = model.Genres;
                anime.Image = model.Image;
                anime.NoOfEpisodes = model.NoOfEpisodes;
                anime.Title = model.Title;
                anime.Synopsis = model.Synopsis;
                anime.Synonyms = model.Synonyms;
                anime.Status = model.Status;
                anime.Type = model.Type;
                anime.Score = model.Score;
                anime.EpisodeLength = model.EpisodeLength;
                anime.BigImage = model.BigImage;

                Context.Update(anime);
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

        [HttpPost]
        public async Task<ActionResult> SetAnimeTranslateStatus(AnimeViewModel model)
        {
            if(!Enum.IsDefined(typeof(TranslateStatus), model.TranslateStatus.ToString()))
                return BadRequest();

            var anime = await Context.Anime.FirstOrDefaultAsync(x => x.Id == model.Id);
            if(anime == null)
                return BadRequest();

            anime.TranslateStatus = model.TranslateStatus;
            await Context.SaveChangesAsync();

            return Ok();
        }

        #endregion

        #region Episode

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

            var video = Request.Form.Files["LocalSource"];
            if(video != null && video.Length != 0)
            {
                if (!VideoUtils.VideoAcceptedFormats.Contains(video.ContentType.ToLower()))
                    return BadRequest("Formatul video-ului nu este suportat.");

                var videoFileName = await VideoUtils.SaveVideo(video);
                if (string.IsNullOrEmpty(videoFileName))
                    return BadRequest();

                var localSource = new Source()
                {
                    Label = "Local",
                    Origin = SourceOrigin.Local,
                    FileName = videoFileName
                };

                episode.Sources.Add(localSource);
            }

            var date = DateTime.UtcNow;
            episode.Date = date;
            anime.LatestEpisode = date;
            anime.Episodes.Add(episode);
            Context.Anime.Update(anime);
            
            

            await Context.SaveChangesAsync();
            await SendNotification($"{anime.Title} #{episode.Order}", episode.Title, null, anime.Image, true);
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult> GetEpisodePartial(int id)
        {
            var episode = await Context.Episodes.Include(x => x.Anime).FirstOrDefaultAsync(x => x.Id == id);

            var next = await Context.Episodes.Where(x => x.Order > episode.Order && x.AnimeId == episode.AnimeId).OrderBy(x => x.Order).FirstOrDefaultAsync();
            var previous = await Context.Episodes.Where(x => x.Order < episode.Order && x.AnimeId == episode.AnimeId).OrderByDescending(x => x.Order).FirstOrDefaultAsync();

            if (episode == null)
                return BadRequest();

            var sources = await Context.Sources.Where(x => x.EpisodeId == episode.Id).Select(x => new SourceViewModel() {
                Id = x.Id,
                Label = x.Label,
                Origin = x.Origin
            }).ToListAsync();


            if (sources.Count == 0)
                return BadRequest();

            episode.ViewCount++;
            Context.Update(episode);
            await Context.SaveChangesAsync();

            var model = AutoMapper.Map<EpisodeViewModel>(episode);
            model.Sources = sources;

            model.Next = (next != null && next.Order - episode.Order <= 1) ? next.Id : -1;
            model.Previous = (previous != null && episode.Order - previous.Order <= 1) ? previous.Id : -1;

            return PartialView("Partials/_EpisodePartial", model);
        }

        public async Task<ActionResult> GetEditEpisodePartial(int id)
        {
            var episode = await Context.Episodes.Include(x => x.Anime).Include(x => x.Sources).FirstOrDefaultAsync(x => x.Id == id);

            if (episode == null)
                return BadRequest();

            var model = AutoMapper.Map<EpisodeViewModel>(episode);
            return PartialView("Partials/_EditEpisodePartial", model);
        }

        [HttpPost]
        public async Task<ActionResult> EditEpisode(EpisodeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var episode = await Context.Episodes.Include(x => x.Sources).FirstOrDefaultAsync(x => x.Id == model.Id);
                if (episode == null)
                    return BadRequest();

                episode.Title = model.Title;
                episode.Order = model.Order;
                episode.Sources = model.Sources.Select(x => AutoMapper.Map<Source>(x)).ToList();

                Context.Update(episode);
                await Context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }

        [HttpGet]
        public async Task<ActionResult> VideoStream(int id)
        {
            var source = await Context.Sources.FirstOrDefaultAsync(x => x.EpisodeId == id && x.Origin == SourceOrigin.Local);
            if (source == null)
                return BadRequest("Sursa nu exista.");

            return new VideoUtils.VideoResult(source.FileName);
        }

        #endregion

        #region Comments

        [HttpGet]
        public async Task<ActionResult> GetComments(int id)
        {
            var model = await Context.Comments.Where(x => x.EpisodeId == id).Include(x => x.Replies).Include(x => x.User).Select(x => AutoMapper.Map<CommentViewModel>(x)).ToListAsync();
            return PartialView("Partials/_CommentsPartial", model);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> AddComment(CommentViewModel model)
        {
            var comments = await Context.Comments.Include(x => x.Replies).Where(x => x.EpisodeId == model.EpisodeId).ToListAsync();
            var userId = _userManager.GetUserId(User);

            if (comments == null)
                return BadRequest();

            if (userId == null)
                return BadRequest();

            model.UserId = userId;
            model.Date = DateTime.UtcNow;
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

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> DeleteComment(int id)
        {
            var comment = await Context.Comments.Include(x => x.User).Include(x => x.Replies).FirstOrDefaultAsync(x => x.Id == id);
            var userId = _userManager.GetUserId(User);

            if (comment == null || comment.User.Id != userId)
                return BadRequest("Commentul nu-ti apartine.");
            try
            {
                var replies = await Context.Comments.Include(x => x.User).Include(x => x.Replies).Where(x => x.ParentCommentId == comment.Id).ToListAsync();
                Context.RemoveRange(replies);
                Context.Remove(comment);
                await Context.SaveChangesAsync();
            }
            catch (Exception e)
            {

            }

            return Ok();
        }

        #endregion

        #region Announcements
        [HttpGet]
        [Authorize]
        public ActionResult GetAddAnnouncementPartial()
        {
            return PartialView("Partials/_AddAnnouncementPartial");
        }

        [HttpGet]
        public async Task<ActionResult> GetAnnouncementPartial(int id)
        {
            var announcement = await Context.Announcements.Include(x => x.Author).FirstOrDefaultAsync(x => x.Id == id);
            if (announcement == null)
                return BadRequest();

            var model = AutoMapper.Map<AnnouncementViewModel>(announcement);
            return PartialView("Partials/_AnnouncementPartial", model);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> AddAnnouncement(AnnouncementViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return BadRequest();

            var announcement = AutoMapper.Map<Announcement>(model);
            announcement.AuthorId = userId;
            announcement.Date = DateTime.UtcNow;

            Context.Announcements.Add(announcement);
            await Context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        [Authorize]
        public ActionResult GetEditAnnouncementPartial(int id)
        {
            var userId = _userManager.GetUserId(User);
            var announcement = Context.Announcements.FirstOrDefaultAsync(x => x.AuthorId == userId && x.Id == id);
            if (announcement == null)
                return BadRequest("Anuntul nu exista sau nu-ti apartine");

            var model = AutoMapper.Map<AnnouncementViewModel>(announcement);
            return PartialView("Partials/_EditAnnouncementPartial", model);
        }
        #endregion

        #region Notification

        [HttpPost]
        public async Task<ActionResult> SetNotificationToken(string token)
        {
            if (token != null)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    user.NotificationTokens = token;
                    await _userManager.UpdateAsync(user);
                }
            }

            return Ok();
        }

        public async Task SendNotification(string title, string body, string token, string image, bool toAll = false)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://fcm.googleapis.com/fcm/send");
            httpWebRequest.Headers[HttpRequestHeader.Authorization] = "key=AAAAU4LNi9w:APA91bHGLZtLyCgYvlV5n485_4ouG_6bTh-yWTpxbUEsvtTmCESZUObrqC5n5MqQ93mJaP46H_FPeKIT2oRKiOpQmaaZ16B-1EZe_0W1-MjkxwFMQNYLv5xZgGQTg6W5dcyXQc8wj4Fu";
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            var tokenList = Context.Users.Where(x => x.NotificationTokens != null).Select(x => x.NotificationTokens).ToList();
            using (var streamWriter = new StreamWriter(await httpWebRequest.GetRequestStreamAsync()))
            {

                var json = JsonConvert.SerializeObject(new
                {
                    notification = new
                    {
                        title = title,
                        body = body,
                        icon = image,
                        click_action = $"https://{HttpContext.Request.Host}"
                    },
                    registration_ids = tokenList
                });

                streamWriter.Write(json);
                streamWriter.Flush();
            }

            var httpResponse = (HttpWebResponse)(await httpWebRequest.GetResponseAsync());
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }
        }

        #endregion
    }
}
