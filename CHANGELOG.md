# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.9.0] - 2023-10-16

### Changed

- Set "4.14.0" as the latest version of Internet Explorer driver.

## [2.8.0] - 2023-09-01

### Fixed

- Fix downloading of the latest Internet Explorer driver.

## [2.7.0] - 2023-07-19

### Added

- Add support of Chrome driver v115+.
  Fixes issue: [#14 Webdriver can't be downloaded for new chrome ver. 115 and higher](https://github.com/atata-framework/atata-webdriversetup/issues/14).

## [2.6.0] - 2023-05-02

### Added

- Add `bool CheckCertificateRevocationList` option to disable check of certificate revocation
  and `WithCheckCertificateRevocationList(bool)` configuration method.
- Add `Action<HttpClientHandler> HttpClientHandlerConfigurationAction` option to configure `HttpClientHandler`
  and `WithHttpClientHandlerConfiguration(Action<HttpClientHandler>)` configuration method.

### Fixed

- Fix downloading of Internet Explorer driver v4.8.1 (latest).

## [2.5.0] - 2023-01-20

### Added

- Add `Arm64` value to `Architecture` enum.
- Add `WithArm64Architecture` method to `DriverSetupOptionsBuilder<TBuilder, TContext>`.
- Add support of ARM64 driver versions for different operation systems for Chrome, Firefox and Edge.
- Add support of Internet Explorer driver v4.x.

## [2.4.0] - 2022-09-06

### Added

- Automatic detection of the installed version of the Firefox browser and determination of the corresponding driver version.
- Automatic detection of the installed version of the Edge browser on macOS and Linux.
- Add `BrowserDetector` static class for a detection of browser installations.

### Changed

- Catch possible exceptions of `Registry.GetValue()` method call and return `null`.
- Catch possible exceptions of `FileVersionInfo.GetVersionInfo()` method call and return `null`.
- Update `AppVersionDetector.GetThroughOSXApplicationCli` method to handle possible `null` result of `GetThroughCli` method.
- Update `AppVersionDetector.GetThroughCli` method to trim result string.
- Upgrade Atata.Cli package to v2.2.0.

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