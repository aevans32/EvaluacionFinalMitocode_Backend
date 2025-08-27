using System.ComponentModel.DataAnnotations;

namespace EvaluacionFinalMitocode_backend.DTO.Request;

public class RegisterRequestDTO
{
    [Required]
    [StringLength(200)]
    public string FirstName { get; set; } = default!;

    [Required]
    [StringLength(200)]
    public string LastName { get; set; } = default!;

    [EmailAddress]
    public string Email { get; set; } = default!;

    [Required]
    [StringLength(20)]
    public string DocumentNumber { get; set; } = default!;

    public int DocumentType { get; set; } = default!;

    public int Age { get; set; }

    [Required]
    public string Password { get; set; } = default!;

    [Compare(nameof(Password), ErrorMessage = "Las contrasenas deben coincidir.")]
    public string ConfirmPassword { get; set; } = default!;
}
