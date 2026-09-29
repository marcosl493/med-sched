# Deploy da API no Azure

## Arquitetura recomendada

Use Azure Bicep como IaC em vez de CloudFormation: e declarativo e integrado ao Azure Resource Manager. Para a API, o mapeamento do Compose fica assim:

| Compose | Azure |
| --- | --- |
| `webapi` | Azure Container Apps com ingress externo na porta 8080 |
| `db` | Azure Database for PostgreSQL Flexible Server; executar `scripts/1-init.sql` uma vez na primeira configuracao |
| `kafka` | Azure Event Hubs Standard com endpoint Kafka, listener TLS na porta 9093 |
| `kafka-ui` | Omitir em producao e usar observabilidade Azure |
| imagem | Azure Container Registry (ACR) |
| configuracoes secretas | Azure Key Vault com identidade gerenciada do Container App |

Event Hubs mantem o protocolo Kafka e a biblioteca `Confluent.Kafka` usados pela API. Azure Service Bus tambem e uma alternativa para o evento de agendamento, mas requer trocar a integracao Kafka no codigo.

## Preparar o Azure

Provisionar com Bicep (ou portal/CLI) um Resource Group, ACR, Key Vault, Container Apps Environment, um Container App para API, PostgreSQL Flexible Server e um namespace Event Hubs Standard. Crie o Event Hub `appointment-created`. Configure rede privada entre Container Apps e PostgreSQL para producao; nao exponha o banco publicamente.

O endpoint Kafka do Event Hubs e `<namespace>.servicebus.windows.net:9093`. Crie uma regra SAS com permissao `Send` para a API e grave a connection string no Key Vault.

Cadastre estes segredos no Key Vault, diretamente no Azure:

- `db-username` e `db-password`
- `jwt-secret-key`
- `eventhub-producer-connection`

O usuario/senha precisam corresponder a um usuario PostgreSQL existente. A API tambem precisa dos valores nao secretos `Jwt__Issuer` e `Jwt__Audience`.

Habilite a identidade gerenciada system-assigned do Container App e conceda a role **Key Vault Secrets User** no escopo do vault. Configure referencias Key Vault versionless nos segredos do app:

```bash
az containerapp identity assign -g "$RESOURCE_GROUP" -n "$API_CONTAINER_APP" --system-assigned
az role assignment create --assignee-object-id "$API_PRINCIPAL_ID" --assignee-principal-type ServicePrincipal --role "Key Vault Secrets User" --scope "$KEY_VAULT_ID"

az containerapp secret set -g "$RESOURCE_GROUP" -n "$API_CONTAINER_APP" --secrets \
  "db-username=keyvaultref:$KEY_VAULT_URI/secrets/db-username,identityref:system" \
  "db-password=keyvaultref:$KEY_VAULT_URI/secrets/db-password,identityref:system" \
  "jwt-secret-key=keyvaultref:$KEY_VAULT_URI/secrets/jwt-secret-key,identityref:system" \
  "eventhub-producer-connection=keyvaultref:$KEY_VAULT_URI/secrets/eventhub-producer-connection,identityref:system"
```

Conceda `AcrPull` a identidade gerenciada do Container App no escopo do ACR e configure o registry com `az containerapp registry set --server "$ACR_LOGIN_SERVER" --identity system`.

Configure as variaveis de ambiente do Container App (valores nao secretos):

- `DOTNET_ENVIRONMENT=Production`, `ASPNETCORE_HTTP_PORTS=8080`
- `DB_USERNAME=secretref:db-username`, `DB_PASSWORD=secretref:db-password`
- `ConnectionStrings__MedSchedDbContext=Host=<servidor>.postgres.database.azure.com;Port=5432;Database=MedSched;SSL Mode=Require;Trust Server Certificate=true`
- `Jwt__Issuer`, `Jwt__Audience`, `Jwt__ExpiresInMinutes=15`, `Jwt__SecretKey=secretref:jwt-secret-key`
- `Kafka__Producer__BootstrapServers=<namespace>.servicebus.windows.net:9093`
- `Kafka__Producer__SaslUsername=$ConnectionString`
- `Kafka__Producer__SaslPassword=secretref:eventhub-producer-connection`

Aplique `scripts/1-init.sql` no PostgreSQL na primeira implantacao. Em producao, prefira um usuario PostgreSQL dedicado com permissoes minimas em vez do administrador do servidor.

## GitHub Actions

O workflow `.github/workflows/deploy-api.yml` roda os testes em pull requests e publica/deploya em push para `main` ou `develop`; tambem pode ser executado manualmente. Ele usa OIDC, sem client secret Azure no GitHub. Configure uma federated credential para o repositorio/branch no Microsoft Entra ID. A identidade federada precisa de `AcrPush` no ACR e permissao para atualizar o Container App.

Cadastre como **Actions Variables** (nao secrets): `AZURE_CLIENT_ID`, `AZURE_TENANT_ID`, `AZURE_SUBSCRIPTION_ID`, `AZURE_RESOURCE_GROUP`, `ACR_NAME`, `ACR_LOGIN_SERVER` (por exemplo `meuacr.azurecr.io`) e `API_CONTAINER_APP`. O ACR deve permitir login da identidade OIDC pelo `az acr login`.

O workflow atualiza um Container App ja provisionado. O provisionamento de recursos e secrets e feito uma vez no Azure; nenhum valor do Key Vault vai para Actions Variables ou Secrets.
