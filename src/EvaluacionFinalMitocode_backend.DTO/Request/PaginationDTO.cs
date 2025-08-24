namespace EvaluacionFinalMitocode_backend.DTO.Request
{
    public class PaginationDTO
    {
        public int Page { get; set; } = 1;          // Empieza mostrando la primera pagina por default.
        private int recordsPerPage = 10;            // Empieza con 10 como default.
        private readonly int maxRecordsPerPage = 50; // Usuario no puede elegir mas de 50 records por pagina.

        public int RecordsPerPage
        {
            get { return recordsPerPage; }
            set { recordsPerPage = (value > maxRecordsPerPage) ? maxRecordsPerPage : value; }
        }
    }
}
