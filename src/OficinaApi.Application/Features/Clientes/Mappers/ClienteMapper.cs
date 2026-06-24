using Riok.Mapperly.Abstractions;
using OficinaApi.Domain.Entities;
using OficinaApi.Application.Features.Clientes.DTOs;
// CORRE??O: Adicionado o namespace onde os comandos residem
using OficinaApi.Application.Features.Clientes.Validators; 

namespace OficinaApi.Application.Features.Clientes.Mappers
{
    /// <summary>
    /// Mapper para convers?o entre entidades Cliente e seus DTOs/Commands.
    /// Utilizamos Mapperly para gerar mapeamentos automaticamente em tempo de compila??o.
    /// </summary>
    [Mapper]
    public partial class ClienteMapper
    {
        /// <summary>
        /// Mapeia uma entidade Cliente para ClienteResponseDto.
        /// </summary>
        public partial ClienteResponseDto ToResponseDto(Cliente cliente);

        /// <summary>
        /// Mapeia um CreateClienteCommand para uma entidade Cliente.
        /// </summary>
        public partial Cliente ToEntity(CreateClienteCommand command);

        /// <summary>
        /// Mapeia um UpdateClienteCommand para uma entidade Cliente.
        /// </summary>
        public partial Cliente ToEntity(UpdateClienteCommand command);
    }
}
