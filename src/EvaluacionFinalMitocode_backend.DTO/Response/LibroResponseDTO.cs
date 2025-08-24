namespace EvaluacionFinalMitocode_backend.DTO.Response
{
    public class LibroResponseDTO
    {
        public string Id { get; set; } = null!;
        public string Titulo { get; set; } = null!;
        public string Autor { get; set; } = null!;
        public string Description { get; set; } = default!;
        public string ExtendedDescription { get; set; } = default!;
        public decimal UnitPrice { get; set; }  // Use decimal to avoid rounding errors.
        public int GenreId { get; set; }
        public string? ImageUrl { get; set; }
        public string ISBN { get; set; } = null!;
        public bool Disponible { get; set;  }
        public bool ActiveStatus { get; set; }
    }
}
