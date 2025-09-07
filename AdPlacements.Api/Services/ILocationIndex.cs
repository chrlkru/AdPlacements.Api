namespace AdPlacements.Api.Services;

public interface ILocationIndex
{
    void Clear();
    void Add(string location, string platformName);
    IEnumerable<string> Query(string location); // имена площадок по префиксам
}
