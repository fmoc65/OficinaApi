# ??? MAPA DE NAVEGAÇÃO - OficinaApi

## ?? Onde Começar?

### ?? Tenho 5 minutos
?? Leia: **QUICK-START.md**
- Como rodar a API
- Comandos básicos
- Acesso ao Swagger

### ?? Tenho 15 minutos
?? Leia: **README.md**
- Visão geral do projeto
- Entidades e relacionamentos
- Exemplos de requisição
- Features principais

### ?? Tenho 30 minutos
?? Leia: **ARCHITECTURE.md**
- Diagramas completos
- Fluxo de requisição passo-a-passo
- Padrões visuais
- Relacionamentos

### ?? Tenho 1 hora
?? Leia nesta ordem:
1. **QUICK-START.md** (5 min)
2. **README.md** (15 min)
3. **ARCHITECTURE.md** (10 min)
4. **DEVELOPMENT.md** (15 min)
5. Explore código (15 min)

### ?? Tenho 2 horas
?? Caminho Completo:
1. **00-START-HERE.md** - Este início (3 min)
2. **QUICK-START.md** - Rodar (5 min)
3. **README.md** - Overview (15 min)
4. **ARCHITECTURE.md** - Arquitetura (10 min)
5. **SOLID-PRINCIPLES.md** - Padrões (10 min)
6. **DEVELOPMENT.md** - Estender (15 min)
7. Explore código Clientes (40 min)
8. INDEX.md - Referência (5 min)

---

## ?? Por Perfil

### ????? Sou Desenvolvedor Novo

**Seu Caminho:**
1. ? QUICK-START.md ? Rodar a API
2. ?? README.md ? Entender features
3. ??? ARCHITECTURE.md ? Ver diagramas
4. ??? DEVELOPMENT.md ? Aprender a estender
5. ?? Abra projeto em ClienteEndpoints.cs
6. ?? Copie padrão para nova feature

**Tempo**: ~1 hora
**Resultado**: Sou capaz de rodar, entender e estender

### ??? Sou Arquiteto/Tech Lead

**Seu Caminho:**
1. ??? ARCHITECTURE.md ? Visão completa
2. ?? SOLID-PRINCIPLES.md ? Validar padrões
3. ?? PROJECT-SUMMARY.md ? Estatísticas
4. ?? README.md ? Detalhes técnicos
5. ?? INDEX.md ? Referência rápida
6. ?? Explore código da camada Domain

**Tempo**: ~45 minutos
**Resultado**: Entendo a arquitetura, padrões e escalabilidade

### ?? Sou QA/Testador

**Seu Caminho:**
1. ? QUICK-START.md ? Rodar
2. ?? README.md ? Endpoints e exemplos
3. ?? Acesse http://localhost:5000/swagger
4. ?? Teste cada endpoint (POST, GET, PUT, DELETE)
5. ?? Estude DEVELOPMENT.md para casos de teste

**Tempo**: ~30 minutos
**Resultado**: Consigo testar toda a API via Swagger

### ?? Sou Product Manager

**Seu Caminho:**
1. ?? README.md ? Features
2. ?? PROJECT-SUMMARY.md ? Capacidades
3. ?? INDEX.md ? Endpoints disponíveis

**Tempo**: ~20 minutos
**Resultado**: Entendo o que a API faz

---

## ??? Estrutura de Pastas

