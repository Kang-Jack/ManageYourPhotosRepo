# Project Context

## Purpose
ManageYourPhotosRepo ("foto_list") is a small command-line utility and supporting libraries to help locate, compare, and manage photo files across file systems. It includes a production library (`foto_list`), a console program (`Program.cs`) and a test project (`mtest`) with unit tests that exercise file-system operations, move/clean modes, and cross-platform behaviors.

This repository's goals:
- Provide reliable file discovery and movement utilities for photo organization workflows.
- Offer a well-tested reference implementation to validate file system operations across platforms.
- Be easy to extend and integrate into other tools or CI scripts.

## Tech Stack
- Primary language: C# targeting .NET 6 (net6.0)
- Solution: `foto_list.sln`
- Projects:
	- `foto_list/` — main library and console app (includes `FotoManager`, `FileSystem` abstractions)
	- `mtest/` — xUnit test project containing unit tests
- Build system: dotnet CLI (SDK 6.x)
- CI-friendly: tests are small and fast; easily run with `dotnet test`.

## Project Conventions

### Code Style
- Follow idiomatic C# conventions (PascalCase for public types/methods, camelCase for private fields and parameters).
- Use 4-space indentation.
- Keep methods focused and small; prefer expressing intent with well-named methods.
- XML doc comments are used where an API is public; internal/private methods may omit them.
- Formatting: rely on `dotnet format` or IDE defaults (Visual Studio/VS Code C# extension) for automatic formatting.

### Architecture Patterns
- Project is structured as a small library plus a console app. The library exposes core operations via `IFotoManger` and `IFileSystem` interfaces to allow testing with fakes/mocks.
- Separation of concerns:
	- File system access is abstracted behind `IFileSystem` to allow tests to simulate errors and different platforms.
	- Foto management logic lives in `FotoManager` and `FotoManagerUtils`.
- Keep business logic free of direct Console/IO calls; the `Program.cs` is the thin CLI wiring layer.

### Testing Strategy
- Tests live in the `mtest` project and use xUnit.
- Focus areas:
	- Core operations on `FotoManager` (file discovery, diff reports, move/clean modes).
	- File system error handling via `IFileSystem` fakes and error scenarios.
	- Concurrent operations and cross-platform path behavior.
- Tests should be fast and hermetic. Avoid long-running IO to the real file system when possible; use fakes/mocks provided in the test suite.

### Git Workflow
- Branching: use `main` for stable code. Create feature branches named `feat/<short-description>` and bug branches `fix/<short-description>`.
- Commits: use concise imperative messages, e.g. "Add unit tests for file move logic".
- PRs: include a short description, link to related issue (if any), and mention tests added/changed.

## Domain Context
- The tool operates on file systems that may include photos organized by directories, dates, or camera-generated folders.
- The code assumes file metadata (names, timestamps, sizes) are the primary signals used to detect duplicates or moved files. EXIF extraction is not a core part of the current project.
- Cross-platform file path differences (backslash vs forward slash, case-sensitivity) are considered in tests — see `UnitTestCrossPlatform.cs` for examples.

## Important Constraints
- Targets .NET 6 (net6.0). If upgrading the target framework, update projects and CI accordingly.
- Avoid depending on native/unmanaged libraries unless wrapped and optional.
- Keep runtime-only dependencies minimal; prefer using built-in BCL features where possible.

## External Dependencies
- NuGet packages are managed through the project files and `NuGet.config`.
- There are no mandatory external web services required to build or test the repository.

## Build, Test and Run
- Build:
	- Install .NET SDK 6.x.
	- From repository root: `dotnet build` (will build both `foto_list` and `mtest`).
- Test:
	- Run `dotnet test` from the repo root or from `mtest/`.
	- Tests are xUnit-based and should run quickly.
- Run CLI (example):
	- From `foto_list/` run `dotnet run --project foto_list -- <args>` or build the Release binary in `foto_list/bin/Release/net6.0/` and execute it directly.

## Cross-platform support (Windows / Linux / macOS)

- The codebase targets .NET 6 which is cross-platform. Ensure developers have the .NET 6 SDK installed for their OS.
- Platform-specific considerations:
  - Path separators and normalization: prefer using `Path.Combine`, `Path.GetFullPath`, and `Path.DirectorySeparatorChar` rather than string manipulation.
  - Case-sensitivity: tests and logic should treat file names carefully; Windows is case-insensitive while many Linux systems are case-sensitive. Use canonical normalization in tests where appropriate.
  - File locks and concurrent access semantics differ by OS; unit tests that simulate concurrency should use `IFileSystem` fakes rather than relying on real file locks.
  - Line endings are not critical for binaries, but if editing text files or generating reports, normalize line endings using Environment.NewLine or explicit normalization in tests.
- Testing on each platform: CI or contributors should run `dotnet test` on Windows, Linux, and macOS runners when possible. Use GitHub Actions matrix builds to cover platform differences.

## Future: Web service migration plan

This project is currently a CLI and library. If you plan to convert it into a web service in the future, follow this incremental migration plan:

1. Prepare the library boundary
	- Ensure all business logic is encapsulated in the `foto_list` library and exposed via interfaces (existing `IFotoManger`, `IFileSystem`). The CLI (`Program.cs`) should only do argument parsing and call library services.
2. Add HTTP-friendly service layer
	- Create an ASP.NET Core Web API project (target net6.0 or later) in the solution, e.g., `foto_list.WebApi`.
	- Implement thin controllers that call into the `foto_list` library. Keep controllers minimal; validation, mapping, and error handling should be clear and tested.
3. Design API contracts
	- Define REST endpoints for key operations (list, compare, move, dry-run, status). Prefer JSON payloads and clear error responses (use Problem Details RFC 7807).
	- Consider adding OpenAPI/Swagger for discoverability and faster client integration.
4. Security and multi-tenant considerations
	- Implement authentication/authorization (e.g., cookie, JWT, API keys) depending on deployment needs.
	- Carefully design file-system access controls — do not allow arbitrary paths; use configured roots and permission checks.
5. Background processing and long-running tasks
	- Large move/compare operations should be performed asynchronously. Consider a job queue (in-memory for single instance, or backed by Redis/RabbitMQ for scaling).
	- Offer job status endpoints and webhooks or polling for completion.
6. Containerization and deployment
	- Containerize the Web API with a Dockerfile and use multi-stage builds to produce small images.
	- Provide Kubernetes manifests or simple Docker Compose examples for local testing.
7. Observability and monitoring
	- Add structured logging (Microsoft.Extensions.Logging), health checks, and metrics (Prometheus, Application Insights) as needed.
8. Migration testing
	- Reuse existing unit tests for the library. Add integration tests for the Web API (using WebApplicationFactory and test server).

Migration checklist (short):
- [ ] Create `foto_list.WebApi` project and add to solution
- [ ] Add API surface (controllers) mapped to `IFotoManger` operations
- [ ] Implement authentication and safe path configuration
- [ ] Add background job processing for long tasks
- [ ] Add Dockerfile and CI pipeline to build and publish images
- [ ] Add end-to-end integration tests

Notes:
- Keep file-system access restricted by configuration to avoid arbitrary path traversal. Use a 'roots' whitelist and validate input paths before acting.
- Consider exposing a read-only mode and a dry-run endpoint to preview operations without changing files.


## Contribution Notes
- Run tests locally before opening a PR: `dotnet test`.
- Follow code style and keep public API changes backwards compatible where possible.
- Add unit tests for new behaviors and update `mtest` accordingly.

## Where to look in the repo
- Main code: `foto_list/` (implements `IFotoManger`, `IFileSystem`, `FotoManager`, and utilities)
- Tests: `mtest/` (unit tests exercising various behaviors)
- Solution file: `foto_list.sln`

---

If you'd like, I can also:
- run a quick `dotnet test` and report results, or
- create a CONTRIBUTING.md and CODE_STYLE.md from these conventions.
