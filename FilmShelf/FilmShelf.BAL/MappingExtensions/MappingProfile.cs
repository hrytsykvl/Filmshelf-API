using AutoMapper;
using FilmShelf.BAL.DTOs;
using FilmShelf.DAL.Entities;

namespace FilmShelf.BAL.MappingExtensions;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Movie, MovieDTO>();
    }
}
