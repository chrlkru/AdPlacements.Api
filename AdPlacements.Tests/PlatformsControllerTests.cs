using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AdPlacements.Api.Controllers;
using AdPlacements.Api.Services;
using AdPlacements.Api.DTO;

public class PlatformsControllerTests
{

    private class DummyStore : IAdPlatformStore
    {
        public (int loadedPlatforms, int skippedLines) Reload(Stream data)
            => (LoadedResult, SkippedResult);
        public IEnumerable<string> FindByLocation(string location)
            => FindResult ?? Array.Empty<string>();

        public int LoadedResult { get; set; }
        public int SkippedResult { get; set; }
        public IEnumerable<string>? FindResult { get; set; }
    }

    // Проверяет, что при загрузке валидного файла возвращается Ok с корректным результатом (кол-во загруженных и пропущенных).
    [Fact]
    public async Task Upload_ValidFile_ReturnsOkWithResult()
    {
    
        var fileContent = "TestPlatform: /path"; 
        var fileBytes = System.Text.Encoding.UTF8.GetBytes(fileContent);
        var formFile = new FormFile(new MemoryStream(fileBytes), 0, fileBytes.Length, "file", "test.txt");
        var dummyStore = new DummyStore { LoadedResult = 2, SkippedResult = 1 };
        var controller = new PlatformsController(dummyStore);

    
        ActionResult<UploadResultDto> actionResult = await controller.Upload(formFile, CancellationToken.None);

       
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var resultDto = Assert.IsType<UploadResultDto>(okResult.Value);
        Assert.Equal(2, resultDto.LoadedPlatforms);    // количество загруженных площадок соответствует мок-значению
        Assert.Equal(1, resultDto.SkippedLines);       // количество пропущенных строк соответствует мок-значению
    }

    // Проверяет, что при загрузке пустого файла возвращается BadRequest с сообщением об ошибке.
    [Fact]
    public async Task Upload_EmptyFile_ReturnsBadRequest()
    {
        var emptyStream = new MemoryStream(); 
        var emptyFile = new FormFile(emptyStream, 0, 0, "file", "empty.txt");
        var dummyStore = new DummyStore();
        var controller = new PlatformsController(dummyStore);

        ActionResult<UploadResultDto> actionResult = await controller.Upload(emptyFile, CancellationToken.None);

        var badResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        Assert.Equal("Файл не передан или пуст", badResult.Value);
    }

    // Проверяет, что при вызове GET без параметра location возвращается BadRequest с соответствующим сообщением.
    [Fact]
    public void Get_MissingLocation_ReturnsBadRequest()
    {
     
        var dummyStore = new DummyStore();
        var controller = new PlatformsController(dummyStore);

     
        ActionResult<IEnumerable<PlatformDto>> actionResult = controller.Get(null);

       
        var badResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        Assert.Equal("Укажите параметр location, например /ru/svrd/revda", badResult.Value);
    }

    // Проверяет, что при передаче корректного параметра location возвращается Ok и список площадок соответствует мок-данным.
    [Fact]
    public void Get_WithLocation_ReturnsExpectedPlatforms()
    {
        
        var dummyStore = new DummyStore
        {
            FindResult = new[] { "Platform1", "Platform2" } 
        };
        var controller = new PlatformsController(dummyStore);

   
        ActionResult<IEnumerable<PlatformDto>> actionResult = controller.Get("/some/location");

        // Assert: получен успешный ответ Ok с данными, соответствующими мок-списку
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var platforms = Assert.IsAssignableFrom<IEnumerable<PlatformDto>>(okResult.Value);
        var platformList = platforms.ToList();
        Assert.Equal(2, platformList.Count);
        Assert.Equal("Platform1", platformList[0].Name);
        Assert.Equal("Platform2", platformList[1].Name);
    }
}
