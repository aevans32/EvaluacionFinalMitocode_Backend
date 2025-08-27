using System.ComponentModel.DataAnnotations;

namespace EvaluacionFinalMitocode_backend.DTO.Request;

public class LibrosPorDniRequestDTO
{
    [Required, StringLength(8, MinimumLength = 8)]
    public string DNI { get; set; } = default!;

    /// <summary>true = solo pedidos activos (no devueltos)</summary>
    public bool SoloActivos { get; set; } = true;
}