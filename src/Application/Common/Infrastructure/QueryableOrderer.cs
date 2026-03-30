using Desic.Application.Common.Interfaces;
using System.Linq.Expressions;

namespace Desic.Application.Common.Infrastructure;

internal class QueryableOrderer<TSource, TKey> : IQueryableOrderer<TSource>
{
    public required Expression<Func<TSource, TKey>> KeySelector { get; set; }
    public IOrderedQueryable<TSource> OrderBy(IQueryable<TSource> query, bool ascending) => ascending ? query.OrderBy(keySelector: KeySelector) : query.OrderByDescending(keySelector: KeySelector);
    public IOrderedQueryable<TSource> ThenBy(IOrderedQueryable<TSource> query, bool ascending) => ascending ? query.ThenBy(keySelector: KeySelector) : query.ThenByDescending(keySelector: KeySelector);
}
