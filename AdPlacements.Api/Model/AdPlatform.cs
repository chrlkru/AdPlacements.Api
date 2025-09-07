namespace AdPlacements.Api.Models;

public class AdPlatform
{
    public string Name { get; init; } = default!;
    public IReadOnlyCollection<string> Locations { get; init; } = Array.Empty<string>();
}
