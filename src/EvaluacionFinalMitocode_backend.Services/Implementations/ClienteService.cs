using AutoMapper;
using EvaluacionFinalMitocode_backend.DTO.Request;
using EvaluacionFinalMitocode_backend.DTO.Response;
using EvaluacionFinalMitocode_backend.Entities;
using EvaluacionFinalMitocode_backend.Entities.Auth;
using EvaluacionFinalMitocode_backend.Persistence;
using EvaluacionFinalMitocode_backend.Repositories.Interfaces;
using EvaluacionFinalMitocode_backend.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security;
using System.Security.Claims;
using System.Text;

namespace EvaluacionFinalMitocode_backend.Services.Implementations;

public class ClienteService : IClienteService
{
    private readonly UserManager<EFUserIdentity> userManager;
    private readonly SignInManager<EFUserIdentity> signInManager;
    private readonly IClienteRepository clienteRepository;
    private readonly IMapper mapper;
    private readonly ILogger<ClienteService> logger;
    private readonly IConfiguration config;

    public ClienteService(
        UserManager<EFUserIdentity> _userManager,
        SignInManager<EFUserIdentity> _signInManager,
        IClienteRepository _clienteRepository,
        IMapper _mapper,
        ILogger<ClienteService> _logger,
        IConfiguration _config)
    {
        userManager = _userManager;
        signInManager = _signInManager;
        clienteRepository = _clienteRepository;
        mapper = _mapper;
        logger = _logger;
        config = _config;
    }

    public async Task<BaseResponseGeneric<IReadOnlyList<LibroAlquiladoResponseDTO>>> GetLibrosAlquiladosPorDniAsync(LibrosPorDniRequestDTO request, CancellationToken ct = default)
    {
        var resp = new BaseResponseGeneric<IReadOnlyList<LibroAlquiladoResponseDTO>>();
        try
        {
            var dni = (request.DNI ?? string.Empty).Trim();

            if (dni.Length != 8 || !dni.All(char.IsDigit))
            {
                resp.Success = false;
                resp.ErrorMessage = "DNI inválido. Debe tener 8 dígitos.";
                return resp;
            }

            var data = await clienteRepository.GetLibrosAlquiladosPorDniAsync(dni, request.SoloActivos, ct);
            resp.Success = true;
            resp.Data = data;
            return resp;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error listando libros alquilados por DNI {DNI}", request.DNI);
            resp.Success = false;
            resp.ErrorMessage = "Ocurrió un error al consultar los libros alquilados.";
            return resp;
        }
    }

    // ======================= AUTH =======================

