# SqlBuilder-Net 🚀

SqlBuilder-Net é uma biblioteca .NET criada para facilitar e controlar a construção de comandos SQL dinâmicos com uso seguro de variáveis escalares. Ela ajuda desenvolvedores a montar queries SQL de forma mais legível, evitando concatenação manual de strings e riscos de SQL Injection.

## ✨ Principais vantagens

- **Segurança**: uso de parâmetros escalares evita SQL Injection.
- **Facilidade**: construa SQLs complexos de forma fluida e intuitiva.
- **Organização**: seu código fica mais limpo e fácil de manter.
- **Flexibilidade**: suporta diferentes cenários e bancos de dados.

## 🚦 Instalação

Adicione o pacote via NuGet:

```bash
dotnet add package SqlBuilder-Net
```

## 🛠️ Como usar

Abaixo, um exemplo prático do uso da biblioteca. O usuário deve:

1. Criar um filtro que implemente `IFilterParam`, onde as variáveis escalares serão consultadas.
2. Implementar uma classe que herda de `SqlBuilder<TR, TFilter>`.
3. Instanciar a classe e acessar a query montada.

```csharp
// Filtro implementando IFilterParam
public record FindByUserFilter(Guid Id) : IFilterParam
{
}

// Classe customizada estendendo SqlBuilder
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
  // Utilização
  var sql = new FindByUserQuery(new FindByUserFilter(id));

  string query = sql.Query;
  // query: SELECT * FROM (SELECT Id AS Identifier, Name, Gender AS Gender FROM User WHERE Id = @Id) t
}
```

## 📚 Exemplos de uso comum

- **Montar filtros dinâmicos**: adicione condições conforme regras do negócio.
- **Evitar concatenação de strings**: use métodos fluentes para compor queries.

## 👨‍💻 Testes

Exemplos mais completos podem ser encontrados em `SqlBuilder.Tests.SqlBuilderTest.cs`. Recomenda-se acompanhar os testes para entender diferentes cenários de uso.

## 🤝 Contribua

Sugestões, PRs e issues são bem-vindos! Sinta-se à vontade para colaborar.

---

Feito com ❤️ por [@GuilhermeNono](https://github.com/GuilhermeNono)
