# ?? Quick Start Guide - OficinaApi

## ? Iniciar em 5 minutos

### 1. Pré-requisitos
```
? .NET 10 SDK instalado
? SQL Server LocalDB (já vem com Visual Studio)
? Visual Studio 2022+ OU VS Code + C# Dev Kit
```

### 2. Restaurar pacotes
```bash
cd d:\Projetos\Services\Oficina\OficinaApi
dotnet restore
```

### 3. Executar a API
```bash
cd src/OficinaApi.Api
dotnet run
```

### 4. Acessar Swagger
```
http://localhost:5000/swagger
```

---

## ?? Estrutura Rápida

```
OficinaApi/
??? src/
?   ??? OficinaApi.Api/           ? Endpoints aqui
?   ??? OficinaApi.Application/   ? Lógica de negócio
?   ??? OficinaApi.Infrastructure/? Banco de dados
?   ??? OficinaApi.Domain/        ? Entidades
??? README.md                      ? Documentação completa
??? DEVELOPMENT.md                 ? Guia de desenvolvimento
??? SOLID-PRINCIPLES.md           ? Princípios SOLID explicados
```

---

## ?? Fluxo de uma Requisição

```
1. Cliente ? POST /api/clientes
2. ClienteEndpoints ? CreateClienteEndpoint() recebe CreateClienteDto
3. Mapeia para CreateClienteCommand
4. Wolverine ? IMessageBus despacha para handler
5. FluentValidation ? CreateClienteValidator valida
6. CreateClienteCommandHandler ? Processa
7. ClienteMapper ? Mapeia para entidade
8. ClienteRepository ? Persiste no banco
9. Retorna ClienteResponseDto ? JSON para cliente
```

---

## ?? Exemplo de Requisição

### Criar Cliente
```bash
curl -X POST http://localhost:5000/api/clientes \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "João Silva",
    "telefone": "(11)98765-4321",
    "endereco": "Rua das Flores, 123"
  }'
```

### Resposta
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "nome": "João Silva",
  "telefone": "(11)98765-4321",
  "endereco": "Rua das Flores, 123",
  "dataCriacao": "2026-06-19T10:15:30.123Z",
  "dataAtualizacao": null
}
```

---

## ??? Banco de Dados

### String de Conexão (appsettings.json)
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=OficinaDb;Trusted_Connection=true;"
}
```

### Criar Banco pela primeira vez
```bash
cd src/OficinaApi.Api
dotnet ef database update
```

O banco será criado automaticamente com todas as tabelas.

---

## ?? Entidades

### 4 Entidades Principais
1. **Cliente** - Clientes da oficina
2. **Carro** - Veículos cadastrados
3. **Peça** - Peças de reposição
4. **OrdenServico** - Serviços realizados

### 20 Endpoints (5 por entidade)
- `POST /api/{entidade}` - Criar
- `GET /api/{entidade}` - Listar
- `GET /api/{entidade}/{id}` - Detalhes
- `PUT /api/{entidade}/{id}` - Atualizar
- `DELETE /api/{entidade}/{id}` - Deletar

---

## ?? Logs

### Localização
```
c:\logs\OficinaApi\oficina-api-2026-06-19.txt
```

### Formato
```
2026-06-19 10:15:30.123 +00:00 [INF] Iniciando OficinaApi
2026-06-19 10:15:31.456 +00:00 [INF] Cliente criado com sucesso
```

---

## ??? Adicionar Nova Feature

### Exemplo: Criar feature "Agendamentos"

1. **Criar pasta**
   ```
   src/OficinaApi.Application/Features/Agendamentos/
   ```

2. **Criar entidade**
   ```csharp
   // Domain/Entities/Agendamento.cs
   public class Agendamento : BaseEntity
   {
       public DateTime Data { get; set; }
       public Guid IdCliente { get; set; }
       // ...
   }
   ```

3. **Copiar padrão de Clientes**
   - DTOs
   - Commands
   - Validators
   - Mapper
   - Handlers
   - Endpoints

4. **Registrar em Program.cs**
   ```csharp
   builder.Services.AddScoped<AgendamentoMapper>();
   app.MapAgendamentoEndpoints();
   ```

5. **Resultado**: Nova feature completa com validação, logging, etc.

---

## ? Checklist de Desenvolvimento

Ao adicionar nova feature:
- [ ] Criar entidade em Domain
- [ ] Criar DTOs com records
- [ ] Criar Commands (Create, Update, Delete)
- [ ] Criar Validators (FluentValidation)
- [ ] Criar Mapper (Maperly)
- [ ] Criar Handlers (CQRS)
- [ ] Criar Endpoints (Minimal API)
- [ ] Registrar em Program.cs
- [ ] Adicionar comentários explicativos
- [ ] Testar via Swagger

---

## ?? Troubleshooting

### Erro: "Database does not exist"
```bash
cd src/OficinaApi.Api
dotnet ef database update
```

### Erro: ".NET 10 not found"
```bash
dotnet --version
# Se versão < 10, instale do site
```

### Erro: "Port 5000 already in use"
Mudar em `launchSettings.json`:
```json
"applicationUrl": "http://localhost:5001"
```

### Logs não aparecem
Verificar:
1. Pasta `c:\logs\OficinaApi\` existe?
2. Permissões de escrita na pasta?
3. Serilog configurado em Program.cs?

---

## ?? Documentação Completa

- **README.md** - Visão geral completa
- **DEVELOPMENT.md** - Guia de desenvolvimento
- **SOLID-PRINCIPLES.md** - Princípios explicados
- **PROJECT-SUMMARY.md** - Sumário técnico

---

## ?? Comandos Úteis

```bash
# Build
dotnet build

# Executar
dotnet run

# Restaurar pacotes
dotnet restore

# Atualizar banco
dotnet ef database update

# Criar migration
dotnet ef migrations add NomeMigration

# Ver versão do .NET
dotnet --version

# Listar templates
dotnet new --list

# Clean
dotnet clean
```

---

## ?? Segurança (To-do)

Próximos passos para produção:
- [ ] Adicionar JWT authentication
- [ ] Validar permissões
- [ ] HTTPS enforced
- [ ] CORS restritivo
- [ ] Rate limiting
- [ ] Input sanitization

---

## ?? Health Check

Verificar se API está rodando:
```bash
curl http://localhost:5000/health
```

Resposta:
```json
{"status":"Healthy"}
```

---

## ?? Conceitos-Chave

### CQRS (Command Query Responsibility Segregation)
- **Commands**: Modificam estado (Create, Update, Delete)
- **Queries**: Leem dados (Get, List)
- Implementado com Wolverine

### Repository Pattern
- Abstrai acesso ao banco
- Facilita testes com mocks
- Genéricos para reutilização

### DTOs (Data Transfer Objects)
- Separam modelos de domínio de API
- Validação centralizadaintegração

### Slice Architecture
- Cada feature é independente
- Facilita escalabilidade
- Claro e organizado

---

## ?? Contribuindo

Para adicionar código ao projeto:
1. Seguir padrões existentes
2. Adicionar comentários explicativos
3. Usar interfaces e injeção de dependência
4. Validar com FluentValidation
5. Testar via Swagger

---

## ?? Dúvidas?

- Consulte **README.md** para detalhes
- Veja **DEVELOPMENT.md** para desenvolvimento
- Leia **SOLID-PRINCIPLES.md** para padrões

Cada arquivo tem comentários explicando POR QUE cada decisão foi tomada.

---

**Pronto para começar! Acesse http://localhost:5000/swagger agora! ??**
