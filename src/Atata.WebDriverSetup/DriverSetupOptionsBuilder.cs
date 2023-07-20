namespace Atata.WebDriverSetup;

/// <summary>
/// Represents the driver setup options builder.
/// </summary>
public class DriverSetupOptionsBuilder : DriverSetupOptionsBuilder<DriverSetupOptionsBuilder, DriverSetupOptions>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DriverSetupOptionsBuilder"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    public DriverSetupOptionsBuilder(DriverSetupOptions context)
        : base(context)
    {
    }
}
