using OficinaApi.Domain.Entities;
using OficinaApi.Infrastructure.Repositories;
using OficinaApi.Application.Features.Pecas.Commands;
using OficinaApi.Application.Features.Pecas.Mappers;
using OficinaApi.Application.Features.Pecas.DTOs;

/// <summary>
/// Handlers para commands de Peça.
/// Processam operações CRUD com CQRS.
/// </summary>
namespace OficinaApi.Application.Features.Pecas.Handlers
{
    public class CreatePecaCommandHandler
    {
        private readonly IRepository<Peca, Guid> _pecaRepository;
        private readonly PecaMapper _mapper;

        public CreatePecaCommandHandler(
            IRepository<Peca, Guid> pecaRepository,
            PecaMapper mapper)
        {
            _pecaRepository = pecaRepository;
            _mapper = mapper;
        }

        public async Task<PecaResponseDto> Handle(CreatePecaCommand command)
        {
            var peca = _mapper.ToEntity(command);
            var pecaAdicionada = await _pecaRepository.AddAsync(peca);
            await _pecaRepository.SaveChangesAsync();
            return _mapper.ToResponseDto(pecaAdicionada);
        }
    }

    public class UpdatePecaCommandHandler
    {
        private readonly IRepository<Peca, Guid> _pecaRepository;
        private readonly PecaMapper _mapper;

        public UpdatePecaCommandHandler(
            IRepository<Peca, Guid> pecaRepository,
            PecaMapper mapper)
        {
            _pecaRepository = pecaRepository;
            _mapper = mapper;
        }

        public async Task<PecaResponseDto> Handle(UpdatePecaCommand command)
        {
            var peca = await _pecaRepository.GetByIdAsync(command.Id);

            if (peca == null)
                throw new InvalidOperationException($"Peça com ID {command.Id} não encontrada");

            peca.IdPeca = command.IdPeca;
            peca.IdCarro = command.IdCarro;
            peca.Quantidade = command.Quantidade;
            peca.Valor = command.Valor;
            peca.DataAtualizacao = DateTime.UtcNow;

            var pecaAtualizada = await _pecaRepository.UpdateAsync(peca);
            await _pecaRepository.SaveChangesAsync();
            return _mapper.ToResponseDto(pecaAtualizada);
        }
    }

    public class DeletePecaCommandHandler
    {
        private readonly IRepository<Peca, Guid> _pecaRepository;
        private readonly ILogger<DeletePecaCommandHandler> _logger;

        public DeletePecaCommandHandler(
            IRepository<Peca, Guid> pecaRepository,
            ILogger<DeletePecaCommandHandler> logger)
        {
            _pecaRepository = pecaRepository;
            _logger = logger;
        }

        public async Task Handle(DeletePecaCommand command)
        {
            var foiDeletada = await _pecaRepository.DeleteAsync(command.Id);

            if (!foiDeletada)
            {
                _logger.LogWarning($"Peça com ID {command.Id} não encontrada para deleção");
                throw new InvalidOperationException($"Peça com ID {command.Id} não encontrada");
            }

            await _pecaRepository.SaveChangesAsync();
            _logger.LogInformation($"Peça com ID {command.Id} foi deletada com sucesso");
        }
    }
}
