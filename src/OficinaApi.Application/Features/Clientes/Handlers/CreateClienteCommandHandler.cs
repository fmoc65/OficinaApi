using OficinaApi.Infrastructure.Repositories;
using OficinaApi.Application.Features.Clientes.Mappers;
using OficinaApi.Application.Features.Clientes.DTOs;

/// <summary>
/// Handler para o comando CreateClienteCommand.
/// Utilizamos o padrão CQRS com Wolverine para separar operações de escrita (Commands) de leitura (Queries).
/// Este handler é invocado automaticamente por Wolverine quando um CreateClienteCommand é despachado.
/// Aplicamos SOLID (Single Responsibility) - responsável apenas por criar clientes.
/// </summary>
namespace OficinaApi.Application.Features.Clientes.Handlers
{
    public class CreateClienteCommandHandler
    {
        /// <summary>
        /// Repository injetado para acesso aos dados de Cliente.
        /// Aplicamos injeção de dependência para permitir testes e flexibilidade.
        /// </summary>
        private readonly IClienteRepository _clienteRepository;

        /// <summary>
        /// Mapper injetado para conversão entre Command e Entidade.
        /// Maperly gera o código otimizado de mapeamento em tempo de compilação.
        /// </summary>
        private readonly ClienteMapper _mapper;

        /// <summary>
        /// Construtor com injeção de dependências.
        /// </summary>
        public CreateClienteCommandHandler(
            IClienteRepository clienteRepository,
            ClienteMapper mapper)
        {
            _clienteRepository = clienteRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Método chamado pelo Wolverine ao processar CreateClienteCommand.
        /// Nome "Handle" é convenção do Wolverine.
        /// </summary>
        public async Task<ClienteResponseDto> Handle(CreateClienteCommand command)
        {
            // Mapeia o command para uma entidade Cliente
            // Maperly gera este mapeamento de forma otimizada em tempo de compilação
            var cliente = _mapper.ToEntity(command);

            // Adiciona o cliente ao repository
            // Não persiste ainda, apenas adiciona ao context
            var clienteAdicionado = await _clienteRepository.AddAsync(cliente);

            // Salva as mudanças no banco de dados
            // SaveChangesAsync executa o INSERT na tabela Clientes
            await _clienteRepository.SaveChangesAsync();

            // Mapeia a entidade para DTO de resposta
            // Retorna apenas os dados públicos, sem exposição desnecessária
            return _mapper.ToResponseDto(clienteAdicionado);
        }
    }
}
