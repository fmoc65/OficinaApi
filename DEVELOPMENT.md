## Guia de Desenvolvimento - OficinaApi

### Antes de Começar

1. **Instalar dependências**
   ```bash
   dotnet restore
   ```

2. **Build da solução**
   ```bash
   dotnet build
   ```

### Execução Local

#### Opção 1: Via terminal
```bash
cd src/OficinaApi.Api
dotnet run
```

#### Opção 2: Via Visual Studio
- Abrir `OficinaApi.sln`
- Definir `OficinaApi.Api` como projeto inicial
- Pressionar F5 ou Ctrl+F5

#### Opção 3: Via VS Code
```bash
# Terminal integrado
dotnet watch run
```

### Banco de Dados

#### Criar migrations
```bash
cd src/OficinaApi.Api
dotnet ef migrations add NomeDaMigration -p ../OficinaApi.Infrastructure
```

#### Aplicar migrations
```bash
dotnet ef database update -p ../OficinaApi.Infrastructure
```

#### Remover última migration (se necessário)
```bash
dotnet ef migrations remove
```

### Estrutura de Pastas - Explicação

#### `OficinaApi.Api`
- **Responsabilidade**: Camada de apresentação
- **O que contém**: Endpoints, middlewares, configuração Startup
- **Por que separado**: Facilita trocar framework web sem afetar lógica

#### `OficinaApi.Application`
- **Responsabilidade**: Lógica de aplicação
- **O que contém**: Commands, Handlers, DTOs, Validators, Mappers
- **Por que separado**: Orquestra casos de uso sem conhecer detalhes técnicos

#### `OficinaApi.Domain`
- **Responsabilidade**: Modelos de negócio puros
- **O que contém**: Entidades, interfaces, value objects
- **Por que separado**: Core da aplicação, independente de tecnologia

#### `OficinaApi.Infrastructure`
- **Responsabilidade**: Detalhes técnicos
- **O que contém**: DbContext, Repositories, dados externos
- **Por que separado**: Facilita trocar banco/ORM sem afetar domínio

### Adicionando Nova Feature

Exemplo: Adicionar feature "Agendamentos"

1. **Criar pastas**
   ```
   src/OficinaApi.Application/Features/Agendamentos/
   ?   ??? Commands/
   ?   ??? DTOs/
   ?   ??? Handlers/
   ?   ??? Mappers/
   ?   ??? Validators/
   ```

2. **Criar entidade** em `OficinaApi.Domain/Entities/Agendamento.cs`
   ```csharp
   public class Agendamento : BaseEntity
   {
       // Implementar...
   }
   ```

3. **Criar DTOs**
   ```csharp
   // CreateAgendamentoDto.cs
   public record CreateAgendamentoDto(...);
   ```

4. **Criar Commands**
   ```csharp
   public record CreateAgendamentoCommand(...) : ICommand;
   ```

5. **Criar Validators**
   ```csharp
   public class CreateAgendamentoValidator : AbstractValidator<CreateAgendamentoCommand>
   {
       // Implementar validações...
   }
   ```

6. **Criar Mapper**
   ```csharp
   [Mapper]
   public partial class AgendamentoMapper
   {
       // Maperly gera automaticamente
   }
   ```

7. **Criar Handler**
   ```csharp
   public class CreateAgendamentoCommandHandler
   {
       // Implementar lógica...
   }
   ```

8. **Criar Endpoints**
   ```csharp
   public static class AgendamentoEndpoints
   {
       public static void MapAgendamentoEndpoints(this WebApplication app)
       {
           // Declarar endpoints...
       }
   }
   ```

9. **Registrar em Program.cs**
   ```csharp
   // Mapper
   builder.Services.AddScoped<AgendamentoMapper>();
   
   // Validator
   builder.Services.AddValidatorsFromAssemblyContaining(...);
   
   // Endpoint
   app.MapAgendamentoEndpoints();
   ```

### Padrões a Seguir

#### 1. Repository Pattern
```csharp
// ? Evitar - acesso direto ao DbContext
var cliente = _context.Clientes.FirstOrDefault(c => c.Id == id);

// ? Usar - através de repository
var cliente = await _repository.GetByIdAsync(id);
```

