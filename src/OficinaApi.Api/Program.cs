// 1. TODOS OS USINGS DEVEM FICAR EXCLUSIVAMENTE NO TOPO DO ARQUIVO
using Serilog;
using Microsoft.EntityFrameworkCore;
using OficinaApi.Infrastructure.Data;
using OficinaApi.Infrastructure.Repositories;
using OficinaApi.Application.Features.Clientes.Mappers;
using OficinaApi.Application.Features.Clientes.Validators;
using Wolverine;
using Wolverine.FluentValidation;
using OficinaApi.Domain.Entities;
using Wolverine.Http;
using FluentValidation;

// 2. CONFIGURAÇÃO INICIAL DO LOG (NENHUM USING PODE IR ABAIXO DISSO)
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        path: Path.Combine(AppContext.BaseDirectory, "logs", "oficina-api-.txt"),
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30)
    .CreateLogger();

try
{
    Log.Information("=== Iniciando OficinaApi ===");

    var builder = WebApplication.CreateBuilder(args);
    
    // Adiciona o Serilog ao Host do builder
    builder.Host.UseSerilog();

    // ===== BANCO DE DADOS =====
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Server=(localdb)\\mssqllocaldb;Database=OficinaDb;Trusted_Connection=true;";
    
    builder.Services.AddDbContext<OficinaDbContext>(options =>
        options.UseSqlServer(connectionString));

    // ===== REPOSITORIES =====
    builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
    builder.Services.AddScoped<IRepository<Carro, Guid>>(provider => 
        new Repository<Carro, Guid>(provider.GetRequiredService<OficinaDbContext>()));
    builder.Services.AddScoped<IRepository<Peca, Guid>>(provider => 
        new Repository<Peca, Guid>(provider.GetRequiredService<OficinaDbContext>()));
    builder.Services.AddScoped<IRepository<OrdenServico, Guid>>(provider => 
        new Repository<OrdenServico, Guid>(provider.GetRequiredService<OficinaDbContext>()));

    // ===== MAPPERLY =====
    builder.Services.AddSingleton<ClienteMapper>();
    builder.Services.AddSingleton<OficinaApi.Application.Features.Carros.Mappers.CarroMapper>();
    builder.Services.AddSingleton<OficinaApi.Application.Features.Pecas.Mappers.PecaMapper>();
    builder.Services.AddSingleton<OficinaApi.Application.Features.OrdenServicos.Mappers.OrdenServicoMapper>();

    // ===== VALIDADORES =====
    builder.Services.AddValidatorsFromAssemblyContaining<CreateClienteValidator>();


    // ===== WOLVERINE FX =====
    builder.Host.UseWolverine(wolverineOptions =>
    {
        wolverineOptions.Discovery.IncludeAssembly(
            typeof(OficinaApi.Application.Features.Clientes.Commands.CreateClienteCommand).Assembly);

        wolverineOptions.UseFluentValidation(); 
    });

    // ===== SWAGGER =====
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "OficinaApi",
            Version = "v1",
            Description = "API para gerenciamento de oficina automotiva"
        });
    });

    // ===== CORS =====
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        });
    });

    var app = builder.Build();

    // ===== MIGRATIONS ASSÍNCRONAS =====
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<OficinaDbContext>();
        try
        {
            dbContext.Database.MigrateAsync().GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao aplicar migrations");
        }
    }

    // ===== MIDDLEWARES =====
    app.UseCors("AllowAll");

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    // Mapeamento automático de rotas HTTP do Wolverine
    app.MapWolverineEndpoints(); 

    Log.Information("Servidor pronto. Rodando aplicação...");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "A aplicação falhou ao iniciar corretamente.");
}
finally
{
    await Log.CloseAndFlushAsync();
}
