using Wolverine;
using OficinaApi.Application.Features.Carros.Commands;
using OficinaApi.Application.Features.Carros.DTOs;
using OficinaApi.Application.Features.Carros.Mappers;
using OficinaApi.Infrastructure.Repositories;
using OficinaApi.Domain.Entities;

/// <summary>
/// Endpoints da API para gerenciamento de Carros.
/// Separados do Program.cs conforme arquitetura slice solicitada.
/// Aplicam SOLID (Single Responsibility) - responsáveis apenas por Carros.
/// </summary>
namespace OficinaApi.Api.Endpoints
{
    public static class CarroEndpoints
    {
        /// <summary>
        /// Registra todos os endpoints de Carro.
        /// Deve ser chamado no Program.cs durante configuração.
        /// </summary>
        public static void MapCarroEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/carros")
                .WithName("Carros")
                .WithOpenApi()
                .WithTags("Carros");

            // POST /api/carros - Criar novo carro
            group.MapPost("/", CreateCarroEndpoint)
                .WithName("CreateCarro")
                .WithSummary("Criar novo carro")
                .Produces<CarroResponseDto>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);

            // GET /api/carros/{id} - Obter carro por ID
            group.MapGet("/{id}", GetCarroByIdEndpoint)
                .WithName("GetCarroById")
                .WithSummary("Obter carro por ID")
                .Produces<CarroResponseDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            // GET /api/carros - Listar todos os carros
            group.MapGet("/", GetAllCarrosEndpoint)
                .WithName("GetAllCarros")
                .WithSummary("Listar todos os carros")
                .Produces<List<CarroResponseDto>>(StatusCodes.Status200OK);

            // PUT /api/carros/{id} - Atualizar carro
            group.MapPut("/{id}", UpdateCarroEndpoint)
                .WithName("UpdateCarro")
                .WithSummary("Atualizar carro")
                .Produces<CarroResponseDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            // DELETE /api/carros/{id} - Deletar carro
            group.MapDelete("/{id}", DeleteCarroEndpoint)
                .WithName("DeleteCarro")
                .WithSummary("Deletar carro")
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound);
        }

        /// <summary>
        /// Cria um novo carro.
        /// POST /api/carros
        /// </summary>
        private static async Task<IResult> CreateCarroEndpoint(
            CreateCarroDto dto,
            IMessageBus messageBus,
            ILogger<CarroEndpoints> logger)
        {
            try
            {
                logger.LogInformation("Criando novo carro: {@Carro}", dto);

                var command = new CreateCarroCommand(dto.Modelo, dto.Ano, dto.IdCliente);
                var resultado = await messageBus.InvokeAsync<CarroResponseDto>(command);

                logger.LogInformation("Carro criado com sucesso. ID: {CarroId}", resultado.Id);

                return Results.Created($"/api/carros/{resultado.Id}", resultado);
            }
            catch (FluentValidation.ValidationException ex)
            {
                logger.LogWarning("Erro de validação ao criar carro: {@Errors}", ex.Errors);
                return Results.BadRequest(new
                {
                    message = "Dados inválidos",
                    errors = ex.Errors.GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao criar carro");
                return Results.StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Obtém um carro pelo ID.
        /// GET /api/carros/{id}
        /// </summary>
        private static async Task<IResult> GetCarroByIdEndpoint(
            Guid id,
            IRepository<Carro, Guid> repository,
            CarroMapper mapper,
            ILogger<CarroEndpoints> logger)
        {
            try
            {
                logger.LogInformation("Buscando carro com ID: {CarroId}", id);

                var carro = await repository.GetByIdAsync(id);

                if (carro == null)
                {
                    logger.LogWarning("Carro com ID {CarroId} não encontrado", id);
                    return Results.NotFound(new { message = "Carro não encontrado" });
                }

                var dto = mapper.ToResponseDto(carro);
                return Results.Ok(dto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao buscar carro {CarroId}", id);
                return Results.StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Lista todos os carros.
        /// GET /api/carros
        /// </summary>
        private static async Task<IResult> GetAllCarrosEndpoint(
            IRepository<Carro, Guid> repository,
            CarroMapper mapper,
            ILogger<CarroEndpoints> logger)
        {
            try
            {
                logger.LogInformation("Listando todos os carros");

                var carros = await repository.GetAllAsync();
                var dtos = carros.Select(c => mapper.ToResponseDto(c)).ToList();

                logger.LogInformation("Retornados {Count} carros", dtos.Count);

                return Results.Ok(dtos);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao listar carros");
                return Results.StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Atualiza um carro existente.
        /// PUT /api/carros/{id}
        /// </summary>
        private static async Task<IResult> UpdateCarroEndpoint(
            Guid id,
            UpdateCarroDto dto,
            IMessageBus messageBus,
            ILogger<CarroEndpoints> logger)
        {
            try
            {
                logger.LogInformation("Atualizando carro {CarroId}", id);

                var command = new UpdateCarroCommand(id, dto.Modelo, dto.Ano, dto.IdCliente);
                var resultado = await messageBus.InvokeAsync<CarroResponseDto>(command);

                logger.LogInformation("Carro {CarroId} atualizado com sucesso", id);

                return Results.Ok(resultado);
            }
            catch (FluentValidation.ValidationException ex)
            {
                logger.LogWarning("Erro de validação ao atualizar carro {CarroId}", id);
                return Results.BadRequest(new
                {
                    message = "Dados inválidos",
                    errors = ex.Errors.GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
                });
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning("Carro {CarroId} não encontrado", id);
                return Results.NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao atualizar carro {CarroId}", id);
                return Results.StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Deleta um carro.
        /// DELETE /api/carros/{id}
        /// </summary>
        private static async Task<IResult> DeleteCarroEndpoint(
            Guid id,
            IMessageBus messageBus,
            ILogger<CarroEndpoints> logger)
        {
            try
            {
                logger.LogInformation("Deletando carro {CarroId}", id);

                var command = new DeleteCarroCommand(id);
                await messageBus.InvokeAsync(command);

                logger.LogInformation("Carro {CarroId} deletado com sucesso", id);

                return Results.NoContent();
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning("Carro {CarroId} não encontrado", id);
                return Results.NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao deletar carro {CarroId}", id);
                return Results.StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
