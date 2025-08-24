using EvaluacionFinalMitocode_backend.DTO.Request;
using EvaluacionFinalMitocode_backend.DTO.Response;

namespace EvaluacionFinalMitocode_backend.Services.Interfaces
{
    public interface ILibroService
    {
        Task<BaseResponseGeneric<List<LibroResponseDTO>>> SearchAsync(string? q, PaginationDTO pagination);
        Task<BaseResponseGeneric<LibroResponseDTO>> GetByIdAsync(string id);
        Task<BaseResponseGeneric<LibroResponseDTO>> CreateAsync(LibroCreateRequestDTO request);
        Task<BaseResponse> UpdateAsync(string id, LibroUpdateRequestDTO request);
        Task<BaseResponse> DeleteAsync(string id);        // soft delete (ActiveStatus = false)
        Task<BaseResponse> CheckoutAsync(string id);      // mark as rented (Disponible=false)
        Task<BaseResponse> CheckinAsync(string id);       // mark as returned (Disponible=true)
    }
}
