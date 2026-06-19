using Wolverine;
using OficinaApi.Application.Features.OrdenServicos.Commands;
using OficinaApi.Application.Features.OrdenServicos.DTOs;
using OficinaApi.Application.Features.OrdenServicos.Mappers;
using OficinaApi.Infrastructure.Repositories;
using OficinaApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using OficinaApi.Infrastructure.Data;

/// <summary>
/// Endpoints da API para gerenciamento de Ordens de Serviço.
/// Feature mais complexa com relacionamentos múltiplos.
/// </summary>
namespace OficinaApi.Api.Endpoints
{
    public static class OrdenServicoEndpoints
    {
        /// <summary>
        /// Registra todos os endpoints de Ordem de Serviço.
        /// </summary>
        public static void MapOrdenServicoEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/ordens-servico")
                .WithName("OrdensServico")
                .WithOpenApi()
                .WithTags("Ordens de Serviço");

            group.MapPost("/", CreateOrdenServicoEndpoint)
                .WithName("CreateOrdenServico")
                .WithSummary("Criar nova ordem de serviço")
                .Produces<OrdenServicoResponseDto>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);

            group.MapGet("/{id}", GetOrdenServicoByIdEndpoint)
                .WithName("GetOrdenServicoById")
                .WithSummary("Obter ordem de serviço por ID")
                .Produces<OrdenServicoResponseDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            group.MapGet("/", GetAllOrdensServicoEndpoint)
                .WithName("GetAllOrdensServico")
                .WithSummary("Listar todas as ordens de serviço")
                .Produces<List<OrdenServicoResponseDto>>(StatusCodes.Status200OK);

            group.MapPut("/{id}", UpdateOrdenServicoEndpoint)
                .WithName("UpdateOrdenServico")
                .WithSummary("Atualizar ordem de serviço")
                .Produces<OrdenServicoResponseDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            group.MapDelete("/{id}", DeleteOrdenServicoEndpoint)
                .WithName("DeleteOrdenServico")
                .WithSummary("Deletar ordem de serviço")
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound);
        }

        private static async Task<IResult> CreateOrdenServicoEndpoint(
            CreateOrdenServicoDto dto,
            IMessageBus messageBus,
            ILogger<OrdenServicoEndpoints> logger)
        {
            try
            {
                logger.LogInformation("Criando nova ordem de serviço: {@Ordem}", dto);

                var pecas = dto.Pecas.Select(p => new PecaOrdenCommand(p.IdPeca, p.Quantidade, p.Valor)).ToList();
                var command = new CreateOrdenServicoCommand(dto.IdCarro, dto.IdCliente, dto.Servicos, pecas);

                var resultado = await messageBus.InvokeAsync<OrdenServicoResponseDto>(command);

                logger.LogInformation("Ordem de serviço criada com sucesso. ID: {OrdenId}", resultado.Id);

                return Results.Created($"/api/ordens-servico/{resultado.Id}", resultado);
            }
            catch (FluentValidation.ValidationException ex)
            {
                logger.LogWarning("Erro de validação ao criar ordem de serviço");
                return Results.BadRequest(new
                {
                    message = "Dados inválidos",
                    errors = ex.Errors.GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
                });
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning("Erro ao criar ordem: {Message}", ex.Message);
                return Results.BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao criar ordem de serviço");
                return Results.StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private static async Task<IResult> GetOrdenServicoByIdEndpoint(
            Guid id,
            OficinaDbContext context,
            OrdenServicoMapper mapper,
            ILogger<OrdenServicoEndpoints> logger)
        {
            try
            {
                logger.LogInformation("Buscando ordem de serviço com ID: {OrdenId}", id);

                var ordem = await context.OrdensServico
                    .Include(o => o.PecasUtilizadas)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (ordem == null)
                {
                    logger.LogWarning("Ordem de serviço com ID {OrdenId} não encontrada", id);
                    return Results.NotFound(new { message = "Ordem de serviço não encontrada" });
                }

                var dto = mapper.ToResponseDto(ordem);
                return Results.Ok(dto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao buscar ordem de serviço {OrdenId}", id);
                return Results.StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private static async Task<IResult> GetAllOrdensServicoEndpoint(
            OficinaDbContext context,
            OrdenServicoMapper mapper,
            ILogger<OrdenServicoEndpoints> logger)
        {
            try
            {
                logger.LogInformation("Listando todas as ordens de serviço");

                var ordens = await context.OrdensServico
                    .Include(o => o.PecasUtilizadas)
                    .ToListAsync();

                var dtos = ordens.Select(o => mapper.ToResponseDto(o)).ToList();

                logger.LogInformation("Retornadas {Count} ordens de serviço", dtos.Count);

                return Results.Ok(dtos);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao listar ordens de serviço");
                return Results.StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private static async Task<IResult> UpdateOrdenServicoEndpoint(
            Guid id,
            UpdateOrdenServicoDto dto,
            IMessageBus messageBus,
            ILogger<OrdenServicoEndpoints> logger)
        {
            try
            {
                logger.LogInformation("Atualizando ordem de serviço {OrdenId}", id);

                var pecas = dto.Pecas.Select(p => new PecaOrdenCommand(p.IdPeca, p.Quantidade, p.Valor)).ToList();
                var command = new UpdateOrdenServicoCommand(id, dto.IdCarro, dto.IdCliente, dto.Servicos, dto.Status, pecas);

                var resultado = await messageBus.InvokeAsync<OrdenServicoResponseDto>(command);

                logger.LogInformation("Ordem de serviço {OrdenId} atualizada com sucesso", id);

                return Results.Ok(resultado);
            }
            catch (FluentValidation.ValidationException ex)
            {
                logger.LogWarning("Erro de validação ao atualizar ordem de serviço {OrdenId}", id);
                return Results.BadRequest(new
                {
                    message = "Dados inválidos",
                    errors = ex.Errors.GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
                });
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning("Ordem de serviço {OrdenId} não encontrada", id);
                return Results.NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao atualizar ordem de serviço {OrdenId}", id);
                return Results.StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private static async Task<IResult> DeleteOrdenServicoEndpoint(
            Guid id,
            IMessageBus messageBus,
            ILogger<OrdenServicoEndpoints> logger)
        {
            try
            {
                logger.LogInformation("Deletando ordem de serviço {OrdenId}", id);

                var command = new DeleteOrdenServicoCommand(id);
                await messageBus.InvokeAsync(command);

                logger.LogInformation("Ordem de serviço {OrdenId} deletada com sucesso", id);

                return Results.NoContent();
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning("Ordem de serviço {OrdenId} não encontrada", id);
                return Results.NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao deletar ordem de serviço {OrdenId}", id);
                return Results.StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
