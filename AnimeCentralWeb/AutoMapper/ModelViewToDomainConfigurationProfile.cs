using AnimeCentralWeb.Domain;
using AnimeCentralWeb.Models.DomainViewModels;
using AutoMapper;

namespace AnimeCentralWeb.AutoMapper
{
    public class ModelViewToDomainConfigurationProfile : Profile
    {
        public ModelViewToDomainConfigurationProfile()
        {
            CreateMap<AnimeViewModel, Anime>();
        }
    }
}
