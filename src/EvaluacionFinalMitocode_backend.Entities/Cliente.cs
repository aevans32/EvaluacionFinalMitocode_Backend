using EvaluacionFinalMitocode_backend.Entities.Core;

namespace EvaluacionFinalMitocode_backend.Entities;

public class Cliente : EntityBase
{
    public string? UserId { get; set; }           
    public string Nombres { get; set; } = default!;
    public string Apellidos { get; set; } = default!;
    public string DNI { get; set; } = default!;
    public int Edad { get; set; }
    public string Email { get; set; } = default!;

}
