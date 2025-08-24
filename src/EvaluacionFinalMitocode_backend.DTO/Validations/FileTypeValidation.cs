using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EvaluacionFinalMitocode_backend.DTO.Validations
{
    public class FileTypeValidation : ValidationAttribute // Custom validation attribute to check file type
    {
        private readonly string[]? validTypes;
        public FileTypeValidation(string[] validTypes)
        {
            this.validTypes = validTypes;
        }

        public FileTypeValidation(FileTypeGroup fileTypeGroup)
        {
            // Initialize validTypes based on the FileTypeGroup
            if (fileTypeGroup is FileTypeGroup.Image)
                validTypes = ["image/jpeg", "image/png", "image/jpg"];
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null)
                return ValidationResult.Success; // Allow null values

            if (validTypes is null || validTypes.Length == 0)
                return new ValidationResult("No valid file types specified.");

            IFormFile? formFile = value as IFormFile;

            if (formFile is null)
                return ValidationResult.Success;

            if (!validTypes.Contains(formFile.ContentType))
                return new ValidationResult($"Invalid file type. Allowed types are: {string.Join(", ", validTypes)}");

            return ValidationResult.Success; // File type is valid
        }
    }

    public enum FileTypeGroup
    {
        Image
    }
}