```
OficinaApi/
?
??? ?? DOCUMENTAÇÃO (LEIA AQUI)
?   ??? 00-START-HERE.md ? Você está aqui! ??
?   ??? QUICK-START.md ? (5 min)
?   ??? README.md ?? (15 min)
?   ??? ARCHITECTURE.md ??? (10 min)
?   ??? DEVELOPMENT.md ??? (15 min)
?   ??? SOLID-PRINCIPLES.md ?? (10 min)
?   ??? PROJECT-SUMMARY.md ?? (10 min)
?   ??? INDEX.md ?? (5 min)
?   ??? VALIDATION-CHECKLIST.md ? (5 min)
?   ??? FINAL-SUMMARY.md ?? (3 min)
?
??? ?? src/ (CÓDIGO AQUI)
?   ??? OficinaApi.Domain/
?   ?   ??? Entities/ ? Comece aqui para entender
?   ?       ??? Cliente.cs (exemplo base)
?   ?
?   ??? OficinaApi.Application/
?   ?   ??? Features/
?   ?       ??? Clientes/ ? Padrão para copiar
?   ?       ??? Carros/
?   ?       ??? Pecas/
?   ?       ??? OrdenServicos/
?   ?
?   ??? OficinaApi.Infrastructure/
?   ?   ??? Data/ ? Banco de dados
?   ?   ??? Repositories/ ? Acesso a dados
?   ?
?   ??? OficinaApi.Api/
?       ??? Endpoints/ ? Rotas da API
?       ??? Program.cs ? Startup
?       ??? appsettings.json ? Config
?
??? OficinaApi.sln ? Solução Visual Studio
??? .gitignore, global.json, etc
```

---

## ?? Fluxo de Aprendizado

### Fase 1: COMEÇAR (30 min)
```
1. Leia QUICK-START.md
   ?
2. Rode: dotnet run
   ?
3. Acesse: http://localhost:5000/swagger
   ?
4. Veja os 20 endpoints funcionando
```

### Fase 2: ENTENDER (30 min)
```
1. Leia README.md
   ?
2. Leia ARCHITECTURE.md
   ?
3. Veja os diagramas
   ?
4. Entenda o fluxo de requisição
```

### Fase 3: APRENDER (45 min)
```
1. Abra ClienteEndpoints.cs
   ?
2. Veja como funciona POST /api/clientes
   ?
3. Rastreie: Endpoint ? Command ? Handler ? Repository
   ?
4. Entenda cada camada
```

### Fase 4: ESTENDER (60 min)
```
1. Leia DEVELOPMENT.md
   ?
2. Copie estrutura de Clientes
   ?
3. Adicione nova entidade (ex: Agendamento)
   ?
4. Registre em Program.cs
   ?
5. Teste via Swagger
```

**Total**: ~2 horas para dominar o projeto

---

## ?? Buscar por Tópico

### ??? "Quero entender a arquitetura"
? ARCHITECTURE.md
? README.md (seção Arquitetura)

### ??? "Quero adicionar nova feature"
? DEVELOPMENT.md
? Copie estrutura de Features/Clientes

### ?? "Quero entender SOLID"
? SOLID-PRINCIPLES.md
? Cada princípio com exemplos

### ? "Quero rodar agora"
? QUICK-START.md
? 5 minutos e está rodando

### ?? "Quero estatísticas"
? PROJECT-SUMMARY.md
? Números e métricas

### ?? "Quero validar requisitos"
? VALIDATION-CHECKLIST.md
? Checklist completo

### ?? "Quero índice de arquivos"
? INDEX.md
? Estrutura completa

### ?? "Quero sumário final"
? FINAL-SUMMARY.md
? Tudo resumido

---

## ?? Quick Actions

### ?? Rodar a API
```bash
cd src/OficinaApi.Api
dotnet run
# Pronto em http://localhost:5000/swagger
```

### ?? Ler Documentação
```
Abrir: QUICK-START.md (5 min)
Depois: README.md (15 min)
```

### ?? Testar API
```
1. Acesse http://localhost:5000/swagger
2. Expanda "Clientes"
3. Clique "Try it out"
4. Execute um POST
```

### ?? Abrir Código
```
Arquivo mais importante: src/OficinaApi.Api/Endpoints/ClienteEndpoints.cs
Este arquivo contém o padrão para copiar
```

### ?? Adicionar Feature
```
1. Leia: DEVELOPMENT.md
2. Copie: Features/Clientes
3. Cole: Features/MeuDominio
4. Edite: Trocar Cliente por MeuDominio
5. Registre: Program.cs
```

