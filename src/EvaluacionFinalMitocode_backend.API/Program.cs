using EvaluacionFinalMitocode_backend.API.Filters;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;


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
    });

    /**
     * Para arriba estan los servicios!
     * Desde var app hacia abajo se le conoce como el 'pipeline' de la aplicacion.
     * Indicatr que elementos y funcionalidades se van a utilizar en la aplicacion.
     */

    var app = builder.Build();

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
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
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