using Serilog;
using Microsoft.EntityFrameworkCore;
using OficinaApi.Infrastructure.Data;
using OficinaApi.Infrastructure.Repositories;
using OficinaApi.Application.Features.Clientes.Mappers;
using OficinaApi.Application.Features.Clientes.Validators;
using OficinaApi.Api.Endpoints;
using Wolverine;
using FluentValidation;

/// <summary>
/// Arquivo Program.cs - Ponto de entrada da aplicação.
/// Aqui configuramos todas as dependências, middleware e serviços.
/// Aplicamos o padrão Builder para configuração fluente e testável.
/// </summary>

// Configurar Serilog antes de criar o host
// Serilog é uma biblioteca de logging estruturado que grava logs em arquivo e console
// Utilizamos Serilog para auditoria e debugging de problemas em produção
Log.Logger = new LoggerConfiguration()
    // Nível mínimo: Information (ignora Debug e Verbose)
    .MinimumLevel.Information()
    // Enriquece logs com propriedades como timestamp, thread ID, etc
    .Enrich.FromLogContext()
    // Grava em console para desenvolvimento
    .WriteTo.Console()
    // Grava em arquivo com rolagem diária
    // Path: c:\logs\OficinaApi
    // Formato: {timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{level:u3}] {message:lj}{NewLine}{Exception}
    .WriteTo.File(
        path: @"c:\logs\OficinaApi\oficina-api-.txt",
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
        rollingInterval: RollingInterval.Day, // Novo arquivo a cada dia
        retainedFileCountLimit: 30) // Mantém 30 dias de logs
    .CreateLogger();

