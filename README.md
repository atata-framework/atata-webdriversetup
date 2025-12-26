# Atata.WebDriverSetup

[![NuGet](http://img.shields.io/nuget/v/Atata.WebDriverSetup.svg?style=flat)](https://www.nuget.org/packages/Atata.WebDriverSetup/)
[![GitHub release](https://img.shields.io/github/release/atata-framework/atata-webdriversetup.svg)](https://github.com/atata-framework/atata-webdriversetup/releases)
[![Build status](https://dev.azure.com/atata-framework/atata-webdriversetup/_apis/build/status/atata-webdriversetup-ci?branchName=main)](https://dev.azure.com/atata-framework/atata-webdriversetup/_build/latest?definitionId=39&branchName=main)
[![Atata Templates](https://img.shields.io/badge/get-Atata_Templates-green.svg?color=4BC21F)](https://marketplace.visualstudio.com/items?itemName=YevgeniyShunevych.AtataTemplates)\
[![Slack](https://img.shields.io/badge/join-Slack-green.svg?colorB=4EB898)](https://join.slack.com/t/atata-framework/shared_invite/zt-5j3lyln7-WD1ZtMDzXBhPm0yXLDBzbA)
[![Atata docs](https://img.shields.io/badge/docs-Atata_Framework-orange.svg)](https://atata.io)
[![X](https://img.shields.io/badge/follow-@AtataFramework-blue.svg)](https://x.com/AtataFramework)

**Atata.WebDriverSetup** is a C#/.NET library that sets up browser drivers for Selenium WebDriver,
e.g. `chromedriver`, `geckodriver`, etc.
Basically, it provides functionality similar to Selenium Manager or Java `WebDriverManager`.

*The package targets .NET 8.0 and .NET Framework 4.6.2.*

## Table of contents

- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
  - [Set up version corresponding to locally installed browser version](#set-up-version-corresponding-to-locally-installed-browser-version)
  - [Set up latest version](#set-up-latest-version)
  - [Set up specific version](#set-up-specific-version)
  - [Set up version corresponding to specific browser version](#set-up-version-corresponding-to-specific-browser-version)
- [DriverSetup members](#driversetup-members)
  - [DriverSetup properties](#driversetup-properties)
  - [DriverSetup methods](#driversetup-methods)
- [Configuration](#configuration)
  - [Global configuration](#global-configuration)
  - [Driver-specific configuration](#driver-specific-configuration)
  - [Configuration methods](#configuration-methods)
  - [Handle HTTPS certificate errors](#handle-https-certificate-errors)
- [BrowserDetector](#browserdetector)
  - [BrowserDetector methods](#browserdetector-methods)
  - [BrowserDetector usage](#browserdetector-usage)
- [Feedback](#feedback)
- [SemVer](#semver)
- [License](#license)

## Features

- Sets up drivers for browsers: Chrome, Firefox, Edge, Internet Explorer and Opera.
- Supports Windows, Linux and macOS platforms.
- Can download latest or specific driver versions.
- Can auto-detect locally installed browser version and download corresponding driver version.
- Caches the used driver versions.
- After a driver is set up, adds the driver path to environment "PATH" variable, which is consumed by WebDriver.

## Installation

Install [`Atata.WebDriverSetup`](https://www.nuget.org/packages/Atata.WebDriverSetup/) NuGet package.

- Package Manager:
  ```
  Install-Package Atata.WebDriverSetup
  ```

- .NET CLI:
  ```
  dotnet add package Atata.WebDriverSetup
  ```

The package depends on:

- [`Microsoft.Win32.Registry`](https://www.nuget.org/packages/Microsoft.Win32.Registry/) - is used to detect locally installed browser version through Windows Registry.
- [`Atata.Cli`](https://www.nuget.org/packages/Atata.Cli/) - is used to detect locally installed browser version through CLI on Linux and macOS.
- [`System.Text.Json`](https://www.nuget.org/packages/System.Text.Json/) - is used to read JSON HTTP responses.

## Usage

The main class is `DriverSetup`.
The recommended place to perform driver(s) setup is a global set up method.

NUnit example:

```cs
[SetUpFixture]
public class SetUpFixture
{
    [OneTimeSetUp]
    public async Task SetUp()
    {
        await DriverSetup.AutoSetUpAsync(BrowserNames.Chrome);
    }
}
```

After a driver is set up, the driver path is added to environment "PATH" variable,
which is read by WebDriver's DriverService classes.

```cs
ChromeDriver chromeDriver = new ChromeDriver();
```

### Set up version corresponding to locally installed browser version

1. Configure with default configuration options:
   ```cs
   await DriverSetup.AutoSetUpAsync(BrowserNames.Chrome);
   ```
1. Configure with custom configuration options:
   ```cs
   await DriverSetup.ConfigureChrome()
       .WithAutoVersion()
       // Additional options can be set here.
       .SetUpAsync();
   ```

`DriverSetup.AutoSetUpAsync` method also supports multiple drivers setup:

```cs
DriverSetup.AutoSetUpAsync([BrowserNames.Chrome, BrowserNames.Edge]);
```

**Note:**
If the version of browser cannot be detected automatically, latest driver version is used.
Version auto-detection is currently supported for Chrome, Firefox and Edge browsers.

### Set up latest version

```cs
await DriverSetup.ConfigureChrome()
    .WithLatestVersion()
    .SetUpAsync();
```

### Set up specific version

```cs
await DriverSetup.ConfigureChrome()
    .WithVersion("87.0.4280.88")
    .SetUpAsync();
```

**Version format:**
- Chrome: `"87.0.4280.88"`
- Firefox: `"0.28.0"`
- Edge: `"89.0.774.4"`
- Opera: `"86.0.4240.80"`
- InternetExplorer: `"3.141.59"`

### Set up version corresponding to specific browser version

```cs
await DriverSetup.ConfigureChrome()
    .ByBrowserVersion("87")
    .SetUpAsync();
```

**Note:**
This feature is currently supported for Chrome, Firefox and Edge browsers.

**Version format:**
- Chrome: `"87"` or `"87.0.4280"`
- Firefox: `"104"`, `"104.0"` or `"104.0.1"`
- Edge: `"89.0.774.4"`

## DriverSetup members

`DriverSetup` is a static class, so all its members are static too.

### DriverSetup properties

- **`DriverSetupOptions GlobalOptions { get; }`**\
  Gets the global setup options.
- **`DriverSetupOptionsBuilder GlobalConfiguration { get; }`**\
  Gets the global setup configuration builder.
  Configures `GlobalOptions`.
- **`List<DriverSetupConfigurationBuilder> PendingConfigurations { get; }`**\
  Gets the pending driver setup configurations,
  the configurations that were created but were not set up.

### DriverSetup methods

- **`DriverSetupConfigurationBuilder ConfigureChrome()`**\
  Creates the Chrome driver setup configuration builder.
- **`DriverSetupConfigurationBuilder ConfigureFirefox()`**\
  Creates the Firefox/Gecko driver setup configuration builder.
- **`DriverSetupConfigurationBuilder ConfigureEdge()`**\
  Creates the Edge driver setup configuration builder.
- **`DriverSetupConfigurationBuilder ConfigureOpera()`**\
  Creates the Opera driver setup configuration builder.
- **`DriverSetupConfigurationBuilder ConfigureInternetExplorer()`**\
  Creates the Internet Explorer driver setup configuration builder.
- **`DriverSetupConfigurationBuilder Configure(string browserName)`**\
  Creates the driver setup configuration builder for the specified `browserName`.
  Supported browser names are defined in `BrowserNames` static class.
- **`DriverSetupConfigurationBuilder Configure(string browserName, Func<IHttpRequestExecutor, IDriverSetupStrategy> driverSetupStrategyFactory)`**\
  Creates the driver setup configuration builder using `driverSetupStrategyFactory`
  that instantiates specific `IDriverSetupStrategy`.
- **`DriverSetupResult AutoSetUp(string browserName)`** &\
  **`Task<DriverSetupResult> AutoSetUpAsync(string browserName, CancellationToken cancellationToken = default)`**\
  Sets up driver with auto version detection for the browser with the specified name.
  Supported browser names are defined in `BrowserNames` static class.
- **`DriverSetupResult[] AutoSetUp(params string[] browserNames)`** &\
  **`DriverSetupResult[] AutoSetUp(IEnumerable<string> browserNames)`** &\
  **`Task<DriverSetupResult[]> AutoSetUpAsync(IEnumerable<string> browserNames, CancellationToken cancellationToken = default)`**\
  Sets up drivers with auto version detection for the browsers with the specified names.
  Supported browser names are defined in `BrowserNames` static class.
- **`DriverSetupResult[] AutoSetUpSafely(IEnumerable<string> browserNames)`** &\
  **`Task<DriverSetupResult[]> AutoSetUpSafelyAsync(IEnumerable<string> browserNames, CancellationToken cancellationToken = default)`**\
  Sets up drivers with auto version detection for the browsers with the specified names.
  Supported browser names are defined in `BrowserNames` static class.
  Skips invalid/unsupported browser names.
- **`DriverSetupOptionsBuilder GetDefaultConfiguration(string browserName)`**\
  Gets the default driver setup configuration builder.
- **`DriverSetupResult[] SetUpPendingConfigurations()`** &\
  **`Task<DriverSetupResult[]> SetUpPendingConfigurationsAsync(CancellationToken cancellationToken = default)`**\
  Sets up pending configurations that are stored in `PendingConfigurations` property.
- **`void RegisterStrategyFactory(string browserName, Func<IHttpRequestExecutor, IDriverSetupStrategy> driverSetupStrategyFactory)`**\
  Registers the driver setup strategy factory.

## Configuration

It's possible to set configuration options globally and separately for a specific driver.

### Global configuration

#### Using fluent builder

```cs
DriverSetup.GlobalConfiguration
    .WithStorageDirectoryPath("...")
    .WithVersionCache(false);
```

#### Using options properties

```cs
DriverSetup.GlobalOptions.StorageDirectoryPath = "...";
DriverSetup.GlobalOptions.UseVersionCache = false;
```

### Default driver configuration

```cs
DriverSetup.GetDefaultConfiguration(BrowserNames.InternetExplorer)
    .WithX32Architecture();
```

### Driver-specific configuration

```cs
await DriverSetup.ConfigureChrome()
    .WithStorageDirectoryPath("...")
    .WithVersionCache(false)
    .SetUpAsync();
```

*Don't forget to call `SetUpAsync()` or `SetUp()` at the end.*

### Configuration methods

#### Driver-specific configuration methods

- **`WithAutoVersion()`**\
  Sets the automatic driver version detection by installed browser version.
  If the version cannot be detected automatically, latest driver version should be used.
- **`WithLatestVersion()`**\
  Sets the latest version of driver.
- **`ByBrowserVersion(string version)`**\
  Sets the browser version.
  It will find driver version corresponding to the browser version.
- **`WithVersion(string version)`**\
  Sets the version of driver to use.
- **`WithEnvironmentVariableName(string variableName)`**\
  Sets the name of the environment variable
  that will be set with a value equal to the driver directory path.
  The default value is specific to the driver being configured.
  It has `"{BrowserName}Driver"` format.
  For example: `"ChromeDriver"` or `"InternetExplorerDriver"`.
  The `null` value means that none variable should be set.

#### Common configuration methods

- **`WithStorageDirectoryPath(string path)`**\
  Sets the storage directory path.
  The default value is `"{basedir}/drivers")`.
- **`WithX32Architecture()`**\
  Sets the x32 (x86) architecture.
- **`WithX64Architecture()`**\
  Sets the x64 architecture.
- **`WithArm64Architecture()`**\
  Sets the ARM64 architecture.
- **`WithArchitecture(Architecture architecture)`**\
  Sets the architecture.
  The default value is `Architecture.Auto`.
- **`WithProxy(IWebProxy proxy)`**\
  Sets the web proxy.
- **`WithCheckCertificateRevocationList(bool checkCertificateRevocationList)`**\
  Sets a value indicating whether the certificate is automatically picked
  from the certificate store or if the caller is allowed to pass in a specific
  client certificate.
  The default value is `true`.
- **`WithHttpClientHandlerConfiguration(Action<HttpClientHandler> httpClientHandlerConfigurationAction)`**\
  Sets the configuration action of `HttpClientHandler`.
  The `HttpClientHandler` instance is used to get a driver version information
  and to download a driver archive.
- **`WithInterProcessSynchronization(bool isEnabled)`**\
  Sets a value indicating whether to use inter-process synchronization to synchronize driver setup across machine.
  The default value is `false`.
- **`WithVersionCache(bool isEnabled)`**\
  Sets a value indicating whether to use version cache.
  The default value is `true`.
- **`WithLatestVersionCheckInterval(TimeSpan interval)`**\
  Sets the latest version check interval.
  The default values is `2` hours.
- **`WithSpecificVersionCheckInterval(TimeSpan interval)`**\
  Sets the specific version check interval.
  The default values is `2` hours.
- **`WithHttpRequestTryCount(int count)`**\
  Sets the HTTP request try count.
  The default values is `3`.
- **`WithHttpRequestRetryInterval(TimeSpan interval)`**\
  Sets the HTTP request retry interval.
  The default values is `3` seconds.
- **`WithEnabledState(bool isEnabled)`**\
  Sets a value indicating whether the configuration is enabled.
  The default values is `true`.
- **`WithAddToEnvironmentPathVariable(bool isEnabled)`**\
  Sets a value indicating whether to add the driver directory path
  to environment "Path" variable.
  The default value is `true`.

### Handle HTTPS certificate errors

Rarely you can get HTTP certificate errors during driver setup.
In order to handle such errors you can try one or both of the configuration settings below.

```cs
DriverSetup.GlobalConfiguration
    .WithCheckCertificateRevocationList(false)
    .WithHttpClientHandlerConfiguration(x => x.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator);
```

## BrowserDetector

`BrowserDetector` - provides a set of static methods for a detection of browser installations.
Browser detection is supported for Chrome, Firefox and Edge,
so as a browser name the following constants can be used:

- `BrowserNames.Chrome`
- `BrowserNames.Firefox`
- `BrowserNames.Edge`

### BrowserDetector methods

- **`Task<string?> GetInstalledBrowserVersionAsync(string browserName, CancellationToken cancellationToken = default)`** &\
  **`string? GetInstalledBrowserVersion(string browserName)`**\
  Gets the installed browser version by the browser name.
- **`Task<bool> IsBrowserInstalledAsync(string browserName, CancellationToken cancellationToken = default)`** &\
  **`bool IsBrowserInstalled(string browserName)`**\
  Determines whether the browser with the specified name is installed.
- **`Task<string?> GetFirstInstalledBrowserNameAsync(IEnumerable<string> browserNames, CancellationToken cancellationToken = default)`** &\
  **`string? GetFirstInstalledBrowserName(IEnumerable<string> browserNames)`** &\
  **`string? GetFirstInstalledBrowserName(params string[] browserNames)`**\
 Gets the name of the first installed browser among the `browserNames`.

### BrowserDetector usage

#### Get first installed browser name

```cs
string? browserName = BrowserDetector.GetFirstInstalledBrowserName(
    BrowserNames.Chrome,
    BrowserNames.Firefox,
    BrowserNames.Edge);
```

#### Is browser installed

```cs
bool isChromeInstalled = BrowserDetector.IsBrowserInstalled(BrowserNames.Chrome);
```

#### Get installed browser version

```cs
string? chromeVersion = BrowserDetector.GetInstalledBrowserVersion(BrowserNames.Chrome);
```

## Community

- Slack: [https://atata-framework.slack.com](https://join.slack.com/t/atata-framework/shared_invite/zt-5j3lyln7-WD1ZtMDzXBhPm0yXLDBzbA)
- X: https://x.com/AtataFramework
- Stack Overflow: https://stackoverflow.com/questions/tagged/atata

## Feedback

Any feedback, issues and feature requests are welcome.

If you faced an issue please report it to [Atata.WebDriverSetup Issues](https://github.com/atata-framework/atata-webdriversetup/issues),
[ask a question on Stack Overflow](https://stackoverflow.com/questions/ask?tags=atata+csharp) using [atata](https://stackoverflow.com/questions/tagged/atata) tag
or use another [Atata Contact](https://atata.io/contact/) way.

## Contributing

Check out [Contributing Guidelines](CONTRIBUTING.md) for details.

## SemVer

Atata Framework follows [Semantic Versioning 2.0](https://semver.org/).
Thus backward compatibility is followed and updates within the same major version
(e.g. from 1.3 to 1.4) should not require code changes.

## License

Atata is an open source software, licensed under the Apache License 2.0.
See [LICENSE](LICENSE) for details.
