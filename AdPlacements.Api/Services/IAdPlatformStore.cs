namespace AdPlacements.Api.Services;

public interface IAdPlatformStore
{
    (int loadedPlatforms, int skippedLines) Reload(Stream data);
    IEnumerable<string> FindByLocation(string location);
}
