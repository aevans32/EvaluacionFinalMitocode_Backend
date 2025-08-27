using System.ComponentModel.DataAnnotations;

namespace EvaluacionFinalMitocode_backend.DTO.Request;

public class NewPasswordRequestDTO
{
    [EmailAddress]
    public string Email { get; set; } = default!;
    [Required]
    public string Token { get; set; } = default!;
    [Required]
    public string NewPassword { get; set; } = default!;
    [Compare(nameof(NewPassword))]
    public string ConfirmNewPassword { get; set; } = default!;
}
