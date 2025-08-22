using Microsoft.AspNetCore.Mvc.Filters;

namespace EvaluacionFinalMitocode_backend.API.Filters
{
    /**
     * Cuando sucede una excepcion en la aplicacion a nivel global, se captura y se logea.
     * Esto es un filtro de excepciones que se aplica a nivel de controlador.
     */
    public class FilterExceptions(ILogger<FilterExceptions> logger) : ExceptionFilterAttribute
    {
        private readonly ILogger<FilterExceptions> logger = logger;
        public override void OnException(ExceptionContext context)
        {
            logger.LogError(context.Exception, "Un Error ocurrio en la aplicacion.");
            base.OnException(context);
        }
    }
}
