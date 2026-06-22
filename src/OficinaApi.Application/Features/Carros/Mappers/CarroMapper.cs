using OficinaApi.Application.Features.Carros.Commands;
using OficinaApi.Application.Features.Carros.DTOs;
using OficinaApi.Domain.Entities;
using Riok.Mapperly.Abstractions;

/// <summary>
/// Mapper para Carro.
/// Maperly gera mapeamentos otimizados em tempo de compilação.
/// Garante performance máxima e rastreabilidade do código.
/// </summary>
namespace OficinaApi.Application.Features.Carros.Mappers
{
    [Mapper]
    public partial class CarroMapper
    {
        /// <summary>
        /// Mapeia entidade para DTO de resposta.
        /// </summary>
        public partial CarroResponseDto ToResponseDto(Carro carro);

        /// <summary>
        /// Mapeia command para entidade.
        /// </summary>
        public partial Carro ToEntity(CreateCarroCommand command);

        /// <summary>
        /// Mapeia update command para entidade.
        /// </summary>
        public partial Carro ToEntity(UpdateCarroCommand command);
    }
}
