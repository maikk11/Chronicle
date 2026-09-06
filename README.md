# Chronicle

[![Build](https://github.com/maikk11/Chronicle/actions/workflows/ci.yml/badge.svg)](https://github.com/maikk11/Chronicle/actions/workflows/ci.yml)
[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

Chronicle is an editorial platform for a newsroom. Writers submit articles,
revisors accept or reject them before publication, and readers browse what has
been accepted, by category and by author.

> **Project status: early setup.** The repository currently contains the
> scaffolded ASP.NET Core MVC application and the project infrastructure
> (CI pipeline, issue templates, branch protections). None of the editorial
> features described above are implemented yet — see [Roadmap](#roadmap).

## Requirements

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) or later

## Tech stack

- ASP.NET Core MVC
- Razor Views

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

**The connection string is not needed yet.** The application has no database
access at this stage, so it starts and runs without it. The key is documented
here because the mechanism is in place and the connection string becomes
required once Entity Framework and the data layer are added.

### 3. Run

```bash
dotnet run --project Chronicle
```

The application is served at `http://localhost:5267`.

## Running the tests

<!-- PLACEHOLDER: to be filled in SETUP-05 -->

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