using System.IO;
using System.Linq;
using Xunit;
using AdPlacements.Api.Services;
using AdPlacements.Api.Models;

public class InMemoryAdPlatformStoreTests
{
    // Проверяет, что при повторной загрузке (Reload) старые данные заменяются новыми.
    [Fact]
    public void Reload_OverwritesPreviousData()
    {
    
        var store = new InMemoryAdPlatformStore(new SimplePlacementsParser(), new PrefixIndex());

        string firstData = "OldPlatform: /old";
        using var stream1 = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(firstData));
        store.Reload(stream1);
    
        Assert.Contains("OldPlatform", store.FindByLocation("/old"));

 
        string secondData = "NewPlatform: /new";
        using var stream2 = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(secondData));
        var (loaded, skipped) = store.Reload(stream2);

        Assert.Equal(1, loaded);
        Assert.Equal(0, skipped);
        Assert.Empty(store.FindByLocation("/old"));              // старая площадка удалена
        Assert.Contains("NewPlatform", store.FindByLocation("/new")); // новая площадка добавлена
    }

    // Проверяет поиск площадок по локации /ru/svrd/revda и получение ожидаемых результатов.
    [Fact]
    public void FindByLocation_ReturnsPlatformsForNestedLocation()
    {
     
        string data = string.Join('\n', new[] {
            "Global: /ru",
            "Regional: /ru/svrd",
            "Local: /ru/svrd/revda"
        });
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(data));
        var store = new InMemoryAdPlatformStore(new SimplePlacementsParser(), new PrefixIndex());
        store.Reload(stream);

   
        var result = store.FindByLocation("/ru/svrd/revda").ToList();

        // Assert: полученный список должен содержать все соответствующие имена площадок
        Assert.Equal(3, result.Count);
        Assert.Contains("Global", result);
        Assert.Contains("Regional", result);
        Assert.Contains("Local", result);
    }
}
