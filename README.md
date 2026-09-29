# Med-Sched

Sistema de agendamento médico desenvolvido em **.NET 9 Minimal API**, utilizando **Clean Architecture** e boas práticas de desenvolvimento.

---

## 📝 Sobre

O Med-Sched permite gerenciar agendamentos de pacientes e médicos, garantindo integridade e segurança das informações. O projeto segue princípios de Clean Architecture, separando camadas de aplicação, domínio e infraestrutura.
Em ambiente não produtivo, a API disponibiliza documentação interativa via Doc Scalar na rota `/scalar`.

---

## ⚙️ Pré-requisitos

* [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
* [PostgreSQL 17+](https://www.postgresql.org/download/)
* Docker (opcional, para facilitar execução do banco)
* Visual Studio 2022 ou VS Code

---

## 🏗️ Instalação e execução

1. Clone o repositório:

```bash
git clone -b develop https://github.com/marcosl493/med-sched.git
cd med-sched
```

2. Configure o banco de dados no `appsettings.json` (ou via variáveis de ambiente):

```json
"ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=medsched;"
}
```

3. Aplicar migrations (Na pasta raiz med-sched, executar:):

```bash
dotnet ef database update --project src/Infrastructure --startup-project  src/WebApi
```

4. Adicionar variáveis de ambiente obrigatórias no container ou no Windows:
```
DOTNET_ENVIRONMENT
DB_USERNAME
DB_PASSWORD
Jwt__Audience
Jwt__Issuer
Jwt__SecretKey
```
5. Rodar a API:

```bash
cd src/WebApi
dotnet run
```

A API estará disponível em `https://localhost:7271` ou `http://localhost:5051`.

6. Executando com Podman Compose:

O arquivo `src/.env.example` lista as variáveis esperadas. Copie-o para `src/.env`, ajuste os valores e execute na pasta `src`:

```bash
cp .env.example .env
podman compose up --build
```

* O Compose irá iniciar tanto a API quanto o banco PostgreSQL.
* Ao criar o volume do PostgreSQL, o **script inicial do banco é executado automaticamente**.

A API estará disponível em `http://localhost:4000`.

---

## 💾 Banco de dados

* PostgreSQL
* Migrations gerenciadas via EF Core
* Scripts SQL podem ser aplicados manualmente em `src/Infrastructure/Migrations` se necessário
* Recomenda-se manter consistência entre migrations e banco inicializado via Docker

---

## 🛠️ Decisões técnicas

* **Clean Architecture**: separação clara entre domínio, aplicação e infraestrutura
* **Minimal API**: endpoints leves e simplificados
* **EF Core com PostgreSQL**: ORM robusto para manipulação de dados
* **Docker**: facilidade para inicialização de ambientes de desenvolvimento
* **Versionamento e branch develop**: branch principal de desenvolvimento, commits estruturados

---

## 📚 Recursos

* Organização baseada em **camadas**: `Domain`, `Application`, `Infrastructure`, `WebApi`
* Implementação de **boas práticas de backend**, como injeção de dependência, CQRS e validações

---

## 🧪 Testes

O projeto contém testes unitários implementados utilizando a biblioteca MSTest, garantindo a validação das principais funcionalidades da aplicação. Esses testes foram desenvolvidos para assegurar que as regras de negócio essenciais funcionem corretamente e que possíveis regressões sejam rapidamente identificadas durante o desenvolvimento.

---

## 📄 Licença

MIT License
