/// <summary>
/// DTO para criação de Carro.
/// Utilizamos record para imutabilidade e eficiência em transferência de dados.
/// </summary>
namespace OficinaApi.Application.Features.Carros.DTOs
{
    public record CreateCarroDto(
        /// <summary>
        /// Modelo/marca do carro a ser criado.
        /// Campo obrigatório e validado.
        /// </summary>
        string Modelo,

        /// <summary>
        /// Ano de fabricação do carro.
        /// Importante para diagnóstico e compatibilidade.
        /// </summary>
        int Ano,

        /// <summary>
        /// Id do cliente proprietário.
        /// Chave estrangeira que vincula o carro a um cliente.
        /// </summary>
        Guid IdCliente
    );
}
