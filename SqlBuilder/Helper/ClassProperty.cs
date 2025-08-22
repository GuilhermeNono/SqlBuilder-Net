using System.Linq.Expressions;

namespace SqlBuilder.Helper;

internal static class ClassProperty
{
    private static Expression RemoveUnary(Expression toUnwrap)
    {
        return toUnwrap is UnaryExpression expression ? expression.Operand : toUnwrap;
    }

    public static string PropertyName<T, TProperty>(Expression<Func<T, TProperty>> expression)
    {
        if (RemoveUnary(expression.Body) is not MemberExpression memberExp)
            return string.Empty;

        var currentExpr = memberExp.Expression;
        while (true)
        {
            currentExpr = RemoveUnary(currentExpr!);

            if (currentExpr.NodeType == ExpressionType.MemberAccess)
                currentExpr = ((MemberExpression)currentExpr).Expression;
            else
                break;
        }

        return currentExpr.NodeType != ExpressionType.Parameter ? string.Empty : memberExp.Member.Name;
    }
}
