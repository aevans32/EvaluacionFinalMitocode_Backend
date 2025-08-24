using EvaluacionFinalMitocode_backend.DTO.Request;
using EvaluacionFinalMitocode_backend.Entities;
using EvaluacionFinalMitocode_backend.Entities.Info;
using EvaluacionFinalMitocode_backend.Repositories.Base.Interface;

namespace EvaluacionFinalMitocode_backend.Repositories.Interfaces
{
    public interface ILibroRepository : IRepositoryBase<Libro>
    {
        Task<ICollection<Libro>> GetLibrosByAutorAsync(string autor);
        Task<ICollection<LibroInfo>> GetAsync(string? title, PaginationDTO pagination);
        Task FinalizeAsync(string id);
    }
}
