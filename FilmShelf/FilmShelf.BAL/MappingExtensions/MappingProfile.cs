using AutoMapper;
using FilmShelf.BAL.DTOs;
using FilmShelf.BAL.Helpers;
using FilmShelf.DAL.Entities;
using FilmShelf.TMDbClient.Responses;

namespace FilmShelf.BAL.MappingExtensions;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Movie, MovieDTO>();

        CreateMap<SearchMovieResponse, MovieDTO>()
            .ForMember(dest => dest.PosterPath, act => act.MapFrom(src => PhotoPathGenerator.GeneratePosterPath(src.PosterPath)));
    }
}
