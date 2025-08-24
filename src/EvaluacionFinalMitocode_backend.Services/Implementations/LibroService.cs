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
        private readonly ILibroRepository repository = repository;
        private readonly ILogger<LibroService> logger = logger;
        private readonly IMapper mapper = mapper;
        private readonly IFileStorage fileStorage = fileStorage;
        private readonly string container = "libros";

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
                response.Success = false;
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
                response.Success = false;
                response.ErrorMessage = "Ocurrio un error al obtener los datos";
                logger.LogError(ex, "{ErrorMessage} {Message}", response.ErrorMessage, ex.Message);
            }
            return response;
        }

        public async Task<BaseResponseGeneric<string>> CreateAsync(LibroRequestDTO request)
        {
            var response = new BaseResponseGeneric<string>();
            Libro entity = new();
            try
            {
                entity = mapper.Map<Libro>(request);
                if (request.Image is not null)
                {
                    using (var memoryStream = new MemoryStream())
                    { 
                        await request.Image.CopyToAsync(memoryStream);
                        var content = memoryStream.ToArray();                       // Extract the raw byte[] that we’ll pass to the storage service.
                        var extension = Path.GetExtension(request.Image.FileName);  // Get the file extension from the original filename (e.g., ".jpg").
                        entity.ImageUrl = await fileStorage.SaveFile(content, extension, container, request.Image.ContentType);
                    }
                }
                response.Data = await repository.AddAsync(entity);
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = "An error occurred while processing your request.";
                logger.LogError("{Error Message} {Message}:", response.ErrorMessage, ex.Message);
            }
            return response;
        }

        public async Task<BaseResponse> UpdateAsync(string id, LibroRequestDTO request)
        {
            var response = new BaseResponse();
            try
            {
                var data = await repository.GetAsync(id);
                if (data is null)
                {
                    response.ErrorMessage = "The record you are trying to update does not exist.";
                    response.Success = false;
                    return response;
                }
                mapper.Map(request, data);

                if (request.Image is not null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await request.Image.CopyToAsync(memoryStream);
                        var content = memoryStream.ToArray();
                        var extension = Path.GetExtension(request.Image.FileName);
                        data.ImageUrl = await fileStorage.EditFile(content, extension, container, data.ImageUrl ?? string.Empty, request.Image.ContentType);
                    }
                }
                else 
                {
                    data.ImageUrl = string.Empty;
                }
                await repository.UpdateAsync();
                response.Success = true;
            }
            catch (Exception ex) 
            {
                response.Success = false;
                response.ErrorMessage = "An error occurred while processing your request.";
                logger.LogError("{Error Message} {Message}:", response.ErrorMessage, ex.Message);
            }
            return response;
        }

        public async Task<BaseResponse> DeleteAsync(string id)
        {
            var response = new BaseResponse();
            try 
            { 
                var data = await repository.GetAsync(id);
                if (data is null)
                {
                    response.ErrorMessage = "The record you are trying to delete does not exist.";
                    response.Success = false;
                    return response;
                }
                await fileStorage.DeleteFile(data.ImageUrl ?? string.Empty, container);
                await repository.DeleteAsync(id);
                response.Success = true;
            }
            catch (Exception ex) 
            {
                response.Success = false;
                response.ErrorMessage = "An error occurred while processing your deletion request.";
                logger.LogError("{Error Message} {Message}:", response.ErrorMessage, ex.Message);
            }
            return response;
        }

        public async Task<BaseResponse> CheckinAsync(string id)
        {
            var response = new BaseResponse();
            try
            {
                var ok = await repository.CheckinAsync(id);
                if (!ok)
                {
                    response.ErrorMessage = "Book already available or not found.";
                    response.Success = false;
                    return response;
                }
                response.Success = true;
            }
            catch (Exception ex) 
            {
                response.Success = false;
                response.ErrorMessage = "An error occurred while processing your checkin request.";
                logger.LogError("{Error Message} {Message}:", response.ErrorMessage, ex.Message);
            }
            return response;
        }

        public async Task<BaseResponse> CheckoutAsync(string id)
        {
            var response = new BaseResponse();
            try
            {
                var ok = await repository.CheckoutAsync(id);
                if (!ok)
                {
                    response.ErrorMessage = "Book not available or not found.";
                    response.Success = false;
                    return response;
                }
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = "An error occurred while processing your checkout request.";
                logger.LogError("{Error Message} {Message}:", response.ErrorMessage, ex.Message);
            }
            return response;
        }
    }
}
