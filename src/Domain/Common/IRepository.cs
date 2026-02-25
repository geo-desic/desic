// source: https://github.com/ardalis/Specification/blob/main/src/Ardalis.Specification/IRepositoryBase.cs
// modified replacing specification references with linq expressions
using System.Linq.Expressions;

namespace Desic.Domain.Common;

/// <summary>
/// <para>
/// A <see cref="IRepository{T}" /> can be used to query and save instances of <typeparamref name="T" />.
/// </para>
/// </summary>
/// <typeparam name="T">The type of entity being operated on by this repository.</typeparam>
public interface IRepository<T> : IReadRepository<T> where T : class
{
    /// <summary>
    /// Adds an entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the <typeparamref name="T" />.
    /// </returns>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds the given entities in the repository.
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of state entries written to the repository.</returns>
    Task<int> UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the given entities in the repository.
    /// </summary>
    /// <param name="entities">The entities to update.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of state entries written to the repository.</returns>
    Task<int> UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of state entries written to the repository.</returns>
    Task<int> DeleteAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the given entities in the repository.
    /// </summary>
    /// <param name="entities">The entities to remove.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of state entries written to the repository.</returns>
    Task<int> DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the all entities that satisfy the condition.
    /// </summary>
    /// <param name="predicate">A predicate to test each element for a condition.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of state entries written to the repository.</returns>
    Task<int> DeleteWhereAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists changes to the repository.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
