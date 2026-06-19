using Riok.Mapperly.Abstractions;
using OficinaApi.Domain.Entities;
using OficinaApi.Application.Features.Clientes.DTOs;

/// <summary>
/// Mapper para conversão entre entidades Cliente e seus DTOs.
/// Utilizamos Maperly para gerar mapeamentos automaticamente em tempo de compilação.
/// Maperly é mais eficiente que reflection-based mappers (como AutoMapper) pois gera código fonte.
/// Isso garante performance máxima e rastreabilidade do código gerado.
/// </summary>
namespace OficinaApi.Application.Features.Clientes.Mappers
{
    [Mapper]
    public partial class ClienteMapper
    {
        /// <summary>
        /// Mapeia uma entidade Cliente para ClienteResponseDto.
        /// Maperly gera este método em tempo de compilação de forma otimizada.
        /// Essencial para retornar dados ao cliente sem expor a entidade de domínio diretamente.
        /// </summary>
        public partial ClienteResponseDto ToResponseDto(Cliente cliente);

        /// <summary>
        /// Mapeia um CreateClienteCommand para uma entidade Cliente.
        /// O Maperly casará propriedades com nomes iguais automaticamente.
        /// </summary>
        public partial Cliente ToEntity(CreateClienteCommand command);

        /// <summary>
        /// Mapeia um UpdateClienteCommand para uma entidade Cliente.
        /// Utilizado quando precisamos atualizar uma entidade existente.
        /// </summary>
        public partial Cliente ToEntity(UpdateClienteCommand command);
    }
}
