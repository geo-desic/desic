// source: https://github.com/ardalis/Specification/blob/main/src/Ardalis.Specification/IRepositoryBase.cs
// modified replacing specification references with linq expressions
using System.Linq.Expressions;

namespace Desic.Data.Shared;

/// <summary>
/// <para>
/// A <see cref="IReadRepository{T}" /> can be used to query instances of <typeparamref name="T" />.
/// </para>
/// </summary>
/// <typeparam name="T">The type of entity being operated on by this repository.</typeparam>
public interface IReadRepository<T> where T : class
{
    /// <summary>
    /// Finds an element with the given primary key value.
    /// </summary>
    /// <typeparam name="TId">The type of primary key.</typeparam>
    /// <param name="id">The value of the primary key for the entity to be found.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the <typeparamref name="T" />, or <see langword="null"/>.
    /// </returns>
    Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull;

    /// <summary>
    /// Returns the first element satisfying the condition, or a default value if none exist.
    /// </summary>
    /// <param name="predicate">A predicate to test each element for a condition.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the <typeparamref name="T" />, or <see langword="null"/>.
    /// </returns>
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs the query and returns the first element, or a default value if none exist.
    /// </summary>
    /// <param name="query">The query to perform</param>
    /// <param name="predicate">A predicate to test each element for a condition.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the <typeparamref name="T" />, or <see langword="null"/>.
    /// </returns>
    Task<T?> FirstOrDefaultAsync(Func<IQueryable<T>, IQueryable<T>> query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the only element satisfying the condition, or a default value if none exist; this method throws an exception if there is more than one element.
    /// </summary>
    /// <param name="predicate">A predicate to test each element for a condition.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the <typeparamref name="T" />, or <see langword="null"/>.
    /// </returns>
    Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs the query and returns the only element, or a default value if none exist; this method throws an exception if there is more than one element.
    /// </summary>
    /// <param name="query">The query to perform</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the <typeparamref name="T" />, or <see langword="null"/>.
    /// </returns>
    Task<T?> SingleOrDefaultAsync(Func<IQueryable<T>, IQueryable<T>> query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all elements of a sequence.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a <see cref="List{T}" /> that contains the elements.
    /// </returns>
    Task<List<T>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs the query and returns all elements.
    /// </summary>
    /// <param name="query">The query to perform</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a <see cref="List{T}" /> that contains the elements.
    /// </returns>
    Task<List<T>> ListAsync(Func<IQueryable<T>, IQueryable<T>> query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the count of elements that satisfy the condition.
    /// </summary>
    /// <param name="predicate">A predicate to test each element for a condition.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the
    /// number of elements in the input sequence.
    /// </returns>
    Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the total count of elements.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the
    /// number of elements in the input sequence.
    /// </returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a boolean that represents whether any elements satisfy the condition.
    /// </summary>
    /// <param name="predicate">A predicate to test each element for a condition.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains true if the 
    /// source sequence contains any elements; otherwise, false.
    /// </returns>
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a boolean whether any entity exists or not.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains true if the 
    /// source sequence contains any elements; otherwise, false.
    /// </returns>
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the results of performing the specified query.
    /// </summary>
    /// <param name="query">The query to perform</param>
    /// <returns>
    ///  Returns an IAsyncEnumerable which can be enumerated asynchronously.
    /// </returns>
    IAsyncEnumerable<T> AsAsyncEnumerable(Func<IQueryable<T>, IQueryable<T>> query);
}
