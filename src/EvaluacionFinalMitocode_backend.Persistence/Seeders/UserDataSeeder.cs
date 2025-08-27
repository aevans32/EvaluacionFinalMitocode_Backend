using EvaluacionFinalMitocode_backend.Entities;
using EvaluacionFinalMitocode_backend.Entities.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EvaluacionFinalMitocode_backend.Persistence.Seeders;

public class UserDataSeeder
{
    private readonly IServiceProvider _services;
    private readonly ILogger<UserDataSeeder> _logger;
    private readonly IConfiguration _config;

    public UserDataSeeder(IServiceProvider services,
                          ILogger<UserDataSeeder> logger,
                          IConfiguration config)
    {
        _services = services;
        _logger = logger;
        _config = config;
    }

    public async Task SeedAsync()
    {
        // Repos / managers
        var roleMgr = _services.GetRequiredService<RoleManager<IdentityRole>>();
        var userMgr = _services.GetRequiredService<UserManager<EFUserIdentity>>();
        var db = _services.GetRequiredService<ApplicationDbContext>();

        // 1) Roles (idempotente)
        async Task EnsureRoleAsync(string roleName)
        {
            if (!await roleMgr.RoleExistsAsync(roleName))
            {
                var r = await roleMgr.CreateAsync(new IdentityRole(roleName));
                if (!r.Succeeded)
                    _logger.LogError("Error creando rol {Role}: {Errors}",
                        roleName, string.Join(", ", r.Errors.Select(e => e.Description)));
            }
            else
            {
                _logger.LogInformation("Rol {Role} ya existe.", roleName);
            }
        }

        await EnsureRoleAsync(Constants.RoleAdmin);
        await EnsureRoleAsync(Constants.RoleCustomer);

        // 2) Valores desde appsettings (con fallback por defecto)
        var adminEmail = _config["Seed:AdminEmail"] ?? "admin@mitocode.local";
        var adminPass = _config["Seed:AdminPassword"] ?? "Admin#123";

        var custEmail = _config["Seed:CustomerEmail"] ?? "cliente@mitocode.local";
        var custPass = _config["Seed:CustomerPassword"] ?? "Cliente#123";
        var custDni = _config["Seed:CustomerDni"] ?? "12345678";
        var custName = _config["Seed:CustomerFirstName"] ?? "Juan";
        var custLast = _config["Seed:CustomerLastName"] ?? "Pérez";
        var custAgeStr = _config["Seed:CustomerAge"] ?? "30";
        _ = int.TryParse(custAgeStr, out var custAge); if (custAge <= 0) custAge = 30;

        // 3) Admin (idempotente)
        var admin = await userMgr.FindByEmailAsync(adminEmail);
        if (admin is null)
        {
            admin = new EFUserIdentity
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FirstName = "System",
                LastName = "Administrator",
                Age = 0,
                DocumentType = DocumentTypeEnum.Dni,
                DocumentNumber = "00000000"
            };

            var res = await userMgr.CreateAsync(admin, adminPass);
            if (!res.Succeeded)
            {
                _logger.LogError("Error creando Admin: {Errors}",
                    string.Join(", ", res.Errors.Select(e => e.Description)));
            }
            else
            {
                await userMgr.AddToRoleAsync(admin, Constants.RoleAdmin);
                _logger.LogInformation("Admin {Email} creado y asignado a rol {Role}.", adminEmail, Constants.RoleAdmin);
            }
        }
        else
        {
            // Asegurar rol
            if (!await userMgr.IsInRoleAsync(admin, Constants.RoleAdmin))
                await userMgr.AddToRoleAsync(admin, Constants.RoleAdmin);
        }

        // 4) Customer + fila en Users.Clientes (idempotente)
        var customer = await userMgr.FindByEmailAsync(custEmail);
        if (customer is null)
        {
            customer = new EFUserIdentity
            {
                UserName = custEmail,
                Email = custEmail,
                EmailConfirmed = true,
                FirstName = custName,
                LastName = custLast,
                Age = custAge,
                DocumentType = DocumentTypeEnum.Dni,
                DocumentNumber = custDni
            };

            var res = await userMgr.CreateAsync(customer, custPass);
            if (!res.Succeeded)
            {
                _logger.LogError("Error creando Customer: {Errors}",
                    string.Join(", ", res.Errors.Select(e => e.Description)));
            }
            else
            {
                await userMgr.AddToRoleAsync(customer, Constants.RoleCustomer);
                _logger.LogInformation("Customer {Email} creado y asignado a rol {Role}.", custEmail, Constants.RoleCustomer);
            }
        }
        else
        {
            // Asegurar rol
            if (!await userMgr.IsInRoleAsync(customer, Constants.RoleCustomer))
                await userMgr.AddToRoleAsync(customer, Constants.RoleCustomer);
        }

        // 4.1) Crear/actualizar fila en Users.Clientes vinculada por UserId
        var clienteRow = await db.Clientes
                                 .FirstOrDefaultAsync(c => c.Email == custEmail);

        if (clienteRow is null)
        {
            db.Clientes.Add(new Cliente
            {
                Nombres = custName,
                Apellidos = custLast,
                DNI = custDni,     // tiene UNIQUE y check de 8 dígitos
                Edad = custAge,
                Email = custEmail,   // tiene UNIQUE
                UserId = customer.Id  // FK opcional hacia Auth.User
            });
            await db.SaveChangesAsync();
            _logger.LogInformation("Fila Cliente creada para {Email}.", custEmail);
        }
        else
        {
            // Si existe y no está vinculado, vincular
            if (string.IsNullOrEmpty(clienteRow.UserId))
            {
                clienteRow.UserId = customer.Id;
                await db.SaveChangesAsync();
                _logger.LogInformation("Fila Cliente vinculada a UserId para {Email}.", custEmail);
            }
            else
            {
                _logger.LogInformation("Fila Cliente ya existe y está vinculada ({Email}).", custEmail);
            }
        }
    }
}
