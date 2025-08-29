using EvaluacionFinalMitocode_backend.DTO.Request;
using EvaluacionFinalMitocode_backend.DTO.Response;
using EvaluacionFinalMitocode_backend.Entities.Auth;
using EvaluacionFinalMitocode_backend.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EvaluacionFinalMitocode_backend.API.Controllers;

/// <summary>
/// Endpoints relacionados a clientes: registro, autenticación,
/// gestión de contraseñas y consultas de alquileres.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ClientesController(IClienteService _service, ILogger<ClientesController> _logger) : ControllerBase
{
    private readonly IClienteService service = _service;
    private readonly ILogger<ClientesController> logger = _logger;

    /// <summary>
    /// Registra un nuevo cliente (rol Customer), crea su perfil en Users.Clientes y devuelve un JWT.
    /// </summary>
    /// <param name="request">Datos de registro del cliente.</param>
    /// <returns>Token JWT y expiración, más el Id de usuario recién creado.</returns>
    /// <response code="200">Registro exitoso.</response>
    /// <response code="400">Validación fallida o error al registrar.</response>
    [HttpPost("Register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
    {
        logger.LogInformation("Registering new client with email: {Email}", request.Email);
        var response = await service.RegisterAsync(request);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Inicia sesión y devuelve un JWT.
    /// </summary>
    /// <param name="request">Credenciales del usuario.</param>
    /// <returns>Token JWT y expiración.</returns>
    /// <response code="200">Login exitoso.</response>
    /// <response code="400">Usuario o contraseña incorrectos.</response>
    [HttpPost("Login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
    {
        logger.LogInformation("Client login attempt for email.");
        var response = await service.LoginAsync(request);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Solicita un token de reseteo de contraseña y lo envía por el canal configurado.
    /// </summary>
    /// <param name="request">Email del usuario.</param>
    /// <returns>Operación exitosa o mensaje de error.</returns>
    /// <response code="200">Token generado y enviado.</response>
    /// <response code="400">El usuario no existe u otro error.</response>
    [HttpPost("RequestTokenToResetPassword")]
    [AllowAnonymous]
    public async Task<IActionResult> RequestTokenToResetPassword([FromBody] ResetPasswordRequestDTO request)
    {
        logger.LogInformation("Requesting password reset token for email: {Email}", request.Email);
        var response = await service.RequestTokenToResetPasswordAsync(request);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Cambia la contraseña del usuario autenticado.
    /// </summary>
    /// <param name="request">Contraseña actual y nueva contraseña (confirmación opcional).</param>
    /// <returns>Resultado de la operación.</returns>
    /// <response code="200">Contraseña cambiada correctamente.</response>
    /// <response code="400">Error de validación o contraseña actual incorrecta.</response>
    /// <response code="401">No autenticado.</response>
    [HttpPost("ChangePassword")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDTO request)
    {
        logger.LogInformation("Changing password for authenticated user.");
        var email = HttpContext.User.Claims.First(p => p.Type == ClaimTypes.Email).Value;
        var response = await service.ChangePasswordAsync(email, request);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    /// <summary>
    /// Lista los libros alquilados por el cliente con el DNI indicado.
    /// </summary>
    /// <param name="dni">DNI del cliente (8 dígitos).</param>
    /// <param name="soloActivos">Si es true, solo pedidos activos (no devueltos).</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>Listado de libros alquilados.</returns>
    /// <response code="200">Consulta exitosa.</response>
    /// <response code="400">DNI inválido u otro error de negocio.</response>
    /// <response code="401">No autenticado.</response>
    /// <response code="403">No autorizado (requiere rol Administrator).</response>
    [HttpGet("{dni}/libros-alquilados")]
    [Authorize(
        Roles = Constants.RoleCustomer,
        AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> GetLibrosAlquiladosPorDni(
        [FromRoute] string dni,
        [FromQuery] bool soloActivos = true,
        CancellationToken ct = default)
    {
        var req = new LibrosPorDniRequestDTO { DNI = dni, SoloActivos = soloActivos };
        var response = await service.GetLibrosAlquiladosPorDniAsync(req, ct);
        return response.Success ? Ok(response) : BadRequest(response);
    }

}
