using AutoMapper;
using FilmShelf.API.VMs;
using FilmShelf.BAL.DTOs;

namespace FilmShelf.API.MappingExtensions;

public class MappingProfileVM : Profile
{
    public MappingProfileVM()
    {
        CreateMap<MovieDTO, MovieResponseVM>();
    }
}
