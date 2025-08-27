namespace EvaluacionFinalMitocode_backend.DTO.Response;

public class RegisterResponseDTO : LoginResponseDTO
{
    /**
     * Como este DTO hereda de LoginResponseDTO,
     * tambien obtiene las variables:
     * * public string Token { get; set; }
     * public DateTime Expiration { get; set; }
     */
    public string UserId { get; set; } = default!;
    public string ClienteId { get; set; } = default!;
}
