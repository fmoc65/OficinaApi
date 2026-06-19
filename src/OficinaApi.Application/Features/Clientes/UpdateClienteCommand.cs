/// <summary>
/// Command para atualizar um cliente existente.
/// Utilizamos CQRS com Wolverine para separar comandos de queries.
/// </summary>
namespace OficinaApi.Application.Features.Clientes.Commands
{
    public record UpdateClienteCommand(
        /// <summary>
        /// Identificador do cliente a ser atualizado.
        /// Necessário para localizar a entidade correta.
        /// </summary>
        Guid Id,

        /// <summary>
        /// Nome atualizado.
        /// </summary>
        string Nome,

        /// <summary>
        /// Telefone atualizado.
        /// </summary>
        string Telefone,

        /// <summary>
        /// Endereço atualizado.
        /// </summary>
        string Endereco
    ) : ICommand;
}
