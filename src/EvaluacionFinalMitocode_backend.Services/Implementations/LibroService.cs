using AutoMapper;
using EvaluacionFinalMitocode_backend.DTO.Request;
using EvaluacionFinalMitocode_backend.DTO.Response;
using EvaluacionFinalMitocode_backend.Entities;
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
            var response =  new BaseResponseGeneric<List<LibroResponseDTO>>();
            try
            { 
                var data = await repository.GetAsync(q, pagination);
                response.Data = mapper.Map<List<LibroResponseDTO>>(data);
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.ErrorMessage = "An error occurred while processing your request.";
                logger.LogError("{Error Message} {Message}:", response.ErrorMessage, ex.Message);
            }
            return response;
        }

        public async Task<BaseResponseGeneric<LibroResponseDTO>> GetByIdAsync(string id)
        {
            var response = new BaseResponseGeneric<LibroResponseDTO>();
            try
            { 
                var data = await repository.GetAsync(id);
                response.Data = mapper.Map<LibroResponseDTO>(data);
                response.Success = data != null;
            }
            catch (Exception ex) 
            {
                response.ErrorMessage = "Ocurrio un error al obtener los datos";
                logger.LogError(ex, "{ErrorMessage} {Message}", response.ErrorMessage, ex.Message);
            }
            return response;
        }

        public async Task<BaseResponseGeneric<string>> CreateAsync(LibroCreateRequestDTO request)
        {
            var response = new BaseResponseGeneric<string>();
            Libro entity = new();
            try
            {
                entity = mapper.Map<Libro>(request);
                // TODO: Necesitamos plementar la logica para almacenar la imagen
                if (request.)
                {
                    
                }

            }
            catch (Exception ex)
            {
                response.ErrorMessage = "An error occurred while processing your request.";
                logger.LogError("{Error Message} {Message}:", response.ErrorMessage, ex.Message);
            }
            return response;
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
