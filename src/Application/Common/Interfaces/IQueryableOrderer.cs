namespace Desic.Application.Common.Interfaces;

internal interface IQueryableOrderer<TSource>
{
    IOrderedQueryable<TSource> OrderBy(IQueryable<TSource> query, bool ascending);
    IOrderedQueryable<TSource> ThenBy(IOrderedQueryable<TSource> query, bool ascending);
}
