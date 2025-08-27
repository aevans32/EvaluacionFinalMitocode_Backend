using System.ComponentModel.DataAnnotations;

namespace EvaluacionFinalMitocode_backend.DTO.Request;

public class PedidoCreateRequestDTO
{
    [Required, StringLength(8, MinimumLength = 8)]
    public string ClienteDni { get; set; } = default!;

    [MinLength(1, ErrorMessage = "El pedido debe tener al menos un item.")]
    public List<PedidoItemRequestDTO> Items { get; set; } = new();
}

public class PedidoItemRequestDTO
{
    [Required, StringLength(7, MinimumLength = 7)] // ej. LBR0001
    public string LibroId { get; set; } = default!;

    [Range(1, short.MaxValue)]
    public int Cantidad { get; set; }
}
