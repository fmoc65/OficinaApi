/// <summary>
/// DTO para atualização de Carro.
/// </summary>
namespace OficinaApi.Application.Features.Carros.DTOs
{
    public record UpdateCarroDto(
        /// <summary>
        /// Id do carro a ser atualizado.
        /// </summary>
        Guid Id,

        /// <summary>
        /// Modelo atualizado.
        /// </summary>
        string Modelo,

        /// <summary>
        /// Ano atualizado.
        /// </summary>
        int Ano,

        /// <summary>
        /// Id do cliente (pode mudar o proprietário).
        /// </summary>
        Guid IdCliente
    );
}
