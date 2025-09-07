using System.Collections.Concurrent;
using AdPlacements.Api.Utils;

namespace AdPlacements.Api.Services;

// Индекс: ключ = точная локация, значение = множество имён площадок
public class PrefixIndex : ILocationIndex
{
    private readonly ConcurrentDictionary<string, HashSet<string>> _map =
        new(StringComparer.OrdinalIgnoreCase);

    public void Clear() => _map.Clear();

    public void Add(string location, string platformName)
    {
        var key = LocationPath.Normalize(location);
        var set = _map.GetOrAdd(key, _ => new HashSet<string>(StringComparer.OrdinalIgnoreCase));
        lock (set) set.Add(platformName);
    }

    public IEnumerable<string> Query(string location)
    {
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var prefix in LocationPath.EnumeratePrefixes(location))
        {
            if (_map.TryGetValue(prefix, out var set))
            {
                lock (set)
                    foreach (var name in set) result.Add(name);
            }
        }
        return result;
    }
}
