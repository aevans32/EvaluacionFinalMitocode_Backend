using EvaluacionFinalMitocode_backend.Entities.Core;

namespace EvaluacionFinalMitocode_backend.Entities;

public class Pedido : EntityBase
{
    public string ClienteId { get; set; } = default!;
    public Cliente Cliente { get; set; } = default!;
    public DateTime FechaPedido { get; set; }
    public DateTime? FechaEntrega { get; set; }
    public decimal MontoTotal { get; set; }
    public bool Estado { get; set; }   // true = activo/por entregar; false = cerrado/cancelado
    public List<PedidoLibro> PedidoLibros { get; set; } = new();
}

public class PedidoLibro
{
    public string PedidoId { get; set; } = default!;
    public Pedido Pedido { get; set; } = default!;
    public string LibroId { get; set; } = default!;
    public Libro Libro { get; set; } = default!;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}
