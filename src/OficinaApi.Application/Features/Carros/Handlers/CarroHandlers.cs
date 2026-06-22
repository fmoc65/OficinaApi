using Microsoft.Extensions.Logging;
using OficinaApi.Application.Features.Carros.Commands;
using OficinaApi.Application.Features.Carros.DTOs;
using OficinaApi.Application.Features.Carros.Mappers;
using OficinaApi.Domain.Entities;
using OficinaApi.Infrastructure.Repositories;

/// <summary>
/// Handlers para commands de Carro.
/// Processam operações CRUD via CQRS com Wolverine.
/// Cada handler tem responsabilidade única (SOLID).
/// </summary>
namespace OficinaApi.Application.Features.Carros.Handlers
{
    /// <summary>
    /// Handler para criar novo carro.
    /// </summary>
    public class CreateCarroCommandHandler
    {
        private readonly IRepository<Carro, Guid> _carroRepository;
        private readonly CarroMapper _mapper;

        public CreateCarroCommandHandler(
            IRepository<Carro, Guid> carroRepository,
            CarroMapper mapper)
        {
            _carroRepository = carroRepository;
            _mapper = mapper;
        }

        public async Task<CarroResponseDto> Handle(CreateCarroCommand command)
        {
            // Mapeia command para entidade
            var carro = _mapper.ToEntity(command);

            // Adiciona ao repository
            var carroAdicionado = await _carroRepository.AddAsync(carro);

            // Persiste no banco
            await _carroRepository.SaveChangesAsync();

            // Retorna como DTO
            return _mapper.ToResponseDto(carroAdicionado);
        }
    }

    /// <summary>
    /// Handler para atualizar carro existente.
    /// </summary>
    public class UpdateCarroCommandHandler
    {
        private readonly IRepository<Carro, Guid> _carroRepository;
        private readonly CarroMapper _mapper;

        public UpdateCarroCommandHandler(
            IRepository<Carro, Guid> carroRepository,
            CarroMapper mapper)
        {
            _carroRepository = carroRepository;
            _mapper = mapper;
        }

        public async Task<CarroResponseDto> Handle(UpdateCarroCommand command)
        {
            // Busca carro existente
            var carro = await _carroRepository.GetByIdAsync(command.Id);

            if (carro == null)
                throw new InvalidOperationException($"Carro com ID {command.Id} não encontrado");

            // Atualiza propriedades
            carro.Modelo = command.Modelo;
            carro.Ano = command.Ano;
            carro.IdCliente = command.IdCliente;
            carro.DataAtualizacao = DateTime.UtcNow;

            // Persiste mudanças
            var carroAtualizado = await _carroRepository.UpdateAsync(carro);
            await _carroRepository.SaveChangesAsync();

            // Retorna como DTO
            return _mapper.ToResponseDto(carroAtualizado);
        }
    }

    /// <summary>
    /// Handler para deletar carro.
    /// </summary>
    public class DeleteCarroCommandHandler
    {
        private readonly IRepository<Carro, Guid> _carroRepository;
        private readonly ILogger<DeleteCarroCommandHandler> _logger;

        public DeleteCarroCommandHandler(
            IRepository<Carro, Guid> carroRepository,
            ILogger<DeleteCarroCommandHandler> logger)
        {
            _carroRepository = carroRepository;
            _logger = logger;
        }

        public async Task Handle(DeleteCarroCommand command)
        {
            // Tenta deletar
            var foiDeletado = await _carroRepository.DeleteAsync(command.Id);

            if (!foiDeletado)
            {
                _logger.LogWarning("Carro com ID {0} não encontrado para deleção", command.Id);
                throw new InvalidOperationException($"Carro com ID {command.Id} não encontrado");
            }

            // Persiste deleção
            await _carroRepository.SaveChangesAsync();

            _logger.LogInformation("Carro com ID {0} foi deletado com sucesso", command.Id);
        }
    }
}
