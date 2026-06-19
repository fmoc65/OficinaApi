/// <summary>
/// DTO para atualização de cliente.
/// Utilizamos record para imutabilidade.
/// </summary>
namespace OficinaApi.Application.Features.Clientes.DTOs
{
    public record UpdateClienteDto(
        /// <summary>
        /// Identificador do cliente a ser atualizado.
        /// Necessário para localizar o cliente no banco de dados.
        /// </summary>
        Guid Id,

        /// <summary>
        /// Nome atualizado do cliente.
        /// </summary>
        string Nome,

        /// <summary>
        /// Telefone atualizado do cliente.
        /// </summary>
        string Telefone,

        /// <summary>
        /// Endereço atualizado do cliente.
        /// </summary>
        string Endereco
    );
}
