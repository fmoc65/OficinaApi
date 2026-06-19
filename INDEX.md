# ?? Índice Completo - OficinaApi

## ?? Documentação Principal (Leia Nessa Ordem)

| Arquivo | Descrição | Tempo |
|---------|-----------|-------|
| **QUICK-START.md** | ? Comece aqui - 5 min para rodar a API | 5 min |
| **README.md** | ?? Documentação completa e visão geral | 15 min |
| **ARCHITECTURE.md** | ??? Diagramas de arquitetura e fluxos | 10 min |
| **DEVELOPMENT.md** | ??? Guia de desenvolvimento e extensão | 15 min |
| **SOLID-PRINCIPLES.md** | ?? Explicação dos 5 princípios SOLID | 10 min |
| **PROJECT-SUMMARY.md** | ?? Sumário técnico do projeto | 10 min |

---

## ?? Leitura Recomendada por Perfil

### ????? Desenvolvedor Novo no Projeto
1. **QUICK-START.md** - Get running
2. **README.md** - Understand structure
3. **DEVELOPMENT.md** - Learn how to extend
4. **SOLID-PRINCIPLES.md** - Understand patterns

### ??? Arquiteto / Tech Lead
1. **ARCHITECTURE.md** - Visão completa
2. **SOLID-PRINCIPLES.md** - Validar padrões
3. **PROJECT-SUMMARY.md** - Estatísticas
4. **README.md** - Detalhes técnicos

### ?? QA / Testador
1. **QUICK-START.md** - Como rodar
2. **README.md** - Endpoints
3. **DEVELOPMENT.md** - Casos de teste

### ?? Product Manager
1. **README.md** - Features
2. **PROJECT-SUMMARY.md** - Capacidades

---

## ?? Estrutura de Projetos

### OficinaApi.sln
Solução Visual Studio conectando 4 projetos.

```
OficinaApi/
??? src/
?   ??? OficinaApi.Domain/
?   ??? OficinaApi.Application/
?   ??? OficinaApi.Infrastructure/
?   ??? OficinaApi.Api/
??? Documentação/
    ??? README.md
    ??? QUICK-START.md
    ??? DEVELOPMENT.md
    ??? ARCHITECTURE.md
    ??? SOLID-PRINCIPLES.md
    ??? PROJECT-SUMMARY.md
```

---

## ?? Domain Layer (Entidades)

### BaseEntity.cs
```
Classe base para todas as entidades
?? Id (Guid) - Identificador único
?? DataCriacao (DateTime) - Quando foi criado
?? DataAtualizacao (DateTime?) - Última modificação
```

### Cliente.cs
```
Entidade de cliente da oficina
?? Nome (string) - Nome do cliente
?? Telefone (string) - Contato telefônico
?? Endereco (string) - Endereço residencial
?? Carros (ICollection<Carro>) - Veículos do cliente
?? OrdensServico (ICollection<OrdenServico>) - Serviços realizados
```

### Carro.cs
```
Entidade de veículo
?? Modelo (string) - Marca e modelo
?? Ano (int) - Ano de fabricação
?? IdCliente (Guid FK) - Cliente proprietário
?? Cliente (Cliente) - Navegação para cliente
?? Pecas (ICollection<Peca>) - Peças cadastradas
?? OrdensServico (ICollection<OrdenServico>) - Serviços realizados
```

### Peca.cs
```
Entidade de peça de reposição
?? IdPeca (string) - Código da peça
?? Quantidade (int) - Estoque disponível
?? Valor (decimal) - Preço unitário
?? IdCarro (Guid FK) - Carro que possui
?? Carro (Carro) - Navegação para carro
?? OrdensServicoPecas (ICollection<OrdenServicoPeca>) - Ordens que usaram
```

### OrdenServico.cs
```
Entidade de ordem de serviço/manutenção
?? Servicos (string) - Descrição dos serviços
?? Status (string) - "Aberta", "Em Andamento", "Concluída", "Cancelada"
?? ValorTotal (decimal) - Custo total
?? DataOrdem (DateTime) - Data de criação
?? IdCarro (Guid FK) - Carro relacionado
?? IdCliente (Guid FK) - Cliente relacionado
?? Carro (Carro) - Navegação para carro
?? Cliente (Cliente) - Navegação para cliente
?? PecasUtilizadas (ICollection<OrdenServicoPeca>) - Peças usadas (N:N)
```

### OrdenServicoPeca.cs
```
Tabela junção para relacionamento N:N
?? IdOrdenServico (Guid FK) - Ordem de serviço
?? IdPeca (Guid FK) - Peça utilizada
?? Quantidade (int) - Quantidade usada
?? ValorUnitario (decimal) - Preço no momento do serviço
?? OrdenServico (OrdenServico) - Navegação
?? Peca (Peca) - Navegação
```

---

## ?? Application Layer (CQRS + Features)

### Features/Clientes/

