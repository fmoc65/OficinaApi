using Wolverine;
using OficinaApi.Application.Features.Clientes.Commands;
using OficinaApi.Application.Features.Clientes.DTOs;

/// <summary>
/// Endpoints da API para gerenciamento de Clientes.
/// Utilizamos Minimal APIs que simplificam a criação de endpoints sem necessidade de controllers.
/// Cada endpoint é definido de forma clara e concisa.
/// Endpoints estão separados do Program.cs para manter organização (conforme solicitado).
/// Aplicamos SOLID (Single Responsibility) - este arquivo responsável apenas pelos endpoints de Cliente.
/// </summary>
namespace OficinaApi.Api.Endpoints
{
    public static class ClienteEndpoints
    {
        /// <summary>
        /// Registra todos os endpoints de Cliente na aplicação.
        /// Este método deve ser chamado no Program.cs durante a configuração.
        /// Roteamos tudo sob /api/clientes
        /// </summary>
        public static void MapClienteEndpoints(this WebApplication app)
        {
            // Grupo de rotas para all clientes endpoints
            // /api/clientes
            var group = app.MapGroup("/api/clientes")
                .WithName("Clientes")
                .WithOpenApi() // Habilita suporte a OpenAPI/Swagger
                .WithTags("Clientes"); // Agrupa no Swagger

            // POST /api/clientes - Criar novo cliente
            group.MapPost("/", CreateClienteEndpoint)
                .WithName("CreateCliente")
                .WithSummary("Criar um novo cliente")
                .WithDescription("Cria um novo cliente na oficina")
                .Produces<ClienteResponseDto>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status500InternalServerError);

            // GET /api/clientes/{id} - Obter cliente por ID
            group.MapGet("/{id}", GetClienteByIdEndpoint)
                .WithName("GetClienteById")
                .WithSummary("Obter cliente por ID")
                .WithDescription("Retorna os dados de um cliente específico")
                .Produces<ClienteResponseDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status500InternalServerError);