try
{
    Log.Information("=== Iniciando OficinaApi ===");

    // Builder da aplicação
    var builder = WebApplicationBuilder.CreateBuilder(args);

    // Adiciona Serilog como provider de logging
    builder.Host.UseSerilog();

    // ===== CONFIGURAÇÃO DE BANCO DE DADOS =====
    // Entity Framework Core com SQL Server
    // Utilizamos dependency injection para injetar o DbContext onde necessário
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Server=(localdb)\\mssqllocaldb;Database=OficinaDb;Trusted_Connection=true;";
    
    builder.Services.AddDbContext<OficinaDbContext>(options =>
        // UseSqlServer configura Entity Framework Core para SQL Server
        // Migrations são aplicadas automaticamente (comentado para evitar erros iniciais)
        options.UseSqlServer(connectionString));

    Log.Information("Banco de dados configurado: {ConnectionString}", connectionString);

    // ===== CONFIGURAÇÃO DE REPOSITORIES =====
    // Injeção de dependência para repositories
    // Implementa o padrão Repository que abstrai acesso ao banco
    builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
    
    // Registra repositories genéricos para outras entidades
    builder.Services.AddScoped<IRepository<Carro, Guid>>(provider => 
        new Repository<Carro, Guid>(provider.GetRequiredService<OficinaDbContext>()));
    
    builder.Services.AddScoped<IRepository<Peca, Guid>>(provider => 
        new Repository<Peca, Guid>(provider.GetRequiredService<OficinaDbContext>()));
    
    builder.Services.AddScoped<IRepository<OrdenServico, Guid>>(provider => 
        new Repository<OrdenServico, Guid>(provider.GetRequiredService<OficinaDbContext>()));

    Log.Information("Repositories registrados");

    // ===== CONFIGURAÇÃO DE MAPPERS =====
    // Registra os Mappers para injeção de dependência
    // Maperly gera código otimizado de mapeamento em tempo de compilação
    builder.Services.AddScoped<OficinaApi.Application.Features.Clientes.Mappers.ClienteMapper>();
    builder.Services.AddScoped<OficinaApi.Application.Features.Carros.Mappers.CarroMapper>();
    builder.Services.AddScoped<OficinaApi.Application.Features.Pecas.Mappers.PecaMapper>();
    builder.Services.AddScoped<OficinaApi.Application.Features.OrdenServicos.Mappers.OrdenServicoMapper>();

    Log.Information("Mappers registrados");

    // ===== CONFIGURAÇÃO DE VALIDADORES =====OficinaApi.Application.Features.Clientes.Validators.
    // FluentValidation para validações de regras de negócio
    // Registra todos os validators automaticamente
    builder.Services.AddValidatorsFromAssemblyContaining(typeof(CreateClienteValidator),
        // Registra validadores em escopo de solicitação
        // Cada requisição tem sua própria instância do validador
        ServiceLifetime.Scoped);

    Log.Information("Validadores registrados");

    // ===== CONFIGURAÇÃO DO WOLVERINE (CQRS) =====
    // Wolverine é um framework para CQRS e message bus
    // Permite despachar commands e queries de forma desacoplada
    builder.Host.UseWolverine((wolverineOptions) =>
    {
        // Configura Wolverine para usar descoberta automática de handlers
        // Handlers são métodos que processam commands de todas as features
        wolverineOptions.Discovery.IncludeAssemblies(
            typeof(OficinaApi.Application.Features.Clientes.Commands.CreateClienteCommand).Assembly);

        // Configuração de pipeline: adiciona validação automaticamente
        // Todos os ICommand passam por validação antes de serem processados
        wolverineOptions.Policies.ForMessagesOfType<OficinaApi.Application.Features.Clientes.Commands.ICommand>()
            .AddMiddleware(typeof(ValidationMiddleware<>)); // Middleware genérico para validação
        
        wolverineOptions.Policies.ForMessagesOfType<OficinaApi.Application.Features.Carros.Commands.ICommand>()
            .AddMiddleware(typeof(ValidationMiddleware<>));
        
        wolverineOptions.Policies.ForMessagesOfType<OficinaApi.Application.Features.Pecas.Commands.ICommand>()
            .AddMiddleware(typeof(ValidationMiddleware<>));
        
        wolverineOptions.Policies.ForMessagesOfType<OficinaApi.Application.Features.OrdenServicos.Commands.ICommand>()
            .AddMiddleware(typeof(ValidationMiddleware<>));
    });

    Log.Information("Wolverine CQRS configurado");

    // ===== CONFIGURAÇÃO DO SWAGGER =====
    // Swagger fornece documentação interativa da API
    // Acessível em /swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "OficinaApi",
            Version = "v1",
            Description = "API para gerenciamento de oficina automotiva",
            Contact = new Microsoft.OpenApi.Models.OpenApiContact
            {
                Name = "Suporte",
                Email = "suporte@oficina.com"
            }
        });

        // Incluir comentários XML na documentação
        var xmlPath = Path.Combine(AppContext.BaseDirectory, "OficinaApi.Api.xml");
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }
    });

    Log.Information("Swagger configurado");

    // ===== CONFIGURAÇÃO DE CORS =====
    // CORS permite requisições de origens diferentes
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            // Permite requisições de qualquer origem (desenvolvimento)
            policy
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
    });

    Log.Information("CORS configurado");

    // ===== BUILD DA APLICAÇÃO =====
    var app = builder.Build();

    // ===== APLICAR MIGRATIONS E SEED DATABASE =====
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<OficinaDbContext>();
        
        try
        {
            // Aplica migrations pendentes
            // Cria as tabelas se não existirem
            dbContext.Database.Migrate();
            Log.Information("Migrations aplicadas com sucesso");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao aplicar migrations");
        }
    }

    // ===== CONFIGURAÇÃO DE MIDDLEWARE =====
    // A ordem importa! Middleware executa em ordem de registro

    // Habilita CORS
    app.UseCors("AllowAll");

    // Swagger - documentação da API
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "OficinaApi v1");
            options.RoutePrefix = string.Empty; // Swagger na raiz
        });
    }

    app.MapCarroEndpoints(); // Endpoints de Carro
    app.MapPecaEndpoints(); // Endpoints de Peça
    app.MapOrdenServicoEndpoints(); // Endpoints de Ordem de Serviço
    // HTTPS redirection
    app.UseHttpsRedirection();

    // Wolverine - mapear handlers de message
    app.MapWolverinePosts("/commands"); // Endpoints para commands em /commands

    // ===== REGISTRO DE ENDPOINTS =====
    // Mapeia os endpoints de cada feature
    // Endpoints em arquivo separado do Program.cs (conforme solicitado)
    app.MapClienteEndpoints(); // Endpoints de Cliente

    // Health check
    app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }))
        .WithName("Health")
        .WithOpenApi();

    Log.Information("=== OficinaApi iniciada com sucesso ===");

    // Inicia a aplicação
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Aplicação encerrada com erro");
}
finally
{
    // Garante que logs sejam salvos antes de encerrar
    Log.CloseAndFlush();
}

/// <summary>
/// Middleware customizado para validação de commands
/// Utiliza FluentValidation para validar commands automaticamente
/// Este middleware é aplicado ao pipeline de processamento de mensagens do Wolverine
/// </summary>
public class ValidationMiddleware<T> where T : class
{
    private readonly IValidator<T>? _validator;
    private readonly ILogger<ValidationMiddleware<T>> _logger;

    public ValidationMiddleware(IValidator<T>? validator, ILogger<ValidationMiddleware<T>> logger)
    {
        _validator = validator;
        _logger = logger;
    }

    public async Task Invoke(T command, Wolverine.IMessageContext context, Wolverine.IMessageBus bus)
    {
        // Se não há validador, passa adiante
        if (_validator == null)
        {
            await context.SendAsync(command);
            return;
        }

        // Executa a validação
        var result = await _validator.ValidateAsync(command);

        // Se há erros, lança exceção
        if (!result.IsValid)
        {
            _logger.LogWarning("Validação falhou para comando {CommandType}: {@Errors}", 
                typeof(T).Name, result.Errors);
            throw new FluentValidation.ValidationException(result.Errors);
        }

        // Se validação passou, passa adiante
        await context.SendAsync(command);
    }
}
