using Wolverine;
using OficinaApi.Application.Features.Pecas.Commands;
using OficinaApi.Application.Features.Pecas.DTOs;
using OficinaApi.Application.Features.Pecas.Mappers;
using OficinaApi.Infrastructure.Repositories;
using OficinaApi.Domain.Entities;

/// <summary>
/// Endpoints da API para gerenciamento de Peças.
/// </summary>
namespace OficinaApi.Api.Endpoints
{
    public static class PecaEndpoints
    {
        /// <summary>
        /// Registra todos os endpoints de Peça.
        /// </summary>
        public static void MapPecaEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/pecas")
                .WithName("Pecas")
                .WithOpenApi()
                .WithTags("Pecas");

            group.MapPost("/", CreatePecaEndpoint)
                .WithName("CreatePeca")
                .WithSummary("Criar nova peça")
                .Produces<PecaResponseDto>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);

            group.MapGet("/{id}", GetPecaByIdEndpoint)
                .WithName("GetPecaById")
                .WithSummary("Obter peça por ID")
                .Produces<PecaResponseDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            group.MapGet("/", GetAllPecasEndpoint)
                .WithName("GetAllPecas")
                .WithSummary("Listar todas as peças")
                .Produces<List<PecaResponseDto>>(StatusCodes.Status200OK);

            group.MapPut("/{id}", UpdatePecaEndpoint)
                .WithName("UpdatePeca")
                .WithSummary("Atualizar peça")
                .Produces<PecaResponseDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            group.MapDelete("/{id}", DeletePecaEndpoint)
                .WithName("DeletePeca")
                .WithSummary("Deletar peça")
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound);
        }

        private static async Task<IResult> CreatePecaEndpoint(
            CreatePecaDto dto,
            IMessageBus messageBus,
            ILogger<PecaEndpoints> logger)
        {
            try
            {
                logger.LogInformation("Criando nova peça: {@Peca}", dto);

                var command = new CreatePecaCommand(dto.IdPeca, dto.IdCarro, dto.Quantidade, dto.Valor);
                var resultado = await messageBus.InvokeAsync<PecaResponseDto>(command);

                logger.LogInformation("Peça criada com sucesso. ID: {PecaId}", resultado.Id);

                return Results.Created($"/api/pecas/{resultado.Id}", resultado);
            }
            catch (FluentValidation.ValidationException ex)
            {
                logger.LogWarning("Erro de validação ao criar peça");
                return Results.BadRequest(new
                {
                    message = "Dados inválidos",
                    errors = ex.Errors.GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao criar peça");
                return Results.StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private static async Task<IResult> GetPecaByIdEndpoint(
            Guid id,
            IRepository<Peca, Guid> repository,
            PecaMapper mapper,
            ILogger<PecaEndpoints> logger)
        {
            try
            {
                logger.LogInformation("Buscando peça com ID: {PecaId}", id);

                var peca = await repository.GetByIdAsync(id);

                if (peca == null)
                {
                    logger.LogWarning("Peça com ID {PecaId} não encontrada", id);
                    return Results.NotFound(new { message = "Peça não encontrada" });
                }

                var dto = mapper.ToResponseDto(peca);
                return Results.Ok(dto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao buscar peça {PecaId}", id);
                return Results.StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private static async Task<IResult> GetAllPecasEndpoint(
            IRepository<Peca, Guid> repository,
            PecaMapper mapper,
            ILogger<PecaEndpoints> logger)
        {
            try
            {
                logger.LogInformation("Listando todas as peças");

                var pecas = await repository.GetAllAsync();
                var dtos = pecas.Select(p => mapper.ToResponseDto(p)).ToList();

                logger.LogInformation("Retornadas {Count} peças", dtos.Count);

                return Results.Ok(dtos);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao listar peças");
                return Results.StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private static async Task<IResult> UpdatePecaEndpoint(
            Guid id,
            UpdatePecaDto dto,
            IMessageBus messageBus,
            ILogger<PecaEndpoints> logger)
        {
            try
            {
                logger.LogInformation("Atualizando peça {PecaId}", id);

                var command = new UpdatePecaCommand(id, dto.IdPeca, dto.IdCarro, dto.Quantidade, dto.Valor);
                var resultado = await messageBus.InvokeAsync<PecaResponseDto>(command);

                logger.LogInformation("Peça {PecaId} atualizada com sucesso", id);

                return Results.Ok(resultado);
            }
            catch (FluentValidation.ValidationException ex)
            {
                logger.LogWarning("Erro de validação ao atualizar peça {PecaId}", id);
                return Results.BadRequest(new
                {
                    message = "Dados inválidos",
                    errors = ex.Errors.GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
                });
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning("Peça {PecaId} não encontrada", id);
                return Results.NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao atualizar peça {PecaId}", id);
                return Results.StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private static async Task<IResult> DeletePecaEndpoint(
            Guid id,
            IMessageBus messageBus,
            ILogger<PecaEndpoints> logger)
        {
            try
            {
                logger.LogInformation("Deletando peça {PecaId}", id);

                var command = new DeletePecaCommand(id);
                await messageBus.InvokeAsync(command);

                logger.LogInformation("Peça {PecaId} deletada com sucesso", id);

                return Results.NoContent();
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning("Peça {PecaId} não encontrada", id);
                return Results.NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao deletar peça {PecaId}", id);
                return Results.StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
