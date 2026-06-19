/// <summary>
/// Commands para operações com Peça.
/// Padrão CQRS com imutabilidade garantida por records.
/// </summary>
namespace OficinaApi.Application.Features.Pecas.Commands
{
    public record CreatePecaCommand(
        string IdPeca,
        Guid IdCarro,
        int Quantidade,
        decimal Valor
    ) : ICommand;

    public record UpdatePecaCommand(
        Guid Id,
        string IdPeca,
        Guid IdCarro,
        int Quantidade,
        decimal Valor
    ) : ICommand;

    public record DeletePecaCommand(
        Guid Id
    ) : ICommand;

    public interface ICommand { }
}
