# OficinaApi - API de Gerenciamento de Oficina Automotiva

## ?? Visão Geral

**OficinaApi** é uma API RESTful desenvolvida em **.NET 10** com arquitetura moderna baseada em **Slice Architecture**, implementando os melhores padrões de design e princípios SOLID.

### Arquitetura e Padrões Implementados

- **Slice Architecture**: Cada feature (Clientes, Carros, Peças, Ordens) é organizada em slices independentes
- **CQRS (Command Query Responsibility Segregation)**: Separação clara entre operações de leitura e escrita com Wolverine
- **Repository Pattern**: Abstração de acesso ao banco de dados
- **Minimal APIs**: Endpoints sem necessidade de controllers, declarados em arquivos separados
- **SOLID Principles**: 
  - **S**ingle Responsibility: Cada classe tem uma responsabilidade única
  - **O**pen/Closed: Aberto para extensão, fechado para modificação
  - **L**iskov Substitution: Subtipos podem substituir tipos base
  - **I**nterface Segregation: Interfaces específicas, não genéricas
  - **D**ependency Inversion: Depender de abstrações, não implementações
- **DRY (Don't Repeat Yourself)**: Código reutilizável através de genéricos e herança
- **FluentValidation**: Validações centralizadas e reutilizáveis
- **Maperly**: Mapeamento automático otimizado em tempo de compilação
- **Serilog**: Logging estruturado com gravação em arquivo

## ??? Estrutura do Projeto

```
OficinaApi/
??? src/
?   ??? OficinaApi.Api/                 # Camada de apresentação (Minimal APIs)
?   ?   ??? Endpoints/                  # Endpoints separados por feature
?   ?   ?   ??? ClienteEndpoints.cs
?   ?   ?   ??? CarroEndpoints.cs
?   ?   ?   ??? PecaEndpoints.cs
?   ?   ?   ??? OrdenServicoEndpoints.cs
?   ?   ??? Program.cs                  # Configuração e startup
?   ?   ??? appsettings.json            # Configurações
?   ?
?   ??? OficinaApi.Application/         # Camada de aplicação
?   ?   ??? Features/                   # Organização por feature/slice
?   ?       ??? Clientes/
?   ?       ?   ??? Commands/           # Commands CQRS
?   ?       ?   ??? DTOs/               # Data Transfer Objects
?   ?       ?   ??? Handlers/           # Processadores de commands
?   ?       ?   ??? Mappers/            # Mapeamento com Maperly
?   ?       ?   ??? Validators/         # Validações com FluentValidation
?   ?       ??? Carros/                 # Mesma estrutura para Carros
?   ?       ??? Pecas/                  # Mesma estrutura para Peças
?   ?       ??? OrdenServicos/          # Mesma estrutura para Ordens
?   ?
?   ??? OficinaApi.Domain/              # Camada de domínio (Entidades)
?   ?   ??? Entities/                   # Modelos de domínio
?   ?   ?   ??? Cliente.cs
?   ?   ?   ??? Carro.cs
?   ?   ?   ??? Peca.cs
?   ?   ?   ??? OrdenServico.cs
?   ?   ?   ??? OrdenServicoPeca.cs     # Tabela de junção N:N
?   ?   ??? Common/
?   ?       ??? BaseEntity.cs           # Classe base para entidades
?   ?
?   ??? OficinaApi.Infrastructure/      # Camada de infraestrutura
?       ??? Data/
?       ?   ??? OficinaDbContext.cs     # Contexto EF Core
?       ??? Repositories/
?           ??? IRepository.cs          # Interface genérica
?           ??? Repository.cs           # Implementação genérica
?           ??? IClienteRepository.cs   # Interface específica Cliente
?           ??? ClienteRepository.cs    # Implementação Cliente
?
??? OficinaApi.sln                      # Solução Visual Studio
```

## ??? Modelo de Dados

### Entidades

#### **Cliente**
```
- Id (Guid) - PK
- Nome (string) - Obrigatório
- Telefone (string) - Obrigatório
- Endereco (string) - Obrigatório
- DataCriacao (DateTime)
- DataAtualizacao (DateTime?)
- Relacionamentos: 1 Cliente ? N Carros, 1 Cliente ? N OrdensServico
```

#### **Carro**
```
- Id (Guid) - PK
- Modelo (string) - Obrigatório
- Ano (int) - Obrigatório
- IdCliente (Guid) - FK
- DataCriacao (DateTime)
- DataAtualizacao (DateTime?)
- Relacionamentos: N Carros ? 1 Cliente, 1 Carro ? N Pecas, 1 Carro ? N OrdensServico
```

#### **Peça**
```
- Id (Guid) - PK
- IdPeca (string) - Código único da peça
- IdCarro (Guid) - FK
- Quantidade (int) - Estoque
- Valor (decimal) - Preço unitário
- DataCriacao (DateTime)
- DataAtualizacao (DateTime?)
- Relacionamentos: N Pecas ? 1 Carro, N Pecas ? N OrdensServico (via OrdenServicoPeca)
```

#### **OrdenServico**
```
- Id (Guid) - PK
- IdCarro (Guid) - FK
- IdCliente (Guid) - FK
- Servicos (string) - Descrição dos serviços
- ValorTotal (decimal) - Total da ordem
- DataOrdem (DateTime) - Data do serviço
- Status (string) - Aberta, Em Andamento, Concluída, Cancelada
- DataCriacao (DateTime)
- DataAtualizacao (DateTime?)
- Relacionamentos: N OrdensServico ? 1 Carro, N OrdensServico ? 1 Cliente, N OrdensServico ? N Pecas (via OrdenServicoPeca)
```

#### **OrdenServicoPeca** (Tabela de Junção N:N)
```
- Id (Guid) - PK
- IdOrdenServico (Guid) - FK
- IdPeca (Guid) - FK
- Quantidade (int) - Qtd utilizada
- ValorUnitario (decimal) - Valor no momento da ordem
- DataCriacao (DateTime)
- Relacionamentos: N OrdensServicoPecas ? 1 OrdenServico, N OrdensServicoPecas ? 1 Peca
```

## ?? Como Iniciar

### Pré-requisitos
- .NET 10 SDK instalado
- SQL Server LocalDB ou SQL Server
- Visual Studio 2022+ ou VS Code com C# Dev Kit

### Configuração

1. **Abrir a solução**
   ```bash
   cd d:\Projetos\Services\Oficina\OficinaApi
   dotnet build
   ```

2. **Atualizar string de conexão** (appsettings.json)
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=OficinaDb;Trusted_Connection=true;"
   }
   ```

3. **Aplicar Migrations**
   ```bash
   cd src/OficinaApi.Api
   dotnet ef database update
   ```

4. **Executar a API**
   ```bash
   dotnet run
   ```

A API estará disponível em: `https://localhost:7000` ou `http://localhost:5000`

