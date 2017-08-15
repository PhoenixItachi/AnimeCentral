using AnimeCentralWeb.Domain;
using AnimeCentralWeb.Models.DomainViewModels;
using AutoMapper;

namespace AnimeCentralWeb.AutoMapper
{
    public class DomainToModelViewConfigurationProfile: Profile
    {
        public DomainToModelViewConfigurationProfile()
        {
            CreateMap<Anime, AnimeViewModel>();
        }
    }
}