    public async Task<BaseResponseGeneric<RegisterResponseDTO>> RegisterAsync(RegisterRequestDTO request)
    {
        var resp = new BaseResponseGeneric<RegisterResponseDTO>();
        try
        {
            if (request.Password != request.ConfirmPassword)
                throw new SecurityException("Las contraseñas no coinciden.");

            var user = new EFUserIdentity
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Age = request.Age,
                DocumentType = (DocumentTypeEnum)request.DocumentType,
                DocumentNumber = request.DocumentNumber,
                EmailConfirmed = true
            };

            var create = await userManager.CreateAsync(user, request.Password);
            if (!create.Succeeded)
            {
                resp.Success = false;
                resp.ErrorMessage = string.Join(" ", create.Errors.Select(e => e.Description));
                return resp;
            }

            // rol Customer
            await userManager.AddToRoleAsync(user, Constants.RoleCustomer);

            // fila en Users.Clientes
            var cliente = mapper.Map<Cliente>(request);
            cliente.UserId = user.Id;
            await clienteRepository.AddAsync(cliente);

            // token
            var token = await BuildTokenAsync(user);

            resp.Success = true;
            resp.Data = new RegisterResponseDTO
            {
                UserId = user.Id,
                Token = token.Token,
                Expiration = token.Expiration
            };
            return resp;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error en RegisterAsync");
            resp.Success = false;
            resp.ErrorMessage = "Ocurrió un error al registrar el usuario.";
            return resp;
        }
    }

    // Registro de Admin (sin crear fila en Users.Clientes)
    public async Task<BaseResponseGeneric<RegisterResponseDTO>> RegisterAdminAsync(RegisterRequestDTO request)
    {
        var resp = new BaseResponseGeneric<RegisterResponseDTO>();
        try
        {
            if (request.Password != request.ConfirmPassword)
                throw new SecurityException("Las contraseñas no coinciden.");

            var user = new EFUserIdentity
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Age = request.Age,
                DocumentType = (DocumentTypeEnum)request.DocumentType,
                DocumentNumber = request.DocumentNumber,
                EmailConfirmed = true
            };

            var create = await userManager.CreateAsync(user, request.Password);
            if (!create.Succeeded)
            {
                resp.Success = false;
                resp.ErrorMessage = string.Join(" ", create.Errors.Select(e => e.Description));
                return resp;
            }

            // rol Admin
            await userManager.AddToRoleAsync(user, Constants.RoleAdmin);

            // NO crear fila en Users.Clientes

            var token = await BuildTokenAsync(user);
            resp.Success = true;
            resp.Data = new RegisterResponseDTO
            {
                UserId = user.Id,
                Token = token.Token,
                Expiration = token.Expiration
            };
            return resp;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error en RegisterAdminAsync");
            resp.Success = false;
            resp.ErrorMessage = "Ocurrió un error al registrar el administrador.";
            return resp;
        }
    }


    public async Task<BaseResponseGeneric<LoginResponseDTO>> LoginAsync(LoginRequestDTO request)
    {
        var resp = new BaseResponseGeneric<LoginResponseDTO>();
        try
        {
            var signIn = await signInManager.PasswordSignInAsync(
                request.Username, request.Password, isPersistent: false, lockoutOnFailure: false);

            if (!signIn.Succeeded)
            {
                resp.Success = false;
                resp.ErrorMessage = "Usuario o contraseña incorrectos.";
                return resp;
            }

            var user = await userManager.FindByEmailAsync(request.Username)
                       ?? await userManager.FindByNameAsync(request.Username);

            if (user is null)
            {
                resp.Success = false;
                resp.ErrorMessage = "Usuario no encontrado.";
                return resp;
            }

            var token = await BuildTokenAsync(user);
            resp.Success = true;
            resp.Data = token;
            return resp;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error en LoginAsync");
            resp.Success = false;
            resp.ErrorMessage = "Ocurrió un error al iniciar sesión.";
            return resp;
        }
    }

    public async Task<BaseResponse> RequestTokenToResetPasswordAsync(ResetPasswordRequestDTO request)
    {
        var resp = new BaseResponse();
        try
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null) throw new SecurityException("El usuario no existe.");

            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            // Aquí podrías enviar correo/SMS. Por ahora, log para pruebas:
            logger.LogInformation("Token de reset para {Email}: {Token}", request.Email, token);

            resp.Success = true;
            return resp;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error solicitando token de reseteo");
            resp.Success = false;
            resp.ErrorMessage = "Ocurrió un error al solicitar el token.";
            return resp;
        }
    }

    public async Task<BaseResponse> ResetPasswordAsync(NewPasswordRequestDTO request)
    {
        var resp = new BaseResponse();
        try
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null) throw new SecurityException("El usuario no existe.");

            // Usa ConfirmNewPassword si existe; si no, usa NewPassword
            var newPwd = !string.IsNullOrWhiteSpace(request.ConfirmNewPassword)
                ? request.ConfirmNewPassword
                : request.NewPassword;

            var result = await userManager.ResetPasswordAsync(user, request.Token, newPwd);
            resp.Success = result.Succeeded;
            if (!result.Succeeded)
                resp.ErrorMessage = string.Join(" ", result.Errors.Select(e => e.Description));
            return resp;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error en ResetPasswordAsync");
            resp.Success = false;
            resp.ErrorMessage = "Ocurrió un error al reestablecer la clave.";
            return resp;
        }
    }

    public async Task<BaseResponse> ChangePasswordAsync(string email, ChangePasswordRequestDTO request)
    {
        var response = new BaseResponse();

        try
        {   // TODO: Corregir esta clase 
            var userIdentity = await userManager.FindByEmailAsync(email);
            if (userIdentity is null)
            {
                logger.LogWarning("El usuario con email {Email} no existe.", email);
                throw new SecurityException("El usuario no existe.");
            }

            var result = await userManager.ChangePasswordAsync(userIdentity, request.OldPassword, request.NewPassword);

            response.Success = result.Succeeded;

            if (!response.Success)
            {
                response.ErrorMessage = String.Join(" ", result.Errors.Select(x => x.Description).ToArray());
            }
            else
            {
                logger.LogInformation("La clave del usuario {Email} ha sido cambiada correctamente.", email);

                // TODO: Implementar emailService, Enviar un email al usuario informando que su clave fue cambiada
                //await emailService.SendEmailAsync(email, "Confirmacion de cambio de clave",
                //    @$"
                //            <p>Estimado {userIdentity.FirstName} {userIdentity.LastName}</p>
                //            <p>Su clave ha sido cambiada correctamente.</p>
                //            <hr />
                //            Atte. <br />
                //            <strong>Music Store 2025</strong>    
                //        ");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ocurrio un error al cambiar la clave del usuario {Email}. {Message}", email, ex.Message);
            response.Success = false;
            response.ErrorMessage = "Ocurrio un error al cambiar la clave del usuario.";
        }
        return response;
    }

    // ======================= Helpers =======================

    private async Task<LoginResponseDTO> BuildTokenAsync(EFUserIdentity user)
    {
        var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}".Trim()),
                new Claim("dni", user.DocumentNumber ?? string.Empty)
            };

        var roles = await userManager.GetRolesAsync(user);
        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var keyStr = config["JWT:JWTKey"] ??
                     throw new InvalidOperationException("JWT:JWTKey no está configurado.");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // TTL opcional configurable: JWT:LifeTimeInSeconds (default 3600)
        var ttlStr = config["JWT:LifeTimeInSeconds"];
        _ = int.TryParse(ttlStr, out var ttl); if (ttl <= 0) ttl = 3600;
        var exp = DateTime.UtcNow.AddSeconds(ttl);

        var jwt = new JwtSecurityToken(
            issuer: null,
            audience: null,
            claims: claims,
            expires: exp,
            signingCredentials: creds);

        return new LoginResponseDTO
        {
            Token = new JwtSecurityTokenHandler().WriteToken(jwt),
            Expiration = exp
        };
    }
}