#### DTOs
```
CreateClienteDto
?? Nome (string)
?? Telefone (string)
?? Endereco (string)

UpdateClienteDto
?? Id (Guid)
?? Nome (string)
?? Telefone (string)
?? Endereco (string)

ClienteResponseDto
?? Id (Guid)
?? Nome (string)
?? Telefone (string)
?? Endereco (string)
?? DataCriacao (DateTime)
?? DataAtualizacao (DateTime?)
```

#### Commands
```
CreateClienteCommand : ICommand
?? Nome (string)
?? Telefone (string)
?? Endereco (string)

UpdateClienteCommand : ICommand
?? Id (Guid)
?? Nome (string)
?? Telefone (string)
?? Endereco (string)

DeleteClienteCommand : ICommand
?? Id (Guid)
```

#### Validators
```
CreateClienteValidator
?? Nome: 3-150 caracteres, obrigatório
?? Telefone: Formato brasileiro (11) 98765-4321
?? Endereco: 5-255 caracteres, obrigatório

UpdateClienteValidator
?? Mesmas regras + Id obrigatório

DeleteClienteValidator
?? Id obrigatório, must exist
```

#### Mappers
```
ClienteMapper
?? ToEntity(CreateClienteCommand) ? Cliente
?? ToEntity(UpdateClienteCommand) ? Cliente
?? ToResponseDto(Cliente) ? ClienteResponseDto
?? [Mapper] attribute do Maperly (compile-time)
```

#### Handlers
```
CreateClienteCommandHandler
?? Recebe CreateClienteCommand (validado)
?? Mapeia para Cliente
?? Persiste com repository.AddAsync()
?? Retorna ClienteResponseDto

UpdateClienteCommandHandler
?? Carrega cliente existente
?? Atualiza propriedades
?? Persiste mudanças
?? Retorna ClienteResponseDto

DeleteClienteCommandHandler
?? Localiza cliente
?? Deleta do banco
?? Retorna confirmação
```

### Features/Carros/Pecas/OrdenServicos/
Mesma estrutura replicada para cada feature:
- DTOs (Create, Update, Response)
- Commands (Create, Update, Delete)
- Validators (3 validators)
- Mappers (Maperly)
- Handlers (CQRS handlers)

---

## ?? Infrastructure Layer (Data Access)

### OficinaDbContext.cs
```
Configuração do Entity Framework Core

DbSets:
?? DbSet<Cliente>
?? DbSet<Carro>
?? DbSet<Peca>
?? DbSet<OrdenServico>
?? DbSet<OrdenServicoPeca>

OnModelCreating():
?? Primary Keys
?? Foreign Keys com DeleteBehavior
?? Unique Constraints
?? Indices (Nome, Status, DataOrdem)
?? Default Values (GETUTCDATE())
?? Column Constraints

Connection String:
?? Server=(localdb)\mssqllocaldb;Database=OficinaDb;
```

### IRepository<TEntity, TId>
```
Interface genérica de repository

Métodos:
?? Task<TEntity?> GetByIdAsync(TId id)
?? Task<IEnumerable<TEntity>> GetAllAsync()
?? Task<TEntity> AddAsync(TEntity entity)
?? Task<TEntity> UpdateAsync(TEntity entity)
?? Task<bool> DeleteAsync(TId id)
?? Task<bool> ExistsAsync(TId id)
?? Task<int> SaveChangesAsync()
```

### Repository<TEntity, TId>
```
Implementação genérica

Protected Fields:
?? OficinaDbContext _context
?? DbSet<TEntity> _dbSet

Features:
?? Async/await throughout
?? AsNoTracking para queries de leitura
?? FindAsync para lookups por PK
?? Reutilizável para qualquer entidade
```

### IClienteRepository
```
Interface específica do domínio

Métodos adicionais:
?? Task<Cliente?> GetByNomeAsync(string nome)
?? Task<Cliente?> GetByTelefoneAsync(string telefone)
?? Task<IEnumerable<Cliente>> GetAllWithRelationsAsync()

Extends: IRepository<Cliente, Guid>
```

### ClienteRepository
```
Implementação específica de Cliente

Métodos:
?? GetByNomeAsync() - Busca por nome
?? GetByTelefoneAsync() - Previne duplicatas
?? GetAllWithRelationsAsync() - Eager loading (Include)

Extends: Repository<Cliente, Guid>
Implements: IClienteRepository
```

---

## ?? API Layer (Endpoints)

### Program.cs
```
Startup and Configuration (300+ linhas)

1. Logging with Serilog
   ?? File: c:\logs\OficinaApi\oficina-api-YYYY-MM-DD.txt

2. Services Registration
   ?? DbContext com SQL Server
   ?? Repositories (Generic + Specific)
   ?? Mappers (Maperly)
   ?? FluentValidation

3. Wolverine CQRS
   ?? Auto-discovery de handlers
   ?? ValidationMiddleware
   ?? Message bus configuration

4. Swagger/OpenAPI
   ?? XML documentation
   ?? API versioning
   ?? Auth schemes (futuro)

5. Middlewares
   ?? CORS (AllowAll - dev)
   ?? HTTPS redirect
   ?? Error handling
   ?? Logging

6. Database
   ?? Auto-apply migrations
   ?? Seed data (se configurado)

7. Endpoints
   ?? MapClienteEndpoints()
   ?? MapCarroEndpoints()
   ?? MapPecaEndpoints()
   ?? MapOrdenServicoEndpoints()
```

