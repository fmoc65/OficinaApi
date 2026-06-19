using OficinaApi.Infrastructure.Repositories;
using OficinaApi.Application.Features.Clientes.Mappers;
using OficinaApi.Application.Features.Clientes.DTOs;

/// <summary>
/// Handler para o comando UpdateClienteCommand.
/// Processa a atualização de dados de um cliente existente.
/// Aplicamos SOLID (Single Responsibility) - responsável apenas por atualizar clientes.
/// </summary>
namespace OficinaApi.Application.Features.Clientes.Handlers
{
    public class UpdateClienteCommandHandler
    {
        /// <summary>
        /// Repository para acesso e persistência de dados.
        /// </summary>
        private readonly IClienteRepository _clienteRepository;

        /// <summary>
        /// Mapper para conversão de dados.
        /// </summary>
        private readonly ClienteMapper _mapper;

        /// <summary>
        /// Construtor com injeção de dependências.
        /// </summary>
        public UpdateClienteCommandHandler(
            IClienteRepository clienteRepository,
            ClienteMapper mapper)
        {
            _clienteRepository = clienteRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Método chamado pelo Wolverine ao processar UpdateClienteCommand.
        /// </summary>
        public async Task<ClienteResponseDto> Handle(UpdateClienteCommand command)
        {
            // Busca o cliente existente
            var cliente = await _clienteRepository.GetByIdAsync(command.Id);

            // Valida se o cliente existe
            if (cliente == null)
            {
                // Lança exceção se cliente não encontrado
                throw new InvalidOperationException($"Cliente com ID {command.Id} não encontrado");
            }

            // Atualiza as propriedades do cliente com os novos valores
            // Maperly geraria um método parcial aqui, mas fazemos manualmente para controle
            cliente.Nome = command.Nome;
            cliente.Telefone = command.Telefone;
            cliente.Endereco = command.Endereco;
            cliente.DataAtualizacao = DateTime.UtcNow; // Registra data de atualização

            // Atualiza no repository
            var clienteAtualizado = await _clienteRepository.UpdateAsync(cliente);

            // Persiste as mudanças no banco
            await _clienteRepository.SaveChangesAsync();

            // Retorna o cliente atualizado como DTO
            return _mapper.ToResponseDto(clienteAtualizado);
        }
    }
}
