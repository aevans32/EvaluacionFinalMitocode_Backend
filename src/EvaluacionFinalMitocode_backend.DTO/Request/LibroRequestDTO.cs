using EvaluacionFinalMitocode_backend.DTO.Validations;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace EvaluacionFinalMitocode_backend.DTO.Request;

public class LibroRequestDTO
{
    public string Titulo { get; set; } = null!;
    public string Autor { get; set; } = null!;
    public string Description { get; set; } = default!;
    public string ExtendedDescription { get; set; } = default!;
    public decimal UnitPrice { get; set; }
    public int GenreId { get; set; }
    [FileSizeValidation(1)]                         // Max size 1 MB
    [FileTypeValidation(FileTypeGroup.Image)]
    public IFormFile? Image { get; set; }
    public string ISBN { get; set; } = null!;
    public bool Disponible { get; set; } = true;
}

