using System.Diagnostics.CodeAnalysis;
using SqlBuilder.Test.Tests.Models.Entity.CustomQueries;
using SqlBuilder.Test.Tests.Models.Entity.CustomQueries.Filters;
using Xunit;

namespace SqlBuilder.Test.Tests;

[ExcludeFromCodeCoverage]
public class SqlBuilderTest
{
    [Fact]
    public void QueryBuilder()
    {
        const string queryExpected = """
                                        Select * 
                                        From ( 
                                     SELECT Id As Identifier,
                                            Name,
                                            Gender As Gender
                                       FROM User
                                      Where Id = @Id
                                             ) t 
                                     """;
        
        const string countQueryExpected = """
                                      Select Count(1) as Value
                                        From ( 
                                     SELECT Id As Identifier,
                                            Name,
                                            Gender As Gender
                                       FROM User
                                      Where Id = @Id
                                             ) t 
                                     """;
        
        var id = Guid.NewGuid();
        var sql = new FindByUserQuery(new FindByUserFilter(id));
        
        Assert.NotNull(sql);
        Assert.Equal(queryExpected.Trim(), sql.Query.Trim());
        Assert.Equal(countQueryExpected.Trim(), sql.Count.Trim());
        Assert.NotNull(sql.Parameters());
        Assert.False(sql.IsCountable);
        Assert.Single(sql.Parameters()!);
    } 
}