/// <summary>
/// DTOs para operações com Ordem de Serviço.
/// </summary>
namespace OficinaApi.Application.Features.OrdenServicos.DTOs
{
    public record CreateOrdenServicoDto(
        Guid IdCarro,
        Guid IdCliente,
        string Servicos,
        List<PecaOrdenDto> Pecas
    );

    public record PecaOrdenDto(
        Guid IdPeca,
        int Quantidade,
        decimal ValorUnitario
    );

    public record OrdenServicoResponseDto(
        Guid Id,
        Guid IdCarro,
        Guid IdCliente,
        string Servicos,
        decimal ValorTotal,
        DateTime DataOrdem,
        string Status,
        DateTime DataCriacao,
        DateTime? DataAtualizacao
    );

    public record UpdateOrdenServicoDto(
        Guid Id,
        Guid IdCarro,
        Guid IdCliente,
        string Servicos,
        string Status,
        List<PecaOrdenDto> Pecas
    );
}
