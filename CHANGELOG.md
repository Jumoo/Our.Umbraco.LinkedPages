# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Standard open-source repository documentation (LICENSE, CONTRIBUTING,
  CODE_OF_CONDUCT, SECURITY, issue/PR templates, CI workflow).
- Standard repository tooling to match other Jumoo package repos:
  `.editorconfig`, `global.json`, `GitVersion.yml`, `CLAUDE.md`, and a
  `packages.lock.json` for the package project.
- CodeQL scanning, Dependabot (NuGet, npm, GitHub Actions), and a split
  CI pipeline (`dotnet-build.yml` for PRs, `package-build.yml` for review
  builds on push, `release.yml` for tag-triggered NuGet releases via
  trusted publishing), replacing the single `build.yml` workflow.

## [18.0.0] - 2026-06-30

### Added

- Updated package to support Umbraco 18.

## [17.0.0] - 2026-06-15

### Added

- Created a version 17 compatible version of Linked Pages.

### Changed

- Visual improvements to the Linked Pages backoffice UI.

## [10.0.3] - 2024-01-17

### Fixed

- Ensured a sensible default is used when the relation type config is blank.
- Ensured the relation type is read correctly from config.

### Changed

- Bumped the maximum supported Umbraco version.
- Removed unused gulp scripts (not needed for the RCL-based package).
- Updated `umbraco-marketplace.json`.
- Moved the repository.

## [10.0.0] - 2022-12-02

### Added

- Stable v10/v11 release of Linked Pages.
- Shared strings moved to a constants class to avoid typos.
- Use of a constant for the admin group name.

### Changed

- Renamed `Componet` to `Component`.
- Updated comments to note the correct target framework.

### Security

- Bumped `Microsoft.Owin` from 4.0.1 to 4.1.1 in `LinkedPages.EightSite`.
- Bumped `Newtonsoft.Json` from 12.0.1 to 13.0.1 in `LinkedPages.EightSite`.
- Bumped `SharpZipLib` from 0.86.0 to 1.3.3 in `LinkedPages.EightSite`.

## [9.0.0-beta001] - 2021-07-13

### Added

- Initial beta release of Linked Pages for Umbraco v9.

[Latest]: https://github.com/Jumoo/Our.Umbraco.LinkedPages/compare/release/18.0.0...v18/main
[18.0.0]: https://github.com/Jumoo/Our.Umbraco.LinkedPages/compare/release/17.0.0...release/18.0.0
[17.0.0]: https://github.com/Jumoo/Our.Umbraco.LinkedPages/compare/release/10.0.3...release/17.0.0
[10.0.3]: https://github.com/Jumoo/Our.Umbraco.LinkedPages/compare/release/10.0.0...release/10.0.3
[10.0.0]: https://github.com/Jumoo/Our.Umbraco.LinkedPages/compare/9.0.0-beta001...release/10.0.0
[9.0.0-beta001]: https://github.com/Jumoo/Our.Umbraco.LinkedPages/releases/tag/9.0.0-beta001
