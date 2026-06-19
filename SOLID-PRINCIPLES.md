# SOLID Principles - Aplicação em OficinaApi

## ?? Visão Geral

Este documento descreve como cada princípio SOLID foi implementado na arquitetura da OficinaApi.

---

## **S - Single Responsibility Principle (SRP)**

### Definição
Uma classe deve ter apenas uma razão para mudar, ou seja, uma única responsabilidade.

### Implementação no Projeto

#### ? Violação (Evitar)
```csharp
public class ClienteService
{
    // ? MÁ PRÁTICA: Uma classe faz tudo
    public void CriarCliente(CreateClienteDto dto)
    {
        // Validação
        if (string.IsNullOrEmpty(dto.Nome))
            throw new Exception("Nome obrigatório");
        
        // Mapeamento
        var cliente = new Cliente { Nome = dto.Nome };
        
        // Persistência
        _context.Clientes.Add(cliente);
        _context.SaveChanges();
        
        // Log
        Console.WriteLine("Cliente criado");
    }
}
```

#### ? Solução (Implementado)
```csharp
// 1. Validação - ResponsabilidadeValidator
public class CreateClienteValidator : AbstractValidator<CreateClienteCommand>
{
    public CreateClienteValidator()
    {
        RuleFor(x => x.Nome).NotEmpty();
    }
}

// 2. Mapeamento - Mapper
[Mapper]
public partial class ClienteMapper
{
    public partial Cliente ToEntity(CreateClienteCommand command);
}

// 3. Persistência - Repository
public class ClienteRepository : Repository<Cliente, Guid>
{
    public async Task<Cliente> AddAsync(Cliente entity) { ... }
}

// 4. Orquestração - Handler
public class CreateClienteCommandHandler
{
    public async Task<ClienteResponseDto> Handle(CreateClienteCommand command)
    {
        var cliente = _mapper.ToEntity(command);
        await _repository.AddAsync(cliente);
        await _repository.SaveChangesAsync();
        return _mapper.ToResponseDto(cliente);
    }
}
```

### Benefícios
- **Manutenibilidade**: Cada arquivo tem um propósito claro
- **Testabilidade**: Fácil escrever testes unitários
- **Reutilização**: Validador pode ser reusado em outros places
- **Flexibilidade**: Mudar validação não afeta persistência

---

## **O - Open/Closed Principle (OCP)**

### Definição
Software deve estar aberto para extensão, mas fechado para modificação.

### Implementação no Projeto

#### ? Violação (Evitar)
```csharp
// ? MÁ PRÁTICA: Para adicionar novo tipo de entidade, precisa modificar a classe
public class GenericHandler
{
    public void Handle(object command)
    {
        if (command is CreateClienteCommand)
        {
            // Processar cliente
        }
        else if (command is CreateCarroCommand)
        {
            // Processar carro
        }
        // ... adicionar mais tipos?
    }
}
```

#### ? Solução (Implementado)
```csharp
// Handlers específicos - cada um trata seu command
public class CreateClienteCommandHandler
{
    public async Task<ClienteResponseDto> Handle(CreateClienteCommand command) { ... }
}

public class CreateCarroCommandHandler
{
    public async Task<CarroResponseDto> Handle(CreateCarroCommand command) { ... }
}

// Wolverine descobre automaticamente novos handlers
// Adicionar novo handler = extensão sem modificação existente
```

### Genéricos para Extensão
```csharp
// ? Aberto para extensão via genéricos
public class Repository<TEntity, TId> : IRepository<TEntity, TId>
    where TEntity : class
{
    public async Task<TEntity?> GetByIdAsync(TId id) { ... }
    public async Task<TEntity> AddAsync(TEntity entity) { ... }
}

// Pode ser usado para QUALQUER entidade sem modificação
public class ClienteRepository : Repository<Cliente, Guid> { }
public class CarroRepository : Repository<Carro, Guid> { }
public class PecaRepository : Repository<Peca, Guid> { }
```

### Benefícios
- **Escalabilidade**: Adicionar feature não quebra código existente
- **Manutenção**: Mudanças localizadas
- **Estabilidade**: Código antigo continua funcionando

