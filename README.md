# SqlBuilder-Net 🚀

SqlBuilder-Net is a .NET library designed to simplify and control the construction of dynamic SQL commands, using scalar variables in a safe way. It helps developers build SQL queries more elegantly, avoiding manual string concatenation and SQL Injection risks.

## ✨ Main Advantages

- **Security**: uses scalar parameters to prevent SQL Injection.
- **Ease of Use**: build complex SQLs fluently and intuitively.
- **Organization**: your code becomes cleaner and easier to maintain.
- **Flexibility**: supports various scenarios and databases.

## 🛠️ How to use

Below is a practical example of using the library. The user must:

1. Create a filter that implements `IFilterParam`, where scalar variables will be used.
2. Implement a class that inherits from `SqlBuilder<TEntity, TFilter>`.
3. Instantiate the class and access the generated query.

```csharp
// Filter implementing IFilterParam
public record FindByUserFilter(Guid Id) : IFilterParam
{
}

// Custom class extending SqlBuilder
public class FindByUserQuery(FindByUserFilter filter) : SqlBuilder<UserEntity, FindByUserFilter>(filter)
{
    protected override void Prepare()
    {
        Add($"""
            SELECT Id {Alias(x => x.Identifier)},
                   Name,
                   Gender {Alias(x => x.Gender)}
              FROM User
             WHERE Id = {Param(x => x.Id)}
            """);
    }
}

public class Main()
{
  // Usage
  var sql = new FindByUserQuery(new FindByUserFilter(id));

  string query = sql.Query;
  // query: SELECT * FROM (SELECT Id AS Identifier, Name, Gender AS Gender FROM User WHERE Id = @Id) t
}
```

## 📚 Common Use Cases

- **Build dynamic filters**: add conditions according to business rules.
- **Avoid string concatenation**: use fluent methods to compose queries.

## 👨‍💻 Tests

More complete examples can be found in `SqlBuilder.Tests.SqlBuilderTest.cs`. It's recommended to check the tests to understand different usage scenarios.

## 🤝 Contribute

Suggestions, PRs and issues are welcome! Feel free to collaborate.

---

Made with ❤️ by [@GuilhermeNono](https://github.com/GuilhermeNono)
