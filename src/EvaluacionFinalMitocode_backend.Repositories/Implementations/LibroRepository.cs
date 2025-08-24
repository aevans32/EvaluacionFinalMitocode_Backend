using EvaluacionFinalMitocode_backend.Entities;
using EvaluacionFinalMitocode_backend.Persistence;
using EvaluacionFinalMitocode_backend.Repositories.Base.Implementation;
using EvaluacionFinalMitocode_backend.Repositories.Interfaces;
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
    }
}
