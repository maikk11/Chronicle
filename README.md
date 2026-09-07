# Chronicle

[![Build](https://github.com/maikk11/Chronicle/actions/workflows/ci.yml/badge.svg)](https://github.com/maikk11/Chronicle/actions/workflows/ci.yml)
[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![MySQL](https://img.shields.io/badge/MySQL-8.0-4479A1?logo=mysql&logoColor=white)](https://www.mysql.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

Chronicle is an editorial platform for a newsroom. Writers submit articles,
revisors accept or reject them before publication, and readers browse what has
been accepted, by category and by author.

> **Project status: early setup.** The repository currently contains the
> scaffolded ASP.NET Core MVC application, the data layer (entity models, the
> EF Core `DbContext`, migrations, and category seeding) and the project
> infrastructure (CI pipeline, issue templates, branch protections). None of the
> editorial features described above are implemented yet — see [Roadmap](#roadmap).

## Requirements

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- MySQL 8.0
- dotnet-ef

## Tech stack

- ASP.NET Core MVC
- Razor Views
- Entity Framework Core (8.0.13)
- MySQL with Pomelo (8.0)

## Getting started

### 1. Clone the repository

```bash
git clone https://github.com/maikk11/Chronicle.git
cd Chronicle
```

### 2. Configure secrets

Secrets are never committed to the repository. They are supplied through the
.NET Secret Manager, which keeps them on your machine, outside the project
directory.

`dotnet user-secrets` resolves its storage location from the `UserSecretsId`
declared in the `.csproj`, so the command must be run from the **project**
directory rather than the repository root:

```bash
cd Chronicle
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your value>"
```

If you would rather stay at the repository root, point the command at the
project explicitly:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your value>" --project Chronicle
```

`appsettings.Development.example.json` is the reference for what needs to be
set. It lists every configuration key the application expects, with placeholder
values. Each key in that file needs a matching `dotnet user-secrets set`, using
the same colon-separated path as the key's position in the JSON. Nothing in the
example file is read at runtime — it exists purely as documentation.

To confirm what you have stored:

```bash
dotnet user-secrets list
```

### 3. Apply the migrations

With the connection string in place, create the database schema. Run this from
the **repository root**, pointing at the project:

```bash
dotnet ef database update --project Chronicle
```

### 4. Run

```bash
dotnet run --project Chronicle
```

The application is served at `http://localhost:5267`.

On its first startup the application populates the category table. The
migrations create the schema only, so the categories appear after this step,
not after step 3.

## Running the tests

The test project lives in `Chronicle.Tests` and uses xUnit. Run the suite from the repository root:

```bash
dotnet test
```

The same command runs on every pull request, so a failing test blocks the merge.

## Architecture

<!-- PLACEHOLDER: to be filled once the layered structure exists -->

## Roadmap

Planned functionality, tracked as user stories in the
[issue tracker](https://github.com/maikk11/Chronicle/issues).
**None of it is implemented yet.**

- [ ] **US1** — Registration, login, and article submission by writers
- [ ] **US2** — Public article listing and detail pages, browsable by category and author
- [ ] **US3** — Admin, Revisor and Writer roles; team applications; article review
- [ ] **US4** — Full-text search across accepted articles
- [ ] **US5** — Writers editing and deleting their own articles

## License

Released under the MIT License. See [LICENSE](LICENSE) for the full text.