namespace SqlBuilder.Interfaces;

public interface IBaseQuery<TResult>
{
    public bool IsCountable { get; }
}