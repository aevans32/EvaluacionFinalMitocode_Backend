namespace EvaluacionFinalMitocode_backend.Entities.Info
{
    public class LibroInfo
    {
        public string Id { get; set; } = default!;
        public bool ActiveStatus { get; set; } = true;
        public string Titulo { get; set; } = default!;
        public string Autor { get; set; } = default!;
        public string ISBN { get; set; } = default!;
        public Boolean Disponible { get; set; } = true;
    }
}
