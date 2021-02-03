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
    }
}
