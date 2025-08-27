using EvaluacionFinalMitocode_backend.DTO.Response;
using EvaluacionFinalMitocode_backend.Entities;
using EvaluacionFinalMitocode_backend.Persistence;
using EvaluacionFinalMitocode_backend.Repositories.Base.Implementation;
using EvaluacionFinalMitocode_backend.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EvaluacionFinalMitocode_backend.Repositories.Implementations;

public class ClienteRepository(ApplicationDbContext _context, IHttpContextAccessor _httpContext) : RepositoryBase<Cliente>(_context), IClienteRepository
{
    private readonly IHttpContextAccessor httpContext = _httpContext;
    private readonly ApplicationDbContext context = _context;

    public async Task<Cliente?> GetByEmailAsync(string Email)
    {
        return await context.Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Email == Email);
    }

    public async Task<IReadOnlyList<LibroAlquiladoResponseDTO>> GetLibrosAlquiladosPorDniAsync(string dni, bool soloActivos = true, CancellationToken ct = default)
    {
        var query =
            from c in context.Clientes.AsNoTracking()
            where c.DNI == dni
            join p in context.Pedidos.AsNoTracking() on c.Id equals p.ClienteId
            where !soloActivos || p.Estado == true
            join pl in context.PedidoLibros.AsNoTracking() on p.Id equals pl.PedidoId
            join l in context.Libros.AsNoTracking() on pl.LibroId equals l.Id
            select new LibroAlquiladoResponseDTO
            {
                PedidoId = p.Id,
                FechaPedido = p.FechaPedido,
                FechaEntrega = p.FechaEntrega,
                Estado = p.Estado,
                LibroId = l.Id,
                Titulo = l.Titulo,
                ISBN = l.ISBN,
                Cantidad = pl.Cantidad,
                PrecioUnitario = pl.PrecioUnitario
            };

        return await query
            .OrderByDescending(x => x.FechaPedido)
            .ThenBy(x => x.Titulo)
            .ToListAsync(ct);
    }
}
