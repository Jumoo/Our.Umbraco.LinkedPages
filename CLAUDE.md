# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

`Our.Umbraco.LinkedPages` (packaged as `Umbraco.Community.LinkedPages`) adds a context-menu
option for managing relations between content nodes in the Umbraco backoffice.

Two halves: the C# package (`src/Umbraco.Community.LinkedPages`), and a backoffice client
(TypeScript, Lit, Vite) under `src/Umbraco.Community.LinkedPages/Client`.

`src/Our.Umbraco.LinkedPages` is a legacy project targeting Umbraco v8/v9 (net6.0). It ships
its own package (`Our.Umbraco.LinkedPages`) and is not part of the v18 CI pipeline.

## Commands

The client has to be built before the package - it produces `wwwroot`, which is gitignored,
so a fresh clone has no backoffice assets until you build it.

```bash
npm ci --prefix src/Umbraco.Community.LinkedPages/Client
```

```bash
npm run build --prefix src/Umbraco.Community.LinkedPages/Client
```

```bash
dotnet build src/Umbraco.Community.LinkedPages/Umbraco.Community.LinkedPages.csproj -c Release
```

There are no automated tests. Verification is: dotnet build clean, `tsc` (run as part of
`npm run build`) - and for anything user facing, a click-through in a running backoffice
(`src/LinkedPages.Site`, gitignored, a local test site).

## Repository shape

**Branches are per Umbraco major** - `v18/main` is the default branch and the current
release line. Workflow filters use `[ "main", "*/main" ]` and GitVersion uses
`^(v[0-9]+\/)?main$`, so both forms work and the next major needs no CI change.

**Build and pack the project, never a solution.** `.gitignore` excludes `src/LinkedPages.Site`
and `src/dist` - a local `.slnx` or the `Site` folder is a local test setup, not something CI
sees. CI builds `src/Umbraco.Community.LinkedPages/Umbraco.Community.LinkedPages.csproj`
directly.

**Versioning**: the package was released manually (via `src/dist/buildpackage.ps1`) up to
and including 18.0.3. CI-driven releases pick up from `GitVersion.yml`'s `next-version`,
which is kept above the highest version already on nuget.org - don't reset it back below
that without checking nuget.org first.

`directory.build.props` (repo root, lowercase) applies to every project under `src`,
including the legacy v8/v9 package and the local test site - `NuGetAuditMode` and
`RestorePackagesWithLockFile` are set there deliberately so local and CI builds agree.

## Things that will catch you out

**Two packages, two audiences.** `Umbraco.Community.LinkedPages` (v18, this branch) and
`Our.Umbraco.LinkedPages` (legacy v8/v9) are separate NuGet packages with separate ids -
don't conflate their versioning or changelog entries.

**Line endings.** `.gitattributes` line-ending rules apply - check formatting locally
before assuming a CI failure is a real formatting issue.

**Version numbers** come from `GitVersion.yml` and are stamped onto the package via
`dotnet pack /p:version=` in CI. Don't hardcode a version in the csproj.
