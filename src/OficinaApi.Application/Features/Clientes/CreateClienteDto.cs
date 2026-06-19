/// <summary>
/// DTO (Data Transfer Object) para criação de cliente.
/// Utilizamos record pois é imutável e perfeito para transferência de dados.
/// Records reduzem boilerplate e garantem que dados não sejam modificados inadvertidamente.
/// </summary>
namespace OficinaApi.Application.Features.Clientes.DTOs
{
    public record CreateClienteDto(
        /// <summary>
        /// Nome do cliente a ser criado.
        /// Campo obrigatório e será validado através de FluentValidation.
        /// </summary>
        string Nome,

        /// <summary>
        /// Telefone do cliente para contato.
        /// Será validado para garantir um formato válido.
        /// </summary>
        string Telefone,

        /// <summary>
        /// Endereço do cliente.
        /// Campo obrigatório para localização.
        /// </summary>
        string Endereco
    );
}
