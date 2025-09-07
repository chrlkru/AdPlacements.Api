using System.Linq;
using System.Collections.Generic;
using Xunit;
using AdPlacements.Api.Services;
using AdPlacements.Api.Utils;

public class PrefixIndexTests
{
    // Проверяет добавление площадки и нормализацию префикса (хранение и поиск без учёта регистра).
    [Fact]
    public void Add_NormalizesLocationAndStoresPlatform()
    {
        var index = new PrefixIndex();
        
        index.Add("TeSt/Location", "SiteName");
       
        var result = index.Query("/test/location");
        Assert.Contains("SiteName", result);
    
        result = index.Query("/TEST/LOCATION");
        Assert.Contains("SiteName", result);
    }

    // Проверяет, что запрос вложенной локации возвращает площадки со всех уровней иерархии.
    [Fact]
    public void Query_NestedLocation_ReturnsAllPrefixMatches()
    {
        var index = new PrefixIndex();
   
        index.Add("/ru", "Global");
        index.Add("/ru/svrd", "Regional");
        index.Add("/ru/svrd/revda", "Local");

        var result = index.Query("/ru/svrd/revda").ToList();

        // Assert: должны вернуться все три имени площадок (глобальная, региональная, локальная)
        Assert.Equal(3, result.Count);
        Assert.Contains("Global", result);
        Assert.Contains("Regional", result);
        Assert.Contains("Local", result);
    }
}
