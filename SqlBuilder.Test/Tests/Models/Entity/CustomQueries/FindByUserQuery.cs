using SqlBuilder.Test.Tests.Models.Entity.CustomQueries.Filters;

namespace SqlBuilder.Test.Tests.Models.Entity.CustomQueries;

public class FindByUserQuery(FindByUserFilter filter) : SqlBuilder<UserEntity, FindByUserFilter>(filter)
{
    protected override void Prepare()
    {
        Add($"""
             SELECT Id {Alias(x => x.Identifier)},
                    Name,
                    Gender {Alias(x => x.Gender)}
               FROM User
              Where Id = {Param(x => x.Id)}
             """);
    }
}