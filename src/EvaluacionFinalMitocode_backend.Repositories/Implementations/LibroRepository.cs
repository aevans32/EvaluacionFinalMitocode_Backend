using EvaluacionFinalMitocode_backend.DTO.Request;
using EvaluacionFinalMitocode_backend.Entities;
using EvaluacionFinalMitocode_backend.Entities.Info;
using EvaluacionFinalMitocode_backend.Persistence;
using EvaluacionFinalMitocode_backend.Repositories.Base.Implementation;
using EvaluacionFinalMitocode_backend.Repositories.Interfaces;
using EvaluacionFinalMitocode_backend.Repositories.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EvaluacionFinalMitocode_backend.Repositories.Implementations
{
    public class LibroRepository(ApplicationDbContext context, IHttpContextAccessor httpContext) : RepositoryBase<Libro>(context), ILibroRepository
    {
        private readonly IHttpContextAccessor httpContext = httpContext;
        private readonly ApplicationDbContext context = context;

        public async Task<ICollection<Libro>> GetLibrosByAutorAsync(string autor)
        {
            autor ??= string.Empty;

            return await context.Libros
                .Where(lib => lib.ActiveStatus && lib.Autor.StartsWith(autor))
                .AsNoTracking()
                .OrderBy(lib => lib.Titulo)
                .ToListAsync();
        }

        public async Task<ICollection<LibroInfo>> GetAsync(string? title, PaginationDTO pagination)
        {
            title ??= string.Empty;

            var queryable = context.Libros
                .Where(lib => lib.ActiveStatus &&
                    (
                        lib.Titulo.Contains(title) ||
                        lib.Autor.StartsWith(title) ||
                        lib.ISBN == title
                    ))
                .AsNoTracking()
                .Select(lib => new LibroInfo 
                {
                    Id = lib.Id,
                    ActiveStatus = lib.ActiveStatus,
                    Titulo = lib.Titulo,
                    ISBN = lib.ISBN,
                    Disponible = lib.Disponible
                })
                .AsQueryable();

            await httpContext.HttpContext!.InsertarPaginacionHeader(queryable);
            var response = await queryable.OrderBy(lib => lib.Titulo)
                .Paginate(pagination)
                .ToListAsync();

            return response;
        }

        // Mark a copy as not available (e.g., when rented)
        public async Task<bool> CheckoutAsync(string id)
        {
            var libro = await GetAsync(id);
            if (libro is null || !libro.ActiveStatus || !libro.Disponible) return false;

            libro.Disponible = false;
            await UpdateAsync();
            return true;
        }

        public async Task<bool> CheckinAsync(string id)
        {
            var libro = await GetAsync(id);
            if (libro is null || !libro.ActiveStatus || libro.Disponible) return false;

            libro.Disponible = true;
            await UpdateAsync();
            return true;
        }


    }
}
