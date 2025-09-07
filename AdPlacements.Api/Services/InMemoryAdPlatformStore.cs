using AdPlacements.Api.Services;

namespace AdPlacements.Api.Services;

public class InMemoryAdPlatformStore : IAdPlatformStore
{
    private readonly IPlacementsParser _parser;
    private readonly ILocationIndex _index;
    private readonly object _gate = new();

    public InMemoryAdPlatformStore(IPlacementsParser parser, ILocationIndex index)
    {
        _parser = parser;
        _index = index;
    }

    public (int loadedPlatforms, int skippedLines) Reload(Stream data)
    {
        var (items, skipped) = _parser.Parse(data);

        lock (_gate)
        {
            _index.Clear();
            foreach (var p in items)
                foreach (var loc in p.Locations)
                    _index.Add(loc, p.Name);
        }

        return (items.Count, skipped);
    }

    public IEnumerable<string> FindByLocation(string location)
    {
        if (string.IsNullOrWhiteSpace(location)) return Array.Empty<string>();
        return _index.Query(location);
    }
}
