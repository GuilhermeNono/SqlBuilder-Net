
namespace SqlBuilder.Exceptions.Messages;

public static class ErrorMessage
{
    /// <summary>
    /// Classe Responsável pelas Mensagens de Exceção do Sistema
    /// </summary>
    public static class Exception
    {
        public static string ExternalOrderWithInternalPagination() =>
            "Não é possível declarar a ordenação nos métodos de adição. Faça isso utilizando o Método OrderBy() presente na declaração da Query.";
    }
}