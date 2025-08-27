using AutoMapper;
using EvaluacionFinalMitocode_backend.DTO.Response;
using EvaluacionFinalMitocode_backend.Entities;

namespace EvaluacionFinalMitocode_backend.Services.Profiles;

public class ClienteProfile : Profile
{
    public ClienteProfile()
    {
        CreateMap<Cliente, ClienteResponseDTO>();
    }
}
