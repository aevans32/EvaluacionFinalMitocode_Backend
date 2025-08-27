using EvaluacionFinalMitocode_backend.DTO.Response;
using EvaluacionFinalMitocode_backend.Entities;
using EvaluacionFinalMitocode_backend.Repositories.Base.Interface;

namespace EvaluacionFinalMitocode_backend.Repositories.Interfaces;

public interface IClienteRepository : IRepositoryBase<Cliente>
{
    Task<Cliente?> GetByEmailAsync(string Email);
    Task<IReadOnlyList<LibroAlquiladoResponseDTO>> GetLibrosAlquiladosPorDniAsync(string dni, bool soloActivos = true, CancellationToken ct = default);
}
