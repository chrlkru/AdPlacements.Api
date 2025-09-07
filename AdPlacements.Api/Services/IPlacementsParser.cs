using AdPlacements.Api.Models;

namespace AdPlacements.Api.Services;

public interface IPlacementsParser
{
    // Парсит поток по строкам "Название:/ru,/ru/.."
    // Возвращает успешные записи и счётчик пропусков
    (IReadOnlyCollection<AdPlatform> items, int skipped) Parse(Stream stream);
}
