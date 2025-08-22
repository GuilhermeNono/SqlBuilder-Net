using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Data.SqlClient;
using SqlBuilder.Annotations;
using SqlBuilder.Interfaces;
using SqlBuilder.Queries;
using SqlBuilder.Queries.Pageable.Enums;

namespace SqlBuilder;

public abstract class BaseQuery<TResult>() : QueryConfigurer<TResult>(null), IQuery<TResult>

{
    public bool IsCountable => Pagination.IsPageable;

    /// <summary>
    /// Este método deve ser utilizado quando for necessário incluir uma nova ordenação à consulta SQL.
    /// Se o objeto for paginado, este método é obrigatório para o funcionamento da consulta.
    /// <typeparam name="TResult">Classe que será utilizada para definir quais propriedades serão ordenadas</typeparam>
    /// <typeparam name="TProperty">Propriedade do objeto a ser utilizada na ordenação</typeparam>
    /// <example>Exemplo:
    /// <code>
    /// OrderBy(x => x.Id, Sort.DESC)
    /// </code>
    /// </example>
    /// <returns>Será retornada a instância de IQuery para que você possa continuar a configurar a consulta.</returns>
    /// </summary>
    public IQuery<TResult> OrderBy<TProperty>(Expression<Func<TResult, TProperty>> expression, Sort sort = Sort.Asc)
    {
        Pagination.OrderBy(expression, sort);
        return this;
    }

    /// <summary>
    /// Este método deve ser utilizado quando for necessário incluir uma nova ordenação à consulta SQL.
    /// Se o objeto for paginado, este método é obrigatório para o funcionamento da consulta.
    /// <typeparam name="TResult">Classe que será utilizada para definir quais propriedades serão ordenadas</typeparam>
    /// <example>Exemplo:
    /// <code>
    /// OrderBy("IIF(Position is null, 1, 0), Position", Sort.ASC)
    /// </code>
    /// </example>
    /// <returns>Será retornada a instância de IQuery para que você possa continuar a configurar a consulta.</returns>
    /// </summary>
    public IQuery<TResult> OrderBy(string customOrder, Sort sort = Sort.Asc)
    {
        Pagination.OrderBy(customOrder, sort);
        return this;
    }

    /// <summary>
    /// Este método deve ser utilizado quando for necessário informar o
    /// tamanho de registros por página e a página em que os dados serão procurados.
    /// <example>Exemplo:
    /// <code>
    /// PageConfig(10, 1);
    /// </code>
    /// </example>
    /// <returns>Será retornada a instância de IQuery para que você possa continuar a configurar a consulta.</returns>
    /// </summary>
    public IQuery<TResult> PageConfig(int? pageSize, int? pageNumber)
    {
        Pagination.Size = pageSize;
        Pagination.Page = pageNumber;
        return this;
    }
}

public abstract class BaseQuery<TResult, TFilter>(TFilter filter)
    : QueryConfigurer<TResult>(filter), IQuery<TResult, TFilter> where TFilter : IFilterParam
{
    private TFilter Filter { get; } = filter;
    public bool IsCountable => Pagination.IsPageable;

    /// <summary>
    /// Este método deve ser utilizado quando for necessário incluir uma nova ordenação à consulta SQL.
    /// Se o objeto for paginado, este método é obrigatório para o funcionamento da consulta.
    /// <typeparam name="TResult">Classe que será utilizada para definir quais propriedades serão ordenadas</typeparam>
    /// <typeparam name="TProperty">Propriedade do objeto a ser utilizada na ordenação</typeparam>
    /// <example>Exemplo:
    /// <code>
    /// OrderBy(x => x.Id, Sort.DESC)
    /// </code>
    /// </example>
    /// <returns>Será retornada a instância de IQuery para que você possa continuar a configurar a consulta.</returns>
    /// </summary>
    public IQuery<TResult, TFilter> OrderBy<TProperty>(Expression<Func<TResult, TProperty>> expression,
        Sort sort = Sort.Asc)
    {
        Pagination.OrderBy(expression, sort);
        return this;
    }

    public IQuery<TResult, TFilter> OrderBy(string customOrder, Sort sort = Sort.Asc)
    {
        Pagination.OrderBy(customOrder, sort);
        return this;
    }

    /// <summary>
    /// Este método deve ser utilizado quando for necessário informar o
    /// tamanho de registros por página e a página em que os dados serão procurados.
    /// <example>Exemplo:
    /// <code>
    /// PageConfig(10, 1);
    /// </code>
    /// </example>
    /// <returns>Será retornada a instância de IQuery para que você possa continuar a configurar a consulta.</returns>
    /// </summary>
    public IQuery<TResult, TFilter> PageConfig(int? pageSize, int? pageNumber = 0)
    {
        Pagination.Size = pageSize;
        Pagination.Page = pageNumber;
        return this;
    }

    public object[]? Parameters() =>  FilterPropertiesToSqlParameters()?.ToArray();

    private IEnumerable<object>? FilterPropertiesToSqlParameters()
    {
        var parameters = new List<SqlParameter>();

        foreach (var property in Filter.GetType().GetProperties())
        {
            var value = property.GetValue(Filter);

            var isIgnorable = Attribute.IsDefined(property, typeof(IgnoreFilterPropertyAttribute));

            if (isIgnorable)
                continue;

            if (value == null)
                parameters.Add(new SqlParameter(property.Name, DBNull.Value));

            InsertByPropertyTypeIntoParameter(property, value, ref parameters);

            if (parameters.Any(x => x.ParameterName.Equals(property.Name)))
                continue;
            
            parameters.Add(new SqlParameter(property.Name, value));
        }
        
        return parameters.Count != 0 ? parameters.AsEnumerable() : null;
    }

    private static void InsertByPropertyTypeIntoParameter(PropertyInfo property, object? value, ref List<SqlParameter> parameters)
    {
        //TODO: Refatorar
        // if (value is IDictionary dictionary)
        // {
        //     List<SqlParameter>? dicValues = null;
        //
        //     foreach (DictionaryEntry entry in dictionary)
        //     {
        //         if (entry.Value is IList list)
        //         {
        //             foreach (var item in list)
        //             {
        //                 dicValues ??= new List<SqlParameter>();
        //                 dicValues.Add(new SqlParameter(entry.Key.ToString(), item));
        //             }
        //         }
        //     }
        //
        //     dicValues ??=
        //         ((Dictionary<object, object>)dictionary).Select(x => new SqlParameter(x.Key.ToString(), x.Value))
        //         .ToList();
        //
        //     parameters.AddRange(dicValues);
        //     return;
        // }
        
        if (value is not string && value is IEnumerable enumerable)
        {
            var values = enumerable.Cast<object?>().ToArray();

            for (int i = 0; i < values.Length; i++)
                parameters.Add(new SqlParameter(property.Name + i, values[i] ?? DBNull.Value));
        }
    }
}