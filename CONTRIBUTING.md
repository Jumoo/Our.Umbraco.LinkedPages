# Contributing to Our.Umbraco.LinkedPages

Thanks for taking the time to contribute! The following is a set of guidelines
for contributing to this project.

## Code of Conduct

This project and everyone participating in it is governed by the
[Code of Conduct](CODE_OF_CONDUCT.md). By participating, you are expected to
uphold this code.

## Reporting Issues

Before creating a bug report, please check the existing
[issues](https://github.com/Jumoo/Our.Umbraco.LinkedPages/issues) to see if
the problem has already been reported. When filing an issue, please include:

* The version of Umbraco you are using
* The version of this package you are using
* Steps to reproduce the issue
* What you expected to happen and what actually happened

## Suggesting Enhancements

Feature requests are welcome. Please open an issue describing the enhancement,
why it would be useful, and any alternative solutions you've considered.

## Pull Requests

1. Fork the repository and create your branch from `v18/main` (or the
   relevant version branch).
2. Make your changes, following the existing code style.
3. Ensure the solution builds successfully (`dotnet build`).
4. Update documentation (`README.md`) if relevant.
5. Submit a pull request describing your changes and referencing any related
   issues.

## Development Setup

The solution is at `src/LinkedPages.slnx` and contains:

* `Umbraco.Community.LinkedPages` - the package project
* `LinkedPages.Site` - a test Umbraco site used to run/debug the package

The backoffice client lives under
`src/Umbraco.Community.LinkedPages/Client` and is built with Vite/TypeScript.
See the `Client` folder for build instructions (`npm install` / `npm run build`).

## Style Guidelines

* Follow the existing coding conventions found in the codebase.
* Keep pull requests focused on a single change where possible.

## License

By contributing, you agree that your contributions will be licensed under the
[MIT License](LICENSE).
