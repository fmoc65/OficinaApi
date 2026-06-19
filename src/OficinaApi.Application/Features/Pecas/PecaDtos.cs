/// <summary>
/// DTOs para operações com Peça.
/// Records garantem imutabilidade e eficiência.
/// </summary>
namespace OficinaApi.Application.Features.Pecas.DTOs
{
    public record CreatePecaDto(
        string IdPeca,
        Guid IdCarro,
        int Quantidade,
        decimal Valor
    );

    public record PecaResponseDto(
        Guid Id,
        string IdPeca,
        Guid IdCarro,
        int Quantidade,
        decimal Valor,
        DateTime DataCriacao,
        DateTime? DataAtualizacao
    );

    public record UpdatePecaDto(
        Guid Id,
        string IdPeca,
        Guid IdCarro,
        int Quantidade,
        decimal Valor
    );
}