---

## **L - Liskov Substitution Principle (LSP)**

### Definição
Objetos de uma classe derivada devem poder substituir objetos da classe base sem quebrar a aplicação.

### Implementação no Projeto

#### ? Violação (Evitar)
```csharp
public interface IRepository<T, in TId>
{
    Task<T?> GetByIdAsync(TId id);
    Task<IEnumerable<T>> GetAllAsync();
    Task SaveChangesAsync();
}

// ? MÁ PRÁTICA: Implementação viola contrato
public class ClienteRepository : IRepository<Cliente, Guid>
{
    public async Task<Cliente?> GetByIdAsync(Guid id)
    {
        // Lança exceção em vez de retornar null ou cliente
        throw new NotImplementedException();
    }
    
    public async Task SaveChangesAsync()
    {
        // Não faz nada - viola expectativa de salvar
    }
}
```

#### ? Solução (Implementado)
```csharp
// Implementações respeitam o contrato
public class Repository<TEntity, TId> : IRepository<TEntity, TId>
    where TEntity : class
{
    public async Task<TEntity?> GetByIdAsync(TId id)
    {
        // Sempre retorna T? como esperado
        return await _dbSet.FindAsync(id);
    }
    
    public async Task SaveChangesAsync()
    {
        // Sempre salva como esperado
        await _context.SaveChangesAsync();
    }
}

// Qualquer repositório pode substituir o outro
IRepository<Cliente, Guid> repo = new ClienteRepository(context);
IRepository<Carro, Guid> repo2 = new Repository<Carro, Guid>(context);
// Ambos funcionam igual
```

### Validação no Comportamento
```csharp
// Endpoints não sabem qual repository está sendo usado
public class ClienteEndpoints
{
    private static IResult GetByIdEndpoint(
        Guid id,
        IClienteRepository repository) // Interface, não implementação
    {
        var cliente = await repository.GetByIdAsync(id);
        // Funciona com qualquer ClienteRepository válido
    }
}
```

### Benefícios
- **Intercambiabilidade**: Pode trocar implementações facilmente
- **Testabilidade**: Usar mocks que implementam interface
- **Confiabilidade**: Contrato sempre respeitado

---

## **I - Interface Segregation Principle (ISP)**

### Definição
Clientes não devem ser forçados a depender de interfaces que não usam.

### Implementação no Projeto

#### ? Violação (Evitar)
```csharp
// ? MÁ PRÁTICA: Interface genérica para tudo
public interface IRepository
{
    Task<object?> GetByIdAsync(Guid id);
    Task<IEnumerable<object>> GetAllAsync();
    Task AddAsync(object entity);
    Task UpdateAsync(object entity);
    Task DeleteAsync(Guid id);
    Task SaveChangesAsync();
    Task<bool> ExistsAsync(Guid id);
    // ... + 10 métodos que nem todos precisam
}
```

#### ? Solução (Implementado)
```csharp
// ? Interfaces específicas e pequenas
public interface IRepository<TEntity, in TId>
    where TEntity : class
{
    Task<TEntity?> GetByIdAsync(TId id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity> AddAsync(TEntity entity);
    Task<TEntity> UpdateAsync(TEntity entity);
    Task<bool> DeleteAsync(TId id);
    Task<bool> ExistsAsync(TId id);
    Task<int> SaveChangesAsync();
}

// Interface específica do domínio apenas com métodos necessários
public interface IClienteRepository : IRepository<Cliente, Guid>
{
    Task<Cliente?> GetByNomeAsync(string nome);
    Task<Cliente?> GetByTelefoneAsync(string telefone);
    Task<IEnumerable<Cliente>> GetAllWithRelationsAsync();
}

// Implementação implementa apenas o necessário
public class ClienteRepository : Repository<Cliente, Guid>, IClienteRepository
{
    public async Task<Cliente?> GetByNomeAsync(string nome) { ... }
    public async Task<Cliente?> GetByTelefoneAsync(string telefone) { ... }
    public async Task<IEnumerable<Cliente>> GetAllWithRelationsAsync() { ... }
}
```

