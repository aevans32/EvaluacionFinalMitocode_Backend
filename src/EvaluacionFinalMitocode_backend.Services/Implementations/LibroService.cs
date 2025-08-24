using AutoMapper;
using EvaluacionFinalMitocode_backend.DTO.Request;
using EvaluacionFinalMitocode_backend.DTO.Response;
using EvaluacionFinalMitocode_backend.Repositories.Interfaces;
using EvaluacionFinalMitocode_backend.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace EvaluacionFinalMitocode_backend.Services.Implementations
{
    public class LibroService(
        ILibroRepository repository,
        ILogger<LibroService> logger,
        IMapper mapper,
        IFileStorage fileStorage) : ILibroService
    {
        public async Task<BaseResponseGeneric<List<LibroResponseDTO>>> SearchAsync(string? q, PaginationDTO pagination)
        {
            throw new NotImplementedException();
        }

        public async Task<BaseResponseGeneric<LibroResponseDTO>> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<BaseResponseGeneric<LibroResponseDTO>> CreateAsync(LibroCreateRequestDTO request)
        {
            throw new NotImplementedException();
        }

        public async Task<BaseResponse> UpdateAsync(string id, LibroUpdateRequestDTO request)
        {
            throw new NotImplementedException();
        }

        public async Task<BaseResponse> DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<BaseResponse> CheckinAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<BaseResponse> CheckoutAsync(string id)
        {
            throw new NotImplementedException();
        }
    }
}
