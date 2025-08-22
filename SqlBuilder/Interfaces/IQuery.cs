﻿using System.Linq.Expressions;
using SqlBuilder.Queries.Pageable.Enums;

namespace SqlBuilder.Interfaces;

public interface IQuery<TResult> : IBaseQuery<TResult>, IQueryConfigurer<TResult> 
{
    public IQuery<TResult> OrderBy<TProperty>(Expression<Func<TResult, TProperty>> expression,
        Sort sort = Sort.Asc);

    public IQuery<TResult> OrderBy(string customOrder, Sort sort = Sort.Asc);
    public IQuery<TResult> PageConfig(int? pageSize, int? pageNumber);
}

public interface IQuery<TResult, TFilter> : IBaseQuery<TResult>, IQueryConfigurer<TResult>
{
    public IQuery<TResult, TFilter> OrderBy<TProperty>(Expression<Func<TResult, TProperty>> expression,
        Sort sort = Sort.Asc);
    public IQuery<TResult, TFilter> OrderBy(string customOrder, Sort sort = Sort.Asc);
    public IQuery<TResult, TFilter> PageConfig(int? pageSize, int? pageNumber);
    public object[]? Parameters();
}
