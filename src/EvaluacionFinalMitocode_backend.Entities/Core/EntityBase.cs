namespace EvaluacionFinalMitocode_backend.Entities.Core
{
    public class EntityBase
    {
        public string Id { get; private set; } = default!;
        public bool ActiveStatus { get; set; } = true;
    }
}