## ?? Documentação da API

A API é documentada automaticamente via **Swagger/OpenAPI**.

### Acessar Swagger
- **URL**: `http://localhost:5000/swagger`
- Interface interativa para testar todos os endpoints

### Endpoints Principais

#### **Clientes**
- `POST /api/clientes` - Criar cliente
- `GET /api/clientes` - Listar todos
- `GET /api/clientes/{id}` - Obter por ID
- `PUT /api/clientes/{id}` - Atualizar
- `DELETE /api/clientes/{id}` - Deletar

#### **Carros**
- `POST /api/carros` - Criar carro
- `GET /api/carros` - Listar todos
- `GET /api/carros/{id}` - Obter por ID
- `PUT /api/carros/{id}` - Atualizar
- `DELETE /api/carros/{id}` - Deletar

#### **Peças**
- `POST /api/pecas` - Criar peça
- `GET /api/pecas` - Listar todas
- `GET /api/pecas/{id}` - Obter por ID
- `PUT /api/pecas/{id}` - Atualizar
- `DELETE /api/pecas/{id}` - Deletar

#### **Ordens de Serviço**
- `POST /api/ordens-servico` - Criar ordem
- `GET /api/ordens-servico` - Listar todas
- `GET /api/ordens-servico/{id}` - Obter por ID
- `PUT /api/ordens-servico/{id}` - Atualizar
- `DELETE /api/ordens-servico/{id}` - Deletar

## ?? Exemplos de Uso

### Criar um Cliente
```bash
curl -X POST http://localhost:5000/api/clientes \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "João Silva",
    "telefone": "(11)98765-4321",
    "endereco": "Rua das Flores, 123"
  }'
```

### Criar um Carro
```bash
curl -X POST http://localhost:5000/api/carros \
  -H "Content-Type: application/json" \
  -d '{
    "modelo": "Honda Civic",
    "ano": 2020,
    "idCliente": "00000000-0000-0000-0000-000000000000"
  }'
```

### Criar uma Ordem de Serviço
```bash
curl -X POST http://localhost:5000/api/ordens-servico \
  -H "Content-Type: application/json" \
  -d '{
    "idCarro": "00000000-0000-0000-0000-000000000001",
    "idCliente": "00000000-0000-0000-0000-000000000000",
    "servicos": "Troca de óleo e filtro",
    "pecas": [
      {
        "idPeca": "00000000-0000-0000-0000-000000000002",
        "quantidade": 1,
        "valorUnitario": 45.00
      }
    ]
  }'
```

## ?? Logging

### Configuração Serilog

Logs são gravados em:
- **Console** (durante execução)
- **Arquivo**: `c:\logs\OficinaApi\oficina-api-YYYY-MM-DD.txt`

### Níveis de Log
- `Information` - Operações normais
- `Warning` - Situações incomuns (entidade não encontrada, validação falhou)
- `Error` - Erros operacionais
- `Fatal` - Erros críticos

