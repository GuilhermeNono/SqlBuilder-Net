using SqlBuilder.Queries;

namespace SqlBuilder.Test.Tests.Models.Entity.CustomQueries.Filters;

public record FindByUserFilter(Guid Id) : IFilterParam
{
}