using SqlBuilder.Exceptions.Messages;

namespace SqlBuilder.Exceptions;

public class ExternalOrderWithInternalPaginationException : Exception
{
    public ExternalOrderWithInternalPaginationException() : base(ErrorMessage.Exception.ExternalOrderWithInternalPagination())
    {
    }
}