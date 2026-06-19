/// <summary>
/// Command para deletar um cliente.
/// Utilizamos CQRS para manter a separação entre leitura e escrita.
/// </summary>
namespace OficinaApi.Application.Features.Clientes.Commands
{
    public record DeleteClienteCommand(
        /// <summary>
        /// Identificador do cliente a ser deletado.
        /// </summary>
        Guid Id
    ) : ICommand;
}
