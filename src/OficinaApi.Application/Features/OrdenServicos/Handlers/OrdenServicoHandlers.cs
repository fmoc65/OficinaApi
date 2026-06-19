using OficinaApi.Domain.Entities;
using OficinaApi.Infrastructure.Data;
using OficinaApi.Infrastructure.Repositories;
using OficinaApi.Application.Features.OrdenServicos.Commands;
using OficinaApi.Application.Features.OrdenServicos.Mappers;
using OficinaApi.Application.Features.OrdenServicos.DTOs;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Handlers para commands de Ordem de Serviço.
/// Mais complexos pois envolvem transações e relacionamentos múltiplos.
/// </summary>
namespace OficinaApi.Application.Features.OrdenServicos.Handlers
{
    /// <summary>
    /// Handler para criar nova ordem de serviço.
    /// Envolve criar a ordem e suas peças associadas.
    /// </summary>
    public class CreateOrdenServicoCommandHandler
    {
        private readonly IRepository<OrdenServico, Guid> _ordemRepository;
        private readonly IRepository<Peca, Guid> _pecaRepository;
        private readonly OficinaDbContext _context;
        private readonly OrdenServicoMapper _mapper;
        private readonly ILogger<CreateOrdenServicoCommandHandler> _logger;

        public CreateOrdenServicoCommandHandler(
            IRepository<OrdenServico, Guid> ordemRepository,
            IRepository<Peca, Guid> pecaRepository,
            OficinaDbContext context,
            OrdenServicoMapper mapper,
            ILogger<CreateOrdenServicoCommandHandler> logger)
        {
            _ordemRepository = ordemRepository;
            _pecaRepository = pecaRepository;
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handler que processa a criação de uma ordem de serviço.
        /// Utiliza transação para garantir consistência.
        /// </summary>
        public async Task<OrdenServicoResponseDto> Handle(CreateOrdenServicoCommand command)
        {
            // Inicia uma transação
            // Importante quando temos múltiplas operações que devem suceder juntas
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Cria a ordem de serviço
                    var ordem = new OrdenServico
                    {
                        Id = Guid.NewGuid(),
                        IdCarro = command.IdCarro,
                        IdCliente = command.IdCliente,
                        Servicos = command.Servicos,
                        Status = "Aberta",
                        DataOrdem = DateTime.UtcNow,
                        DataCriacao = DateTime.UtcNow,
                        PecasUtilizadas = new List<OrdenServicoPeca>()
                    };

                    // Calcula valor total e adiciona peças
                    decimal valorTotal = 0;

                    foreach (var pecaComando in command.Pecas)
                    {
                        // Busca a peça para validar existência
                        var peca = await _pecaRepository.GetByIdAsync(pecaComando.IdPeca);

                        if (peca == null)
                        {
                            throw new InvalidOperationException($"Peça com ID {pecaComando.IdPeca} não encontrada");
                        }

                        // Cria associação entre ordem e peça
                        var ordemPeca = new OrdenServicoPeca
                        {
                            Id = Guid.NewGuid(),
                            IdOrdenServico = ordem.Id,
                            IdPeca = pecaComando.IdPeca,
                            Quantidade = pecaComando.Quantidade,
                            ValorUnitario = pecaComando.ValorUnitario,
                            DataCriacao = DateTime.UtcNow
                        };

                        // Adiciona à ordem
                        ordem.PecasUtilizadas.Add(ordemPeca);

                        // Calcula valor total (quantidade × valor unitário)
                        valorTotal += pecaComando.Quantidade * pecaComando.ValorUnitario;
                    }

                    // Define valor total calculado
                    ordem.ValorTotal = valorTotal;

                    // Adiciona a ordem ao repository
                    var ordemAdicionada = await _ordemRepository.AddAsync(ordem);

                    // Salva mudanças
                    await _ordemRepository.SaveChangesAsync();

                    // Confirma a transação
                    await transaction.CommitAsync();

                    _logger.LogInformation("Ordem de serviço {OrdenId} criada com sucesso", ordem.Id);

                    // Retorna como DTO
                    return _mapper.ToResponseDto(ordemAdicionada);
                }
                catch (Exception ex)
                {
                    // Desfaz a transação em caso de erro
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Erro ao criar ordem de serviço");
                    throw;
                }
            }
        }
    }

    /// <summary>
    /// Handler para atualizar uma ordem de serviço existente.
    /// </summary>
    public class UpdateOrdenServicoCommandHandler
    {
        private readonly IRepository<OrdenServico, Guid> _ordemRepository;
        private readonly IRepository<Peca, Guid> _pecaRepository;
        private readonly OficinaDbContext _context;
        private readonly OrdenServicoMapper _mapper;
        private readonly ILogger<UpdateOrdenServicoCommandHandler> _logger;

        public UpdateOrdenServicoCommandHandler(
            IRepository<OrdenServico, Guid> ordemRepository,
            IRepository<Peca, Guid> pecaRepository,
            OficinaDbContext context,
            OrdenServicoMapper mapper,
            ILogger<UpdateOrdenServicoCommandHandler> logger)
        {
            _ordemRepository = ordemRepository;
            _pecaRepository = pecaRepository;
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<OrdenServicoResponseDto> Handle(UpdateOrdenServicoCommand command)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Busca a ordem existente com suas peças
                    var ordem = await _context.OrdensServico
                        .Include(o => o.PecasUtilizadas)
                        .FirstOrDefaultAsync(o => o.Id == command.Id);

                    if (ordem == null)
                        throw new InvalidOperationException($"Ordem de serviço com ID {command.Id} não encontrada");

                    // Atualiza campos básicos
                    ordem.IdCarro = command.IdCarro;
                    ordem.IdCliente = command.IdCliente;
                    ordem.Servicos = command.Servicos;
                    ordem.Status = command.Status;
                    ordem.DataAtualizacao = DateTime.UtcNow;

                    // Remove peças antigas
                    _context.OrdensServicoPecas.RemoveRange(ordem.PecasUtilizadas);

                    // Adiciona novas peças
                    decimal valorTotal = 0;
                    ordem.PecasUtilizadas = new List<OrdenServicoPeca>();

                    foreach (var pecaComando in command.Pecas)
                    {
                        var peca = await _pecaRepository.GetByIdAsync(pecaComando.IdPeca);

                        if (peca == null)
                            throw new InvalidOperationException($"Peça com ID {pecaComando.IdPeca} não encontrada");

                        var ordemPeca = new OrdenServicoPeca
                        {
                            Id = Guid.NewGuid(),
                            IdOrdenServico = ordem.Id,
                            IdPeca = pecaComando.IdPeca,
                            Quantidade = pecaComando.Quantidade,
                            ValorUnitario = pecaComando.ValorUnitario,
                            DataCriacao = DateTime.UtcNow
                        };

                        ordem.PecasUtilizadas.Add(ordemPeca);
                        valorTotal += pecaComando.Quantidade * pecaComando.ValorUnitario;
                    }

                    ordem.ValorTotal = valorTotal;

                    // Atualiza
                    var ordemAtualizada = await _ordemRepository.UpdateAsync(ordem);
                    await _ordemRepository.SaveChangesAsync();

                    await transaction.CommitAsync();

                    _logger.LogInformation("Ordem de serviço {OrdenId} atualizada com sucesso", command.Id);

                    return _mapper.ToResponseDto(ordemAtualizada);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Erro ao atualizar ordem de serviço");
                    throw;
                }
            }
        }
    }

    /// <summary>
    /// Handler para deletar uma ordem de serviço.
    /// </summary>
    public class DeleteOrdenServicoCommandHandler
    {
        private readonly IRepository<OrdenServico, Guid> _ordemRepository;
        private readonly ILogger<DeleteOrdenServicoCommandHandler> _logger;

        public DeleteOrdenServicoCommandHandler(
            IRepository<OrdenServico, Guid> ordemRepository,
            ILogger<DeleteOrdenServicoCommandHandler> logger)
        {
            _ordemRepository = ordemRepository;
            _logger = logger;
        }

        public async Task Handle(DeleteOrdenServicoCommand command)
        {
            var foiDeletada = await _ordemRepository.DeleteAsync(command.Id);

            if (!foiDeletada)
            {
                _logger.LogWarning($"Ordem de serviço com ID {command.Id} não encontrada para deleção");
                throw new InvalidOperationException($"Ordem de serviço com ID {command.Id} não encontrada");
            }

            await _ordemRepository.SaveChangesAsync();

            _logger.LogInformation($"Ordem de serviço com ID {command.Id} foi deletada com sucesso");
        }
    }
}