            // GET /api/clientes - Listar todos os clientes
            group.MapGet("/", GetAllClientesEndpoint)
                .WithName("GetAllClientes")
                .WithSummary("Listar todos os clientes")
                .WithDescription("Retorna uma lista de todos os clientes cadastrados")
                .Produces<List<ClienteResponseDto>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status500InternalServerError);

            // PUT /api/clientes - Atualizar cliente
            group.MapPut("/{id}", UpdateClienteEndpoint)
                .WithName("UpdateCliente")
                .WithSummary("Atualizar cliente")
                .WithDescription("Atualiza os dados de um cliente existente")
                .Produces<ClienteResponseDto>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status500InternalServerError);

            // DELETE /api/clientes/{id} - Deletar cliente
            group.MapDelete("/{id}", DeleteClienteEndpoint)
                .WithName("DeleteCliente")
                .WithSummary("Deletar cliente")
                .WithDescription("Remove um cliente do sistema")
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status500InternalServerError);
        }

        /// <summary>
        /// Cria um novo cliente.
        /// POST /api/clientes
        /// </summary>
        private static async Task<IResult> CreateClienteEndpoint(
            CreateClienteDto dto,
            IMessageBus messageBus,
            ILogger<ClienteEndpoints> logger)
        {
            try
            {
                // Log da operação
                logger.LogInformation("Iniciando criação de novo cliente: {@Cliente}", dto);

                // Cria o command a partir do DTO
                var command = new CreateClienteCommand(dto.Nome, dto.Telefone, dto.Endereco);

                // Despacha o command via Wolverine
                // Wolverine processa o command através do handler registrado
                var resultado = await messageBus.InvokeAsync<ClienteResponseDto>(command);

                // Log de sucesso
                logger.LogInformation("Cliente criado com sucesso. ID: {ClienteId}", resultado.Id);

                // Retorna 201 Created com o cliente criado
                // Location header aponta para GET /api/clientes/{id}
                return Results.Created($"/api/clientes/{resultado.Id}", resultado);
            }
            catch (FluentValidation.ValidationException ex)
            {
                // Erros de validação (4xx)
                logger.LogWarning("Erro de validação ao criar cliente: {@Errors}", ex.Errors);
                // Retorna 400 Bad Request com detalhes dos erros
                return Results.BadRequest(new
                {
                    message = "Dados inválidos",
                    errors = ex.Errors.GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
                });
            }
            catch (Exception ex)
            {
                // Erros inesperados (5xx)
                logger.LogError(ex, "Erro ao criar cliente");
                return Results.StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Obtém um cliente pelo ID.
        /// GET /api/clientes/{id}
        /// </summary>
        private static async Task<IResult> GetClienteByIdEndpoint(
            Guid id,
            IClienteRepository repository,
            OficinaApi.Application.Features.Clientes.Mappers.ClienteMapper mapper,
            ILogger<ClienteEndpoints> logger)
        {
            try
            {
                logger.LogInformation("Buscando cliente com ID: {ClienteId}", id);

                // Busca o cliente no repository
                var cliente = await repository.GetByIdAsync(id);

                // Valida se foi encontrado
                if (cliente == null)
                {
                    logger.LogWarning("Cliente com ID {ClienteId} não encontrado", id);
                    return Results.NotFound(new { message = "Cliente não encontrado" });
                }

                // Mapeia para DTO
                var dto = mapper.ToResponseDto(cliente);

                logger.LogInformation("Cliente {ClienteId} retornado com sucesso", id);

                // Retorna 200 OK com o cliente
                return Results.Ok(dto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao buscar cliente {ClienteId}", id);
                return Results.StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Lista todos os clientes.
        /// GET /api/clientes
        /// </summary>
        private static async Task<IResult> GetAllClientesEndpoint(
            IClienteRepository repository,
            OficinaApi.Application.Features.Clientes.Mappers.ClienteMapper mapper,
            ILogger<ClienteEndpoints> logger)
        {
            try
            {
                logger.LogInformation("Listando todos os clientes");

                // Busca todos os clientes com suas relações
                var clientes = await repository.GetAllWithRelationsAsync();

                // Mapeia para DTOs
                var dtos = clientes.Select(c => mapper.ToResponseDto(c)).ToList();

                logger.LogInformation("Retornados {Count} clientes", dtos.Count);

                // Retorna 200 OK com lista de clientes
                return Results.Ok(dtos);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao listar clientes");
                return Results.StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Atualiza um cliente existente.
        /// PUT /api/clientes/{id}
        /// </summary>
        private static async Task<IResult> UpdateClienteEndpoint(
            Guid id,
            UpdateClienteDto dto,
            IMessageBus messageBus,
            ILogger<ClienteEndpoints> logger)
        {
            try
            {
                logger.LogInformation("Atualizando cliente {ClienteId}", id);

                // Cria o command de atualização
                var command = new UpdateClienteCommand(id, dto.Nome, dto.Telefone, dto.Endereco);

                // Despacha o command via Wolverine
                var resultado = await messageBus.InvokeAsync<ClienteResponseDto>(command);

                logger.LogInformation("Cliente {ClienteId} atualizado com sucesso", id);

                // Retorna 200 OK com o cliente atualizado
                return Results.Ok(resultado);
            }
            catch (FluentValidation.ValidationException ex)
            {
                logger.LogWarning("Erro de validação ao atualizar cliente {ClienteId}", id);
                return Results.BadRequest(new
                {
                    message = "Dados inválidos",
                    errors = ex.Errors.GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
                });
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning("Cliente {ClienteId} não encontrado", id);
                return Results.NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao atualizar cliente {ClienteId}", id);
                return Results.StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Deleta um cliente.
        /// DELETE /api/clientes/{id}
        /// </summary>
        private static async Task<IResult> DeleteClienteEndpoint(
            Guid id,
            IMessageBus messageBus,
            ILogger<ClienteEndpoints> logger)
        {
            try
            {
                logger.LogInformation("Deletando cliente {ClienteId}", id);

                // Cria o command de deleção
                var command = new DeleteClienteCommand(id);

                // Despacha o command via Wolverine
                await messageBus.InvokeAsync(command);

                logger.LogInformation("Cliente {ClienteId} deletado com sucesso", id);

                // Retorna 204 No Content (sucesso sem retorno de dados)
                return Results.NoContent();
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning("Cliente {ClienteId} não encontrado", id);
                return Results.NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao deletar cliente {ClienteId}", id);
                return Results.StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
