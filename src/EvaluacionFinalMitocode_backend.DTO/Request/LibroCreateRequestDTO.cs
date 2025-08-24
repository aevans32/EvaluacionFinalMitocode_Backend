using System.ComponentModel.DataAnnotations;

namespace EvaluacionFinalMitocode_backend.DTO.Request
{
    public class LibroCreateRequestDTO
    {
        public string Titulo { get; set; } = null!;
        public string Autor { get; set; } = null!;
        public string ISBN { get; set; } = null!;
        public bool Disponible { get; set; } = true;
    }
}
