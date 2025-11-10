# Sprint04 - API .NET

Este projeto implementa os itens solicitados na sprint:

- ✅ Endpoint de **Health Checks** em `/health`
- ✅ **Versionamento de API** (v1) com controllers em `api/v{version}/...`
- ✅ **Segurança** por **API Key** (header `X-Api-Key`) — valor em `appsettings.json` (`ApiKey`)
- ✅ **Swagger/OpenAPI** configurado com suporte a API Key
- ✅ Ao menos um endpoint usando **ML.NET** (`POST /api/v1/ml/sentiment`)
- ✅ **Testes unitários** (xUnit) para a lógica de serviços (in-memory EF)
- ✅ **Testes de integração** básicos com `WebApplicationFactory`

## Como rodar

1. **.NET 8 SDK** instalado.
2. Configure a connection string do Oracle em `appsettings.json` (não commitar senhas).
3. Defina a API Key (ex.: `dev-123456`).

```bash
# Restaurar pacotes
dotnet restore

# Rodar a API
dotnet run --project Sprint03
```

Acesse `https://localhost:5001/swagger` (ou porta exibida) e clique no ícone do cadeado para informar a **API Key**.

## Testes

```bash
dotnet test
```

Os testes unitários usam **EF InMemory** e independem do Oracle. Os testes de integração verificam `/health`, exigência de API Key e acesso ao Swagger.

## Endpoints principais

- `GET /health` — healthcheck
- `POST /api/v1/ml/sentiment` — body: `{ "text": "produto ótimo" }`
- CRUD:
  - `GET/POST /api/v1/usuarios`
  - `GET/POST /api/v1/produtos`
  - `GET/POST /api/v1/pedidos`, `GET /api/v1/pedidos/com-itens`

> Observação: Os endpoints de CRUD requerem banco configurado. Para rodar sem Oracle, você pode trocar para `UseInMemoryDatabase` em `Program.cs` só para desenvolvimento e testes locais.
