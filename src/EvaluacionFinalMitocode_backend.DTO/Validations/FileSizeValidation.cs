using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace EvaluacionFinalMitocode_backend.DTO.Validations
{
    public class FileSizeValidation(int _maxSizeInMegabytes) : ValidationAttribute
    {
        private readonly int maxSizeInMegabytes = _maxSizeInMegabytes;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null)
            {
                // Single Responsiblity Principle (SRP):
                // La responsabilidad de esta clase es validar el tamaño del archivo, no si el archivo es nulo.
                return ValidationResult.Success; // No file provided, validation passes
            }

            IFormFile? formFile = value as IFormFile;

            if (formFile is null)
                return ValidationResult.Success;    // Not a file, validation passes

            if (formFile.Length > maxSizeInMegabytes * 1024 * 1024)
                return new ValidationResult($"The file size exceeds the maximum limit of {maxSizeInMegabytes} MB.");

            return ValidationResult.Success; // File size is within the limit
        }
    }


}
