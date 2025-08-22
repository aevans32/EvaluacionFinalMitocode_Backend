using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EvaluacionFinalMitocode_backend.Persistence
{
    /**
     * Usualmente IdentityUser seria suficiente para autenticar
     * Pero esta clase se crea para personalizarla mas.
     * Abierto a la extension, cerrado a la modificacion.
     */
    public class EFUserIdentity : IdentityUser
    {
        [StringLength(100)]
        public string FirstName { get; set; } = default!;
        [StringLength(100)]
        public string LastName { get; set; } = default!;
        public int Age { get; set; }
        public DocumentTypeEnum DocumentType { get; set; }
        [StringLength(20)]
        public string DocumentNumber { get; set; } = default!;
    }

    public enum DocumentTypeEnum : short 
    {
        Dni,
        Passport
    }
}
