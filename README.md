# Chronicle

[![Build](https://github.com/maikk11/Chronicle/actions/workflows/ci.yml/badge.svg)](https://github.com/maikk11/Chronicle/actions/workflows/ci.yml)
[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![MySQL](https://img.shields.io/badge/MySQL-8.0-4479A1?logo=mysql&logoColor=white)](https://www.mysql.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

Chronicle is an editorial platform for a newsroom. Writers submit articles,
revisors accept or reject them before publication, and readers browse what has
been accepted, by category and by author.

> **Project status: feature complete, not deployed.** All five user stories are
> implemented and merged. The application runs locally against MySQL and
> Supabase Storage. It has not been deployed anywhere, and a short list of
> things that should be settled before it is lives under
> [Known limitations](#known-limitations).

## What it does

- Visitors read accepted articles, browse by category and author, and search by
  title, subtitle, category or author name.
- Registered users apply to join the team through a "Work with us" form.
- Admins review those applications from a dashboard, grant or refuse the
  requested role, and manage the category list.
- Revisors see the articles waiting for review and accept or reject each one.
  Only accepted articles are ever public.
- Writers have a dashboard of their own work and can edit or delete it. Editing
  sends the article back for review.

## Roles

Three roles are seeded into the database: `Admin`, `Revisor` and `Writer`.

A registered user holds none of them. To get one they apply through the "Work
with us" link in the footer, and an admin approves the application from the
admin dashboard. `Admin` cannot be applied for — it is rejected server side even
if the request is forged.

Role membership is carried in the authentication cookie, so a user who has just
been granted a role sees nothing change until they sign out and back in.

## Requirements

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- MySQL 8.0
- dotnet-ef
- A [Supabase](https://supabase.com) project, for article images
- A [Mailtrap](https://mailtrap.io) inbox, for the notification emails in
  development

## Tech stack

- ASP.NET Core MVC
- Razor Views
- Entity Framework Core (8.0.13)
- MySQL with Pomelo (8.0)
- ASP.NET Core Identity for authentication and roles
- Supabase Storage for article images
- SMTP for notification emails, against a Mailtrap sandbox in development
- xUnit for the test suite

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

Notification emails go out over SMTP. In development they are pointed at a
Mailtrap sandbox, which captures everything instead of delivering it:

```bash
dotnet user-secrets set "Email:User" "<your mailtrap username>" --project Chronicle
dotnet user-secrets set "Email:Pass" "<your mailtrap password>" --project Chronicle
```

The host, port, sender and admin notification address are not secret and live in
`appsettings.json` under `Email`.

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

The categories, the three roles and a first admin account are all seeded through
the migrations, so the database is usable as soon as this step completes.

### 4. Run

```bash
dotnet run --project Chronicle
```

The application is served at `http://localhost:5267`.

### 5. Sign in as the admin

A first administrator is seeded so that there is somebody able to approve role
applications on a fresh database:
username: admin
password: Admin123!


**This is a development convenience and it is not a secret.** The password is
printed here, and its hash is committed in the migration that seeds the account.
Anyone who can read this repository can sign in as the administrator of an
instance that still uses it. It has to be changed before the application is
reachable by anybody else.

## Running the tests

The test project lives in `Chronicle.Tests` and uses xUnit. Run the suite from
the repository root:

```bash
dotnet test
```

The same command runs on every pull request, so a failing test blocks the merge.

The suite is small and honest about what it covers. Five of the six tests cover
`ArticleOwnership.IsOwnedBy`, the rule that decides whether a writer may edit or
delete a given article — including the case of an article whose author has been
deleted, which must not become editable by anyone. They test the rule itself,
not that the controllers call it; covering that needs integration tests through
the request pipeline.

## Architecture

The application is organised in layers:

- **Controllers** handle HTTP requests and call the services. `AccountController`
  works directly with ASP.NET Core Identity's `UserManager` and `SignInManager`.
- **Services** hold the business rules and coordinate the repositories, Supabase
  Storage and SMTP. A service is where a new article is forced to start
  unreviewed, where an approved application turns into a role assignment, and
  where an edited article is sent back for review.
- **Repositories** encapsulate data access through EF Core.
- **`DbContext`** maps the entities to the MySQL schema and seeds the categories,
  the roles and the first admin.

Views that display data receive DTOs mapped by the services. The article forms
bind directly to the `Article` entity, which is why its navigation properties are
marked `[ValidateNever]`.

Two pieces sit outside that structure. `NotificationBellViewComponent` renders
the pending-work count in the navigation bar, which belongs to the layout and so
has no controller of its own. `ArticleOwnership` is a pure function holding the
one rule that decides whether an article belongs to the signed-in user; it lives
on its own because three actions need it and because a rule that can be tested
without a database is a rule worth testing.

Every state-changing action is a POST with an antiforgery token, and every
action that must not be public is guarded server side regardless of what the
navigation bar shows. Hiding a link is presentation, not protection.

## Known limitations

These are known and tracked, not forgotten:

- **Article visibility is decided in four places, in memory.** The article
  listing, the home page, the search and the detail guard each load their rows
  and then filter on acceptance. The rule belongs in the repository; as it
  stands, changing it means finding all four.
- **Error status codes render an empty page.** `NotFound()` sends the status code
  with no body, so a 404 is a blank screen. Correct over HTTP, poor for a
  visitor.
- **A failed image upload surfaces as an unhandled exception.** If Supabase is
  unreachable, creating an article shows a stack trace instead of a message.
- **Controllers depend on concrete service classes rather than interfaces**,
  which is what keeps them out of reach of unit tests.
- **The seeded admin password is public**, as described above.

## User stories

Tracked in the [issue tracker](https://github.com/maikk11/Chronicle/issues).
All five are implemented.

- [x] **US1** — Registration, login, and article submission by writers
- [x] **US2** — Public article listing and detail pages, browsable by category and author
- [x] **US3** — Admin, Revisor and Writer roles; team applications; article review
- [x] **US4** — Full-text search across accepted articles
- [x] **US5** — Writers editing and deleting their own articles

## License

Released under the MIT License. See [LICENSE](LICENSE) for the full text.