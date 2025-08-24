namespace EvaluacionFinalMitocode_backend.DTO.Response
{
    public class LibroResponseDTO
    {
        public string Id { get; set; } = null!;
        public string Titulo { get; set; } = null!;
        public string Autor { get; set; } = null!;
        public string ISBN { get; set; } = null!;
        public bool Disponible { get; set;  }
        public bool ActiveStatus { get; set; }
    }
}
