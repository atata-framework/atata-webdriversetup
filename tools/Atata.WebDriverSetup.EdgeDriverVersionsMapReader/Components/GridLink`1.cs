using OpenQA.Selenium;

namespace Atata.WebDriverSetup.EdgeDriverVersionsMapReader;

[GetsContentFromSource(ContentSource.LastChildTextNode)]
public sealed class GridLink<TOwner> : Link<TOwner>
    where TOwner : PageObject<TOwner>
{
    public TOwner CtrlClick()
    {
        Log.ExecuteSection(new ClickLogSection(this), () =>
        {
            var element = Scope;
            Driver.Perform(x => x.KeyDown(Keys.LeftControl).Click(element).KeyUp(Keys.LeftControl));
        });

        return Owner;
    }

    public TOwner ClickToOpenInNewTab()
    {
        Log.ExecuteSection(new ClickLogSection(this), () =>
        {
            string href = Href.Value!;

            Script.Execute($"window.open('{href}', '_blank');");
        });

        return Owner;
    }
}
