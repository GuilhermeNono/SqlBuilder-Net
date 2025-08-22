using System.Linq.Expressions;
using SqlBuilder.Helper;
using SqlBuilder.Interfaces;
using SqlBuilder.Queries;

namespace SqlBuilder;

/// <summary>
///Esta classe deve ser utilizada quando você estiver trabalhando com consultas personalizadas sem filtragem.
/// Ou seja, ela é útil para operações que usam consultas SQL para obter resultados sem aplicar condições específicas.
/// <typeparam name="TResult">Classe que será retornada após a finalização da consulta</typeparam>
/// </summary>
public abstract class SqlBuilder<TResult> : BaseQuery<TResult>, ISqlBuilder<TResult> where TResult : notnull
{
}

/// <summary>
/// Esta classe deve ser utilizada quando você estiver a trabalhar com consultas personalizadas com filtragem.
/// Ou seja, ela é útil para operações que usam consultas SQL para obter resultados específicos aplicando condições predefinidas.
/// <typeparam name="TResult">Classe que será retornada após a finalização da consulta</typeparam>
/// <typeparam name="TFilter">Classe para adicionar parâmetros condicionais à consulta</typeparam>
/// </summary>
public abstract class SqlBuilder<TResult, TFilter>(TFilter filter)
    : BaseQuery<TResult, TFilter>(filter), ISqlBuilder<TFilter, TResult> where TFilter : IFilterParam

{
    
    /// <summary>
    /// Utilize este método para incluir parâmetros em sua consulta SQL.
    /// <typeparam name="TFilter">Classe que será utilizada para obter as propriedades</typeparam>
    /// <typeparam name="TProperty">Propriedade que será utilizada na parametrização</typeparam>
    /// <example>Exemplo:
    /// <code>
    /// $"WHERE foo = {Param(x => x.bar)}"
    /// </code>
    /// Se transforma em:
    /// <code>
    /// Where foo = @bar
    /// </code>
    /// </example>
    /// <returns>Será retornado o nome da propriedade parametrizada com o valor <c>"@foo".</c></returns>
    /// </summary>
    protected string Param<TProperty>(Expression<Func<TFilter, TProperty>> expression)
    {
        return $"@{ClassProperty.PropertyName(expression)}";
    }
    protected string Param(string property)
    {
        return $"@{property}";
    }
}