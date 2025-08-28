using EvaluacionFinalMitocode_backend.DTO.Request;
using EvaluacionFinalMitocode_backend.DTO.Response;

namespace EvaluacionFinalMitocode_backend.Services.Interfaces;

public interface IClienteService
{
    Task<BaseResponseGeneric<RegisterResponseDTO>> RegisterAsync(RegisterRequestDTO request);
    Task<BaseResponseGeneric<RegisterResponseDTO>> RegisterAdminAsync(RegisterRequestDTO request);
    Task<BaseResponseGeneric<LoginResponseDTO>> LoginAsync(LoginRequestDTO request);
    Task<BaseResponse> RequestTokenToResetPasswordAsync(ResetPasswordRequestDTO request);
    Task<BaseResponse> ResetPasswordAsync(NewPasswordRequestDTO request);
    Task<BaseResponse> ChangePasswordAsync(string email, ChangePasswordRequestDTO request);
    Task<BaseResponseGeneric<IReadOnlyList<LibroAlquiladoResponseDTO>>> GetLibrosAlquiladosPorDniAsync(LibrosPorDniRequestDTO request, CancellationToken ct = default);
}
