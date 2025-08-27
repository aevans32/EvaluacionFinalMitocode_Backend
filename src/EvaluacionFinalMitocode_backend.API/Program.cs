using EvaluacionFinalMitocode_backend.API.Filters;
using EvaluacionFinalMitocode_backend.Entities.Core;
using EvaluacionFinalMitocode_backend.Persistence;
using EvaluacionFinalMitocode_backend.Persistence.Seeders;
using EvaluacionFinalMitocode_backend.Repositories.Implementations;
using EvaluacionFinalMitocode_backend.Repositories.Interfaces;
using EvaluacionFinalMitocode_backend.Services.Implementations;
using EvaluacionFinalMitocode_backend.Services.Interfaces;
using EvaluacionFinalMitocode_backend.Services.Profiles;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Configuracion de Serilog
var logPath = Path.Combine(AppContext.BaseDirectory, "logs", "log.txt");    // Ruta del archivo de log
var logger = new LoggerConfiguration()
    .WriteTo.Console()  // log to console
    .WriteTo.File(logPath,
        rollingInterval: RollingInterval.Day, // log file will roll daily
        restrictedToMinimumLevel: LogEventLevel.Information, // Log level
        retainedFileCountLimit: 7, // Keep logs for 7 days
        fileSizeLimitBytes: 10 * 1024 * 1024, // Limit each log file to 10 MB
        rollOnFileSizeLimit: true // Roll the file when it reaches the size limit
        ).CreateLogger();

try
{
    // Configurar Serilog como el logger de la aplicacion
    // Cuando cargue la aplicacion, mostrara en que ambiente se esta ejecutando (Development, Staging, Production, etc.)
    builder.Logging.AddSerilog(logger);
    logger.Information($"LOG INITIALIZED in {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "NO ENV"}");


    // Agregar los servicios
    builder.Services.AddControllers(options =>          // Se agregan los controladores con las configuraciones de los filtros encontrados en FiltersExceptions.cs
    {
        options.Filters.Add(typeof(FilterExceptions));
    });

    // Agregar Swagger para documentacion de la API
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "Evaluacion Final - Full Stack Dev - Mitocode",
            Version = "v1",
            Description = "API para la evaluacion final del curso Full Stack Dev de Mitocode por Andres Evans",
            Contact = new OpenApiContact
            {
                Name = "Andres Evans",
                Email = "andres_evans@outlook.com",
                Url = new Uri("https://linkedin.com/in/andresevans")
            }
        });
        var xml = Path.Combine(AppContext.BaseDirectory, "EvaluacionFinalMitocode_backend.API.xml");
        options.IncludeXmlComments(xml);
    });

    // Configurarn Contexto de la BD
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("defaultConnection"));
    });

    // Configurar AppSettings para poder cargar la info de appsettings.json e inyectarla en los servicios con IOptions<AppSettings>
    builder.Services.Configure<AppSettings>(builder.Configuration);

    // Repositories
    builder.Services.AddScoped<ILibroRepository, LibroRepository>();

    // Services
    builder.Services.AddScoped<ILibroService, LibroService>();
    builder.Services.AddScoped<IFileStorage, FileStorageLocal>();

    // TODO: Agregar ApplicationDbContext con SQL Server
    // Registering Health Checks
    builder.Services.AddHealthChecks()
        .AddCheck("selfcheck", () => HealthCheckResult.Healthy()) // Si llega hasta aqui, la aplicacion esta funcionando bien.
                                                                  //.AddDbContextCheck<ApplicationDbContext>(); // Esto verifica que la conexion a la BD es correcta y que la aplicacion puede acceder a ella. Como un ping.
        ;

    // Para Seeders
    builder.Services.AddScoped<UserDataSeeder>();

    // para la paginacion
    builder.Services.AddHttpContextAccessor();

    // Identity
    builder.Services.AddIdentity<EFUserIdentity, IdentityRole>(policies =>
    {
        policies.Password.RequireDigit = true;
        policies.Password.RequiredLength = 6;
        policies.User.RequireUniqueEmail = true;
    })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();


    builder.Services.AddAuthentication(x =>     //primero autenticacion
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(x =>
    {
        var key = Encoding.UTF8.GetBytes(builder.Configuration["JWT:JWTKey"] ??
            throw new InvalidOperationException("JWT key not configured"));
        x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });
    builder.Services.AddAuthorization(); //luego autorizacion



    // Agregar AutoMapper
    builder.Services.AddAutoMapper(config =>
    {
        config.AddProfile<LibroProfile>();
        // TODO: Agrega los demas profiles cuando esten implementados
        //config.AddProfile<GenreProfile>();
        //config.AddProfile<SaleProfile>();
        //config.AddProfile<ClienteProfile>();
    });

    /**
     * Para arriba estan los servicios!
     * Desde var app hacia abajo se le conoce como el 'pipeline' de la aplicacion.
     * Indicatr que elementos y funcionalidades se van a utilizar en la aplicacion.
     */

    var app = builder.Build();

    // Apply migrations and seeding data
    await ApplyMigrationsAndSeedDataAsync(app);

    /**
     * Agregar Swagger 
     * solo si la aplicacion se esta ejecutando en modo de desarrollo.
     */
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles(); // para servir archivos estaticos, como imagenes, css, js, etc. en servidor local.
    app.UseAuthorization();
    app.MapControllers();

    // Configurar HealthChecks
    app.MapHealthChecks("/healthcheck", new()
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    app.Run();
}
catch (Microsoft.Extensions.Hosting.HostAbortedException)
{
    logger.Warning("EF Tools spun up in Program.cs trying to resolve services during design time.");
}
catch (Exception ex)
{
    logger.Fatal(ex, "Una excepcion fatal ocurrio al inicializar la API.");
    throw;
}
finally
{
    Log.CloseAndFlush(); // Asegurarse de que todos los logs se escriban antes de cerrar la aplicacion. 
}

static async Task ApplyMigrationsAndSeedDataAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Aplica migraciones pendientes (si las hay)
    if (db.Database.GetPendingMigrations().Any())
        await db.Database.MigrateAsync();

    // Ejecuta seeders
    var userSeeder = scope.ServiceProvider.GetRequiredService<UserDataSeeder>();
    await userSeeder.SeedAsync();
}