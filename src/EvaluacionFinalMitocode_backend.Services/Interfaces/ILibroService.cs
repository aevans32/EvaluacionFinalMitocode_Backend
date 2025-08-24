using EvaluacionFinalMitocode_backend.DTO.Request;
using EvaluacionFinalMitocode_backend.DTO.Response;

namespace EvaluacionFinalMitocode_backend.Services.Interfaces;

public interface ILibroService
{
    Task<BaseResponseGeneric<List<LibroResponseDTO>>> SearchAsync(string? q, PaginationDTO pagination);
    Task<BaseResponseGeneric<LibroResponseDTO>> GetByIdAsync(string id);
    Task<BaseResponseGeneric<string>> CreateAsync(LibroRequestDTO request);   // Retorna el Id del nuevo libro como string
    Task<BaseResponse> UpdateAsync(string id, LibroRequestDTO request);
    Task<BaseResponse> DeleteAsync(string id);        // soft delete (ActiveStatus = false)
    Task<BaseResponse> CheckoutAsync(string id);      // mark as rented (Disponible=false)
    Task<BaseResponse> CheckinAsync(string id);       // mark as returned (Disponible=true)
}

