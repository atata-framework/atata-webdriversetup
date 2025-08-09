# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Changed

- Update Edge driver versions map.

## [3.5.0] - 2025-07-15

### Changed

- Update Edge driver versions map.

### Fixed

- Fix Edge driver download functionality.
  It was broken because of domain migration from msedgedriver.azureedge.net to msedgedriver.microsoft.com.

## [3.4.0] - 2025-07-12

### Changed

- Update Edge driver versions map.

## [3.3.0] - 2025-04-08

### Changed

- Update Edge driver versions map.

## [3.2.0] - 2025-01-16

### Changed

- Update Edge driver versions map.

## [3.1.0] - 2024-11-21

### Changed

- Improve Edge driver version resolution functionality to get from remote map the closest driver version withing the same major version when exact version number is missing in the map.

## [3.0.0] - 2024-11-20

### Added

- Add predefined map of Edge version/drivers.
- Add remote map of Edge version/drivers.

## [2.14.0] - 2024-10-09

### Changed

- Upgrade System.Text.Json package reference to v8.0.5.

### Fixed

- Fix fallback functionality for Edge driver that in case of failure of downloading auto/latest driver tries to download driver of the closest version.

## [2.13.0] - 2024-08-04

### Changed

- Improve fallback functionality for Edge driver that in case of failure of downloading auto/latest driver tries to download driver of the closest version.

## [2.12.0] - 2024-07-30

### Added

- Add fallback functionality for Edge driver that in case of failure of downloading auto/latest driver tries to download driver of the previous version.
  Fixes issue [#16 Edge web driver creation fails with "End of Central Directory record could not be found."](https://github.com/atata-framework/atata-webdriversetup/issues/16).

### Changed

- Improve error handling of driver downloading.

## [2.11.0] - 2024-07-18

### Changed

- Upgrade System.Text.Json package reference to v8.0.4.
- Add `ConfigureAwait(false)` to all `await` calls.

## [2.10.0] - 2024-02-15

### Fixed

- Fix downloading of Chrome driver starting v121.0.6167.85.
  Fixes issue [#15](https://github.com/atata-framework/atata-webdriversetup/issues/15).

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

[Unreleased]: https://github.com/atata-framework/atata-webdriversetup/compare/v3.5.0...HEAD
[3.5.0]: https://github.com/atata-framework/atata-webdriversetup/compare/v3.4.0...v3.5.0
[3.4.0]: https://github.com/atata-framework/atata-webdriversetup/compare/v3.3.0...v3.4.0
[3.3.0]: https://github.com/atata-framework/atata-webdriversetup/compare/v3.2.0...v3.3.0
[3.2.0]: https://github.com/atata-framework/atata-webdriversetup/compare/v3.1.0...v3.2.0
[3.1.0]: https://github.com/atata-framework/atata-webdriversetup/compare/v3.0.0...v3.1.0
[3.0.0]: https://github.com/atata-framework/atata-webdriversetup/compare/v2.14.0...v3.0.0
[2.14.0]: https://github.com/atata-framework/atata-webdriversetup/compare/v2.13.0...v2.14.0
[2.13.0]: https://github.com/atata-framework/atata-webdriversetup/compare/v2.12.0...v2.13.0
[2.12.0]: https://github.com/atata-framework/atata-webdriversetup/compare/v2.11.0...v2.12.0
[2.11.0]: https://github.com/atata-framework/atata-webdriversetup/compare/v2.10.0...v2.11.0
[2.10.0]: https://github.com/atata-framework/atata-webdriversetup/compare/v2.9.0...v2.10.0
[2.9.0]: https://github.com/atata-framework/atata-webdriversetup/compare/v2.8.0...v2.9.0
[2.8.0]: https://github.com/atata-framework/atata-webdriversetup/compare/v2.7.0...v2.8.0
[2.7.0]: https://github.com/atata-framework/atata-webdriversetup/compare/v2.6.0...v2.7.0
[2.6.0]: https://github.com/atata-framework/atata-webdriversetup/compare/v2.5.0...v2.6.0
[2.5.0]: https://github.com/atata-framework/atata-webdriversetup/compare/v2.4.0...v2.5.0
[2.4.0]: https://github.com/atata-framework/atata-webdriversetup/compare/v2.3.0...v2.4.0
[2.3.0]: https://github.com/atata-framework/atata-webdriversetup/compare/v2.2.0...v2.3.0
[2.2.0]: https://github.com/atata-framework/atata-webdriversetup/compare/v2.1.0...v2.2.0
[2.1.0]: https://github.com/atata-framework/atata-webdriversetup/compare/v2.0.0...v2.1.0
[2.0.0]: https://github.com/atata-framework/atata-webdriversetup/compare/v1.2.1...v2.0.0
[1.2.1]: https://github.com/atata-framework/atata-webdriversetup/compare/v1.2.0...v1.2.1
[1.2.0]: https://github.com/atata-framework/atata-webdriversetup/compare/v1.1.0...v1.2.0
[1.1.0]: https://github.com/atata-framework/atata-webdriversetup/compare/v1.0.0...v1.1.0
[1.0.0]: https://github.com/atata-framework/atata-webdriversetup/releases/tag/v1.0.0
