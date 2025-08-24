using AutoMapper;
using EvaluacionFinalMitocode_backend.DTO.Request;
using EvaluacionFinalMitocode_backend.DTO.Response;
using EvaluacionFinalMitocode_backend.Entities;
using EvaluacionFinalMitocode_backend.Entities.Info;

namespace EvaluacionFinalMitocode_backend.Services.Profiles
{
    public class LibroProfile : Profile
    {
        public LibroProfile()
        {
            // Search results (from projection in repo) → API response
            CreateMap<LibroInfo, LibroResponseDTO>();

            // Entity → API response
            CreateMap<Libro, LibroResponseDTO>();

            // Create DTO → Entity
            CreateMap<LibroCreateRequestDTO, Libro>()
                // optional: trim inputs
                .ForMember(d => d.Titulo, o => o.MapFrom(s => s.Titulo.Trim() ?? string.Empty))
                .ForMember(d => d.Autor, o => o.MapFrom(s => s.Autor.Trim() ?? string.Empty))
                .ForMember(d => d.ISBN, o => o.MapFrom(s => s.ISBN.Trim() ?? string.Empty))
                // ActiveStatus will be true by default (DB default or entity default)
                ;

            // Update DTO → Entity (map onto existing entity instance)
            CreateMap<LibroUpdateRequestDTO, Libro>()
                .ForMember(d => d.Id, o => o.Ignore()) // never touch PK
                .ForMember(d => d.Titulo, o => o.MapFrom(s => s.Titulo.Trim() ?? string.Empty))
                .ForMember(d => d.Autor, o => o.MapFrom(s => s.Autor.Trim() ?? string.Empty))
                .ForMember(d => d.ISBN, o => o.MapFrom(s => s.ISBN.Trim() ?? string.Empty));
        }
    }
}
