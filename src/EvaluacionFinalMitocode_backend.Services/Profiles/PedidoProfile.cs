using AutoMapper;
using EvaluacionFinalMitocode_backend.DTO.Response;
using EvaluacionFinalMitocode_backend.Entities;

namespace EvaluacionFinalMitocode_backend.Services.Profiles;

public class PedidoProfile : Profile
{
    public PedidoProfile()
    {
        CreateMap<Pedido, PedidoResponseDTO>()
            .ForMember(d => d.PedidoId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Items, o => o.MapFrom(s => s.PedidoLibros));

        CreateMap<PedidoLibro, PedidoItemResponseDTO>()
            .ForMember(d => d.LibroId, o => o.MapFrom(s => s.LibroId))
            .ForMember(d => d.Titulo, o => o.MapFrom(s => s.Libro.Titulo))
            .ForMember(d => d.Cantidad, o => o.MapFrom(s => s.Cantidad))
            .ForMember(d => d.PrecioUnitario, o => o.MapFrom(s => s.PrecioUnitario));
    }
}
