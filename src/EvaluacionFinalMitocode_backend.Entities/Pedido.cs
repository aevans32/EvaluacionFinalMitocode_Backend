using EvaluacionFinalMitocode_backend.Entities.Core;

namespace EvaluacionFinalMitocode_backend.Entities
{
    public class Pedido : EntityBase
    {
        public string ClienteId { get; set; } = default!;
        public Cliente Cliente { get; set; } = default!;
        public string LibroId { get; set; } = default!;
        public List<Libro> Libros { get; set; } = default!;
        public DateTime FechaPedido { get; set; }
        public DateTime FechaEntrega { get; set; }
        public decimal MontoTotal { get; set; }
    }
}
