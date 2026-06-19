using Riok.Mapperly.Abstractions;
using OficinaApi.Domain.Entities;
using OficinaApi.Application.Features.Pecas.Commands;
using OficinaApi.Application.Features.Pecas.DTOs;

/// <summary>
/// Mapper para Peça.
/// Maperly gera código otimizado em tempo de compilação.
/// </summary>
namespace OficinaApi.Application.Features.Pecas.Mappers
{
    [Mapper]
    public partial class PecaMapper
    {
        public partial PecaResponseDto ToResponseDto(Peca peca);
        public partial Peca ToEntity(CreatePecaCommand command);
        public partial Peca ToEntity(UpdatePecaCommand command);
    }
}
