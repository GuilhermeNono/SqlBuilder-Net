using SqlBuilder.Interfaces;
using SqlBuilder.Queries;
using SqlBuilder.Queries.Pageable;
using SqlBuilder.Queries.Pageable.Interfaces;
using SqlBuilder.Structs;

namespace SqlBuilder;

public abstract class QueryConfigurer<TResult>(IFilterParam? filter)
    : QueryPropertyConfigurer<TResult>, IQueryConfigurer<TResult>
{
    protected IPagination<TResult> Pagination { get; } = new Pagination<TResult>(null, null);
    private IFilterParam? Filter { get; init; } = filter;

    /// <summary>
    /// Este método deve ser utilizado para construir a consulta SQL.
    /// <example>Exemplo:
    /// <code>
    /// Add("SELECT * ")
    /// Add("FROM foo_bar ")
    /// Add($"WHERE foo = {Param(x => x.bar)} ")
    /// </code>
    /// </example>
    /// </summary>
    protected abstract void Prepare();

    private void GetPreparedQuery()
    {
        Prepare();
        SqlQueryValidator
            .WithQuery(QueryBuilder)
            .Validate()
            .BuildVirtualVariables(Filter);
    }
    
    /// <summary>
    /// Essa propriedade deve ser utilizada para contagem dos registros de uma consulta.
    /// <returns>Será retornada a consulta SQL com a função agregada <c>"Count(1)"</c></returns>
    /// </summary>
    public string Count
    {
        get
        {
            QueryBuilder.Clear();
            SqlSelectWithCountQuery();
            return QueryBuilder.ToString();
        }
    }

    /// <summary>
    /// Essa propriedade deve ser utilizada para executar a consulta que foi desenvolvida no método <c>Prepare()</c>.
    /// <returns>Será retornada a consulta SQL</returns>
    /// </summary>
    public string Query
    {
        get
        {
            QueryBuilder.Clear();

            CheckIfThePaginationContainsOrdering();

            SqlSelectWithPrepareQuery();

            SqlOrderingQuery();

            SqlPaginationQuery();

            return QueryBuilder.ToString();
        }
    }
    

    private void SqlSelectWithCountQuery()
    {
        QueryBuilder.Clear();
        Add(" Select Count(1) as Value");
        Add("   From ( ");
        Prepare();
        Add("        ) t ");
    }

    private void SqlSelectWithPrepareQuery()
    {
        Add($"   Select * ");
        Add("   From ( ");
        GetPreparedQuery();
        Add("        ) t ");
    }

    private void SqlPaginationQuery()
    {
        Pagination.Validate();

        if (!(Pagination?.IsPageable ?? false))
            return;

        Add($"  Offset {Pagination?.Size ?? 10} * ({Pagination?.Page} - 1) ");
        Add($"  Rows Fetch Next {Pagination?.Size} Rows Only");
    }

    private void SqlOrderingQuery()
    {
        Add("  Order By ", Pagination.IsSortable);
        foreach (var order in Pagination.Ordering)
        {
            Add($" {order?.Column} {order?.Sort} ", Pagination.IsSortable);

            if (!Pagination.IsLastInOrder(order!))
                Add(", ", Pagination.IsSortable);
        }
    }

    private void CheckIfThePaginationContainsOrdering()
    {
        if (Pagination is { IsPageable: true, IsSortable: false })
            throw new ArgumentException($"Order property not assigned in class {GetType().Name}.");
    }
}