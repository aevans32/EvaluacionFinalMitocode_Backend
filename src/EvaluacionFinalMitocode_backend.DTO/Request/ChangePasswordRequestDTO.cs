namespace EvaluacionFinalMitocode_backend.DTO.Request;

public class ChangePasswordRequestDTO
{
    public string OldPassword { get; set; } = default!;
    public string NewPassword { get; set; } = default!;
}