### ClienteEndpoints.cs
```
Minimal API Endpoints (400+ linhas)

POST /api/clientes
?? CreateClienteEndpoint()
   ?? Recebe CreateClienteDto
   ?? Cria CreateClienteCommand
   ?? Despacha via Wolverine
   ?? Retorna 201 Created

GET /api/clientes
?? GetAllClientesEndpoint()
   ?? Busca todos os clientes
   ?? Retorna 200 OK

GET /api/clientes/{id}
?? GetClienteByIdEndpoint()
   ?? Busca cliente por ID
   ?? 200 OK se encontrado
   ?? 404 Not Found se não existe

PUT /api/clientes/{id}
?? UpdateClienteEndpoint()
   ?? Atualiza cliente
   ?? 200 OK se sucesso
   ?? 404 Not Found se não existe

DELETE /api/clientes/{id}
?? DeleteClienteEndpoint()
   ?? Deleta cliente
   ?? 204 No Content se sucesso
   ?? 404 Not Found se não existe

Features:
?? Route groups para /api/clientes
?? Swagger metadata (.WithOpenApi())
?? Error handling com try-catch
?? Logging (Info, Warning, Error)
?? Dependency injection em parâmetros
?? Structured error responses
```

### CarroEndpoints.cs / PecaEndpoints.cs / OrdenServicoEndpoints.cs
Mesma estrutura com 5 endpoints cada (POST, GET all, GET by id, PUT, DELETE).

### appsettings.json
```
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;..."
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

### appsettings.Development.json
```
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  }
}
```

### launchSettings.json
```
Perfis de execução:
?? http (porta 5000)
?? https (porta 7000)
?? IIS Express

LaunchBrowser: true
LaunchUrl: swagger
```

---

## ?? Configuração e Build

### OficinaApi.sln
```
Visual Studio Solution
Conecta 4 projetos:
?? OficinaApi.Domain
?? OficinaApi.Application
?? OficinaApi.Infrastructure
?? OficinaApi.Api
```

### .gitignore
```
Arquivos a ignorar no repositório:
?? bin/, obj/
?? *.user
?? appsettings.Development.json
?? launchSettings.json
?? .vs/
?? logs/
```

### global.json
```
{
  "sdk": {
    "version": "10.0.0"
  }
}
```

---

## ?? Resumo de Arquivos

### Contagem
- **Projetos**: 4
- **Arquivos de código**: 40+
- **Documentação**: 6 arquivos
- **Configuração**: 5 arquivos
- **Total**: 51+ arquivos

### Linhas de Código
- **Domain**: ~400 linhas
- **Application**: ~2500 linhas
- **Infrastructure**: ~800 linhas
- **API**: ~1500 linhas
- **Documentação**: ~1500 linhas
- **Total**: ~6700 linhas

### Endpoints
- **Total**: 20 endpoints
- **Por entidade**: 5 (POST, GET all, GET by id, PUT, DELETE)
- **Features**: 4 (Clientes, Carros, Peças, Ordens)

---

## ?? Como Usar Este Índice

### Para Encontrar um Arquivo Específico
1. Comece aqui
2. Localize a seção (Domain, Application, etc.)
3. Encontre o arquivo na tabela
4. Abra o arquivo para ver comentários

### Para Entender a Arquitetura
1. Leia ARCHITECTURE.md (diagramas)
2. Leia este índice (estrutura)
3. Navegue pelos arquivos (código)

### Para Modificar o Código
1. Entenda a feature desejada
2. Localize na seção Application
3. Copie o padrão dos DTOs, Commands, Handlers
4. Adicione em Program.cs

---

## ?? Dúvidas Frequentes

**P: Por onde começar?**
R: QUICK-START.md

**P: Como é a arquitetura?**
R: ARCHITECTURE.md

**P: Como adicionar nova feature?**
R: DEVELOPMENT.md

**P: Como funcionam os padrões?**
R: SOLID-PRINCIPLES.md

**P: Qual é a estrutura completa?**
R: PROJECT-SUMMARY.md

---

## ? Checklist de Exploração

- [ ] Li QUICK-START.md
- [ ] Rodei a API localmente
- [ ] Acessei Swagger em http://localhost:5000/swagger
- [ ] Li README.md
- [ ] Entendi ARCHITECTURE.md
- [ ] Estudei SOLID-PRINCIPLES.md
- [ ] Li DEVELOPMENT.md
- [ ] Explorei o código da feature Clientes
- [ ] Entendo como adicionar uma nova feature
- [ ] Confortável com a arquitetura

---

**?? Bem-vindo ao OficinaApi!**
**Você tem uma API enterprise-grade com melhor prática.**
**Divirta-se desenvolvendo! ??**
