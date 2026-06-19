/// <summary>
/// DTO (Data Transfer Object) de resposta para Cliente.
/// Utilizamos record para garantir imutabilidade e eficiência.
/// Este DTO é retornado nas respostas de API para o cliente.
/// </summary>
namespace OficinaApi.Application.Features.Clientes.DTOs
{
    public record ClienteResponseDto(
        /// <summary>
        /// Identificador único do cliente.
        /// Retornado para referência em operações futuras.
        /// </summary>
        Guid Id,

        /// <summary>
        /// Nome do cliente.
        /// </summary>
        string Nome,

        /// <summary>
        /// Telefone do cliente.
        /// </summary>
        string Telefone,

        /// <summary>
        /// Endereço do cliente.
        /// </summary>
        string Endereco,

        /// <summary>
        /// Data de criação do cliente.
        /// Informação de auditoria.
        /// </summary>
        DateTime DataCriacao,

        /// <summary>
        /// Data da última atualização.
        /// Informação de auditoria.
        /// </summary>
        DateTime? DataAtualizacao
    );
}
