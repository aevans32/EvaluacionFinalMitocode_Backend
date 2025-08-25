using EvaluacionFinalMitocode_backend.DTO.Request;
using EvaluacionFinalMitocode_backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvaluacionFinalMitocode_backend.API.Controllers;

/// <summary>
/// Endpoints para gestionar el catálogo de libros.
/// Permite buscar, obtener por ID, crear, actualizar, eliminar (soft delete),
/// y marcar un libro como alquilado/devuelto.
/// </summary>
/// <remarks>
/// Formato de ID: <c>LBR0001</c> (prefijo "LBR" + 4 dígitos).
/// </remarks>
[ApiController]
[Route("api/[controller]")]
//TODO: Implementar JWT Authentication
public class LibrosController(ILibroService _service, ILogger<LibrosController> _logger) : ControllerBase
{
    private readonly ILibroService service = _service;
    private readonly ILogger<LibrosController> logger = _logger;
    private const string IdConstraint = @"{id:regex(^LBR\d{{4}}$)}";
    private const string CheckoutRoute = "checkout/" + IdConstraint;
    private const string CheckinRoute = "checkin/" + IdConstraint;

    /// <summary>
    /// Busca libros activos por <b>Título</b> (contiene), <b>Autor</b> (empieza con) o <b>ISBN</b> (exacto).
    /// </summary>
    /// <param name="title">
    /// Texto de búsqueda. Si es nulo se considera cadena vacía, lo que devolverá todos los libros activos.
    /// </param>
    /// <param name="pagination">Parámetros de paginación (página y tamaño).</param>
    /// <returns>Lista paginada de libros que coinciden con el filtro.</returns>
    /// <response code="200">Libros encontrados.</response>
    /// <response code="404">No se encontraron libros con el filtro indicado.</response>
    [HttpGet("title")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Get(string? title, [FromQuery] PaginationDTO pagination)
    {
        logger.LogInformation("Getting books with title filter: {Title}", title);
        var response = await service.SearchAsync(title, pagination);
        return response.Success ? Ok(response) : NotFound(response);
    }

    /// <summary>
    /// Obtiene un libro por su identificador.
    /// </summary>
    /// <param name="id">Identificador del libro con formato <c>LBR0001</c>.</param>
    /// <returns>El libro solicitado.</returns>
    /// <response code="200">Libro encontrado.</response>
    /// <response code="404">No existe un libro con el ID especificado.</response>
    [HttpGet(IdConstraint)]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(string id)
    { 
        logger.LogInformation("Getting book by ID: {Id}", id);
        var response = await service.GetByIdAsync(id);
        return response.Success ? Ok(response) : NotFound(response);
    }

    /// <summary>
    /// Crea un nuevo libro.
    /// </summary>
    /// <param name="request">
    /// Datos del libro. Acepta <c>multipart/form-data</c> para enviar una imagen opcional (<c>Image</c>).
    /// Campos principales: <c>Titulo</c>, <c>Autor</c>, <c>Description</c>, <c>ExtendedDescription</c>, 
    /// <c>UnitPrice</c>, <c>GenreId</c>, <c>ISBN</c>, <c>Disponible</c>.
    /// </param>
    /// <returns>ID del libro creado y su información.</returns>
    /// <response code="200">Libro creado correctamente.</response>
    /// <response code="400">Error de validación o datos inválidos.</response>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Post([FromForm] LibroRequestDTO request)
    {
        logger.LogInformation("Creating a new book with title: {Title}", request.Titulo);
        var response = await service.CreateAsync(request);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Actualiza los datos de un libro existente.
    /// </summary>
    /// <param name="id">Identificador del libro con formato <c>LBR0001</c>.</param>
    /// <param name="request">
    /// Datos a actualizar. Acepta <c>multipart/form-data</c> para reemplazar la imagen opcionalmente.
    /// </param>
    /// <returns>Resultado de la operación.</returns>
    /// <response code="200">Libro actualizado correctamente.</response>
    /// <response code="400">Error de validación o datos inválidos.</response>
    [HttpPut(IdConstraint)]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Put(string id, [FromForm] LibroRequestDTO request)
    {
        logger.LogInformation("Updating book with ID: {Id}", id);
        var response = await service.UpdateAsync(id, request);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Elimina lógicamente (soft delete) un libro.
    /// </summary>
    /// <param name="id">Identificador del libro con formato <c>LBR0001</c>.</param>
    /// <returns>Resultado de la operación.</returns>
    /// <response code="200">Libro eliminado lógicamente.</response>
    /// <response code="400">El libro no existe o no pudo eliminarse.</response>
    [HttpDelete(IdConstraint)]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Delete(string id)
    {
        logger.LogInformation("Deleting book with ID: {Id}", id);
        var response = await service.DeleteAsync(id);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Marca un libro como <b>alquilado</b> (no disponible).
    /// </summary>
    /// <param name="id">Identificador del libro con formato <c>LBR0001</c>.</param>
    /// <returns>Resultado de la operación.</returns>
    /// <response code="200">Libro marcado como alquilado.</response>
    /// <response code="400">El libro no existe o ya no está disponible.</response>
    [HttpPost(CheckoutRoute)]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Checkout(string id)
    {
        logger.LogInformation("Checking out book with ID: {Id}", id);
        var response = await service.CheckoutAsync(id);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Marca un libro como <b>devuelto</b> (disponible).
    /// </summary>
    /// <param name="id">Identificador del libro con formato <c>LBR0001</c>.</param>
    /// <returns>Resultado de la operación.</returns>
    /// <response code="200">Libro marcado como disponible.</response>
    /// <response code="400">El libro no existe o ya estaba disponible.</response>
    [HttpPost(CheckinRoute)]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Checkin(string id)
    {
        logger.LogInformation("Checking in book with ID: {Id}", id);
        var response = await service.CheckinAsync(id);
        return response.Success ? Ok(response) : BadRequest(response);
    }
}
