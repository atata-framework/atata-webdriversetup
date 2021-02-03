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
            BuildingContext = context;
        }

        /// <summary>
        /// Gets the building context.
        /// </summary>
        public TContext BuildingContext { get; }

        public TBuilder WithStorageDirectoryPath(string path)
        {
            BuildingContext.StorageDirectoryPath = path.CheckNotNullOrWhitespace(nameof(path));
            return (TBuilder)this;
        }

        public TBuilder WithProxy(IWebProxy proxy)
        {
            BuildingContext.Proxy = proxy;
            return (TBuilder)this;
        }

        public TBuilder WithLatestVersionCheckInterval(TimeSpan interval)
        {
            BuildingContext.LatestVersionCheckInterval = interval;
            return (TBuilder)this;
        }
    }
}
