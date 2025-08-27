using AutoMapper;
using EvaluacionFinalMitocode_backend.DTO.Request;
using EvaluacionFinalMitocode_backend.Entities;

namespace EvaluacionFinalMitocode_backend.Services.Profiles;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        // Register -> Cliente
        CreateMap<RegisterRequestDTO, Cliente>()
            .ForMember(d => d.Nombres, o => o.MapFrom(s => s.FirstName))
            .ForMember(d => d.Apellidos, o => o.MapFrom(s => s.LastName))
            .ForMember(d => d.Email, o => o.MapFrom(s => s.Email))
            .ForMember(d => d.DNI, o => o.MapFrom(s => s.DocumentNumber))
            .ForMember(d => d.Edad, o => o.MapFrom(s => s.Age))
            .ForMember(d => d.UserId, o => o.Ignore());
    }
}
