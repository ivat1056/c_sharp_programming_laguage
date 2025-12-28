using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;

/// <summary>
/// Лабораторная 3, программа 1: анализ производственных показателей с сервисом чтения телеметрии,
/// расчётом статистики и публикацией результата в консоль и XML. Также содержит flood-fill диагностику карты цеха.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        string baseDir = Path.Combine(Directory.GetCurrentDirectory(), "lab_3");
        string dataDir = Path.Combine(baseDir, "telemetry_data");
        string xmlReportPath = Path.Combine(baseDir, "telemetry_report.xml");

        var telemetryService = new TelemetryService(dataDir);
        var reportWriter = new XmlReportWriter(xmlReportPath);

        // 1. Генерируем несколько наборов входных данных по цехам.
        var sampleFiles = telemetryService.GenerateSampleTelemetryFiles();

        // 2. Читаем телеметрию, считаем показатели, выводим в консоль и в XML.
        foreach (string file in sampleFiles)
        {
            var readings = telemetryService.LoadTelemetry(file);
            var stats = Statistics.CalculatePerSensor(readings);

            Console.WriteLine($"\n=== Файл: {Path.GetFileName(file)} ===");
            PrintStats(stats);

            reportWriter.AppendReport(stats, file);
        }

        Console.WriteLine($"\nИстория версий сохранена в XML: {xmlReportPath}");

        // 3. Демонстрация flood-fill диагностики.
        Console.WriteLine("\n=== Flood-fill диагностика аварийной зоны ===");
        var mapLines = new[]
        {
            "..........",
            "..##....#.",
            ".##....###",
            "...~~~....",
            "...~~~..#.",
            "......#..."
        };
        var map = MapHelper.BuildMap(mapLines);
        MapHelper.FloodFill(map, 0, 0, obstacleChars: new HashSet<char> { '#', '~' }, fillChar: '!');

        Console.WriteLine("Исходная карта:");
        Console.WriteLine(string.Join(Environment.NewLine, mapLines));
        Console.WriteLine("\nЗаполненные зоны (\"!\" - зона эвакуации/аварии):");
        Console.WriteLine(MapHelper.MapToString(map));
    }

    private static void PrintStats(Dictionary<string, SensorStats> stats)
    {
        Console.WriteLine("Датчик | Мин | Макс | Медиана | % брака");
        foreach (var kvp in stats)
        {
            var s = kvp.Value;
            Console.WriteLine($"{kvp.Key,-7} {s.Min,6:0.00} {s.Max,6:0.00} {s.Median,8:0.00} {s.DefectPercent,8:0.0}%");
        }
    }
}

/// <summary>
/// Сервис генерации и чтения телеметрии из CSV/JSON.
/// </summary>
class TelemetryService
{
    private readonly string _dataDir;
    private readonly Random _random = new();

    public TelemetryService(string dataDir)
    {
        _dataDir = dataDir;
        Directory.CreateDirectory(_dataDir);
    }

    public List<string> GenerateSampleTelemetryFiles()
    {
        var datasets = new List<(string Shop, List<SensorReading> Readings)>
        {
            ("Цех-А", CreateTelemetrySet("Цех-А", new[] { "T-01", "P-02", "Q-03" })),
            ("Цех-B", CreateTelemetrySet("Цех-B", new[] { "T-11", "P-12" })),
            ("Цех-C", CreateTelemetrySet("Цех-C", new[] { "T-21", "P-22", "V-23", "Q-24" }))
        };

        var files = new List<string>();
        foreach (var (shop, readings) in datasets)
        {
            string csvPath = Path.Combine(_dataDir, $"{shop}_shift.csv");
            string jsonPath = Path.Combine(_dataDir, $"{shop}_shift.json");

            SaveAsCsv(csvPath, readings);
            SaveAsJson(jsonPath, readings);

            files.Add(csvPath);
            files.Add(jsonPath);
        }

        return files;
    }

    public List<SensorReading> LoadTelemetry(string path)
    {
        string extension = Path.GetExtension(path).ToLowerInvariant();
        if (extension == ".csv")
        {
            return LoadFromCsv(path);
        }
        if (extension == ".json")
        {
            return LoadFromJson(path);
        }

        throw new NotSupportedException($"Неизвестный формат файла: {extension}");
    }

    private List<SensorReading> CreateTelemetrySet(string shop, string[] sensors)
    {
        var readings = new List<SensorReading>();
        foreach (string sensor in sensors)
        {
            readings.AddRange(FillSensorArray(shop, sensor));
        }

        return readings;
    }

    private IEnumerable<SensorReading> FillSensorArray(string shop, string sensor)
    {
        // Небольшой набор данных за смену: 12 точек на датчик.
        for (int i = 0; i < 12; i++)
        {
            double value = Math.Round(_random.NextDouble() * 100, 2);
            bool defect = value < 10 || value > 90; // значения за пределами диапазона считаем браком.
            yield return new SensorReading(shop, sensor, value, defect);
        }
    }

    private void SaveAsCsv(string path, IEnumerable<SensorReading> readings)
    {
        using var writer = new StreamWriter(path);
        writer.WriteLine("shop,sensor,value,defect");
        foreach (var r in readings)
        {
            writer.WriteLine($"{r.Shop},{r.Sensor},{r.Value.ToString(CultureInfo.InvariantCulture)},{r.IsDefect}");
        }
    }