### Exemplo de Log
```
2026-06-19 10:15:30.123 +00:00 [INF] Iniciando criação de novo cliente: {@Cliente}
2026-06-19 10:15:31.456 +00:00 [INF] Cliente criado com sucesso. ID: a1b2c3d4-e5f6-47a8-b9c0-d1e2f3a4b5c6
```

## ??? Tecnologias Utilizadas

| Componente | Tecnologia | Versão | Objetivo |
|-----------|-----------|--------|---------|
| Framework | .NET | 10.0 | Plataforma principal |
| Web API | ASP.NET Core | 10.0 | Framework para APIs |
| ORM | Entity Framework Core | 10.0 | Acesso ao banco de dados |
| Banco | SQL Server | LocalDB | Persistência de dados |
| CQRS | Wolverine | 4.1.0 | Padrão CQRS e message bus |
| Mapeamento | Maperly | 3.8.0 | Mapeamento de DTOs |
| Validação | FluentValidation | 11.9.2 | Validação de dados |
| Documentação | Swagger | 6.5.0 | Documentação interativa |
| Logging | Serilog | 8.1.0 | Logging estruturado |

## ? Validações Implementadas

### Cliente
- Nome: Obrigatório, 3-150 caracteres
- Telefone: Obrigatório, formato brasileiro válido
- Endereço: Obrigatório, 5-255 caracteres

### Carro
- Modelo: Obrigatório, 2-100 caracteres
- Ano: Maior que 1900, não futuro
- IdCliente: Obrigatório

### Peça
- IdPeca: Obrigatório, máx 50 caracteres
- Quantidade: Maior que zero
- Valor: Maior que zero
- IdCarro: Obrigatório

### Ordem de Serviço
- Serviços: Obrigatório, 5-500 caracteres
- Status: Uma dos valores válidos
- Pecas: Mínimo 1 peça
- IdCarro e IdCliente: Obrigatórios

## ?? Tratamento de Erros

Todos os endpoints retornam respostas padronizadas:

### Sucesso (200, 201)
```json
{
  "id": "...",
  "nome": "...",
  ...
}
```

### Erro de Validação (400)
```json
{
  "message": "Dados inválidos",
  "errors": {
    "Nome": ["Nome é obrigatório"],
    "Telefone": ["Telefone deve estar em um formato válido"]
  }
}
```

### Não Encontrado (404)
```json
{
  "message": "Cliente não encontrado"
}
```

### Erro Interno (500)
```json
{
  "message": "Erro ao processar requisição"
}
```

## ?? Padrões de Design Aplicados

### 1. **Repository Pattern**
Abstrai acesso ao banco via `IRepository<T, TId>` genérico e `IClienteRepository` específico.

### 2. **CQRS (Command Query Responsibility Segregation)**
- **Commands**: Operações que modificam estado (Create, Update, Delete)
- **Queries**: Operações de leitura (Get, GetAll)
- Implementado com Wolverine para desacoplamento

### 3. **DTO Pattern**
Separação entre modelos de domínio e modelos transferidos via API.

### 4. **Dependency Injection**
Todas as dependências injetadas via construtor.

### 5. **Generic Repository**
`Repository<TEntity, TId>` reutilizável para qualquer entidade.

### 6. **Service Locator via Wolverine**
`IMessageBus` para desacoplar endpoints de handlers.

## ?? Testes (Próximos Passos)

Recomenda-se implementar:
- Testes Unitários com xUnit
- Testes de Integração
- Testes de Validação
- Mock do DbContext

## ?? Convenções de Código

### Nomenclatura
- **Classes**: PascalCase (ex: `ClienteRepository`)
- **Métodos**: PascalCase (ex: `GetByIdAsync`)
- **Variáveis**: camelCase (ex: `cliente`)
- **Constantes**: UPPER_CASE
- **Interfaces**: IPrefix (ex: `IClienteRepository`)

### Assincronismo
Todos os métodos que acessam I/O (banco, rede) são assincronados com `async/await`.

### Comentários
Cada classe, método público e propriedade tem documentação XML com `///` explicando o objetivo.

## ?? Fluxo de uma Requisição

1. **Endpoint recebe requisição** (ClienteEndpoints.CreateClienteEndpoint)
2. **DTO é validado** (CreateClienteDto)
3. **Command é criado** (CreateClienteCommand)
4. **MessageBus despacha** para Wolverine
5. **Validação é executada** (ValidationMiddleware)
6. **Handler processa** (CreateClienteCommandHandler)
7. **Mapper transforma** em entidade (ClienteMapper.ToEntity)
8. **Repository persiste** (IClienteRepository.AddAsync)
9. **Mudanças são salvas** (SaveChangesAsync)
10. **Mapper retorna DTO** (ToResponseDto)
11. **Resposta é retornada** ao cliente

## ?? Suporte

Para dúvidas sobre a arquitetura ou padrões utilizados, consulte os comentários no código.
Cada arquivo contém explicações detalhadas sobre por que cada padrão foi escolhido.

---

**Desenvolvido com ?? usando .NET 10 e Arquitetura Slice**
