namespace SqlBuilder.Test.Tests.Models.Entity;

public class UserEntity
{
    public Guid Identifier { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Gender { get; set; } = "Male";
}