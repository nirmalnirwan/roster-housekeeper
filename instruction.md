# Roaster Housekeeper Project Instructions

## Project Overview

This repository contains a Housekeeper Roster Management System for retirement villages and elder care facilities in New Zealand. It is split into a Next.js frontend and two .NET 8 Web API applications:

- `roster-web-app`: Next.js App Router frontend built with TypeScript, Tailwind CSS, shadcn-style UI components, next-themes, and client-side JWT storage helpers.
- `roster-api-app/roster-api-app`: Main roster domain API for housekeepers, rosters, roster tasks, residents, locations, and cleaning tasks.
- `roster-auth-app/roster-auth-app`: Authentication and identity API using ASP.NET Core Identity, JWT access tokens, refresh tokens, roles, and PostgreSQL.

Do not create a new solution or replace the architecture. Extend the existing projects, folders, services, DTOs, and UI components.

## Repository Structure

```text
roaster-housekeeper/
  roster-web-app/
    app/
      admin/
      dashboard/
      export/
      housekeepers/
      login/
      my-schedule/
      roster/
      hooks/
      services/
      globals.css
      layout.tsx
      page.tsx
    components/
      auth/
      ui/
      footer.tsx
      gradient-header.tsx
      navbar.tsx
      sidenav.tsx
      theme-provider.tsx
      theme-toggle.tsx
    generated/
      prisma/
    lib/
      prisma.ts
      sync-user.ts
      types.ts
      utils.ts
    public/
    types/
    package.json
    next.config.ts
    tsconfig.json
  roster-api-app/
    roster-api-app/
      Controllers/
      Data/
      DTOs/
      Entities/
      Repositories/
      Services/
      Program.cs
  roster-auth-app/
    roster-auth-app/
      Controllers/
      Dtos/
      Extensions/
      Models/
      Seeders/
      Utilities/
      Program.cs
```

Generated and build folders such as `.next`, `node_modules`, `bin`, `obj`, `Migrations`, and `generated/prisma` should not be manually edited unless the task specifically requires regeneration.

## Architecture

The frontend uses Next.js App Router routes under `app/`. Most current pages are client components because they depend on authentication hooks, browser storage, FullCalendar, or event handlers. Shared visual primitives live in `components/ui`, while app-level navigation and layout components live directly under `components`.

The main API follows a layered controller-service-repository pattern:

- Controllers expose REST endpoints under `api/[controller]`.
- Services map between DTOs and EF entities and hold business logic.
- Repositories encapsulate EF Core queries and persistence.
- `ApplicationDbContext` defines entity sets and relationships.

The auth API owns identity concerns:

- ASP.NET Core Identity user and role management.
- JWT access token generation.
- Refresh token storage and rotation.
- Role seeding and admin user seeding.
- Profile and password endpoints.

The main API validates JWTs using the same issuer, audience, and signing key settings as the auth API.

## Frontend Standards

- Keep compatibility with Next.js App Router.
- Use TypeScript strict typing. Avoid `any`; if an existing API is untyped, add focused interfaces in `lib/types.ts`, `types/`, or beside the service using it.
- Prefer existing UI primitives from `components/ui`.
- Use the `cn` helper from `lib/utils.ts` for class composition.
- Use Tailwind utility classes consistently with the existing style.
- Add `"use client"` only where browser APIs, hooks, event handlers, or client-only libraries are required.
- Do not read `window`, `document`, `localStorage`, `Date.now()`, or `Math.random()` during render if the component can be server rendered or hydrated from server HTML.
- Browser-only auth checks should run inside `useEffect` or behind an explicit mounted state so initial server and client markup match.
- When rendering a `Button` as a link, use `<Button asChild><Link ... /></Button>` rather than wrapping a button in a link.
- Keep component APIs stable unless the task explicitly asks for a breaking change.

## Backend Standards

- Keep .NET 8 Web API conventions already present in both APIs.
- Use async EF Core methods throughout controllers, services, and repositories.
- Keep controllers thin. Put business rules and entity-to-DTO mapping in services.
- Keep direct EF Core access inside repositories or DbContext setup.
- Use DTOs for API input/output instead of returning EF entities from controllers.
- Preserve `[ApiController]`, route attributes, and authorization attributes.
- Register services, repositories, DbContexts, authentication, authorization, Swagger, and CORS in `Program.cs`.
- Keep JWT issuer, audience, and signing key compatible between auth API and main API.
- Use PostgreSQL through `UseNpgsql`.

## Authentication Flow

1. The frontend posts login credentials to `AUTH_BASE/api/auth/login`.
2. The auth API validates the user with ASP.NET Core Identity.
3. Active users receive an access token, refresh token, token metadata, and user details.
4. The frontend stores the access token, refresh token, and normalized user in `localStorage`; it also writes a `token` cookie for middleware or server-side checks.
5. `apiClient` attaches `Authorization: Bearer <token>` to API requests when a token exists.
6. The main API uses JWT bearer authentication and protects domain controllers with `[Authorize]`.
7. Refresh token handling should call the auth API refresh endpoint and update stored tokens.
8. Logout should clear local storage and expire the token cookie; when possible, also revoke the refresh token through the auth API.