### Segregação de DTOs
```csharp
// DTOs diferentes para diferentes casos de uso
public record CreateClienteDto(string Nome, string Telefone, string Endereco);
public record UpdateClienteDto(Guid Id, string Nome, string Telefone, string Endereco);
public record ClienteResponseDto(Guid Id, string Nome, string Telefone, string Endereco, ...);

// Cada um com apenas os campos necessários
```

### Benefícios
- **Flexibilidade**: Implementações não precisam de tudo
- **Clareza**: Código documenta exatamente o que precisa
- **Mudança de Escopo**: Fácil adicionar novos métodos sem quebrar clientes

---

## **D - Dependency Inversion Principle (DIP)**

### Definição
Módulos de alto nível não devem depender de módulos de baixo nível. Ambos devem depender de abstrações.

### Implementação no Projeto

#### ? Violação (Evitar)
```csharp
// ? MÁ PRÁTICA: Handler depende de implementação concreta
public class CreateClienteCommandHandler
{
    private ClienteRepository _repository; // Implementação concreta
    
    public CreateClienteCommandHandler()
    {
        _repository = new ClienteRepository(new OficinaDbContext());
    }
    
    public async Task<ClienteResponseDto> Handle(CreateClienteCommand command)
    {
        var cliente = new Cliente { Nome = command.Nome };
        _repository.Add(cliente);
        _repository.SaveChanges();
        return new ClienteResponseDto(...);
    }
}
```

#### ? Solução (Implementado)
```csharp
// ? Handler depende de abstrações (interfaces)
public class CreateClienteCommandHandler
{
    private readonly IClienteRepository _repository; // Interface
    private readonly ClienteMapper _mapper; // Injetada
    
    // Construtor recebe dependências
    public CreateClienteCommandHandler(
        IClienteRepository repository,
        ClienteMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    
    public async Task<ClienteResponseDto> Handle(CreateClienteCommand command)
    {
        var cliente = _mapper.ToEntity(command);
        var clienteAdicionado = await _repository.AddAsync(cliente);
        await _repository.SaveChangesAsync();
        return _mapper.ToResponseDto(clienteAdicionado);
    }
}

// Injeção de dependência no Program.cs
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<ClienteMapper>();

// Para testes, injetar mock
var mockRepository = new Mock<IClienteRepository>();
var handler = new CreateClienteCommandHandler(
    mockRepository.Object,
    new ClienteMapper());
```

### Wolverine como IoC Container
```csharp
// Wolverine descobre automaticamente dependências
builder.Host.UseWolverine(options =>
{
    options.Discovery.IncludeAssemblies(
        typeof(CreateClienteCommand).Assembly);
});

// Handlers são descobertos e injetados automaticamente
// Não precisa registrar cada um manualmente
```

### Benefícios
- **Testabilidade**: Usar mocks em testes
- **Flexibilidade**: Trocar implementação sem alterar handler
- **Desacoplamento**: Handler não conhece detalhes de implementação
- **Manutenção**: Mudanças centralizadas no DI

---

## ?? Checklist SOLID no Código

### Single Responsibility
- [ ] Cada classe tem uma razão para mudar?
- [ ] Arquivo tem propósito claro?
- [ ] Não mistura conceitos diferentes?

### Open/Closed
- [ ] Novo handler não quebra código existente?
- [ ] Genéricos permitem extensão?
- [ ] Usa interfaces para flexibilidade?

### Liskov Substitution
- [ ] Implementação respeita contrato da interface?
- [ ] Pode substituir por outra implementação?
- [ ] Não lança exceções inesperadas?

### Interface Segregation
- [ ] Interfaces são pequenas e focadas?
- [ ] Cliente não depende de método que não usa?
- [ ] DTOs têm apenas campos necessários?

### Dependency Inversion
- [ ] Depende de interfaces, não implementações?
- [ ] Dependências são injetadas?
- [ ] Fácil fazer testes com mocks?

---

## ?? Referências

- Robert C. Martin - "Clean Architecture"
- [Microsoft - SOLID Principles](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/architectural-principles)
- Design Patterns Gang of Four

---

**Aplicação consistente de SOLID resulta em código mais limpo, testável e manutenível.**
