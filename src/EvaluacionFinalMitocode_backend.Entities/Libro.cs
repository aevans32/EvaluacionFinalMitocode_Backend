using EvaluacionFinalMitocode_backend.Entities.Core;

namespace EvaluacionFinalMitocode_backend.Entities
{
    public class Libro : EntityBase
    {
        public string Titulo { get; set; } = default!;
        public string Autor { get; set; } = default!;
        public string ISBN { get; set; } = default!;
    }
}
