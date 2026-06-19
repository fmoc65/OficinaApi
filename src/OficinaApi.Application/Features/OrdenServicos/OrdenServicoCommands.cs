/// <summary>
/// Commands para operações com Ordem de Serviço.
/// </summary>
namespace OficinaApi.Application.Features.OrdenServicos.Commands
{
    public record CreateOrdenServicoCommand(
        Guid IdCarro,
        Guid IdCliente,
        string Servicos,
        List<PecaOrdenCommand> Pecas
    ) : ICommand;

    public record PecaOrdenCommand(
        Guid IdPeca,
        int Quantidade,
        decimal ValorUnitario
    );

    public record UpdateOrdenServicoCommand(
        Guid Id,
        Guid IdCarro,
        Guid IdCliente,
        string Servicos,
        string Status,
        List<PecaOrdenCommand> Pecas
    ) : ICommand;

    public record DeleteOrdenServicoCommand(
        Guid Id
    ) : ICommand;

    public interface ICommand { }
}