Important: keep refresh endpoint names synchronized between frontend and auth API. The current auth controller exposes `POST /api/auth/refresh`.

## API Conventions

- Main API routes follow `api/[controller]`, such as `api/housekeepers` and `api/rosters`.
- Use REST semantics:
  - `GET /api/resource`
  - `GET /api/resource/{id}`
  - `POST /api/resource`
  - `PUT /api/resource/{id}`
  - `DELETE /api/resource/{id}`
- Return `CreatedAtAction` for successful creates.
- Return `NoContent` for successful updates and deletes.
- Return `NotFound` when a requested resource does not exist.
- Export endpoints should return files with correct content types.
- Frontend services should call API paths through `apiClient` and return typed data.

## State Management

The current frontend uses local React state and effects. Continue this approach unless a larger feature clearly needs shared state.

- Use `useState` for local UI state.
- Use `useEffect` for client-side data loading and browser-only checks.
- Keep authentication helpers in `app/services/authService.ts`.
- Keep API transport concerns in `app/services/apiClient.ts`.
- Keep domain-specific service wrappers, such as roster operations, in `app/services`.
- Avoid duplicating token or fetch logic in pages.

## Reusable Component Guidelines

- Put generic primitives in `components/ui`.
- Put app-specific layout/navigation components in `components`.
- Put feature-specific forms or widgets under a feature folder such as `components/auth`.
- Preserve existing props and exports for shared components.
- Use `asChild` composition for primitives that need to render as links or custom elements.
- Keep visual components deterministic during SSR and hydration.
- Avoid invalid HTML such as nested interactive elements.

## Naming Conventions

- React components: `PascalCase`.
- Hooks: `useCamelCase`.
- TypeScript types and interfaces: `PascalCase`.
- Service functions and local variables: `camelCase`.
- .NET controllers: plural resource names ending in `Controller`.
- .NET services and repositories: interface plus implementation pairs, such as `IRosterService` and `RosterService`.
- DTO classes: end with `Dto`.
- EF entities: singular domain names, such as `Roster`, `RosterTask`, and `Housekeeper`.

## Error Handling

Frontend:

- Centralize HTTP error handling in `apiClient`.
- Throw meaningful `Error` objects from service functions.
- Show user-safe messages in forms and pages.
- Log developer diagnostics with `console.error` only where useful during failure handling.
- Do not swallow authentication failures silently; redirect or surface a clear state.

Backend:

- Return appropriate HTTP status codes from controllers.
- Validate request models before creating or updating data.
- Use `BadRequest` for validation errors, `Unauthorized` for auth failures, `NotFound` for missing resources, and `StatusCode(500)` only for unexpected failures.
- Prefer structured response objects such as `{ error = "..." }` for errors consumed by the frontend.
- Avoid leaking sensitive exception details in production responses.

## Environment Configuration

Frontend:

- `NEXT_PUBLIC_API_BASE`: base URL for the main roster API.
- `NEXT_PUBLIC_AUTH_BASE`: base URL for the authentication API. Falls back to `NEXT_PUBLIC_API_BASE` if unset.

Main API:

- `ConnectionStrings:DefaultConnection`: PostgreSQL connection string.
- `Jwt:Issuer`, `Jwt:Audience`, `Jwt:Key`: must match the auth API.

Auth API:

- `ConnectionStrings:DefaultConnection`: PostgreSQL connection string for Identity and refresh token data.
- `Jwt:Issuer`, `Jwt:Audience`, `Jwt:Key`, `Jwt:ExpireMinutes`, `Jwt:RefreshExpireDays`.

Do not commit real secrets. Use local `appsettings.Development.json`, user secrets, or environment variables for sensitive values.

## Build And Run

Frontend:

```powershell
cd C:\Users\tndni\OneDrive\Documents\repo\roaster-housekeeper\roster-web-app
npm install
npm run dev
npm run lint
npm run build
```

Main API:

```powershell
cd C:\Users\tndni\OneDrive\Documents\repo\roaster-housekeeper\roster-api-app\roster-api-app
dotnet restore
dotnet build
dotnet run
```

Auth API:

```powershell
cd C:\Users\tndni\OneDrive\Documents\repo\roaster-housekeeper\roster-auth-app\roster-auth-app
dotnet restore
dotnet build
dotnet run
```

Run PostgreSQL before starting APIs. Verify that CORS origins include the frontend origin used during development.

## Best Practices For Future AI Code Generation

- Read the existing files before editing.
- Make the smallest production-ready change that fits the current architecture.
- Do not scaffold a replacement frontend, API, database layer, auth system, or design system.
- Reuse `apiClient`, `authService`, existing DTOs, repositories, services, and UI primitives.
- Preserve strict TypeScript and avoid new lint errors.
- Keep SSR and hydration deterministic:
  - no browser-only APIs during render,
  - no random/time-dependent values in initial markup,
  - no theme-dependent conditional markup before mount,
  - no invalid client/server component imports,
  - no dynamic class generation that differs between server and client.
- Prefer typed DTOs and explicit interfaces over loosely shaped objects.
- Keep API routes and frontend service paths synchronized.
- Add tests or manual verification notes when changing shared behavior.
- Document any discovered mismatch between frontend expectations and backend contracts before expanding the feature.
