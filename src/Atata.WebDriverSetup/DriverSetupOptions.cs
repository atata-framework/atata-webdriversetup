﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Atata.WebDriverSetup
{
    /// <summary>
    /// Represents the common driver setup options.
    /// </summary>
    public class DriverSetupOptions
    {
        private readonly DriverSetupOptions baseOptions;

        private readonly Dictionary<string, object> optionValues = new Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DriverSetupOptions"/> class.
        /// </summary>
        /// <param name="baseOptions">The base options.</param>
        public DriverSetupOptions(DriverSetupOptions baseOptions = null)
        {
            this.baseOptions = baseOptions;

            if (baseOptions is null)
            {
                StorageDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "drivers");
                UseVersionCache = true;

                LatestVersionCheckInterval = TimeSpan.FromHours(2);
                SpecificVersionCheckInterval = TimeSpan.FromHours(2);

                HttpRequestTryCount = 3;
                HttpRequestRetryInterval = TimeSpan.FromSeconds(3);

                IsEnabled = true;
                AddToEnvironmentPathVariable = true;
            }
        }

        /// <summary>
        /// Gets or sets the storage directory path.
        /// The default value is <c>"{basedir}/drivers")</c>.
        /// </summary>
        public string StorageDirectoryPath
        {
            get => GetOption<string>(nameof(StorageDirectoryPath));
            set => SetOption(nameof(StorageDirectoryPath), value);
        }

        /// <summary>
        /// Gets or sets the architecture (auto, x32 or x64).
        /// The default value is <see cref="Architecture.Auto"/>.
        /// </summary>
        public Architecture Architecture
        {
            get => GetOption<Architecture>(nameof(Architecture));
            set => SetOption(nameof(Architecture), value);
        }

        /// <summary>
        /// Gets or sets the proxy.
        /// </summary>
        public IWebProxy Proxy
        {
            get => GetOption<IWebProxy>(nameof(Proxy));
            set => SetOption(nameof(Proxy), value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use version cache.
        /// The default value is <see langword="true"/>.
        /// </summary>
        public bool UseVersionCache
        {
            get => GetOption<bool>(nameof(UseVersionCache));
            set => SetOption(nameof(UseVersionCache), value);
        }

        /// <summary>
        /// Gets or sets the latest version check interval.
        /// The default value is <c>2</c> hours.
        /// </summary>
        public TimeSpan LatestVersionCheckInterval
        {
            get => GetOption<TimeSpan>(nameof(LatestVersionCheckInterval));
            set => SetOption(nameof(LatestVersionCheckInterval), value);
        }

        /// <summary>
        /// Gets or sets the specific version check interval.
        /// The default value is <c>2</c> hours.
        /// </summary>
        public TimeSpan SpecificVersionCheckInterval
        {
            get => GetOption<TimeSpan>(nameof(SpecificVersionCheckInterval));
            set => SetOption(nameof(SpecificVersionCheckInterval), value);
        }

        /// <summary>
        /// Gets or sets the HTTP request try count.
        /// The default value is <c>3</c>.
        /// </summary>
        public int HttpRequestTryCount
        {
            get => GetOption<int>(nameof(HttpRequestTryCount));
            set => SetOption(nameof(HttpRequestTryCount), value);
        }

        /// <summary>
        /// Gets or sets the HTTP request retry interval.
        /// The default value is <c>3</c> seconds.
        /// </summary>
        public TimeSpan HttpRequestRetryInterval
        {
            get => GetOption<TimeSpan>(nameof(HttpRequestRetryInterval));
            set => SetOption(nameof(HttpRequestRetryInterval), value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is enabled.
        /// The default value is <see langword="true"/>.
        /// </summary>
        public bool IsEnabled
        {
            get => GetOption<bool>(nameof(IsEnabled));
            set => SetOption(nameof(IsEnabled), value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether to add the driver directory path
        /// to environment "Path" variable.
        /// The default value is <see langword="true"/>.
        /// </summary>
        public bool AddToEnvironmentPathVariable
        {
            get => GetOption<bool>(nameof(AddToEnvironmentPathVariable));
            set => SetOption(nameof(AddToEnvironmentPathVariable), value);
        }

        /// <summary>
        /// Gets the option value from this options or a base one.
        /// </summary>
        /// <typeparam name="T">The type of the option.</typeparam>
        /// <param name="optionName">Name of the option.</param>
        /// <returns>The option value or default value.</returns>
        protected T GetOption<T>(string optionName) =>
            optionValues.TryGetValue(optionName, out object value)
                ? (T)value
                : baseOptions != null
                    ? baseOptions.GetOption<T>(optionName)
                    : default;

        /// <summary>
        /// Sets the option value.
        /// </summary>
        /// <param name="optionName">Name of the option.</param>
        /// <param name="value">The value to set.</param>
        protected void SetOption(string optionName, object value) =>
            optionValues[optionName] = value;
    }
}
