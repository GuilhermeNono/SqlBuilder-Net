using System.Linq.Expressions;
using SqlBuilder.Queries.Pageable.Enums;

namespace SqlBuilder.Queries.Pageable.Interfaces;

public interface IPagination<TResult> 
{
    public int? Page { get; set; }
    public int? Size { get; set; }
    public bool IsPageable {get;}
    public bool IsLastInOrder(IOrder<TResult> orderItem);
    public bool IsSortable {get;}
    public List<IOrder<TResult>> Ordering { get; }
    public void OrderBy<TProperty>(Expression<Func<TResult, TProperty>> expression, Sort sort);
    public void OrderBy(string customOrder, Sort sort);
    public void Validate();
}
