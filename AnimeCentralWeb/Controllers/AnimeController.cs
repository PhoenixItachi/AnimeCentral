using AnimeCentralWeb.Data;
using AnimeCentralWeb.Domain;
using AnimeCentralWeb.Models;
using AnimeCentralWeb.Models.DomainViewModels;
using AnimeCentralWeb.Utils;
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
            var model = await Context.Anime.Select(x => AutoMapper.Map<Anime, AnimeViewModel>(x)).ToListAsync();
            model.ForEach(x => x.TranslatedEpisodes = Context.Episodes.Count(y => y.AnimeId == x.Id));

            return PartialView("Partials/_AnimeList", model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddAnime(AnimeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var isAlready = await Context.Anime.AnyAsync(x => x.MalId == model.MalId);
                if (isAlready)
                {
                    ModelState.AddModelError(string.Empty, "Anime-ul deja exista.");
                    Response.StatusCode = AnimeUtils.PartialStatusCode;
                    return PartialView("_Partials/_AddAnimeForm", model);
                }

                var genres = model.Genres.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim());
                if (genres.Any(x => !AnimeUtils.Genres.Select(y => y.ToLower()).Contains(x.Trim().ToLower())))
                    return BadRequest("Unul sau mai multe genuri invalide.");

                model.Genres = string.Join(", ", genres);
                var anime = AutoMapper.Map<AnimeViewModel, Anime>(model);

                anime.TranslateStatus = TranslateStatus.InCursDeTraducere;
                await Context.Anime.AddAsync(anime);
                await Context.SaveChangesAsync();

                return Ok();
            }
            Response.StatusCode = AnimeUtils.PartialStatusCode;
            return PartialView("Partials/_AddAnimeForm", model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetEditAnimePartial(int id)
        {
            var anime = await Context.Anime.FirstOrDefaultAsync(x => x.Id == id);
            if (anime == null)
                return NotFound("Anime-ul nu exista.");

            var model = AutoMapper.Map<AnimeViewModel>(anime);
            return PartialView("Partials/_EditAnimePartial", model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditAnime(AnimeViewModel model)
        {

            if (ModelState.IsValid)
            {
                var anime = await Context.Anime.FirstOrDefaultAsync(x => x.Id == model.Id);
                if (anime == null)
                    return NotFound("Anime-ul nu exista.");

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
                anime.TranslateStatus = model.TranslateStatus;

                Context.Update(anime);
                await Context.SaveChangesAsync();

                return Ok();
            }

            Response.StatusCode = AnimeUtils.PartialStatusCode;
            return PartialView("Partials/_EditAnimePartial", model);
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

        [HttpGet]
        public ActionResult GetAnimeByIdPartial(int id)
        {
            var anime = Context.Anime.Include(x => x.Episodes).FirstOrDefault(x => x.Id == id);
            if (anime == null)
                return NotFound("Anime-ul nu exista.");

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
        [Authorize(Roles = "Admin")]
        public ActionResult GetAddAnimePartial()
        {
            return PartialView("Partials/_AddAnimePartial");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetAnimeForm(int malId, string title)
        {
            if (string.IsNullOrEmpty(title))
                return NotFound("Lipeste titlul anime-ului.");

            if (malId == 0)
                return NotFound("Lipseste MyAnimeList ID.");

            if (Context.Anime.Any(x => x.MalId == malId))
                return NotFound("Anime-ul deja exista.");

            var animeSearchList = await GetAnimeFromMAL(title);
            var animeMal = animeSearchList.FirstOrDefault(x => x.MalId == malId);

            if (animeMal == null)
                return NotFound("Anime-ul deja exista.");

            var model = AutoMapper.Map<Anime, AnimeViewModel>(animeMal);
            return PartialView("Partials/_AddAnimeForm", model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> SetAnimeTranslateStatus(AnimeViewModel model)
        {
            if (!Enum.IsDefined(typeof(TranslateStatus), model.TranslateStatus.ToString()))
                return NotFound("Status-ul nu este corect.");

            var anime = await Context.Anime.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (anime == null)
                return NotFound("Anime-ul nu exista.");

            anime.TranslateStatus = model.TranslateStatus;
            await Context.SaveChangesAsync();
            return Ok();
        }

        #endregion

        #region Episode

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetAddEpisodePartial(int animeId)
        {
            var anime = await Context.Anime.Include(x => x.Episodes).FirstOrDefaultAsync(x => x.Id == animeId);
            if (anime == null)
                return NotFound("Anime-ul nu exista.");

            var model = new EpisodeViewModel()
            {
                AnimeTitle = anime.Title,
                AnimeId = anime.Id,
                Order = anime.Episodes.Count + 1,
            };

            return PartialView("Partials/_AddEpisodePartial", model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddEpisode(EpisodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = AnimeUtils.PartialStatusCode;
                return PartialView("Partials/_AddEpisodePartial", model);
            }

            if (await Context.Episodes.AnyAsync(x => x.AnimeId == model.AnimeId && x.Order == model.Order))
                return BadRequest("Episdul exista deja.");

            if(model.Sources.Count == 0 && model.LocalSource == null)
            {
                Response.StatusCode = AnimeUtils.PartialStatusCode;
                ModelState.AddModelError(string.Empty, "Este nevoie de cel putin o sursa locala sau externa.");
                return PartialView("Partials/_AddEpisodePartial", model);
            }

            var anime = await Context.Anime.Include(x => x.Episodes).FirstOrDefaultAsync(x => x.Id == model.AnimeId);
            if (anime == null)
                return NotFound("Anime-ul nu exista.");

            if (anime.Episodes.Any(x => x.Order == model.Order))
                return NotFound("Episodul nu exista.");

            var episode = AutoMapper.Map<Episode>(model);
            episode.Sources = model.Sources.Select(x => AutoMapper.Map<Source>(x)).ToList();

            var video = model.LocalSource;
            if (video != null && video.Length != 0)
            {
                if (!VideoUtils.VideoAcceptedFormats.Contains(video.ContentType.ToLower()))
                    return BadRequest("Formatul video-ului nu este suportat.");

                var videoFileName = await VideoUtils.SaveVideo(video);
                if (string.IsNullOrEmpty(videoFileName))
                    return BadRequest("A aparut o problema la salvarea video-ului local.");

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
                return NotFound("Episodul nu exista.");

            var sources = await Context.Sources.Where(x => x.EpisodeId == episode.Id).Select(x => new SourceViewModel()
            {
                Id = x.Id,
                Label = x.Label,
                Link = x.Link,
                Origin = x.Origin
            }).ToListAsync();


            if (sources.Count == 0)
                return NotFound("Nu exista surse pentru acest episod.");

            episode.ViewCount++;
            Context.Update(episode);
            await Context.SaveChangesAsync();

            var model = AutoMapper.Map<EpisodeViewModel>(episode);
            model.Sources = sources;

            model.Next = (next != null && next.Order - episode.Order <= 1) ? next.Id : -1;
            model.Previous = (previous != null && episode.Order - previous.Order <= 1) ? previous.Id : -1;

            return PartialView("Partials/_EpisodePartial", model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetEditEpisodePartial(int id)
        {
            var episode = await Context.Episodes.Include(x => x.Anime).Include(x => x.Sources).FirstOrDefaultAsync(x => x.Id == id);

            if (episode == null)
                return NotFound("Episodul nu exista.");

            var model = AutoMapper.Map<EpisodeViewModel>(episode);
            return PartialView("Partials/_EditEpisodePartial", model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
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
                return NotFound("Sursa nu exista.");

            return new VideoUtils.VideoResult(source.FileName);
        }

        #endregion

        #region Comments

        [HttpGet]
        public async Task<ActionResult> GetComments(int id)
        {
            var model = await Context.Comments.Where(x => x.EpisodeId == id).Include(x => x.User).Include(x => x.Replies).ThenInclude(x => x.User).Select(x => AutoMapper.Map<CommentViewModel>(x)).ToListAsync();
            return PartialView("Partials/_CommentsPartial", model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> AddComment(CommentViewModel model)
        {
            var comments = await Context.Comments.Include(x => x.Replies).Where(x => x.EpisodeId == model.EpisodeId).ToListAsync();
            var userId = _userManager.GetUserId(User);

            if (comments == null)
                return NotFound("Comentariul parinte nu mai exista.");

            if (userId == null)
                return NotFound("Utilizatorul nu exista.");

            if (string.IsNullOrEmpty(model.Content) || string.IsNullOrWhiteSpace(model.Content))
                return BadRequest("Comentariu nu poate fi gol.");

            model.UserId = userId;
            model.Date = DateTime.UtcNow;
            if (model.ParentCommentId != null)
            {
                var comment = comments.FirstOrDefault(x => x.Id == model.ParentCommentId);
                if (comment == null)
                    return BadRequest("Comentariul parinte nu mai exista.");

                comment.Replies.Add(AutoMapper.Map<Comment>(model));
            }
            else
                Context.Comments.Add(AutoMapper.Map<Comment>(model));

            await Context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,User")]
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
        [Authorize(Roles = "Admin")]
        public ActionResult GetAddAnnouncementPartial()
        {
            return PartialView("Partials/_AddAnnouncementPartial");
        }

        [HttpGet]
        public async Task<ActionResult> GetAnnouncementPartial(int id)
        {
            var announcement = await Context.Announcements.Include(x => x.Author).FirstOrDefaultAsync(x => x.Id == id);
            if (announcement == null)
                return NotFound("Anuntul nu exista.");

            var model = AutoMapper.Map<AnnouncementViewModel>(announcement);
            return PartialView("Partials/_AnnouncementPartial", model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddAnnouncement(AnnouncementViewModel model)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = AnimeUtils.PartialStatusCode;
                return PartialView("Partials/_AddAnnouncementPartial", model);
            }

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return NotFound("Utilizatorul nu exista.");

            var announcement = AutoMapper.Map<Announcement>(model);
            announcement.AuthorId = userId;
            announcement.Date = DateTime.UtcNow;

            Context.Announcements.Add(announcement);
            await Context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult GetEditAnnouncementPartial(int id)
        {
            var userId = _userManager.GetUserId(User);
            var announcement = Context.Announcements.FirstOrDefaultAsync(x => x.AuthorId == userId && x.Id == id);
            if (announcement == null)
                return NotFound("Anuntul nu exista sau nu-ti apartine.");

            var model = AutoMapper.Map<AnnouncementViewModel>(announcement);
            return PartialView("Partials/_EditAnnouncementPartial", model);
        }

        [HttpGet]
        public async Task<ActionResult> GetAnnouncementsPage(int page)
        {
            var model = await Context.Announcements.OrderByDescending(x => x.Date).Skip(5 * (page - 1)).Take(5).Include(x => x.Author).Select(x => AutoMapper.Map<AnnouncementViewModel>(x)).ToListAsync();
            return PartialView("Partials/_AnnouncementsPartial", model);
        }
        
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteAnnouncement(int id)
        {
            var announcement = await Context.Announcements.FirstOrDefaultAsync(x => x.Id == id);

            if (announcement == null)
                return NotFound("Anuntul nu exista.");

            Context.Announcements.Remove(announcement);
            await Context.SaveChangesAsync();
            return Ok();
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

        [NonAction]
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
