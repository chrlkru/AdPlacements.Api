using System;
using System.IO;
using System.Text;
using System.Linq;
using Xunit;
using AdPlacements.Api.Services;
using AdPlacements.Api.Models;

public class SimplePlacementsParserTests
{
    // Проверяет, что парсер правильно обрабатывает строку с несколькими локациями.
    [Fact]
    public void Parse_MultipleLocationsLine_ReturnsMultiplePlatforms()
    {
        
        string data = "TestSite: /loc1, /loc2, /loc3";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
        var parser = new SimplePlacementsParser();

  
        var (items, skipped) = parser.Parse(stream);

  
        Assert.Equal(1, items.Count);                   // Одна запись
        var platform = items.Single();                  // Получаем единственную площадку
        Assert.Equal("TestSite", platform.Name);        // Имя площадки должно совпадать
        
        Assert.Equal(3, platform.Locations.Count);
        Assert.Contains("/loc1", platform.Locations);
        Assert.Contains("/loc2", platform.Locations);
        Assert.Contains("/loc3", platform.Locations);
        Assert.Equal(0, skipped);  // Ни одной строки не пропущено
    }

    // Проверяет, что парсер пропускает некорректные строки (без двоеточия, пустое имя и т.д.) и считает их число.
    [Fact]
    public void Parse_InvalidLines_SkipsAndCountsThem()
    {
   
        string data = string.Join('\n', new[] {
            "NoColonLine",           // нет двоеточия
            ": /someLocation",       // пустое имя перед двоеточием
            "ValidSite: /valid/path",// корректная строка
            "NameOnly:",             // нет локаций после двоеточия
            "NameEmptyLocation:   , ", // нет ни одной непустой локации
            "RootOnly: /"           // локация = корень "/", игнорируется
        });
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
        var parser = new SimplePlacementsParser();

        
        var (items, skipped) = parser.Parse(stream);

        
        Assert.Equal(1, items.Count);         // Только одна валидная строка
        Assert.Equal(5, skipped);             // Пять строк должно быть пропущено
        // Проверяем, что единственная полученная площадка соответствует корректной строке
        var platform = items.Single();
        Assert.Equal("ValidSite", platform.Name);
        Assert.Equal(new[] { "/valid/path" }, platform.Locations);
    }
}