---

## ?? Checklist de Orientação

### Semana 1: Aprendizado
- [ ] Li QUICK-START.md
- [ ] Rodei a API localmente
- [ ] Acessei Swagger em http://localhost:5000/swagger
- [ ] Li README.md
- [ ] Entendi a arquitetura (ARCHITECTURE.md)
- [ ] Estudei SOLID (SOLID-PRINCIPLES.md)

### Semana 2: Prática
- [ ] Explorei o código de ClienteEndpoints
- [ ] Rastriei uma requisição POST
- [ ] Entendi cada camada (API ? App ? Infra)
- [ ] Li DEVELOPMENT.md
- [ ] Criei minha primeira feature

### Semana 3: Domínio
- [ ] Implementei testes unitários
- [ ] Adicionei 2+ novas features
- [ ] Estendo a API conforme necessário
- [ ] Domino os padrões SOLID
- [ ] Consigo explicar a arquitetura

---

## ?? Status de Cada Arquivo

| Arquivo | Status | Ler? | Tempo |
|---------|--------|------|-------|
| 00-START-HERE.md | ?? Você está aqui | Sim | 3 min |
| QUICK-START.md | ? Prioritário | Sim | 5 min |
| README.md | ?? Essencial | Sim | 15 min |
| ARCHITECTURE.md | ??? Recomendado | Sim | 10 min |
| DEVELOPMENT.md | ??? Para estender | Quando precisar | 15 min |
| SOLID-PRINCIPLES.md | ?? Para aprender | Sim | 10 min |
| PROJECT-SUMMARY.md | ?? Referência | Opcional | 10 min |
| INDEX.md | ?? Índice | Consultar | 5 min |
| VALIDATION-CHECKLIST.md | ? Verificação | Opcional | 5 min |
| FINAL-SUMMARY.md | ?? Resumo | Final | 3 min |

---

## ?? Dicas de Ouro

### ?? Dica 1: Comece pelo QUICK-START
Não pule! Rodar localmente te motiva a continuar lendo.

### ?? Dica 2: Explore via Swagger
Primeira coisa: teste os endpoints via Swagger UI.
Isso te mostra como a API funciona na prática.

### ?? Dica 3: Rastreie o Código
1. Abra ClienteEndpoints.cs
2. Clique em CreateClienteEndpoint
3. Abra CreateClienteCommand
4. Abra CreateClienteCommandHandler
5. Entenda cada passo

### ?? Dica 4: Copie, Não Invente
Ao adicionar feature: copie Clientes, adapte.
Não invente novo padrão!

### ?? Dica 5: Respeite os Comentários
Cada comentário explica POR QUE. Leia-os!

---

## ?? Você Está Pronto!

### Próximo Passo:
?? **Leia QUICK-START.md** (5 minutos)

### Depois:
?? **Rode dotnet run** (automático)

### Então:
?? **Acesse http://localhost:5000/swagger**

### Finalmente:
?? **Explore, aprenda e estenda!**

---

## ?? Estrutura de Documentação

```
00-START-HERE.md ? Você está aqui
    ?
QUICK-START.md ? Rodar
    ?
README.md ? Entender
    ?
ARCHITECTURE.md ? Aprender arquitetura
    ?
DEVELOPMENT.md ? Estender
    ?
SOLID-PRINCIPLES.md ? Aprender padrões
    ?
Explore código ? Colocar em prática
```

---

## ?? Meu Objetivo Enquanto Você Lê

? Você entenda a arquitetura
? Você consiga rodar localmente
? Você saiba estender com nova feature
? Você domine os padrões SOLID
? Você sinta confiança no código

---

**?? Bem-vindo! Vamos começar?**

**Próximo passo: Leia QUICK-START.md em 5 minutos!**

---

*Arquivo: 00-START-HERE.md*
*Versão: 1.0*
*Data: 2026-06-19*
*Status: ? Pronto para começar*
