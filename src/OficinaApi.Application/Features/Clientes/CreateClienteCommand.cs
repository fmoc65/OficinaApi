/// <summary>
/// Command para criar um novo cliente.
/// Utilizamos o padrão CQRS (Command Query Responsibility Segregation) com Wolverine.
/// Cada command representa uma intenção de modificar o estado da aplicação.
/// Records garantem imutabilidade - importante para segurança e rastreamento.
/// </summary>
namespace OficinaApi.Application.Features.Clientes.Commands
{
    public record CreateClienteCommand(
        /// <summary>
        /// Nome do cliente.
        /// Será validado pelo CreateClienteValidator antes de ser processado.
        /// </summary>
        string Nome,

        /// <summary>
        /// Telefone para contato.
        /// Será validado quanto ao formato.
        /// </summary>
        string Telefone,

        /// <summary>
        /// Endereço para localização.
        /// Campo obrigatório.
        /// </summary>
        string Endereco
    ) : ICommand;

    /// <summary>
    /// Interface de marcação para commands.
    /// Utilizamos interfaces de marcação (marker interfaces) para identificar commands
    /// e permitir que Wolverine processe-os automaticamente.
    /// </summary>
    public interface ICommand { }
}
