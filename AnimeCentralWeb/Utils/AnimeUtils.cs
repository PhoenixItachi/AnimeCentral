using AnimeCentralWeb.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace AnimeCentralWeb.Utils
{
    public static class AnimeUtils
    {
        public static List<string> Genres = new List<string>() { "Comedie", "Actiune", "Horror", "Romantic", "Istoric",
                                                                 "Fantezie", "Shounen", "Demoni", "Ecchi", "Mister", "Drama",
                                                                 "Harem", "Aventura", "Masini", "Dementia", "Joc", "Josei", "Copii", "Magie",
                                                                 "Arte Martiale", "Mecha", "Muzica", "Parodie", "Politie", "Samurai", "Scoala",
                                                                 "Sci-Fi", "Shoujo", "Shoujo Ai", "Shounen", "Shounen Ai", "Spatiu", "Sport",
                                                                 "SuperPuteri", "Supernatural", "Vampiri", "Yaoi", "Armata", "Psihologic", "Seinen",
                                                                 "Slice of Life", "Thriller" };

        public static List<SelectListItem> GetAnnouncementTypes()
        {
            var list = new List<SelectListItem>();
            foreach (var value in Enum.GetValues(typeof(AnnType)))
            {
                var displayName = (DisplayAttribute)typeof(AnnType).GetMember(value.ToString())[0].GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault();
                var text = value.ToString();

                if (displayName != null)
                    text = displayName.Name;

                list.Add(new SelectListItem()
                {
                    Value = value.ToString(),
                    Text = displayName.Name
                });
            }

            return list;
        }

        public static string GetDisplayName(this Enum value)
        {
            var type = value.GetType();
            var displayName = (DisplayAttribute)type.GetMember(value.ToString())[0].GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault();
            var text = value.ToString();

            if (displayName != null)
                return displayName.Name;
            return type.ToString();
        }

        public static string GetAnnouncementTypesCssClass(this AnnType value)
        {
            switch (value)
            {
                case AnnType.Anime:
                    return "new-anime";
                case AnnType.Annoucement:
                    return "announcement";
                case AnnType.Movie:
                    return "new-movie";
                case AnnType.Season:
                    return "new-anime-season";
                case AnnType.Special:
                    return "new-anime-others";
                default:
                    return string.Empty;
            }
        }
    }
}
