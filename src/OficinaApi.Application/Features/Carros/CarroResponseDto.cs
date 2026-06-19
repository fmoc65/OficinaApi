/// <summary>
/// DTO de resposta para Carro.
/// Retornado nas respostas de API com dados públicos do carro.
/// </summary>
namespace OficinaApi.Application.Features.Carros.DTOs
{
    public record CarroResponseDto(
        /// <summary>
        /// Identificador único do carro.
        /// </summary>
        Guid Id,

        /// <summary>
        /// Modelo/marca do carro.
        /// </summary>
        string Modelo,

        /// <summary>
        /// Ano de fabricação.
        /// </summary>
        int Ano,

        /// <summary>
        /// Id do cliente proprietário.
        /// </summary>
        Guid IdCliente,

        /// <summary>
        /// Data de criação.
        /// Informação de auditoria.
        /// </summary>
        DateTime DataCriacao,

        /// <summary>
        /// Data de última atualização.
        /// </summary>
        DateTime? DataAtualizacao
    );
}
