
namespace AdPlacements.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<Services.IPlacementsParser, Services.SimplePlacementsParser>();
            builder.Services.AddSingleton<Services.ILocationIndex, Services.PrefixIndex>();
            builder.Services.AddSingleton<Services.IAdPlatformStore, Services.InMemoryAdPlatformStore>();

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.MapControllers();

            app.Run();

        }
    }
}
