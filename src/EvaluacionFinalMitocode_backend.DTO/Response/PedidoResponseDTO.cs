namespace EvaluacionFinalMitocode_backend.DTO.Response;

public class PedidoResponseDTO
{
    public string PedidoId { get; set; } = default!;
    public DateTime FechaPedido { get; set; }
    public DateTime? FechaEntrega { get; set; }
    public decimal MontoTotal { get; set; }
    public bool Estado { get; set; }
    public List<PedidoItemResponseDTO> Items { get; set; } = new();
}

public class PedidoItemResponseDTO
{
    public string LibroId { get; set; } = default!;
    public string Titulo { get; set; } = default!;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal SubTotal => Cantidad * PrecioUnitario;
}
