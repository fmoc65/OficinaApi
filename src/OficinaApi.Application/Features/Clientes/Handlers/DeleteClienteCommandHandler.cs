using OficinaApi.Infrastructure.Repositories;

/// <summary>
/// Handler para o comando DeleteClienteCommand.
/// Processa a deleção de um cliente do sistema.
/// Aplicamos SOLID (Single Responsibility) - responsável apenas por deletar clientes.
/// </summary>
namespace OficinaApi.Application.Features.Clientes.Handlers
{
    public class DeleteClienteCommandHandler
    {
        /// <summary>
        /// Repository para acesso aos dados.
        /// </summary>
        private readonly IClienteRepository _clienteRepository;

        /// <summary>
        /// Logger para registrar operações de deleção.
        /// Útil para auditoria de quando clientes foram deletados.
        /// </summary>
        private readonly ILogger<DeleteClienteCommandHandler> _logger;

        /// <summary>
        /// Construtor com injeção de dependências.
        /// </summary>
        public DeleteClienteCommandHandler(
            IClienteRepository clienteRepository,
            ILogger<DeleteClienteCommandHandler> logger)
        {
            _clienteRepository = clienteRepository;
            _logger = logger;
        }

        /// <summary>
        /// Método chamado pelo Wolverine ao processar DeleteClienteCommand.
        /// </summary>
        public async Task Handle(DeleteClienteCommand command)
        {
            // Tenta deletar o cliente
            var foiDeletado = await _clienteRepository.DeleteAsync(command.Id);

            // Valida se o cliente foi deletado
            if (!foiDeletado)
            {
                // Log de aviso se cliente não encontrado
                _logger.LogWarning($"Cliente com ID {command.Id} não encontrado para deleção");
                // Lança exceção
                throw new InvalidOperationException($"Cliente com ID {command.Id} não encontrado");
            }

            // Persiste a deleção no banco de dados
            await _clienteRepository.SaveChangesAsync();

            // Log da operação realizada
            _logger.LogInformation($"Cliente com ID {command.Id} foi deletado com sucesso");
        }
    }
}
