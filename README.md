# Atata.WebDriverSetup

[![NuGet](http://img.shields.io/nuget/v/Atata.WebDriverSetup.svg?style=flat)](https://www.nuget.org/packages/Atata.WebDriverSetup/)
[![GitHub release](https://img.shields.io/github/release/atata-framework/atata-webdriversetup.svg)](https://github.com/atata-framework/atata-webdriversetup/releases)
[![Build status](https://dev.azure.com/atata-framework/atata-webdriversetup/_apis/build/status/atata-webdriversetup-ci?branchName=main)](https://dev.azure.com/atata-framework/atata-webdriversetup/_build/latest?definitionId=39&branchName=main)
[![Atata Templates](https://img.shields.io/badge/get-Atata_Templates-green.svg?color=4BC21F)](https://marketplace.visualstudio.com/items?itemName=YevgeniyShunevych.AtataTemplates)\
[![Gitter](https://badges.gitter.im/atata-framework/atata-webdriversetup.svg)](https://gitter.im/atata-framework/atata-webdriversetup)
[![Slack](https://img.shields.io/badge/join-Slack-green.svg?colorB=4EB898)](https://join.slack.com/t/atata-framework/shared_invite/zt-5j3lyln7-WD1ZtMDzXBhPm0yXLDBzbA)
[![Atata docs](https://img.shields.io/badge/docs-Atata_Framework-orange.svg)](https://atata.io)
[![Twitter](https://img.shields.io/badge/follow-@AtataFramework-blue.svg)](https://twitter.com/AtataFramework)

**Atata.WebDriverSetup** is a .NET library that sets up browser drivers for Selenium WebDriver,
e.g. `chromedriver`, `geckodriver`, etc.
Basically, it provides functionality similar to Java `WebDriverManager`.

*Targets .NET Standard 2.0*

## Table of Contents

- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
  - [Set Up Version Corresponding to Locally Installed Browser Version](#set-up-version-corresponding-to-locally-installed-browser-version)
  - [Set Up Latest Version](#set-up-latest-version)
  - [Set Up Specific Version](#set-up-specific-version)
  - [Set Up Version Corresponding to Specific Browser Version](#set-up-version-corresponding-to-specific-browser-version)
- [DriverSetup Members](#driversetup-members)
  - [DriverSetup Properties](#driversetup-properties)
  - [DriverSetup Methods](#driversetup-methods)
- [Configuration](#configuration)
  - [Global Configuration](#global-configuration)
  - [Driver-Specific Configuration](#driver-specific-configuration)
  - [Configuration Methods](#configuration-methods)
- [Feedback](#feedback)
- [SemVer](#semver)
- [License](#license)

## Features

- Sets up drivers for browsers: Chrome, Firefox, Edge, Internet Explorer and Opera.
- Supports Windows, Linux and MacOS platforms.
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

The package depends only on: `Microsoft.Win32.Registry` package,
which is used to detect locally installed browser version.

## Usage

The main class is `DriverSetup`.
The recommended place to perform driver(s) setup is a global set up method.

NUnit example:

```cs
[SetUpFixture]
public class SetUpFixture
{
    [OneTimeSetUp]
    public void SetUp()
    {
        DriverSetup.AutoSetUp(BrowserNames.Chrome);
    }
}
```

After a driver is set up, the driver path is added to environment "PATH" variable,
which is read by WebDriver's DriverService classes.

```cs
ChromeDriver chromeDriver = new ChromeDriver();
```

### Set Up Version Corresponding to Locally Installed Browser Version

1. Configure with default configuration options:
   ```cs
   DriverSetup.AutoSetUp(BrowserNames.Chrome);
   ```
1. Configure with custom configuration options:
   ```cs
   DriverSetup.ConfigureChrome()
       .WithAutoVersion()
       // Additional options can be set here.
       .SetUp();
   ```

`DriverSetup.AutoSetUp` method also supports multiple drivers setup:

```cs
DriverSetup.AutoSetUp(BrowserNames.Chrome, BrowserNames.Edge);
```

**Note:**
If the version of browser cannot be detected automatically, latest driver version is used.
Version auto-detection is currently supported only for Chrome and Edge browsers.

### Set Up Latest Version

```cs
DriverSetup.ConfigureChrome()
    .WithLatestVersion()
    .SetUp();
```

### Set Up Specific Version

```cs
DriverSetup.ConfigureChrome()
    .WithVersion("87.0.4280.88")
    .SetUp();
```

**Version format:**
- Chrome: `"87.0.4280.88"`
- Firefox: `"0.28.0"`
- Edge: `"89.0.774.4"`
- Opera: `"86.0.4240.80"`
- InternetExplorer: `"3.141.59"`

### Set Up Version Corresponding to Specific Browser Version

```cs
DriverSetup.ConfigureChrome()
    .ByBrowserVersion("87")
    .SetUp();
```

**Note:**
This feature is currently supported only for Chrome and Edge browsers.

**Version format:**
- Chrome: `"87"` or `"87.0.4280"`
- Edge: `"89.0.774.4"`

## DriverSetup Members

`DriverSetup` is a static class, so all its members are static too.

### DriverSetup Properties

- **`DriverSetupOptions GlobalOptions { get; }`**\
  Gets the global setup options.
- **`DriverSetupOptionsBuilder GlobalConfiguration { get; }`**\
  Gets the global setup configuration builder.
  Configures `GlobalOptions`.
- **`List<DriverSetupConfigurationBuilder> PendingConfigurations { get; }`**\
  Gets the pending driver setup configurations,
  the configurations that were created but were not set up.

### DriverSetup Methods

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
- **`DriverSetupResult AutoSetUp(string browserName)`**\
  Sets up driver with auto version detection for the browser with the specified name.
  Supported browser names are defined in `BrowserNames` static class.
- **`DriverSetupResult[] AutoSetUp(params string[] browserNames)`**\
  Sets up drivers with auto version detection for the browsers with the specified names.
  Supported browser names are defined in `BrowserNames` static class.
- **`DriverSetupResult[] AutoSetUp(IEnumerable<string> browserNames)`**\
  Sets up drivers with auto version detection for the browsers with the specified names.
  Supported browser names are defined in `BrowserNames` static class.
- **`DriverSetupResult[] AutoSetUpSafely(IEnumerable<string> browserNames)`**\
  Sets up drivers with auto version detection for the browsers with the specified names.
  Supported browser names are defined in `BrowserNames` static class.
  Skips invalid/unsupported browser names.
- **`DriverSetupResult[] SetUpPendingConfigurations()`**\
  Sets up pending configurations that are stored in `PendingConfigurations` property.
- **`void RegisterStrategyFactory(string browserName, Func<IHttpRequestExecutor, IDriverSetupStrategy> driverSetupStrategyFactory)`**\
  Registers the driver setup strategy factory.

## Configuration

It's possible to set configuration options globally and separately for a specific driver.

### Global Configuration

#### Using Fluent Builder

```cs
DriverSetup.GlobalConfiguration
    .WithStorageDirectoryPath("...")
    .WithVersionCache(false);
```

#### Using Options Properties

```cs
DriverSetup.GlobalOptions.StorageDirectoryPath = "...";
DriverSetup.GlobalOptions.UseVersionCache = false;
```

### Driver-Specific Configuration

```cs
DriverSetup.ConfigureChrome()
    .WithStorageDirectoryPath("...")
    .WithVersionCache(false)
    .SetUp();
```

*Don't forget to call `SetUp()` at the end.*

### Configuration Methods

#### Driver-Specific Configuration Methods

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

#### Common Configuration Methods

- **`WithStorageDirectoryPath(string path)`**\
  Sets the storage directory path.
  The default value is `"{basedir}/drivers")`.
- **`WithProxy(IWebProxy proxy)`**\
  Sets the web proxy.
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

## Feedback

Any feedback, issues and feature requests are welcome.

If you faced an issue please report it to [Atata.WebDriverSetup Issues](https://github.com/atata-framework/atata-webdriversetup/issues),
[ask a question on Stack Overflow](https://stackoverflow.com/questions/ask?tags=atata+csharp) using [atata](https://stackoverflow.com/questions/tagged/atata) tag
or use another [Atata Contact](https://atata.io/contact/) way.

## SemVer

Atata Framework follows [Semantic Versioning 2.0](https://semver.org/).
Thus backward compatibility is followed and updates within the same major version
(e.g. from 1.3 to 1.4) should not require code changes.

## License

Atata is an open source software, licensed under the Apache License 2.0.
See [LICENSE](LICENSE) for details.
