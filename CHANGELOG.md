# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Installed Firefox browser version auto-detect and resolve of corresponding driver version.

### Changed

- Catch possible exceptions of `Registry.GetValue()` method call and return `null`.
- Catch possible exceptions of `FileVersionInfo.GetVersionInfo()` method call and return `null`.
- Update `AppVersionDetector.GetThroughOSXApplicationCli` method to handle possible `null` result of `GetThroughCli` method.
- Update `AppVersionDetector.GetThroughCli` method to trim result string.

## [2.3.0] - 2022-07-26

### Changed

- Update `HttpRequestExecutor` to use `HttpClient` instead of `WebClient`.

### Fixed

- Fix unzipping of ".tar.gz" archives. Fixes Firefox driver setup for .NET 6.0 on Linux and macOS.

## [2.2.0] - 2022-07-21

### Changed

- Upgrade Atata.Cli package to v2.1.0.

## [2.1.0] - 2022-05-24

### Added

- Add `bool UseMutex` option to use mutex for driver setup sync
  and `WithMutex(bool isEnabled)` configuration method.
  This functionality is disabled by default.

## [2.0.0] - 2022-05-11

### Added

- Add support of Edge driver on Linux.

### Changed

- Upgrade Atata.Cli package to v2.0.0.

## [1.2.1] - 2021-10-26

### Fixed

- #10 Thread synchronization bug when setting up multiple drivers simultaneously.

## [1.2.0] - 2021-07-30

### Added

- #7 Add Atata.Cli package reference.
- #8 Add functionality of installed Chrome version detection for Linux and macOS.

## [1.1.0] - 2021-02-22

### Added

- #2 Add configuration to specify x32/x64 architecture.
- #3 Default driver setup configurations.
- #4 Add async `SetUp*` methods.

## [1.0.0] - 2021-02-16

Initial version release.