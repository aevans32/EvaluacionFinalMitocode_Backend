namespace EvaluacionFinalMitocode_backend.DTO.Response;

public class ClienteResponseDTO
{
    public string Id { get; set; } = default!;
    public string Nombres { get; set; } = default!;
    public string Apellidos { get; set; } = default!;
    public string DNI { get; set; } = default!;
    public int Edad { get; set; }
    public string Email { get; set; } = default!;
}
