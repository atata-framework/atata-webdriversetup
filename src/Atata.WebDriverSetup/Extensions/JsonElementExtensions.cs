namespace Atata.WebDriverSetup;

internal static class JsonElementExtensions
{
    internal static JsonElement GetPropertyByChain(this JsonElement jsonElement, params string[] propertyNames)
    {
        jsonElement.CheckNotNull(nameof(jsonElement));
        propertyNames.CheckNotNullOrEmpty(nameof(propertyNames));

        JsonElement currentElement = jsonElement;

        for (int i = 0; i < propertyNames.Length; i++)
        {
            if (!currentElement.TryGetProperty(propertyNames[i], out currentElement))
            {
                string jsonPath = string.Join("/", propertyNames.Take(i + 1));
                throw new JsonException($"Failed to find \"{jsonPath}\" property in JSON:{Environment.NewLine}{jsonElement}");
            }
        }

        return currentElement;
    }
}
