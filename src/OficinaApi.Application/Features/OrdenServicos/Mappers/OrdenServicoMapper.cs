using Riok.Mapperly.Abstractions;
using OficinaApi.Domain.Entities;
using OficinaApi.Application.Features.OrdenServicos.Commands;
using OficinaApi.Application.Features.OrdenServicos.DTOs;

/// <summary>
/// Mapper para Ordem de Serviço.
/// Mapeia comandos e entidades para DTOs.
/// </summary>
namespace OficinaApi.Application.Features.OrdenServicos.Mappers
{
    [Mapper]
    public partial class OrdenServicoMapper
    {
        /// <summary>
        /// Mapeia entidade para DTO de resposta.
        /// </summary>
        public partial OrdenServicoResponseDto ToResponseDto(OrdenServico ordemServico);

        /// <summary>
        /// Mapeia command para entidade.
        /// Nota: Como OrdenServico tem relacionamentos complexos, 
        /// este mapeamento pode precisar de customização manual.
        /// </summary>
        public partial OrdenServico ToEntity(CreateOrdenServicoCommand command);
    }
}
