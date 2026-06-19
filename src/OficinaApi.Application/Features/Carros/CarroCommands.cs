/// <summary>
/// Commands para operações com Carro.
/// Utilizamos CQRS com Wolverine para separar leitura e escrita.
/// Records garantem imutabilidade essencial para segurança.
/// </summary>
namespace OficinaApi.Application.Features.Carros.Commands
{
    public record CreateCarroCommand(
        string Modelo,
        int Ano,
        Guid IdCliente
    ) : ICommand;

    public record UpdateCarroCommand(
        Guid Id,
        string Modelo,
        int Ano,
        Guid IdCliente
    ) : ICommand;

    public record DeleteCarroCommand(
        Guid Id
    ) : ICommand;

    /// <summary>
    /// Interface de marcação para commands de Carros.
    /// </summary>
    public interface ICommand { }
}
