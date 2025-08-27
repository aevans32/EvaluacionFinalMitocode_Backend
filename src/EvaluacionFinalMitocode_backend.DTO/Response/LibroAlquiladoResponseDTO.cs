namespace EvaluacionFinalMitocode_backend.DTO.Response;

public class LibroAlquiladoResponseDTO
{
    public string PedidoId { get; set; } = default!;
    public DateTime FechaPedido { get; set; }
    public DateTime? FechaEntrega { get; set; }
    public bool Estado { get; set; }              // true = activo (no devuelto)
    public string LibroId { get; set; } = default!;
    public string Titulo { get; set; } = default!;
    public string ISBN { get; set; } = default!;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal SubTotal => Cantidad * PrecioUnitario;
}