#### 2. Dependency Injection
```csharp
// ? Evitar - criar instâncias manualmente
var repository = new ClienteRepository(context);

// ? Usar - injetar via construtor
public class Handler
{
    public Handler(IClienteRepository repository) { }
}
```

#### 3. Assincronismo
```csharp
// ? Evitar - operações síncronas
var cliente = _repository.GetById(id); // Bloqueia thread

// ? Usar - operações assincronamente
var cliente = await _repository.GetByIdAsync(id); // Libera thread
```

#### 4. Validação
```csharp
// ? Evitar - validação no handler
public void Handle(Command cmd)
{
    if (string.IsNullOrEmpty(cmd.Nome))
        throw new Exception("Nome obrigatório");
}

// ? Usar - validação separada
public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(x => x.Nome).NotEmpty().WithMessage("Nome obrigatório");
    }
}
```

#### 5. Mapeamento
```csharp
// ? Evitar - mapeamento manual
var dto = new ClienteDto
{
    Id = cliente.Id,
    Nome = cliente.Nome,
    // ... mais campos
};

// ? Usar - Maperly automático
var dto = _mapper.ToResponseDto(cliente);
```

### Convenções de Nomenclatura

- **Tabelas no BD**: Plural (Clientes, Carros, Pecas)
- **Classes C#**: Singular (Cliente, Carro, Peca)
- **Métodos async**: Terminar com `Async`
- **DTOs de entrada**: `Create{Entidade}Dto`
- **DTOs de saída**: `{Entidade}ResponseDto`
- **Commands**: `{Acao}{Entidade}Command`
- **Handlers**: `{Acao}{Entidade}CommandHandler`
- **Validators**: `{Acao}{Entidade}Validator`
- **Endpoints**: `{Entidade}Endpoints`

### Debugging

#### Breakpoints
- F9 para adicionar/remover breakpoint
- Shift+F9 para listar breakpoints
- F10 para avançar linha
- F11 para entrar em função
- Ctrl+Shift+F10 para executar até cursor

#### Watch
- Adicione variáveis em "Watch" para monitorar valores
- Especialmente útil em loops e operações complexas

#### Logs
- Ver logs em tempo real: `dotnet run --configuration Debug`
- Verificar arquivo: `c:\logs\OficinaApi\`

### Performance

#### Problemas Comuns

1. **N+1 Queries**
   ```csharp
   // ? Problema - múltiplas queries
   var clientes = await _repository.GetAllAsync();
   foreach (var cliente in clientes)
   {
       var carros = await _context.Carros
           .Where(c => c.IdCliente == cliente.Id)
           .ToListAsync();
   }

   // ? Solução - eager loading
   var clientes = await _context.Clientes
       .Include(c => c.Carros)
       .ToListAsync();
   ```

2. **Tracking desnecessário**
   ```csharp
   // ? Problema - rastreamento sem modificação
   var clientes = await _context.Clientes.ToListAsync();

   // ? Solução - desativar tracking
   var clientes = await _context.Clientes
       .AsNoTracking()
       .ToListAsync();
   ```

3. **Índices no Banco**
   - Campos de busca devem ter índice
   - Verificado em `OficinaDbContext.OnModelCreating`

### Versionamento de API

Quando alterar contratos:
1. Criar novo DTO v2
2. Criar novo endpoint com versão
3. Manter compatibilidade com versão antiga

### Deployment

#### Build para produção
```bash
dotnet publish -c Release -o ./publish
```

#### Gerar Migration Script SQL
```bash
dotnet ef migrations script > migration.sql
```

### Troubleshooting

#### Erro: "The model backing the 'OficinaDbContext' context has changed"
**Solução**: Remover e recriar migrations ou deletar banco local

#### Erro: "No parameterless constructor"
**Solução**: Adicionar construtor sem parâmetros ou registrar no DI

#### Erro: "Invalid column name"
**Solução**: Verificar migration, possível desincronização entre código e BD

---

**Dúvidas? Consulte README.md para visão geral da arquitetura.**
