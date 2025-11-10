# Sistema de Gerenciamento de Pedidos — 3ª Sprint

## 👥 Integrantes do Grupo
- **rm97677** — Lucas Felix VASSILIADES  
- **rm556588** — Gabriel Yuji Suzuki

---

## 🏗️ Justificativa da Arquitetura

- **Arquitetura em Camadas (Controllers → Services → Repositories → Domain/Entities)**  
  Separação clara de responsabilidades, baixo acoplamento e alta coesão, facilitando testes e manutenção.
- **Entity Framework Core + Oracle**  
  EF Core como ORM, com `AppDbContext` mapeando `Usuarios`, `Produtos`, `Pedidos` e `PedidoItens`.  
  *(Em testes/integração usamos EF InMemory para isolar do Oracle.)*
- **Injeção de Dependência (DI)**  
  Repositórios e serviços registrados no container para permitir mocks/stubs em testes.
- **API RESTful + Versionamento de API (`v1`)**  
  Rotas padronizadas `api/v{version}/...`, verbos HTTP corretos e códigos de status adequados.
- **Segurança por API Key (header `X-Api-Key`)**  
  Middleware simples validando a chave (libera `/swagger` e `/health/*`).
- **Observabilidade (Health Checks)**  
  - **Liveness**: `/health/live`  
  - **Readiness**: `/health/ready` (inclui checagem do DB)
- **Swagger/OpenAPI**  
  Documentação interativa com suporte a API Key.
- **ML.NET (Demo)**  
  Endpoint de **análise de sentimento** para cumprir o requisito de IA/ML da sprint.
- **Testes (xUnit + WebApplicationFactory)**  
  Testes unitários e de integração com EF InMemory e servidor em memória.

---

## 🧰 Principais Pacotes NuGet

**Projeto Web**
- `Microsoft.EntityFrameworkCore`
- `Oracle.EntityFrameworkCore`
- `Oracle.ManagedDataAccess.Core`
- `Swashbuckle.AspNetCore`
- `Microsoft.AspNetCore.Mvc.Versioning`
- `Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer`
- `Microsoft.ML` *(para o endpoint de ML)*

**Projeto de Testes**
- `Microsoft.AspNetCore.Mvc.Testing`
- `Microsoft.EntityFrameworkCore.InMemory`
- `xunit`, `xunit.runner.visualstudio`
- `FluentAssertions`

---

## 🚀 Instruções de Execução

### Pré-requisitos
- **.NET 8 SDK**
- Acesso a um **Oracle** (ou use InMemory para desenvolvimento local, veja abaixo).



### 2) Restaurar e executar
```bash
dotnet restore
dotnet run --project Sprint03
```

- **Swagger**: acesse `/swagger`  
  No topo, clique no **cadeado**, selecione **ApiKey** e informe o valor de `X-Api-Key`.

---

## 🧪 Testes (xUnit + WebApplicationFactory)

O repositório inclui o projeto **`Sprint03.Tests`**.  

**Rodar:**
```bash
dotnet test
```

**Cobertura:**
- **Integração**: `/health/live`, `/swagger/v1/swagger.json`, autenticação por `X-Api-Key` nos endpoints `v1`, `POST /api/v1/ml/sentiment`.
- **Unitários**: CRUD básico de `UsuarioService` e `ProdutoService`.

---

## 🌡️ Health Checks

- **Liveness**: `GET /health/live`  
  Verifica se a aplicação está de pé.
- **Readiness**: `GET /health/ready`  
  Executa todas as checagens registradas (inclui DB) e retorna JSON com status.

> Em desenvolvimento, se o Oracle não estiver acessível, o `/health/ready` pode retornar **503**.  
> Para evitar isso durante dev local, use **EF InMemory** temporariamente.

---

## 🔒 Segurança (API Key)

- Header: **`X-Api-Key`**  
- Endpoints **liberados**: `/swagger/*` e `/health/*`  
- Todos os demais exigem chave válida.

**Exemplo (curl):**
```bash
curl -H "X-Api-Key: dev-123456" https://localhost:5001/api/v1/Usuarios
```

---

## 🧠 Endpoint de ML.NET (Demo)

- **POST** `/api/v1/ml/sentiment`  
  **Body:**
  ```json
  { "text": "produto excelente" }
  ```
  **Resposta:**
  ```json
  { "label": "positivo", "probability": 0.93 }
  ```

---

## 📚 Endpoints Principais (v1)

> Todas as rotas seguem o padrão `api/v1/[controller]`.  
> A paginação (quando aplicável) usa `pageNumber` e `pageSize`.

### Usuários
- `GET /api/v1/Usuarios`
- `GET /api/v1/Usuarios/{id}`
- `POST /api/v1/Usuarios`
  ```json
  { "nome": "Ana", "email": "ana@ex.com" }
  ```
- `PUT /api/v1/Usuarios/{id}`
- `DELETE /api/v1/Usuarios/{id}`

### Produtos
- `GET /api/v1/Produtos`
- `GET /api/v1/Produtos/{id}`
- `POST /api/v1/Produtos`
  ```json
  { "nome": "Mouse Gamer", "preco": 99.90, "descricao": "RGB" }
  ```
- `PUT /api/v1/Produtos/{id}`
- `DELETE /api/v1/Produtos/{id}`

### Pedidos
- `GET /api/v1/Pedidos`
- `GET /api/v1/Pedidos/{id}`
- `GET /api/v1/Pedidos/com-itens`
- `POST /api/v1/Pedidos`
  ```json
  {
    "usuarioId": 1,
    "itens": [
      { "produtoId": 1, "quantidade": 2 },
      { "produtoId": 2, "quantidade": 1 }
    ]
  }
  ```
- *(Se disponível)* `DELETE /api/v1/Pedidos/{id}`

---

## 🧪 Exemplos rápidos (curl)

```bash
# Health
curl -i https://localhost:5001/health/live
curl -s https://localhost:5001/health/ready

# Usuário (com API Key)
curl -X POST https://localhost:5001/api/v1/Usuarios   -H "Content-Type: application/json" -H "X-Api-Key: dev-123456"   -d '{ "nome": "Ana", "email": "ana@ex.com" }'

# Produto
curl -X POST https://localhost:5001/api/v1/Produtos   -H "Content-Type: application/json" -H "X-Api-Key: dev-123456"   -d '{ "nome": "Mouse", "preco": 99.9, "descricao": "RGB" }'

# Pedido
curl -X POST https://localhost:5001/api/v1/Pedidos   -H "Content-Type: application/json" -H "X-Api-Key: dev-123456"   -d '{ "usuarioId": 1, "itens": [ { "produtoId": 1, "quantidade": 2 } ] }'

# ML.NET
curl -X POST https://localhost:5001/api/v1/ml/sentiment   -H "Content-Type: application/json" -H "X-Api-Key: dev-123456"   -d '{ "text": "produto excelente" }'
```


