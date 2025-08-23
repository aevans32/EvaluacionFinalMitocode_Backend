using EvaluacionFinalMitocode_backend.Entities.Core;
using System.Linq.Expressions;

namespace EvaluacionFinalMitocode_backend.Repositories.Interfaces
{
    /**
     * Estas interfaces genericas hacen uso de predicados
     * TEntity depende de EntityBase al ser nuestra clase raiz.
     */
    public interface IRepositoryBase<TEntity> where TEntity: EntityBase
    {
        // Esta es simple, solo invoca GetAsync() y devuelve un objeto generico TEntity
        Task<ICollection<TEntity>> GetAsync();
        // Get con filtro
        Task<ICollection<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate);
        // Get con filtro ordenado. Expressions es uan clase de LinQ para usar funciones como filtros. 
        Task<ICollection<TEntity>> GetAsync<TKey>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TKey>> orderBy);
        // El id es string porque nuestros IDs son string (ver EntityBase.cs)
        Task<TEntity?> GetAsync(string id);
        // Agregar una entidad y devolver su ID
        Task<string> AddAsync(TEntity entity);
        // Actualizar una entidad
        Task UpdateAsync();
        // Eliminar una entidad por su ID
        Task DeleteAsync(string id);
    }
}
