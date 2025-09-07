namespace AdPlacements.Api.Utils;

public static class LocationPath
{
    // Возвращает /ru, /ru/svrd, /ru/svrd/revda
    public static IEnumerable<string> EnumeratePrefixes(string location)
    {
        if (string.IsNullOrWhiteSpace(location)) yield break;
        var norm = Normalize(location);
        var parts = norm.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) yield break;

        var acc = "";
        foreach (var part in parts)
        {
            acc += "/" + part;
            yield return acc;
        }
    }

    public static string Normalize(string location)
    {
        var s = location.Trim();
        if (!s.StartsWith('/')) s = "/" + s;
        return s.Replace('\\', '/').TrimEnd('/').ToLowerInvariant();
    }
}