    private void SaveAsJson(string path, IEnumerable<SensorReading> readings)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(path, JsonSerializer.Serialize(readings, options));
    }

    private List<SensorReading> LoadFromCsv(string path)
    {
        var result = new List<SensorReading>();
        using var reader = new StreamReader(path);
        string? header = reader.ReadLine(); // пропускаем заголовок
        while (!reader.EndOfStream)
        {
            string? line = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var parts = line.Split(',');
            if (parts.Length < 4)
            {
                continue;
            }

            string shop = parts[0];
            string sensor = parts[1];
            double value = double.Parse(parts[2], CultureInfo.InvariantCulture);
            bool defect = bool.Parse(parts[3]);
            result.Add(new SensorReading(shop, sensor, value, defect));
        }

        return result;
    }

    private List<SensorReading> LoadFromJson(string path)
    {
        string content = File.ReadAllText(path);
        var data = JsonSerializer.Deserialize<List<SensorReading>>(content);
        return data ?? new List<SensorReading>();
    }
}

record SensorReading(string Shop, string Sensor, double Value, bool IsDefect);

struct SensorStats
{
    public double Min;
    public double Max;
    public double Median;
    public double DefectPercent;
}

/// <summary>
/// Модуль расчёта статистики: выделены методы для мин/макс/медианы и процента брака.
/// </summary>
static class Statistics
{
    public static Dictionary<string, SensorStats> CalculatePerSensor(List<SensorReading> readings)
    {
        var result = new Dictionary<string, SensorStats>();
        var groups = readings.GroupBy(r => $"{r.Shop}:{r.Sensor}");

        foreach (var group in groups)
        {
            var values = group.Select(g => g.Value).ToList();
            double min = CalculateMin(values);
            double max = CalculateMax(values);
            double median = CalculateMedian(values);
            double defectPercent = CalculateDefectPercent(group);

            result[group.Key] = new SensorStats
            {
                Min = min,
                Max = max,
                Median = median,
                DefectPercent = defectPercent
            };
        }

        return result;
    }

    public static double CalculateMin(List<double> values) => values.Min();

    public static double CalculateMax(List<double> values) => values.Max();

    public static double CalculateMedian(List<double> values)
    {
        var ordered = values.OrderBy(v => v).ToList();
        int n = ordered.Count;
        if (n == 0)
        {
            return 0;
        }
        if (n % 2 == 1)
        {
            return ordered[n / 2];
        }

        return (ordered[(n / 2) - 1] + ordered[n / 2]) / 2.0;
    }

    public static double CalculateDefectPercent(IEnumerable<SensorReading> readings)
    {
        var list = readings.ToList();
        if (list.Count == 0)
        {
            return 0;
        }

        double defects = list.Count(r => r.IsDefect);
        return defects / list.Count * 100.0;
    }
}

/// <summary>
/// Сохранение истории версий отчётов в XML.
/// </summary>
class XmlReportWriter
{
    private readonly string _reportPath;

    public XmlReportWriter(string reportPath)
    {
        _reportPath = reportPath;
    }

    public void AppendReport(Dictionary<string, SensorStats> stats, string sourceFile)
    {
        XDocument doc = File.Exists(_reportPath)
            ? XDocument.Load(_reportPath)
            : new XDocument(new XElement("TelemetryReports"));

        int nextVersion = doc.Root!.Elements("Report").Count() + 1;
        var reportElement = new XElement("Report",
            new XAttribute("version", nextVersion),
            new XAttribute("timestamp", DateTime.Now.ToString("s")),
            new XAttribute("source", Path.GetFileName(sourceFile)));

        foreach (var kvp in stats)
        {
            var s = kvp.Value;
            reportElement.Add(new XElement("Sensor",
                new XAttribute("id", kvp.Key),
                new XAttribute("min", s.Min.ToString("0.00", CultureInfo.InvariantCulture)),
                new XAttribute("max", s.Max.ToString("0.00", CultureInfo.InvariantCulture)),
                new XAttribute("median", s.Median.ToString("0.00", CultureInfo.InvariantCulture)),
                new XAttribute("defectPercent", s.DefectPercent.ToString("0.0", CultureInfo.InvariantCulture))));
        }

        doc.Root.Add(reportElement);
        doc.Save(_reportPath);
    }
}

/// <summary>
/// Утилиты для работы с текстовой картой цеха и flood-fill диагностикой.
/// </summary>
static class MapHelper
{
    public static char[,] BuildMap(string[] lines)
    {
        int rows = lines.Length;
        int cols = lines.Max(l => l.Length);
        var map = new char[rows, cols];
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                map[r, c] = c < lines[r].Length ? lines[r][c] : ' ';
            }
        }
        return map;
    }

    public static void FloodFill(char[,] map, int startRow, int startCol, HashSet<char> obstacleChars, char fillChar)
    {
        int rows = map.GetLength(0);
        int cols = map.GetLength(1);

        if (!IsInside(startRow, startCol, rows, cols) || obstacleChars.Contains(map[startRow, startCol]) || map[startRow, startCol] == fillChar)
        {
            return;
        }

        var stack = new Stack<(int R, int C)>();
        stack.Push((startRow, startCol));

        while (stack.Count > 0)
        {
            var (r, c) = stack.Pop();
            if (!IsInside(r, c, rows, cols) || obstacleChars.Contains(map[r, c]) || map[r, c] == fillChar)
            {
                continue;
            }

            map[r, c] = fillChar;

            stack.Push((r - 1, c));
            stack.Push((r + 1, c));
            stack.Push((r, c - 1));
            stack.Push((r, c + 1));
        }
    }

    public static string MapToString(char[,] map)
    {
        int rows = map.GetLength(0);
        int cols = map.GetLength(1);
        var lines = new List<string>(rows);
        for (int r = 0; r < rows; r++)
        {
            var chars = new char[cols];
            for (int c = 0; c < cols; c++)
            {
                chars[c] = map[r, c];
            }
            lines.Add(new string(chars));
        }

        return string.Join(Environment.NewLine, lines);
    }

    private static bool IsInside(int r, int c, int rows, int cols) => r >= 0 && r < rows && c >= 0 && c < cols;
}
