using System;
using System.Net;

namespace Atata.WebDriverSetup
{
    /// <summary>
    /// Represents the base class for driver setup options builder.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the builder.</typeparam>
    /// <typeparam name="TContext">The type of the context.</typeparam>
    public abstract class DriverSetupOptionsBuilder<TBuilder, TContext>
        where TBuilder : DriverSetupOptionsBuilder<TBuilder, TContext>
        where TContext : DriverSetupOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DriverSetupOptionsBuilder{TBuilder, TContext}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        protected DriverSetupOptionsBuilder(TContext context)
        {
            BuildingContext = context.CheckNotNull(nameof(context));
        }

        /// <summary>
        /// Gets the building context.
        /// </summary>
        public TContext BuildingContext { get; }

        /// <summary>
        /// Sets the storage directory path.
        /// The default value is <c>"{basedir}/drivers")</c>.
        /// </summary>
        /// <param name="path">The directory path.</param>
        /// <returns>The same builder instance.</returns>
        public TBuilder WithStorageDirectoryPath(string path)
        {
            BuildingContext.StorageDirectoryPath = path.CheckNotNullOrWhitespace(nameof(path));
            return (TBuilder)this;
        }

        /// <summary>
        /// Sets the x32 (x86) architecture.
        /// </summary>
        /// <returns>The same builder instance.</returns>
        public TBuilder WithX32Architecture() =>
            WithArchitecture(Architecture.X32);

        /// <summary>
        /// Sets the x64 architecture.
        /// </summary>
        /// <returns>The same builder instance.</returns>
        public TBuilder WithX64Architecture() =>
            WithArchitecture(Architecture.X64);

        /// <summary>
        /// Sets the architecture.
        /// The default value is <see cref="Architecture.Auto"/>.
        /// </summary>
        /// <param name="architecture">The architecture.</param>
        /// <returns>The same builder instance.</returns>
        public TBuilder WithArchitecture(Architecture architecture)
        {
            BuildingContext.Architecture = architecture;
            return (TBuilder)this;
        }

        /// <summary>
        /// Sets the web proxy.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>The same builder instance.</returns>
        public TBuilder WithProxy(IWebProxy proxy)
        {
            BuildingContext.Proxy = proxy;
            return (TBuilder)this;
        }

        /// <summary>
        /// Sets a value indicating whether to use mutex to sync driver setup across machine.
        /// The default value is <see langword="false"/>.
        /// </summary>
        /// <param name="isEnabled">Whether to use mutex.</param>
        /// <returns>The same builder instance.</returns>
        public TBuilder WithMutex(bool isEnabled)
        {
            BuildingContext.UseMutex = isEnabled;
            return (TBuilder)this;
        }

        /// <summary>
        /// Sets a value indicating whether to use version cache.
        /// The default value is <see langword="true"/>.
        /// </summary>
        /// <param name="isEnabled">Whether to use version cache.</param>
        /// <returns>The same builder instance.</returns>
        public TBuilder WithVersionCache(bool isEnabled)
        {
            BuildingContext.UseVersionCache = isEnabled;
            return (TBuilder)this;
        }

        /// <summary>
        /// Sets the latest version check interval.
        /// The default values is <c>2</c> hours.
        /// </summary>
        /// <param name="interval">The interval.</param>
        /// <returns>The same builder instance.</returns>
        public TBuilder WithLatestVersionCheckInterval(TimeSpan interval)
        {
            BuildingContext.LatestVersionCheckInterval = interval;
            return (TBuilder)this;
        }

        /// <summary>
        /// Sets the specific version check interval.
        /// The default values is <c>2</c> hours.
        /// </summary>
        /// <param name="interval">The interval.</param>
        /// <returns>The same builder instance.</returns>
        public TBuilder WithSpecificVersionCheckInterval(TimeSpan interval)
        {
            BuildingContext.SpecificVersionCheckInterval = interval;
            return (TBuilder)this;
        }

        /// <summary>
        /// Sets the HTTP request try count.
        /// The default values is <c>3</c>.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <returns>The same builder instance.</returns>
        public TBuilder WithHttpRequestTryCount(int count)
        {
            BuildingContext.HttpRequestTryCount = count;
            return (TBuilder)this;
        }

        /// <summary>
        /// Sets the HTTP request retry interval.
        /// The default values is <c>3</c> seconds.
        /// </summary>
        /// <param name="interval">The interval.</param>
        /// <returns>The same builder instance.</returns>
        public TBuilder WithHttpRequestRetryInterval(TimeSpan interval)
        {
            BuildingContext.HttpRequestRetryInterval = interval;
            return (TBuilder)this;
        }

        /// <summary>
        /// Sets a value indicating whether the configuration is enabled.
        /// The default values is <see langword="true"/>.
        /// </summary>
        /// <param name="isEnabled">Whether is enabled.</param>
        /// <returns>The same builder instance.</returns>
        public TBuilder WithEnabledState(bool isEnabled)
        {
            BuildingContext.IsEnabled = isEnabled;
            return (TBuilder)this;
        }

        /// <summary>
        /// Sets a value indicating whether to add the driver directory path
        /// to environment "Path" variable.
        /// The default value is <see langword="true"/>.
        /// </summary>
        /// <param name="isEnabled">Whether is enabled.</param>
        /// <returns>The same builder instance.</returns>
        public TBuilder WithAddToEnvironmentPathVariable(bool isEnabled)
        {
            BuildingContext.AddToEnvironmentPathVariable = isEnabled;
            return (TBuilder)this;
        }
    }
}
