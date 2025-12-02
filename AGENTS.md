# Repository Guidelines

## Project Structure & Module Organization
- Root: this repository contains the .NET 10 CLI in `src/okai`.
- `src/okai/Program.cs` (+ services under `src/okai/`): entry point, DI setup, chat loop, handlers.
- Build artifacts land in `src/okai/bin` and `src/okai/obj`; keep these out of commits.

## Build, Test, and Development Commands
- Restore/build: `dotnet build src/okai/okai.csproj` (restores NuGet packages and compiles).
- Run locally: `dotnet run --project src/okai/okai.csproj` (oder `dnx run --project src/okai/okai.csproj`).
- Tests: `dotnet test tests/okai.Tests/okai.Tests.csproj`.

## Coding Style & Naming Conventions
- Language: C# 12/13 (net10.0) with nullable enabled and implicit usings on.
- Follow standard .NET naming: PascalCase for types/methods, camelCase for locals/fields, UPPER_SNAKE for constants.
- Follow clean code principles (KISS, DRY); avoid anti-patterns like magic values-prefer named constants and small, focused methods.
- Aim for CLEAN-style code: clear intent, low coupling, encapsulated invariants, avoid needless abstraction, and keep duplication minimal and purposeful (especially in tests).
- Keep single responsibility per class/method, use explicit error handling (no silent swallowing), and prefer readable names over abbreviations; keep methods short.
- Follow common .NET architecture principles (clear layers/boundaries, dependency injection, SOLID; avoid god objects and tight coupling).
- Formatting: prefer `dotnet format` before committing; keep lines reasonably short and avoid unnecessary comments. Run `dotnet format` on every change set manually and do not auto-commit/auto-stage the results.
- DI & Lifetimes: choose service lifetimes deliberately (avoid mutable state in singletons), prefer constructor injection, and keep container setup minimal.
- Validation & Guards: validate inputs early (null/empty, paths, ranges) with clear argument guards; fail fast with actionable errors.
- Resources & Cleanup: respect `IDisposable`, avoid leaking streams/clients/processes; tests and tools must clean up temp files and dirs.

## Testing Guidelines
- Framework: xUnit under `tests/okai.Tests`.
- Name tests after behavior (e.g., `CompletesChat_WhenToolCallSucceeds`).
- Whenever functionality changes or new behavior is added, add or update tests to cover it.
- Run `dotnet test` and include results in PRs.

## Commit & Pull Request Guidelines
- Commits: concise, imperative subject lines (e.g., `Add chat tool wiring`, `Fix path resolution`). Group related changes; avoid mixing refactors with feature work.
- PRs: include a short summary, key changes, manual test notes (commands run, env vars), and any screenshots/log snippets if UX or output changes.
- Link to relevant issues/work items when available; call out breaking changes or new configuration requirements.

## Security & Configuration Tips
- Required env vars to run: `AZURE_AI_PROJECT_ENDPOINT` (Foundry project endpoint), `AZURE_AI_MODEL` (e.g., `gpt-4o-mini`, optional), `AZURE_AI_ROOT` (optional root for file tools; defaults to current directory).
- Never commit secrets or connection strings. Use user-specific env vars or local `dotnet user-secrets` if you add sensitive config.
- Apply least-privilege for service principals/keys; do not log or echo secrets and scrub them from test fixtures or samples.
- Keep approval/history/theme config files (`.okai_approvals.json`, `.okai_history.json`, themes) within the repo root you intend; avoid writing tool outputs outside the configured root.
- File tools are restricted to the configured root; keep relative paths inside the repo to avoid unintended writes.
- Optional UX env vars: `OKAI_THEME` (default: `default`; options include `default`, `highcontrast`, `solarized`, `tomorrownightblue`, `vsdark`/`dark+`, `vslight`/`light+`, `vshighcontrast`). Profiles in `okai.config.json` can also set `"theme"`. Theme definitions load from `OKAI_THEMES_PATH` (default: `~/.okai/okai.themes.json`); a sample `okai.themes.json` is included. Shell selection via `OKAI_SHELL` (`cmd` default; `powershell` or `sh` supported).
- Content under `Ignore/` must always be analyzed when relevant but should never be modified.
