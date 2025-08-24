using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EvaluacionFinalMitocode_backend.Repositories.Utils
{
    public static class HttpContextExtensions
    {
        public async static Task InsertarPaginacionHeader<T>(this HttpContext httpContext, IQueryable<T> queryable)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            List<T> totalRecords = await queryable.ToListAsync();
            httpContext.Response.Headers.Add("TotalRecordsQuantity", totalRecords.Count.ToString());
        }
    }
}
