using Castle.Components.DictionaryAdapter.Xml;
using EvaluacionFinalMitocode_backend.DTO.Request;
using EvaluacionFinalMitocode_backend.Entities;
using EvaluacionFinalMitocode_backend.Entities.Info;
using EvaluacionFinalMitocode_backend.Persistence;
using EvaluacionFinalMitocode_backend.Repositories.Base.Implementation;
using EvaluacionFinalMitocode_backend.Repositories.Interfaces;
using EvaluacionFinalMitocode_backend.Repositories.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EvaluacionFinalMitocode_backend.Repositories.Implementations;

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

    /// <summary>
    /// Retrieves a paginated collection of books that match the specified title or related criteria.
    /// </summary>
    /// <remarks>The method only includes books with an active status. The results are ordered by the
    /// book title. Pagination headers are added to the HTTP response to indicate the total number of items and
    /// pages.</remarks>
    /// <param name="title">The title or partial title to search for. The search also matches books where the author's name starts with
    /// the specified title or the ISBN exactly matches the specified title. If <see langword="null"/> or empty, all
    /// active books are included.</param>
    /// <param name="pagination">The pagination parameters, including the page number and page size, used to control the size and position of
    /// the result set.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of <see
    /// cref="LibroInfo"/> objects representing the books that match the search criteria. The collection is empty if
    /// no books match.</returns>
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
                Autor = lib.Autor,
                Description = lib.Description,
                ExtendedDescription = lib.ExtendedDescription,
                UnitPrice = lib.UnitPrice,
                GenreId = lib.GenreId,
                ImageUrl = lib.ImageUrl,
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
        // 1) DNI desde el token
        var dni = httpContext.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == "dni")?.Value?.Trim();
        if (string.IsNullOrEmpty(dni)) return false;

        // 2) Cliente
        var cliente = await context.Clientes.FirstOrDefaultAsync(c => c.DNI == dni);
        if (cliente is null) return false;

        // 3) Libro (disponible)
        var libro = await context.Libros.FirstOrDefaultAsync(l => l.Id == id);
        if (libro is null || !libro.ActiveStatus || !libro.Disponible) return false;

        // 4) Registrar pedido + marcar no disponible
        await using var trx = await context.Database.BeginTransactionAsync();
        try
        {
            var pedido = new Pedido
            {
                ClienteId = cliente.Id,
                FechaPedido = DateTime.UtcNow,
                Estado = true, // activo
                PedidoLibros = new List<PedidoLibro>
            {
                new PedidoLibro
                {
                    LibroId = libro.Id,
                    Cantidad = 1,
                    PrecioUnitario = libro.UnitPrice
                }
            }
            };
            pedido.MontoTotal = pedido.PedidoLibros.Sum(x => x.Cantidad * x.PrecioUnitario);

            await context.Pedidos.AddAsync(pedido);

            libro.Disponible = false;

            await context.SaveChangesAsync();

            await trx.CommitAsync();
            return true;

            //var libro = await GetAsync(id);
            //if (libro is null || !libro.ActiveStatus || !libro.Disponible) return false;

            //libro.Disponible = false;
            //await UpdateAsync();
            //return true;
        }
        catch
        {
            await trx.RollbackAsync();
            throw;
        }
    }


    public async Task<bool> CheckinAsync(string id)
    {
        // 1) DNI desde el token
        var dni = httpContext.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == "dni")?.Value?.Trim();
        if (string.IsNullOrEmpty(dni)) return false;

        // 2) Cliente
        var cliente = await context.Clientes.FirstOrDefaultAsync(c => c.DNI == dni);
        if (cliente is null) return false;

        // 3) Libro
        var libro = await context.Libros.FirstOrDefaultAsync(l => l.Id == id);
        if (libro is null || !libro.ActiveStatus || libro.Disponible) return false;

        // 4) Buscar pedido activo del cliente que contenga ese libro
        var pedido = await context.Pedidos
            .Include(p => p.PedidoLibros)
            .FirstOrDefaultAsync(p =>
                p.ClienteId == cliente.Id &&
                p.Estado == true &&
                p.PedidoLibros.Any(pl => pl.LibroId == id));

        if (pedido is null) return false;

        // 5) Cerrar pedido + marcar disponible
        await using var trx = await context.Database.BeginTransactionAsync();
        try
        {
            pedido.Estado = false;
            pedido.FechaEntrega = DateTime.UtcNow;

            libro.Disponible = true;

            // context.Pedidos.Update(pedido); // tracking ya detecta cambios
            // context.Libros.Update(libro);

            await context.SaveChangesAsync();

            await trx.CommitAsync();
            return true;
        }
        catch
        {
            await trx.RollbackAsync();
            throw;
        }
    }



    //public async Task<bool> CheckinAsync(string id)
    //{
    //    var libro = await GetAsync(id);
    //    if (libro is null || !libro.ActiveStatus || libro.Disponible) return false;

    //    libro.Disponible = true;
    //    await UpdateAsync();
    //    return true;
    //}


}
