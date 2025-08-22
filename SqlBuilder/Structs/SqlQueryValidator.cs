using System.Collections;
using System.Reflection;
using System.Text;
using SqlBuilder.Exceptions;
using SqlBuilder.Queries;

namespace SqlBuilder.Structs;

public readonly struct SqlQueryValidator
{
    private ReadOnlyMemory<char> QueryMemory => _query.ToString().AsMemory();
    private readonly StringBuilder _query;
    private ReadOnlySpan<char> QuerySpan => QueryMemory.Span;

    private SqlQueryValidator(ref StringBuilder query)
    {
        _query = query;
    }

    public SqlQueryValidator Validate()
    {
        if (IsUsingQueryOrderingInsteadOfInternalOrderBy())
            throw new ExternalOrderWithInternalPaginationException();

        return this;
    }

    #region | Validations |

    private bool IsUsingQueryOrderingInsteadOfInternalOrderBy() =>
        QuerySpan.IndexOf("order by", StringComparison.OrdinalIgnoreCase) >= 0;

    #endregion

    public void BuildVirtualVariables(IFilterParam? filter)
    {
        if(filter is null)
            return;
        
        UpdateCollectionVariableToScalarParams(filter);
    }
    
    public static SqlQueryValidator WithQuery(StringBuilder query) => new(ref query);
    
    public static implicit operator string(SqlQueryValidator validator) => validator.QueryMemory.ToString();

    private void UpdateCollectionVariableToScalarParams(IFilterParam filterParam)
    {
        var properties = filterParam.GetType().GetProperties();
        
        foreach (var property in properties)
        {
            if (!IsACollection(property.GetValue(filterParam), out var collection))
                continue;

            var scalarName = ToParam(property);

            var scalarVariables = new List<string>();
            
            for (int i = 0; i < collection.Length; i++)
                scalarVariables.Add($"{scalarName + i}");

            var indexParam = SbIndexOf(_query, scalarName);
            
            if(indexParam < 0)
                continue;

            _query.Remove(indexParam, scalarName.Length);
            _query.Insert(indexParam, string.Join(',', scalarVariables));
        }
    }
    
    private static int SbIndexOf(StringBuilder sb, string value)
    {
        for (int i = 0; i <= sb.Length - value.Length; i++)
        {
            int j;
            for (j = 0; j < value.Length; j++)
            {
                if (sb[i + j] != value[j])
                    break;
            }
            if (j == value.Length)
                return i;
        }
        return -1;
    }
    
    private static string ToParam(PropertyInfo param) => $"@{param.Name}";
    private static bool IsACollection(object? value, out object?[] collectionProp)
    {
        if (value is not string && value is IEnumerable prop)
        {
            collectionProp = prop.Cast<object?>().ToArray();
            
            return true;
        }

        collectionProp = [];
        return false;
    }
}
