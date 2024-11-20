namespace Atata.WebDriverSetup.EdgeDriverVersionsMapReader;

[ControlDefinition(ContainingClass = "-loading", ComponentTypeName = "loading indicator")]
[FindFirst]
[WaitFor(Until.VisibleThenMissingOrHidden, TriggerEvents.Init, AbsenceTimeout = 15, ThrowOnPresenceFailure = false)]
public sealed class LoadingIndicator<TOwner> : Control<TOwner>
    where TOwner : PageObject<TOwner>
{
}
