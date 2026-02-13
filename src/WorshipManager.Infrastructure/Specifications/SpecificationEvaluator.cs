using Microsoft.EntityFrameworkCore;
using WorshipManager.Core.Specifications;

namespace WorshipManager.Infrastructure.Specifications;

public static class SpecificationEvaluator
{
    public static IQueryable<T> GetQuery<T>(IQueryable<T> inputQuery, ISpecification<T> spec) where T : class
    {
        var query = inputQuery;
        if (spec.Criteria != null) query = query.Where(spec.Criteria);
        query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));
        query = spec.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));
        if (spec.OrderBy != null) query = query.OrderBy(spec.OrderBy);
        else if (spec.OrderByDescending != null) query = query.OrderByDescending(spec.OrderByDescending);
        if (spec.IsPagingEnabled) query = query.Skip(spec.Skip ?? 0).Take(spec.Take ?? 10);
        return query;
    }
}
