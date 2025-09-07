// Services/SimplePlacementsParser.cs
using System.Text;
using AdPlacements.Api.Models;

namespace AdPlacements.Api.Services
{
    public sealed class SimplePlacementsParser : IPlacementsParser
    {
        // Возвращаем именно IReadOnlyCollection<AdPlatform>, как в интерфейсе
        public (IReadOnlyCollection<AdPlatform> items, int skipped) Parse(Stream input)
        {
            var items = new List<AdPlatform>();
            int skipped = 0;

            using var sr = new StreamReader(input, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                line = line.Trim();
                if (string.IsNullOrEmpty(line)) { skipped++; continue; }

                int colon = line.IndexOf(':');
                if (colon <= 0 || colon == line.Length - 1) { skipped++; continue; }

                var name = line[..colon].Trim();
                if (string.IsNullOrEmpty(name)) { skipped++; continue; }

                var raw = line[(colon + 1)..];
                var locs = raw
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(Normalize)
                    .Where(IsValidLocation)            // корень "/" считаем невалидным (согласно ТЗ)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (locs.Count == 0) { skipped++; continue; }

                items.Add(new AdPlatform { Name = name, Locations = locs });
            }

            return (items, skipped);
        }

        static string Normalize(string s)
        {
            s = s.Trim().Replace('\\', '/');
            if (!s.StartsWith('/')) s = "/" + s;
            if (s.Length > 1 && s.EndsWith('/')) s = s[..^1];
            return s.ToLowerInvariant();
        }

        static bool IsValidLocation(string s)
        {
            // важно: "/" отбрасываем, чтобы не появлялась «площадка на весь мир»,
            // в ТЗ верхний реальный уровень — "/ru"
            return s != "/";
        }
    }
}
