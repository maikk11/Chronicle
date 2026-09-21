# Chronicle

[![Build](https://github.com/maikk11/Chronicle/actions/workflows/ci.yml/badge.svg)](https://github.com/maikk11/Chronicle/actions/workflows/ci.yml)
[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![MySQL](https://img.shields.io/badge/MySQL-8.0-4479A1?logo=mysql&logoColor=white)](https://www.mysql.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

Chronicle is an editorial platform for a newsroom. Writers submit articles,
revisors accept or reject them before publication, and readers browse what has
been accepted, by category and by author.

> **Project status: early development.** The repository currently contains the
> scaffolded ASP.NET Core MVC application, the data layer, authentication,
> article creation with an optional cover image stored on Supabase, and the
> public pages: home, article index, article detail, and filtering by category
> and by author. Article review is not implemented yet, so no article is
> accepted and the public pages stay empty until US3; see [Roadmap](#roadmap).

## Requirements

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- MySQL 8.0
- dotnet-ef

## Tech stack

- ASP.NET Core MVC
- Razor Views
- Entity Framework Core (8.0.13)
- MySQL with Pomelo (8.0)
- Supabase Storage for article images

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

Article images are stored on Supabase. Create a project and a storage bucket,
then set the project URL (without a trailing slash) and the API key:

```bash
dotnet user-secrets set "Supabase:Url" "https://<your-project>.supabase.co" --project Chronicle
dotnet user-secrets set "Supabase:Key" "<your key>" --project Chronicle
```

The bucket paths live in `appsettings.json`. If your bucket is not named
`ChronicleDB`, update `Supabase:Bucket` and `Supabase:PublicUrl` there.

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

The category data is seeded through the migrations, so the category table is
already populated once this step completes.

### 4. Run

```bash
dotnet run --project Chronicle
```

The application is served at `http://localhost:5267`.

## Running the tests

The test project lives in `Chronicle.Tests` and uses xUnit. Run the suite from the repository root:

```bash
dotnet test
```

The same command runs on every pull request, so a failing test blocks the merge.

## Architecture

The application is organised in layers:

- **Controllers** handle HTTP requests and call the services. Some filtering
  and ordering still happens in the controllers; moving it into the
  repositories is tracked as technical debt. `AccountController` works
  directly with ASP.NET Core Identity's `UserManager` and `SignInManager`.
- **Services** hold the business rules and coordinate the repositories and Supabase Storage.
- **Repositories** encapsulate data access through EF Core.
- **`DbContext`** maps the entities to the MySQL schema.

Views that display data receive DTOs mapped by the services. The article
creation form binds directly to the `Article` entity, which is why its
navigation properties are marked `[ValidateNever]`.

## Roadmap

Planned functionality, tracked as user stories in the
[issue tracker](https://github.com/maikk11/Chronicle/issues).
**US1** and **US2** are complete; the remaining user stories below are not implemented yet.

- [x] **US1** — Registration, login, and article submission by writers
- [x] **US2** — Public article listing and detail pages, browsable by category and author
- [ ] **US3** — Admin, Revisor and Writer roles; team applications; article review
- [ ] **US4** — Full-text search across accepted articles
- [ ] **US5** — Writers editing and deleting their own articles

## License

Released under the MIT License. See [LICENSE](LICENSE) for the full text.