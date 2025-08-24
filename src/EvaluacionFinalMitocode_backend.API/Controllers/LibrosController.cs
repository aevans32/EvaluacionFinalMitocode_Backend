using EvaluacionFinalMitocode_backend.DTO.Request;
using EvaluacionFinalMitocode_backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvaluacionFinalMitocode_backend.API.Controllers;
/**
 * Query param: cuando quieres hacer filtros para obtener data ordenada
 * Body param: los demas parametros dentro del body, data compleja, json, para guardar un nuevo registro
 * Route param: cuando quieres obtener la info de un recurso le pasa el ID
 */
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

    [HttpGet("title")]
    [AllowAnonymous]
    public async Task<IActionResult> Get(string? title, [FromQuery] PaginationDTO pagination)
    {
        logger.LogInformation("Getting books with title filter: {Title}", title);
        var response = await service.SearchAsync(title, pagination);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpGet(IdConstraint)]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(string id)
    { 
        logger.LogInformation("Getting book by ID: {Id}", id);
        var response = await service.GetByIdAsync(id);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromForm] LibroRequestDTO request)
    {
        logger.LogInformation("Creating a new book with title: {Title}", request.Titulo);
        var response = await service.CreateAsync(request);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPut(IdConstraint)]
    public async Task<IActionResult> Put(string id, [FromForm] LibroRequestDTO request)
    {
        logger.LogInformation("Updating book with ID: {Id}", id);
        var response = await service.UpdateAsync(id, request);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpDelete(IdConstraint)]
    public async Task<IActionResult> Delete(string id)
    {
        logger.LogInformation("Deleting book with ID: {Id}", id);
        var response = await service.DeleteAsync(id);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPost(CheckoutRoute)]
    public async Task<IActionResult> Checkout(string id)
    {
        logger.LogInformation("Checking out book with ID: {Id}", id);
        var response = await service.CheckoutAsync(id);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPost(CheckinRoute)]
    public async Task<IActionResult> Checkin(string id)
    {
        logger.LogInformation("Checking in book with ID: {Id}", id);
        var response = await service.CheckinAsync(id);
        return response.Success ? Ok(response) : BadRequest(response);
    }
}
