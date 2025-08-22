namespace SqlBuilder.Interfaces;

public interface ISqlBuilder<TResult> : IQuery<TResult> 
{
}

public interface ISqlBuilder<TFilter, TResult> :  IQuery<TResult, TFilter> 
{
}