using System;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Runtime.Serialization;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Xml.Serialization;
using static Helpers;

internal enum Formatting { None, Indented }

internal static class JsonConvert
{
    private static readonly JsonSerializerOptions CompactOptions = new()
    {
        WriteIndented = false,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        TypeInfoResolver = new DefaultJsonTypeInfoResolver()
    };

    private static readonly JsonSerializerOptions IndentedOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        TypeInfoResolver = new DefaultJsonTypeInfoResolver()
    };

    internal static string SerializeObject(object obj, Formatting formatting = Formatting.None)
    {
        var options = formatting == Formatting.Indented ? IndentedOptions : CompactOptions;
        return JsonSerializer.Serialize(obj, obj?.GetType() ?? typeof(object), options);
    }

    internal static T? DeserializeObject<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, CompactOptions);
    }
}

internal static class Helpers
{
    internal static string ToJson(object data, bool indented = true) =>
        JsonConvert.SerializeObject(data, indented ? Formatting.Indented : Formatting.None);

    internal static T? FromJson<T>(string json) =>
        JsonConvert.DeserializeObject<T>(json);

    internal static void SerializeXml<T>(T data, string path)
    {
        try
        {
            var serializer = new XmlSerializer(typeof(T));
            using var writer = new StreamWriter(path, false, Encoding.UTF8);
            serializer.Serialize(writer, data);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Не удалось записать XML {path}: {ex.Message}. Файл пропущен.");
        }
    }

    internal static T? DeserializeXml<T>(string path)
    {
        try
        {
            var serializer = new XmlSerializer(typeof(T));
            using var reader = new StreamReader(path, Encoding.UTF8);
            return (T?)serializer.Deserialize(reader);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Не удалось прочитать XML {path}: {ex.Message}. Файл будет перезаписан.");
            return default;
        }
    }

    internal static string ArchiveFile(string filePath)
    {
        var zipPath = Path.ChangeExtension(filePath, ".zip");
        File.Copy(filePath, zipPath, overwrite: true);
        return zipPath;
    }
}

public static class Program
{
    private const int MaxGroupingOutput = 1_000_000;

    public static void Main()
    {
        while (true)
        {
            Console.WriteLine("ЛР 2. Дополнительные задания (program_2)");
            Console.WriteLine("1  - Анализ производственных показателей");
            Console.WriteLine("2  - Ежедневник с обменом");
            Console.WriteLine("3  - Группировка по делимости");
            Console.WriteLine("4  - Стековый интерпретатор формул");
            Console.WriteLine("5  - Кадровый реестр");
            Console.WriteLine("6  - Информационная система департаментов");
            Console.WriteLine("7  - Файловый аудитор");
            Console.WriteLine("8  - Личный ассистент CLI");
            Console.WriteLine("9  - Сервис прогнозирования нагрузки");
            Console.WriteLine("10 - Симулятор логистики");
            Console.WriteLine("11 - Матричный калькулятор");
            Console.WriteLine("12 - Система управления доступом");
            Console.WriteLine("13 - Редактор конфигураций");
            Console.WriteLine("14 - Планировщик задач с приоритетами");
            Console.WriteLine("15 - API для обработки изображений (эмуляция)");
            Console.WriteLine("16 - Финансовые операции");
            Console.WriteLine("17 - Конструктор тестов производительности");
            Console.WriteLine("18 - Инструмент миграции данных");
            Console.WriteLine("19 - Расписание поездов");
            Console.WriteLine("20 - Система оповещений о погоде");
            Console.WriteLine("21 - Каталог медиафайлов");
            Console.WriteLine("22 - Менеджер проектов (Kanban)");
            Console.WriteLine("23 - Платёжный шлюз (эмуляция)");
            Console.WriteLine("24 - Система бронирования переговорных");
            Console.WriteLine("25 - Платформа онлайн-курсов");
            Console.WriteLine("26 - Симулятор биржевых торгов");
            Console.WriteLine("27 - Генератор отчётов по качеству кода");
            Console.WriteLine("28 - Система мониторинга IoT-устройств");
            Console.WriteLine("0  - Выход");
            Console.Write("Выбор: ");
            var choice = Console.ReadLine();
            Console.WriteLine();

            switch (choice)
            {
                case "1": ProductionAnalytics.Run(); break;
                case "2": DiaryApp.Run(); break;
                case "3": DivisibilityGroups.Run(); break;
                case "4": StackInterpreterApp.Run(); break;
                case "5": PersonnelRegistry.Run(); break;
                case "6": DepartmentSystem.Run(); break;
                case "7": FileAuditor.Run(); break;
                case "8": CliAssistant.Run(); break;
                case "9": LoadForecastService.Run(); break;
                case "10": LogisticsSimulator.Run(); break;
                case "11": MatrixCalculator.Run(); break;
                case "12": AccessControlSystem.Run(); break;
                case "13": ConfigEditor.Run(); break;
                case "14": PriorityScheduler.Run(); break;
                case "15": ImageProcessingApi.Run(); break;
                case "16": FinanceModule.Run(); break;
                case "17": PerfTestConstructor.Run(); break;
                case "18": DataMigrationTool.Run(); break;
                case "19": TrainSchedule.Run(); break;
                case "20": WeatherAlerts.Run(); break;
                case "21": MediaCatalog.Run(); break;
                case "22": KanbanManager.Run(); break;
                case "23": PaymentGateway.Run(); break;
                case "24": RoomBooking.Run(); break;
                case "25": OnlineCourses.Run(); break;
                case "26": TradingSimulator.Run(); break;
                case "27": CodeQualityReports.Run(); break;
                case "28": IotMonitoring.Run(); break;
                case "0": return;
                default: Console.WriteLine("Неизвестная команда"); break;
            }

            Console.WriteLine();
        }
    }

    // -------- 1. Анализ производственных показателей --------
    private static class ProductionAnalytics
    {
        public static void Run()
        {
            Console.WriteLine("Анализ производственных показателей");
            var samples = GenerateSampleData();
            Console.WriteLine("Сгенерировано образцов: " + samples.Count);

            Console.Write("Путь к входному файлу (CSV/JSON, пусто для пропуска): ");
            var path = Console.ReadLine();
            var telemetry = new List<TelemetryEntry>(samples);
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                telemetry.AddRange(ReadTelemetry(path!));
                Console.WriteLine($"Загружено из файла: {telemetry.Count - samples.Count}");
            }

            var stats = CalculateStats(telemetry);
            PrintStats(stats);
            SaveStatsToXml(stats, "telemetry_stats_history.xml");

            Console.WriteLine("Демонстрация flood-fill (X - авария):");
            var map = new[]
            {
                "..........",
                "..XXX.....",
                "..X.X.....",
                "..XXX..X..",
                "......XX..",
                ".........."
            };
            var filled = FloodFill(map, 1, 2, 'O');
            foreach (var row in filled) Console.WriteLine(row);
        }

        private static List<TelemetryEntry> GenerateSampleData()
        {
            var rnd = new Random(123);
            var list = new List<TelemetryEntry>();
            for (var workshop = 1; workshop <= 3; workshop++)
            for (var sensor = 1; sensor <= 5; sensor++)
            {
                for (var i = 0; i < 20; i++)
                {
                    list.Add(new TelemetryEntry
                    {
                        Workshop = $"Цех {workshop}",
                        Sensor = $"S{sensor}",
                        Value = Math.Round(rnd.NextDouble() * 100, 2),
                        IsDefect = rnd.NextDouble() < 0.05
                    });
                }
            }

            return list;
        }

        private static IEnumerable<TelemetryEntry> ReadTelemetry(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return ext switch
            {
                ".json" => FromJson<List<TelemetryEntry>>(File.ReadAllText(path)) ?? new List<TelemetryEntry>(),
                _ => ReadTelemetryCsv(path)
            };
        }

        private static IEnumerable<TelemetryEntry> ReadTelemetryCsv(string path)
        {
            foreach (var line in File.ReadLines(path))
            {
                var parts = line.Split(';', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 4) continue;
                if (!double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var value)) continue;
                if (!bool.TryParse(parts[3], out var defect)) defect = false;
                yield return new TelemetryEntry { Workshop = parts[0], Sensor = parts[1], Value = value, IsDefect = defect };
            }
        }

        private static List<SensorStats> CalculateStats(IEnumerable<TelemetryEntry> telemetry)
        {
            var grouped = telemetry
                .GroupBy(t => (t.Workshop, t.Sensor))
                .ToDictionary(g => g.Key, g => g.ToList());

            var stats = new List<SensorStats>();
            foreach (var kv in grouped)
            {
                var values = ExtractValuesArray(kv.Value);
                var defects = ExtractDefectArray(kv.Value);
                Array.Sort(values);
                var min = values.Length > 0 ? values[0] : 0;
                var max = values.Length > 0 ? values[^1] : 0;
                var median = values.Length == 0
                    ? 0
                    : values.Length % 2 == 1
                        ? values[values.Length / 2]
                        : (values[values.Length / 2 - 1] + values[values.Length / 2]) / 2.0;
                var defectPercent = defects.Length == 0 ? 0 : defects.Count(d => d) * 100.0 / defects.Length;

                stats.Add(new SensorStats
                {
                    Workshop = kv.Key.Workshop,
                    Sensor = kv.Key.Sensor,
                    Min = min,
                    Max = max,
                    Median = Math.Round(median, 2),
                    DefectPercent = Math.Round(defectPercent, 2)
                });
            }

            return stats.OrderBy(s => s.Workshop).ThenBy(s => s.Sensor).ToList();
        }

        private static double[] ExtractValuesArray(List<TelemetryEntry> entries) =>
            entries.Select(e => e.Value).ToArray();

        private static bool[] ExtractDefectArray(List<TelemetryEntry> entries) =>
            entries.Select(e => e.IsDefect).ToArray();

        private static void PrintStats(IEnumerable<SensorStats> stats)
        {
            Console.WriteLine("Статистика по датчикам:");
            foreach (var s in stats)
            {
                Console.WriteLine($"{s.Workshop}/{s.Sensor}: min={s.Min:F2} max={s.Max:F2} median={s.Median:F2} defect={s.DefectPercent:F2}%");
            }
        }

        private static void SaveStatsToXml(IEnumerable<SensorStats> stats, string path)
        {
            var history = new StatsHistory
            {
                Timestamp = DateTime.UtcNow,
                Items = stats.ToList()
            };

            List<StatsHistory> histories;
            if (File.Exists(path))
            {
                histories = DeserializeXml<List<StatsHistory>>(path) ?? new List<StatsHistory>();
            }
            else
            {
                histories = new List<StatsHistory>();
            }

            histories.Add(history);
            SerializeXml(histories, path);
            Console.WriteLine($"XML сохранен в {path}");
        }

        private static IReadOnlyList<string> FloodFill(IReadOnlyList<string> map, int startRow, int startCol, char fillChar)
        {
            var rows = map.Count;
            var cols = map[0].Length;
            var grid = map.Select(r => r.ToCharArray()).ToArray();
            if (startRow < 0 || startRow >= rows || startCol < 0 || startCol >= cols) return map.ToList();
            var target = grid[startRow][startCol];
            if (target == fillChar) return map.ToList();

            var stack = new Stack<(int r, int c)>();
            stack.Push((startRow, startCol));
            while (stack.Count > 0)
            {
                var (r, c) = stack.Pop();
                if (r < 0 || r >= rows || c < 0 || c >= cols) continue;
                if (grid[r][c] != target) continue;
                grid[r][c] = fillChar;
                stack.Push((r + 1, c));
                stack.Push((r - 1, c));
                stack.Push((r, c + 1));
                stack.Push((r, c - 1));
            }

            return grid.Select(chars => new string(chars)).ToList();
        }
    }

    [DataContract]
    public sealed class TelemetryEntry
    {
        [DataMember] public string Workshop { get; set; } = "";
        [DataMember] public string Sensor { get; set; } = "";
        [DataMember] public double Value { get; set; }
        [DataMember] public bool IsDefect { get; set; }
    }

    public sealed class SensorStats
    {
        public string Workshop { get; set; } = "";
        public string Sensor { get; set; } = "";
        public double Min { get; set; }
        public double Max { get; set; }
        public double Median { get; set; }
        public double DefectPercent { get; set; }
    }

    public sealed class StatsHistory
    {
        public DateTime Timestamp { get; set; }
        public List<SensorStats> Items { get; set; } = new();
    }

    // -------- 2. Ежедневник с обменом --------
    private static class DiaryApp
    {
        private static readonly List<DiaryEntry> Entries = new();

        public static void Run()
        {
            Console.WriteLine("Ежедневник");
            while (true)
            {
                Console.WriteLine("1 - Добавить, 2 - Показать, 3 - Редактировать, 4 - Удалить, 5 - Импорт, 6 - Экспорт, 0 - назад");
                var cmd = Console.ReadLine();
                switch (cmd)
                {
                    case "1": AddEntry(); break;
                    case "2": ListEntries(); break;
                    case "3": EditEntry(); break;
                    case "4": DeleteEntry(); break;
                    case "5": ImportEntries(); break;
                    case "6": ExportEntries(); break;
                    case "0": return;
                    default: Console.WriteLine("Нет команды"); break;
                }
            }
        }

        private static void AddEntry()
        {
            Console.Write("Заголовок: ");
            var title = Console.ReadLine() ?? "";
            Console.Write("Дата (yyyy-MM-dd): ");
            DateTime.TryParse(Console.ReadLine(), out var date);
            Console.Write("Текст: ");
            var text = Console.ReadLine() ?? "";
            var entry = new DiaryEntry(title, date, text);
            Entries.Add(entry);
        }

        private static void ListEntries()
        {
            Console.Write("Сортировка (date/title): ");
            var sort = Console.ReadLine();
            IEnumerable<DiaryEntry> query = Entries;
            if (sort == "date") query = query.OrderBy(e => e.Date);
            else if (sort == "title") query = query.OrderBy(e => e.Title);

            Console.Write("Фильтр по дате (yyyy-MM-dd, пусто чтобы пропустить): ");
            var filter = Console.ReadLine();
            if (DateTime.TryParse(filter, out var fdate))
            {
                query = query.Where(e => e.Date.Date == fdate.Date);
            }

            foreach (var e in query)
            {
                Console.WriteLine($"{e.Id} | {e.Date:yyyy-MM-dd} | {e.Title}: {e.Text}");
            }
        }

        private static void EditEntry()
        {
            Console.Write("ID: ");
            if (!Guid.TryParse(Console.ReadLine(), out var id)) return;
            var entry = Entries.FirstOrDefault(e => e.Id == id);
            if (entry == null) return;
            Console.Write("Новый текст: ");
            entry.UpdateText(Console.ReadLine());
        }

        private static void DeleteEntry()
        {
            Console.Write("ID: ");
            if (!Guid.TryParse(Console.ReadLine(), out var id)) return;
            Entries.RemoveAll(e => e.Id == id);
        }

        private static void ImportEntries()
        {
            Console.Write("Файл импорта (xml/json/gzip): ");
            var path = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path)) return;
            var ext = Path.GetExtension(path).ToLowerInvariant();
            List<DiaryEntry>? imported = null;
            switch (ext)
            {
                    case ".xml": imported = DeserializeXml<List<DiaryEntry>>(path); break;
                    case ".json": imported = JsonConvert.DeserializeObject<List<DiaryEntry>>(File.ReadAllText(path)); break;
                    case ".gz":
                        imported = JsonConvert.DeserializeObject<List<DiaryEntry>>(File.ReadAllText(path));
                        break;
            }

            if (imported != null) Entries.AddRange(imported);
            Console.WriteLine($"Импортировано: {imported?.Count ?? 0}");
        }

        private static void ExportEntries()
        {
            Console.Write("Формат (xml/json/gzip): ");
            var format = Console.ReadLine();
            Console.Write("Файл: ");
            var path = Console.ReadLine() ?? "diary_export";
            switch (format)
            {
                case "xml":
                    if (!path.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)) path += ".xml";
                    SerializeXml(Entries, path);
                    break;
                case "json":
                    if (!path.EndsWith(".json", StringComparison.OrdinalIgnoreCase)) path += ".json";
                    File.WriteAllText(path, JsonConvert.SerializeObject(Entries, Formatting.Indented));
                    break;
                case "gzip":
                case "gz":
                    if (!path.EndsWith(".gz", StringComparison.OrdinalIgnoreCase)) path += ".gz";
                    File.WriteAllText(path, JsonConvert.SerializeObject(Entries, Formatting.None));
                    break;
                default:
                    Console.WriteLine("Неизвестный формат");
                    return;
            }

            Console.WriteLine("Экспорт выполнен: " + path);
        }
    }

    private sealed class DiaryEntry
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; } = "";
        public DateTime Date { get; private set; }
        public string Text { get; private set; } = "";

        // Для сериализации
        private DiaryEntry() { }

        public DiaryEntry(string title, DateTime date, string text)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Заголовок не может быть пустым", nameof(title));
            Id = Guid.NewGuid();
            Title = title.Trim();
            Date = date == default ? DateTime.Today : date.Date;
            Text = text ?? string.Empty;
        }

        public void UpdateText(string? text)
        {
            Text = text ?? Text;
        }
    }

    // -------- 3. Группировка по делимости --------
    private static class DivisibilityGroups
    {
        public static void Run()
        {
            Console.Write("Путь к файлу с N: ");
            var path = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                Console.WriteLine("Файл не найден");
                return;
            }

            long n;
            using (var reader = new StreamReader(path))
            {
                var raw = reader.ReadLine();
                if (!long.TryParse(raw, out n) || n < 1)
                {
                    Console.WriteLine("Некорректное N");
                    return;
                }
            }

            Console.WriteLine("Режим: 1 - только M, 2 - группы в файл");
            var mode = Console.ReadLine();
            var start = DateTime.UtcNow;
            var m = CalculateGroupCount(n);

            if (mode == "1")
            {
                Console.WriteLine($"M = {m}, время {DateTime.UtcNow - start}");
                return;
            }

            if (mode != "2")
            {
                Console.WriteLine("Нет такого режима");
                return;
            }

            if (n > MaxGroupingOutput)
            {
                Console.WriteLine($"Группировка доступна для N <= {MaxGroupingOutput}");
                return;
            }

            var output = Path.Combine(Path.GetDirectoryName(path) ?? ".", "groups_output.txt");
            WriteGroupsStreamed((int)n, m, output);
            Console.WriteLine($"Готово за {DateTime.UtcNow - start}, файл: {output}");
            Console.Write("Архивировать? (y/n): ");
            if (string.Equals(Console.ReadLine(), "y", StringComparison.OrdinalIgnoreCase))
            {
                var zip = ArchiveFile(output);
                Console.WriteLine("ZIP: " + zip);
            }
        }

        private static int CalculateGroupCount(long n)
        {
            var count = 1;
            var value = 1L;
            while (value * 2 <= n)
            {
                value *= 2;
                count++;
            }

            return count;
        }

        private static void WriteGroupsStreamed(int n, int expectedGroups, string path)
        {
            using var writer = new StreamWriter(path, false, Encoding.UTF8);
            var current = 1;
            var nextPow = 2;
            writer.Write("Группа 1: 1");
            for (var i = 2; i <= n; i++)
            {
                if (i == nextPow)
                {
                    writer.WriteLine();
                    current++;
                    writer.Write($"Группа {current}:");
                    nextPow *= 2;
                }

                writer.Write(' ');
                writer.Write(i);
            }

            writer.WriteLine();
            if (current != expectedGroups)
            {
                writer.WriteLine($"Предупреждение: получено групп {current}, ожидалось {expectedGroups}");
            }
            writer.WriteLine($"M = {expectedGroups}");
        }
    }

    // -------- 4. Стековый интерпретатор --------
    private static class StackInterpreterApp
    {
        public static void Run()
        {
            Console.Write("Путь к файлу со списком выражений (пусто для ручного ввода): ");
            var path = Console.ReadLine();
            var expressions = new List<string>();
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                expressions.AddRange(File.ReadAllLines(path).Where(l => !string.IsNullOrWhiteSpace(l)));
            }
            else
            {
                Console.Write("Введите выражение: ");
                var expr = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(expr)) expressions.Add(expr);
            }

            Console.Write("Пошаговый вывод стеков? (y/n): ");
            var verbose = string.Equals(Console.ReadLine(), "y", StringComparison.OrdinalIgnoreCase);
            Console.Write("Режим парсера (1 - стековый, 2 - рекурсивный без полного скобочного): ");
            var parserMode = Console.ReadLine();

            var results = new List<ExpressionResult>();
            foreach (var expr in expressions)
            {
                try
                {
                    var value = parserMode == "2" ? EvaluateRecursive(expr) : Evaluate(expr, verbose);
                    results.Add(new ExpressionResult { Expression = expr, Value = value });
                    Console.WriteLine($"{expr} = {value}");
                }
                catch (Exception ex)
                {
                    results.Add(new ExpressionResult { Expression = expr, Error = ex.Message });
                    Console.WriteLine($"{expr} -> ошибка: {ex.Message}");
                }
            }

            File.WriteAllText("expressions_result.json", ToJson(results));
            Console.WriteLine("Результаты сохранены в expressions_result.json");
        }

        private static double Evaluate(string expression, bool verbose)
        {
            var output = new Stack<double>();
            var ops = new Stack<char>();

            void Apply(char op)
            {
                if (output.Count < 2) throw new InvalidOperationException("Недостаточно операндов");
                var b = output.Pop();
                var a = output.Pop();
                double res = op switch
                {
                    '+' => a + b,
                    '-' => a - b,
                    '*' => a * b,
                    '/' => b == 0 ? throw new DivideByZeroException() : a / b,
                    '^' => Math.Pow(a, b),
                    _ => throw new InvalidOperationException("Неизвестный оператор")
                };
                output.Push(res);
            }

            int Priority(char op) => op switch
            {
                '+' or '-' => 1,
                '*' or '/' => 2,
                '^' => 3,
                _ => 0
            };
            bool IsRightAssociative(char op) => op == '^';

            var tokens = Tokenize(expression);
            foreach (var token in tokens)
            {
                if (token.Type == TokenType.Number)
                {
                    output.Push(token.Value);
                }
                else if (token.Type == TokenType.Operator)
                {
                    while (ops.Count > 0 && ops.Peek() != '(')
                    {
                        var top = ops.Peek();
                        var shouldPop = IsRightAssociative(token.Op)
                            ? Priority(top) > Priority(token.Op)
                            : Priority(top) >= Priority(token.Op);
                        if (!shouldPop) break;
                        Apply(ops.Pop());
                    }
                    ops.Push(token.Op);
                }
                else if (token.Op == '(')
                {
                    ops.Push('(');
                }
                else if (token.Op == ')')
                {
                    while (ops.Count > 0 && ops.Peek() != '(') Apply(ops.Pop());
                    if (ops.Count == 0) throw new InvalidOperationException("Лишняя скобка");
                    ops.Pop();
                }

                if (verbose) DumpState(output, ops);
            }

            while (ops.Count > 0)
            {
                var op = ops.Pop();
                if (op == '(') throw new InvalidOperationException("Не хватает скобки");
                Apply(op);
                if (verbose) DumpState(output, ops);
            }

            return output.Pop();
        }

        private static double EvaluateRecursive(string expression)
        {
            var idx = 0;
            double ParseExpression(int minPriority = 0)
            {
                double left;
                SkipSpaces();
                if (Match('-'))
                {
                    left = -ParseExpression(3);
                }
                else if (Match('('))
                {
                    left = ParseExpression();
                    Expect(')');
                }
                else
                {
                    left = ParseNumber();
                }

                while (true)
                {
                    SkipSpaces();
                    if (idx >= expression.Length) break;
                    var op = expression[idx];
                    var priority = op switch
                    {
                        '+' or '-' => 1,
                        '*' or '/' => 2,
                        '^' => 3,
                        _ => 0
                    };
                    if (priority < minPriority || priority == 0) break;
                    idx++; // consume op
                    var nextMin = op == '^' ? priority : priority + 1;
                    var right = ParseExpression(nextMin);
                    left = op switch
                    {
                        '+' => left + right,
                        '-' => left - right,
                        '*' => left * right,
                        '/' => right == 0 ? throw new DivideByZeroException() : left / right,
                        '^' => Math.Pow(left, right),
                        _ => left
                    };
                }

                return left;
            }

            double ParseNumber()
            {
                SkipSpaces();
                var start = idx;
                while (idx < expression.Length && (char.IsDigit(expression[idx]) || expression[idx] == '.')) idx++;
                if (start == idx) throw new InvalidOperationException("Ожидалось число");
                var slice = expression[start..idx];
                return double.Parse(slice, CultureInfo.InvariantCulture);
            }

            void SkipSpaces()
            {
                while (idx < expression.Length && char.IsWhiteSpace(expression[idx])) idx++;
            }

            bool Match(char c)
            {
                SkipSpaces();
                if (idx < expression.Length && expression[idx] == c)
                {
                    idx++;
                    return true;
                }
                return false;
            }

            void Expect(char c)
            {
                if (!Match(c)) throw new InvalidOperationException($"Ожидался символ {c}");
            }

            var result = ParseExpression();
            SkipSpaces();
            if (idx != expression.Length) throw new InvalidOperationException("Лишние символы в выражении");
            return result;
        }

        private static void DumpState(Stack<double> values, Stack<char> ops)
        {
            Console.WriteLine($"Values: [{string.Join(", ", values.Reverse())}], Ops: [{string.Join(", ", ops)}]");
        }

        private static List<Token> Tokenize(string expr)
        {
            var tokens = new List<Token>();
            var i = 0;
            while (i < expr.Length)
            {
                var ch = expr[i];
                if (char.IsWhiteSpace(ch)) { i++; continue; }
                if ("+-*/^()".Contains(ch))
                {
                    var prevIsOp = tokens.Count == 0 || tokens[^1].Type == TokenType.Operator || tokens[^1].Op == '(';
                    if (ch == '-' && prevIsOp)
                    {
                        var j = i + 1;
                        while (j < expr.Length && char.IsWhiteSpace(expr[j])) j++;
                        if (j < expr.Length && expr[j] == '(')
                        {
                            // трактуем -(...) как 0 - (...)
                            tokens.Add(new Token { Type = TokenType.Number, Value = 0 });
                            tokens.Add(new Token { Type = TokenType.Operator, Op = '-' });
                            i++; // только минус
                            continue;
                        }
                        var num = ParseNumber(expr, ref i);
                        tokens.Add(new Token { Type = TokenType.Number, Value = num });
                        continue;
                    }

                    tokens.Add(new Token { Type = TokenType.Operator, Op = ch });
                    i++;
                    continue;
                }

                if (char.IsDigit(ch) || ch == '.')
                {
                    var num = ParseNumber(expr, ref i);
                    tokens.Add(new Token { Type = TokenType.Number, Value = num });
                }
                else
                {
                    throw new InvalidOperationException($"Неожиданный символ {ch}");
                }
            }

            return tokens;
        }

        private static double ParseNumber(string expr, ref int index)
        {
            var start = index;
            if (expr[index] == '-') index++;
            while (index < expr.Length && (char.IsDigit(expr[index]) || expr[index] == '.')) index++;
            var slice = expr[start..index];
            return double.Parse(slice, CultureInfo.InvariantCulture);
        }
    }

    private sealed class ExpressionResult
    {
        public string Expression { get; set; } = "";
        public double? Value { get; set; }
        public string? Error { get; set; }
    }

    private enum TokenType { Number, Operator }

    private sealed class Token
    {
        public TokenType Type { get; set; }
        public double Value { get; set; }
        public char Op { get; set; }
    }

    // -------- 5. Кадровый реестр --------
    private static class PersonnelRegistry
    {
        public static void Run()
        {
            Console.Write("CSV с сотрудниками (табель;ФИО;Возраст;Зарплата): ");
            var path = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                Console.WriteLine("Файл не найден, используется демо");
            }

            var repo = new EmployeeRepository();
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                repo.Load(path!);
            }
            else
            {
                repo.Add(new Employee { Id = "1", Name = "Иванов И.И.", Age = 30, Salary = 50_000 });
                repo.Add(new Employee { Id = "2", Name = "Петров П.П.", Age = 25, Salary = 45_000 });
            }

            Console.WriteLine("Доступ по индексу 0: " + repo[0]?.Name);
            Console.WriteLine("По табельному 2: " + repo.GetById("2")?.Name);
            Console.WriteLine("По шаблону 'Ив': " + string.Join(", ", repo.FindByName("Ив").Select(e => e.Name)));
            repo.Save("employees_out.csv");
            Console.WriteLine("Сохранено в employees_out.csv");
        }
    }

    public sealed class Employee
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public int Age { get; set; }
        public decimal Salary { get; set; }
    }

    private sealed class EmployeeRepository
    {
        private readonly List<Employee> _employees = new();
        private readonly HashSet<string> _ids = new();
        private readonly Dictionary<string, Employee> _byId = new(StringComparer.OrdinalIgnoreCase);
        private readonly List<string> _log = new();

        public Employee? this[int index] => index >= 0 && index < _employees.Count ? _employees[index] : null;
        public Employee? this[string id] => GetById(id);

        public void Load(string path)
        {
            foreach (var line in File.ReadLines(path))
            {
                var parts = line.Split(';');
                if (parts.Length < 4) continue;
                var employee = new Employee
                {
                    Id = parts[0],
                    Name = parts[1],
                    Age = int.Parse(parts[2]),
                    Salary = decimal.Parse(parts[3], CultureInfo.InvariantCulture)
                };
                Add(employee);
            }
            Log($"Load {path}");
        }

        public void Save(string path)
        {
            using var writer = new StreamWriter(path, false, Encoding.UTF8);
            foreach (var e in _employees)
            {
                writer.WriteLine($"{e.Id};{e.Name};{e.Age};{e.Salary}");
            }

            File.WriteAllLines(Path.ChangeExtension(path, ".log"), _log);
            SerializeXml(_employees, Path.ChangeExtension(path, ".xml"));
            File.WriteAllText(Path.ChangeExtension(path, ".json"), JsonConvert.SerializeObject(_employees, Formatting.Indented));
            Log($"Save {path}");
        }

        public void Add(Employee employee)
        {
            if (_ids.Contains(employee.Id)) return;
            _employees.Add(employee);
            _ids.Add(employee.Id);
            _byId[employee.Id] = employee;
            Log($"Add {employee.Id}");
        }

        public void Remove(string id)
        {
            _employees.RemoveAll(e => e.Id == id);
            _ids.Remove(id);
            _byId.Remove(id);
            Log($"Remove {id}");
        }

        public Employee? GetById(string id) => _byId.TryGetValue(id, out var e) ? e : null;

        public IEnumerable<Employee> FindByName(string pattern) =>
            _employees.Where(e => e.Name.Contains(pattern, StringComparison.OrdinalIgnoreCase));

        private void Log(string message) => _log.Add($"[{DateTime.UtcNow:O}] {message}");
    }

    // -------- 6. Информационная система департаментов --------
    private static class DepartmentSystem
    {
        public static void Run()
        {
            var dep = new Department { Name = "R&D" };
            dep.Employees.AddRange(new[]
            {
                new DepartmentEmployee { Name = "Alice", Age = 35, Salary = 120000 },
                new DepartmentEmployee { Name = "Bob", Age = 28, Salary = 90000 },
                new DepartmentEmployee { Name = "Charlie", Age = 40, Salary = 150000 }
            });

            Console.WriteLine("Сортировка по возрасту:");
            foreach (var e in dep.SortByAge()) Console.WriteLine($"{e.Name} {e.Age}");
            Console.WriteLine("Сортировка по зарплате:");
            foreach (var e in dep.SortBySalary()) Console.WriteLine($"{e.Name} {e.Salary}");

            SerializeXml(dep, "department.xml");
            File.WriteAllText("department.json", ToJson(dep));
            Console.WriteLine("Экспортирован dep в XML/JSON");
            Console.WriteLine("REST-подобный вызов Get(\"R&D\", minSalary:100000):");
            foreach (var e in dep.Get("R&D", minSalary: 100000))
            {
                Console.WriteLine($"{e.Name} {e.Salary}");
            }
        }
    }

    public sealed class Department
    {
        public string Name { get; set; } = "";
        public List<DepartmentEmployee> Employees { get; set; } = new();

        public IEnumerable<DepartmentEmployee> SortByAge() => Employees.OrderBy(e => e.Age);
        public IEnumerable<DepartmentEmployee> SortBySalary() => Employees.OrderBy(e => e.Salary).ThenBy(e => e.Age);

        public IEnumerable<DepartmentEmployee> Get(string name, int? minAge = null, decimal? minSalary = null)
        {
            return Employees
                .Where(e => string.Equals(Name, name, StringComparison.OrdinalIgnoreCase))
                .Where(e => !minAge.HasValue || e.Age >= minAge)
                .Where(e => !minSalary.HasValue || e.Salary >= minSalary);
        }

        public void Import(string path)
        {
            if (!File.Exists(path)) return;
            var ext = Path.GetExtension(path).ToLowerInvariant();
            if (ext == ".json")
            {
                var dep = FromJson<Department>(File.ReadAllText(path));
                if (dep != null) Employees = dep.Employees;
            }
            else
            {
                var dep = DeserializeXml<Department>(path);
                if (dep != null) Employees = dep.Employees;
            }
        }
    }

    public sealed class DepartmentEmployee
    {
        public string Name { get; set; } = "";
        public int Age { get; set; }
        public decimal Salary { get; set; }
    }

    // -------- 7. Файловый аудитор --------
    private static class FileAuditor
    {
        public static void Run()
        {
            Console.Write("Каталог для аудита: ");
            var path = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
            {
                Console.WriteLine("Каталог не найден");
                return;
            }

            Console.Write("Фильтр расширений (через запятую, пусто - все): ");
            var filterRaw = Console.ReadLine();
            var filters = string.IsNullOrWhiteSpace(filterRaw)
                ? Array.Empty<string>()
                : filterRaw.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(f => f.Trim().ToLowerInvariant()).ToArray();

            var files = new List<FileInfo>();
            System.Threading.Tasks.Parallel.ForEach(Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories), file =>
            {
                if (filters.Length > 0 && !filters.Any(f => file.ToLowerInvariant().EndsWith(f))) return;
                lock (files)
                {
                    files.Add(new FileInfo(file));
                }
            });

            var oldest = files.OrderBy(f => f.LastWriteTimeUtc).Take(10).ToList();
            var largest = files.OrderByDescending(f => f.Length).Take(10).ToList();
            Console.WriteLine("Топ-старые:");
            foreach (var f in oldest) Console.WriteLine($"{f.FullName} {f.LastWriteTime}");
            Console.WriteLine("Топ-крупные:");
            foreach (var f in largest) Console.WriteLine($"{f.FullName} {f.Length} байт");

            using (var writer = new StreamWriter("file_audit.csv", false, Encoding.UTF8))
            {
                writer.WriteLine("Path;Size;LastWriteTime;LastAccess;Attributes");
                foreach (var f in files)
                {
                    writer.WriteLine($"{f.FullName};{f.Length};{f.LastWriteTime};{f.LastAccessTime};{f.Attributes}");
                }
            }

            SerializeXml(files.Select(f => new FileAuditItem
            {
                Path = f.FullName,
                Size = f.Length,
                LastWrite = f.LastWriteTimeUtc,
                LastAccess = f.LastAccessTimeUtc,
                Attributes = f.Attributes.ToString()
            }).ToList(), "file_audit.xml");

            Console.Write("Архивировать отчеты? (y/n): ");
            if (string.Equals(Console.ReadLine(), "y", StringComparison.OrdinalIgnoreCase))
            {
                ArchiveFile("file_audit.csv");
                ArchiveFile("file_audit.xml");
            }

            Console.WriteLine("Отчеты file_audit.csv и file_audit.xml созданы");
        }
    }

    public sealed class FileAuditItem
    {
        public string Path { get; set; } = "";
        public long Size { get; set; }
        public DateTime LastWrite { get; set; }
        public DateTime LastAccess { get; set; }
        public string Attributes { get; set; } = "";
    }

    // -------- 8. Личный ассистент CLI --------
    private static class CliAssistant
    {
        private static readonly List<string> Notes = new();
        public static void Run()
        {
            Console.WriteLine("Команды: greet, timer, code, note, list, test, back");
            while (true)
            {
                Console.Write("> ");
                var cmd = Console.ReadLine();
                switch (cmd)
                {
                    case "greet":
                        Console.WriteLine("Привет! Готов помочь.");
                        break;
                    case "timer":
                        Console.Write("Секунд: ");
                        if (int.TryParse(Console.ReadLine(), out var s))
                        {
                            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(s));
                            Console.WriteLine("Таймер сработал");
                        }
                        break;
                    case "code":
                        Console.WriteLine("Одноразовый код: " + Guid.NewGuid().ToString("N")[..6]);
                        break;
                    case "note":
                        Console.Write("Текст: ");
                        Notes.Add(Console.ReadLine() ?? "");
                        break;
                    case "list":
                        foreach (var n in Notes) Console.WriteLine(n);
                        break;
                    case "test":
                        RunSelfTests();
                        break;
                    case "back":
                        return;
                    default:
                        Console.WriteLine("Не знаю команду");
                        break;
                }
            }
        }

        private static void RunSelfTests()
        {
            var sb = new StringBuilder();
            using var writer = new StringWriter(sb);
            var originalOut = Console.Out;
            Console.SetOut(writer);
            Console.WriteLine("test");
            Console.SetOut(originalOut);
            var output = sb.ToString();
            Console.WriteLine($"Перехваченный вывод: {output.Trim()}");
        }
    }

    // -------- 9. Сервис прогнозирования нагрузки --------
    private static class LoadForecastService
    {
        public static void Run()
        {
            var queue = new Queue<double>();
            Console.Write("Режим хранения (mem/disk): ");
            var mode = Console.ReadLine();
            var filePath = "load_history.txt";
            Console.Write("Порог уведомления (число, пусто чтобы пропустить): ");
            double? threshold = double.TryParse(Console.ReadLine(), out var th) ? th : null;

            Console.WriteLine("Вводите нагрузки (число), пусто - прогноз");
            while (true)
            {
                var line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) break;
                if (double.TryParse(line, out var value))
                {
                    queue.Enqueue(value);
                    if (queue.Count > 20) queue.Dequeue();
                    if (string.Equals(mode, "disk", StringComparison.OrdinalIgnoreCase))
                    {
                        File.AppendAllText(filePath, value + Environment.NewLine);
                    }
                    if (threshold.HasValue && value > threshold.Value)
                    {
                        Console.WriteLine($"Уведомление: значение {value} превышает порог {threshold}");
                    }
                }
            }

            var forecast = Forecast(queue);
            Console.WriteLine($"Прогноз на час вперед: {forecast:F2}");
            File.WriteAllText("load_forecast.json", ToJson(new { Forecast = forecast, Timestamp = DateTime.UtcNow }));
        }

        private static double Forecast(IEnumerable<double> history)
        {
            var xs = history.Select((v, i) => (x: (double)i, y: v)).ToArray();
            if (xs.Length == 0) return 0;
            var xMean = xs.Average(p => p.x);
            var yMean = xs.Average(p => p.y);
            var num = xs.Sum(p => (p.x - xMean) * (p.y - yMean));
            var den = xs.Sum(p => Math.Pow(p.x - xMean, 2));
            var slope = den == 0 ? 0 : num / den;
            var intercept = yMean - slope * xMean;
            var nextX = xs.Length;
            return intercept + slope * nextX;
        }
    }

    // -------- 10. Симулятор логистики --------
    private static class LogisticsSimulator
    {
        public static void Run()
        {
            var graph = new Dictionary<string, List<(string to, int weight)>>()
            {
                ["A"] = new() { ("B", 5), ("C", 10) },
                ["B"] = new() { ("C", 3), ("D", 7) },
                ["C"] = new() { ("D", 1) },
                ["D"] = new()
            };
            var path = Dijkstra(graph, "A", "D", out var distance);
            Console.WriteLine("Кратчайший путь A->D: " + string.Join("->", path) + $" ({distance})");

            var schedule = new SimpleMinPriorityQueue<LogisticsEvent, DateTime>();
            schedule.Enqueue(new LogisticsEvent { Truck = "T1", Action = "Load", Location = "A", Time = DateTime.Now.AddMinutes(5) }, DateTime.Now.AddMinutes(5));
            schedule.Enqueue(new LogisticsEvent { Truck = "T1", Action = "Depart", Location = "A", Time = DateTime.Now.AddMinutes(10) }, DateTime.Now.AddMinutes(10));
            schedule.Enqueue(new LogisticsEvent { Truck = "T1", Action = "Arrive", Location = "D", Time = DateTime.Now.AddMinutes(20) }, DateTime.Now.AddMinutes(20));
            Console.WriteLine("События:");
            while (schedule.TryDequeue(out var ev, out _))
            {
                Console.WriteLine($"{ev.Time:HH:mm} {ev.Truck} {ev.Action} {ev.Location}");
            }
        }

        private static List<string> Dijkstra(Dictionary<string, List<(string to, int weight)>> graph, string start, string goal, out int distance)
        {
            var dist = new Dictionary<string, int>();
            var prev = new Dictionary<string, string?>();
            var unvisited = new HashSet<string>(graph.Keys);
            foreach (var v in graph.Keys) dist[v] = int.MaxValue;
            dist[start] = 0;
            while (unvisited.Count > 0)
            {
                var u = unvisited.OrderBy(v => dist[v]).First();
                unvisited.Remove(u);
                if (u == goal) break;
                foreach (var (to, w) in graph[u])
                {
                    var alt = dist[u] + w;
                    if (alt < dist[to])
                    {
                        dist[to] = alt;
                        prev[to] = u;
                    }
                }
            }

            var path = new List<string>();
            string? cur = goal;
            while (cur != null && prev.ContainsKey(cur))
            {
                path.Add(cur);
                cur = prev[cur];
            }
            path.Add(start);
            path.Reverse();
            distance = dist[goal];
            return path;
        }
    }

    // -------- 11. Матричный калькулятор --------
    private static class MatrixCalculator
    {
        public static void Run()
        {
            var a = new[,] { { 1.0, 2.0 }, { 3.0, 4.0 } };
            var b = new[,] { { 5.0, 6.0 }, { 7.0, 8.0 } };
            var sum = Add(a, b);
            var mul = Multiply(a, b);
            Console.WriteLine($"Сумма: [{sum[0,0]} {sum[0,1]} ; {sum[1,0]} {sum[1,1]}]");
            Console.WriteLine($"Детерминант A: {Determinant(a)}");
            var transposed = Transpose(a);
            Console.WriteLine($"Транспонирование A[0,1]={transposed[0,1]}");

            var sparse = ToSparse(a);
            var dense = FromSparse(sparse, 2, 2);
            Console.WriteLine($"Sparse->Dense сохранен элемент (1,1)={dense[1,1]}");

            WriteCsv(a, "matrix_a.csv");
            var loaded = ReadCsv("matrix_a.csv");
            WriteBinary(loaded, "matrix_a.bin");
            var loadedBin = ReadBinary("matrix_a.bin");
            Console.WriteLine($"Загружено из CSV/BIN элемент (0,0)={loadedBin[0,0]}");

            RunRandomInvariantTests();
        }

        private static double[,] Add(double[,] a, double[,] b)
        {
            var rows = a.GetLength(0);
            var cols = a.GetLength(1);
            var r = new double[rows, cols];
            for (var i = 0; i < rows; i++)
            for (var j = 0; j < cols; j++)
                r[i, j] = a[i, j] + b[i, j];
            return r;
        }

        private static double[,] Multiply(double[,] a, double[,] b)
        {
            var rows = a.GetLength(0);
            var cols = b.GetLength(1);
            var r = new double[rows, cols];
            for (var i = 0; i < rows; i++)
            for (var j = 0; j < cols; j++)
            {
                var sum = 0.0;
                for (var k = 0; k < a.GetLength(1); k++) sum += a[i, k] * b[k, j];
                r[i, j] = sum;
            }
            return r;
        }

        private static double[,] Transpose(double[,] a)
        {
            var r = new double[a.GetLength(1), a.GetLength(0)];
            for (var i = 0; i < a.GetLength(0); i++)
            for (var j = 0; j < a.GetLength(1); j++)
                r[j, i] = a[i, j];
            return r;
        }

        private static double Determinant(double[,] a)
        {
            if (a.GetLength(0) != a.GetLength(1)) throw new ArgumentException("Матрица должна быть квадратной");
            if (a.Length == 4) return a[0, 0] * a[1, 1] - a[0, 1] * a[1, 0];
            //  разложение по первой строке для демонстрации
            var det = 0.0;
            for (var col = 0; col < a.GetLength(1); col++)
            {
                det += Math.Pow(-1, col) * a[0, col] * Determinant(Minor(a, 0, col));
            }
            return det;
        }

        private static double[,] Minor(double[,] a, int row, int col)
        {
            var size = a.GetLength(0);
            var m = new double[size - 1, size - 1];
            for (int i = 0, mi = 0; i < size; i++)
            {
                if (i == row) continue;
                for (int j = 0, mj = 0; j < size; j++)
                {
                    if (j == col) continue;
                    m[mi, mj++] = a[i, j];
                }
                mi++;
            }
            return m;
        }

        private static Dictionary<(int r, int c), double> ToSparse(double[,] dense)
        {
            var dict = new Dictionary<(int r, int c), double>();
            for (var i = 0; i < dense.GetLength(0); i++)
            for (var j = 0; j < dense.GetLength(1); j++)
            {
                var v = dense[i, j];
                if (Math.Abs(v) > 1e-9) dict[(i, j)] = v;
            }
            return dict;
        }

        private static double[,] FromSparse(Dictionary<(int r, int c), double> sparse, int rows, int cols)
        {
            var dense = new double[rows, cols];
            foreach (var kv in sparse) dense[kv.Key.r, kv.Key.c] = kv.Value;
            return dense;
        }

        private static Dictionary<(int r, int c), double> MultiplySparse(Dictionary<(int r, int c), double> a, Dictionary<(int r, int c), double> b, int size)
        {
            var result = new Dictionary<(int r, int c), double>();
            foreach (var kvA in a)
            {
                for (var k = 0; k < size; k++)
                {
                    var keyB = (kvA.Key.c, k);
                    if (!b.TryGetValue(keyB, out var bv)) continue;
                    var key = (kvA.Key.r, k);
                    result.TryGetValue(key, out var cur);
                    var next = cur + kvA.Value * bv;
                    if (Math.Abs(next) < 1e-9) result.Remove(key);
                    else result[key] = next;
                }
            }
            return result;
        }

        private static void WriteCsv(double[,] matrix, string path)
        {
            using var writer = new StreamWriter(path, false, Encoding.UTF8);
            for (var i = 0; i < matrix.GetLength(0); i++)
            {
                var row = new string[matrix.GetLength(1)];
                for (var j = 0; j < matrix.GetLength(1); j++) row[j] = matrix[i, j].ToString(CultureInfo.InvariantCulture);
                writer.WriteLine(string.Join(';', row));
            }
        }

        private static double[,] ReadCsv(string path)
        {
            var lines = File.ReadAllLines(path);
            var rows = lines.Length;
            var cols = lines[0].Split(';').Length;
            var m = new double[rows, cols];
            for (var i = 0; i < rows; i++)
            {
                var parts = lines[i].Split(';');
                for (var j = 0; j < cols; j++) m[i, j] = double.Parse(parts[j], CultureInfo.InvariantCulture);
            }
            return m;
        }

        private static void WriteBinary(double[,] matrix, string path)
        {
            using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            using var bw = new BinaryWriter(fs, Encoding.UTF8, leaveOpen: false);
            bw.Write(matrix.GetLength(0));
            bw.Write(matrix.GetLength(1));
            for (var i = 0; i < matrix.GetLength(0); i++)
            for (var j = 0; j < matrix.GetLength(1); j++) bw.Write(matrix[i, j]);
        }

        private static double[,] ReadBinary(string path)
        {
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            using var br = new BinaryReader(fs, Encoding.UTF8, leaveOpen: false);
            var rows = br.ReadInt32();
            var cols = br.ReadInt32();
            var m = new double[rows, cols];
            for (var i = 0; i < rows; i++)
            for (var j = 0; j < cols; j++) m[i, j] = br.ReadDouble();
            return m;
        }

        private static void RunRandomInvariantTests()
        {
            var rnd = new Random(7);
            for (var t = 0; t < 3; t++)
            {
                var a = new double[2, 2];
                var b = new double[2, 2];
                for (var i = 0; i < 2; i++)
                for (var j = 0; j < 2; j++)
                {
                    a[i, j] = rnd.Next(1, 5);
                    b[i, j] = rnd.Next(1, 5);
                }
                var sum = Add(a, b);
                var sum2 = Add(b, a);
                if (Math.Abs(sum[0,0] - sum2[0,0]) > 1e-9) Console.WriteLine("Нарушена коммутативность");
                var sparseA = ToSparse(a);
                var sparseB = ToSparse(b);
                var sparseMul = MultiplySparse(sparseA, sparseB, 2);
                var denseMul = Multiply(a, b);
                var check = FromSparse(sparseMul, 2, 2);
                if (Math.Abs(denseMul[0,0] - check[0,0]) > 1e-9) Console.WriteLine("Sparse умножение не совпало с dense");
            }
        }
    }

    // -------- 12. Система управления доступом --------
    private static class AccessControlSystem
    {
        public static void Run()
        {
            var adminRole = new Role { Name = "Admin", Permissions = new HashSet<string> { "read", "write" } };
            var viewerRole = new Role { Name = "Viewer", Permissions = new HashSet<string> { "read" }, Parent = adminRole };
            var delegatedRole = new Role { Name = "Delegate", Permissions = new HashSet<string> { "sign" }, DelegatedTo = "User2" };
            var user = new User { Name = "User1", Roles = new List<Role> { viewerRole, delegatedRole } };
            var token = TokenService.Issue(user, TimeSpan.FromHours(1));
            Console.WriteLine("Токен: " + token);
            Console.WriteLine("Проверка: " + TokenService.Validate(token));
            File.AppendAllText("access_audit.log", $"[{DateTime.UtcNow:O}] Issued token for {user.Name}{Environment.NewLine}");
        }
    }

    private sealed class User
    {
        public string Name { get; set; } = "";
        public List<Role> Roles { get; set; } = new();

        public HashSet<string> GetAllPermissions()
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var role in Roles) Collect(role, set);
            return set;
        }

        private static void Collect(Role role, HashSet<string> target)
        {
            foreach (var p in role.Permissions) target.Add(p);
            if (role.Parent != null) Collect(role.Parent, target);
        }
    }

    private sealed class Role
    {
        public string Name { get; set; } = "";
        public HashSet<string> Permissions { get; set; } = new();
        public Role? Parent { get; set; }
        public string? DelegatedTo { get; set; }
    }

    private static class TokenService
    {
        private static readonly byte[] Secret = Encoding.UTF8.GetBytes("secret-key");

        public static string Issue(User user, TimeSpan ttl)
        {
            var permissions = string.Join(',', user.GetAllPermissions());
            var payload = $"{user.Name}|{permissions}|{DateTime.UtcNow.Add(ttl):O}";
            var signature = Sign(payload);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(payload + "|" + signature));
        }

        public static bool Validate(string token)
        {
            try
            {
                var data = Encoding.UTF8.GetString(Convert.FromBase64String(token)).Split('|');
                if (data.Length < 4) return false;
                var expires = DateTime.Parse(data[2], null, DateTimeStyles.RoundtripKind);
                var signature = data[3];
                var payload = string.Join('|', data.Take(3));
                if (signature != Sign(payload)) return false;
                return expires > DateTime.UtcNow;
            }
            catch
            {
                return false;
            }
        }

        private static string Sign(string payload)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA256(Secret);
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            var sb = new StringBuilder(hash.Length * 2);
            foreach (var b in hash) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }

    // -------- 13. Редактор конфигураций --------
    private static class ConfigEditor
    {
        private static readonly List<string> History = new();
        private static bool _draftMode;
        private static readonly List<string> Log = new();

        public static void Run()
        {
            Console.WriteLine("Режим черновика? (y/n): ");
            _draftMode = string.Equals(Console.ReadLine(), "y", StringComparison.OrdinalIgnoreCase);

            Console.WriteLine("Введите конфигурацию (строка), пусто - выход");
            while (true)
            {
                var line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) break;
                History.Add(line);
                Log.Add($"[{DateTime.UtcNow:O}] add line");
            }

            Console.Write("Сравнить две версии? (формат a,b): ");
            var diffInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(diffInput) && diffInput.Contains(','))
            {
                var parts = diffInput.Split(',');
                if (int.TryParse(parts[0], out var a) && int.TryParse(parts[1], out var b) &&
                    a >= 0 && a < History.Count && b >= 0 && b < History.Count)
                {
                    var diff = BuildDiff(History[a], History[b]);
                    File.WriteAllLines("config_diff.txt", diff);
                    Console.WriteLine("Дифф сохранен в config_diff.txt");
                }
            }

            Console.Write("Откат к версии (0..n-1): ");
            if (int.TryParse(Console.ReadLine(), out var idx) && idx >= 0 && idx < History.Count)
            {
                if (ConfirmDanger("Откатить к версии?"))
                {
                    Console.WriteLine("Выбрана версия: " + History[idx]);
                    Log.Add($"[{DateTime.UtcNow:O}] rollback to {idx}");
                }
            }

            File.WriteAllLines("config_history.txt", History);
            File.WriteAllLines("config_audit.log", Log);
            Console.WriteLine("История сохранена");
        }

        private static IEnumerable<string> BuildDiff(string a, string b)
        {
            if (a == b) return new[] { "Версии идентичны" };
            return new[]
            {
                "---- A",
                a,
                "---- B",
                b
            };
        }

        private static bool ConfirmDanger(string message)
        {
            if (_draftMode) return true;
            Console.Write($"{message} (y/n): ");
            return string.Equals(Console.ReadLine(), "y", StringComparison.OrdinalIgnoreCase);
        }
    }

    // -------- 14. Планировщик задач --------
    private static class PriorityScheduler
    {
        public static void Run()
        {
            var pq = new SimplePriorityQueue();
            pq.Enqueue(new ScheduledTask("A", priority: 2, delay: TimeSpan.FromSeconds(1), timeout: TimeSpan.FromSeconds(2)));
            pq.Enqueue(new ScheduledTask("B", priority: 1, delay: TimeSpan.Zero, timeout: TimeSpan.FromSeconds(1)));
            pq.Enqueue(new ScheduledTask("C", priority: 3, delay: TimeSpan.FromSeconds(2), timeout: TimeSpan.FromSeconds(2)));
            pq.Save("scheduler_state.json");

            var restored = SimplePriorityQueue.Load("scheduler_state.json");
            restored.Reschedule("C", TimeSpan.FromSeconds(5));
            while (restored.Count > 0)
            {
                var task = restored.Dequeue();
                Console.WriteLine($"Выполняется {task.Name} (priority {task.Priority})");
                if (task.Delay > TimeSpan.Zero) System.Threading.Thread.Sleep(task.Delay);
                // эмуляция тайм-аута
                var started = DateTime.UtcNow;
                if (DateTime.UtcNow - started > task.Timeout)
                {
                    Console.WriteLine($"Тайм-аут задачи {task.Name}");
                }
            }
        }
    }

    private sealed class ScheduledTask
    {
        public string Name { get; }
        public int Priority { get; }
        public TimeSpan Delay { get; }
        public TimeSpan Timeout { get; }

        public ScheduledTask(string name, int priority, TimeSpan delay, TimeSpan timeout)
        {
            Name = name;
            Priority = priority;
            Delay = delay;
            Timeout = timeout;
        }
    }

    private sealed class SimplePriorityQueue
    {
        private readonly List<ScheduledTask> _list = new();
        public int Count => _list.Count;
        public void Enqueue(ScheduledTask task) { _list.Add(task); }
        public ScheduledTask Dequeue()
        {
            var max = _list.OrderBy(t => t.Priority).First();
            _list.Remove(max);
            return max;
        }

        public void Save(string path)
        {
            var items = _list.Select(t => new SerializableTask
            {
                Name = t.Name,
                Priority = t.Priority,
                Delay = t.Delay,
                Timeout = t.Timeout
            }).ToList();
            File.WriteAllText(path, ToJson(items));
        }

        public static SimplePriorityQueue Load(string path)
        {
            var queue = new SimplePriorityQueue();
            if (!File.Exists(path)) return queue;
            var items = FromJson<List<SerializableTask>>(File.ReadAllText(path)) ?? new List<SerializableTask>();
            foreach (var i in items) queue.Enqueue(new ScheduledTask(i.Name, i.Priority, i.Delay, i.Timeout));
            return queue;
        }

        private sealed class SerializableTask
        {
            public string Name { get; set; } = "";
            public int Priority { get; set; }
            public TimeSpan Delay { get; set; }
            public TimeSpan Timeout { get; set; }
        }

        public void Reschedule(string name, TimeSpan newDelay)
        {
            var found = _list.FirstOrDefault(t => t.Name == name);
            if (found == null) return;
            _list.Remove(found);
            Enqueue(new ScheduledTask(found.Name, found.Priority, newDelay, found.Timeout));
        }
    }

    private sealed class SimpleMinPriorityQueue<TItem, TPriority> where TPriority : IComparable<TPriority>
    {
        private readonly List<(TItem item, TPriority priority)> _items = new();

        public void Enqueue(TItem item, TPriority priority) => _items.Add((item, priority));

        public bool TryDequeue(out TItem item, out TPriority priority)
        {
            if (_items.Count == 0)
            {
                item = default!;
                priority = default!;
                return false;
            }

            var min = _items.OrderBy(p => p.priority).First();
            _items.Remove(min);
            item = min.item;
            priority = min.priority;
            return true;
        }
    }

    // -------- 15. API обработки изображений (эмуляция) --------
    private static class ImageProcessingApi
    {
        private static readonly LruCache<string, int[,]> Cache = new(3);
        private static readonly Queue<Action> WorkQueue = new();

        public static void Run()
        {
            Console.WriteLine("Эмуляция фильтра: инверсия пикселей 3x3");
            var img = new[,] { { 0, 128, 255 }, { 64, 128, 64 }, { 255, 0, 128 } };
            Cache.Set("original", img);

            EnqueueWork(() => Cache.Set("inverted", ApplyFilter(img, v => 255 - v)));
            EnqueueWork(() => Cache.Set("rotated", Rotate90(img)));
            EnqueueWork(() => Cache.Set("resized", Resize(img, 2, 2)));
            ProcessQueue();

            var inverted = Cache.Get("inverted");
            Console.WriteLine($"Pixel (0,0) до={img[0,0]}, после={inverted?[0,0]}");
            File.WriteAllText("image_metrics.json", ToJson(new { Cached = Cache.Count }));
        }

        private static int[,] ApplyFilter(int[,] source, Func<int, int> filter)
        {
            var r = new int[source.GetLength(0), source.GetLength(1)];
            for (var i = 0; i < source.GetLength(0); i++)
            for (var j = 0; j < source.GetLength(1); j++)
                r[i, j] = filter(source[i, j]);
            return r;
        }

        private static int[,] Rotate90(int[,] source)
        {
            var rows = source.GetLength(0);
            var cols = source.GetLength(1);
            var r = new int[cols, rows];
            for (var i = 0; i < rows; i++)
            for (var j = 0; j < cols; j++)
                r[j, rows - 1 - i] = source[i, j];
            return r;
        }

        private static int[,] Resize(int[,] source, int newRows, int newCols)
        {
            var r = new int[newRows, newCols];
            for (var i = 0; i < newRows; i++)
            for (var j = 0; j < newCols; j++)
            {
                var srcI = i * source.GetLength(0) / newRows;
                var srcJ = j * source.GetLength(1) / newCols;
                r[i, j] = source[srcI, srcJ];
            }
            return r;
        }

        private static void EnqueueWork(Action action) => WorkQueue.Enqueue(action);

        private static void ProcessQueue()
        {
            var tasks = new List<System.Threading.Tasks.Task>();
            while (WorkQueue.Count > 0)
            {
                var work = WorkQueue.Dequeue();
                tasks.Add(System.Threading.Tasks.Task.Run(work));
            }
            System.Threading.Tasks.Task.WaitAll(tasks.ToArray());
        }
    }

    private sealed class LruCache<TKey, TValue> where TKey : notnull
    {
        private readonly int _capacity;
        private readonly Dictionary<TKey, LinkedListNode<(TKey key, TValue value)>> _map = new();
        private readonly LinkedList<(TKey key, TValue value)> _list = new();

        public int Count => _map.Count;

        public LruCache(int capacity)
        {
            _capacity = capacity;
        }

        public void Set(TKey key, TValue value)
        {
            if (_map.TryGetValue(key, out var node))
            {
                _list.Remove(node);
            }
            else if (_map.Count >= _capacity)
            {
                var lru = _list.Last;
                if (lru != null)
                {
                    _map.Remove(lru.Value.key);
                    _list.RemoveLast();
                }
            }

            var newNode = new LinkedListNode<(TKey key, TValue value)>((key, value));
            _list.AddFirst(newNode);
            _map[key] = newNode;
        }

        public TValue? Get(TKey key)
        {
            if (_map.TryGetValue(key, out var node))
            {
                _list.Remove(node);
                _list.AddFirst(node);
                return node.Value.value;
            }
            return default;
        }
    }

    // -------- 16. Финансовые операции --------
    private static class FinanceModule
    {
        public static void Run()
        {
            Console.WriteLine("Импорт CSV или JSON операций amount;category");
            Console.Write("Файл: ");
            var path = Console.ReadLine();
            var ops = Import(path);
            var classified = Classify(ops);
            var avg = ops.Average(o => o.Amount);
            var medianArr = ops.Select(o => o.Amount).OrderBy(a => a).ToArray();
            var med = medianArr.Length == 0 ? 0 : (medianArr.Length % 2 == 1 ? medianArr[medianArr.Length / 2] : (medianArr[medianArr.Length / 2 - 1] + medianArr[medianArr.Length / 2]) / 2);
            var sigma = StdDev(ops.Select(o => o.Amount));
            var mean = (double)avg;
            var anomalies = ops.Where(o => Math.Abs((double)o.Amount - mean) > 3 * sigma).ToList();
            Console.WriteLine($"Среднее: {avg}, медиана: {med}, sigma: {sigma}, аномалий: {anomalies.Count}");
            File.WriteAllText("finance_report.json", ToJson(classified));
            File.WriteAllText("finance_anomalies.json", ToJson(anomalies));
            Console.WriteLine("Отчеты finance_report.json и finance_anomalies.json сохранены");
        }

        private static List<FinanceOperation> Import(string? path)
        {
            var ops = new List<FinanceOperation>();
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                var ext = Path.GetExtension(path).ToLowerInvariant();
                if (ext == ".json")
                {
                    ops = FromJson<List<FinanceOperation>>(File.ReadAllText(path)) ?? ops;
                }
                else
                {
                    foreach (var line in File.ReadLines(path))
                    {
                        var p = line.Split(';');
                        if (p.Length < 2) continue;
                        if (!decimal.TryParse(p[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var amt)) continue;
                        ops.Add(new FinanceOperation { Amount = amt, Category = p[1] });
                    }
                }
            }
            else
            {
                ops.Add(new FinanceOperation { Amount = 1000, Category = "Доход" });
                ops.Add(new FinanceOperation { Amount = -200, Category = "Расход" });
            }

            return ops;
        }

        private static List<FinanceOperation> Classify(List<FinanceOperation> ops)
        {
            var rules = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["зарплата"] = "Доход",
                ["refund"] = "Возврат",
                ["shop"] = "Покупка"
            };
            foreach (var op in ops)
            {
                foreach (var rule in rules)
                {
                    if (op.Category.Contains(rule.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        op.Category = rule.Value;
                    }
                }
                op.ExternalCheck = MockExternalCheck(op);
            }
            return ops;
        }

        private static string MockExternalCheck(FinanceOperation op)
        {
            // Эмуляция сверки с внешним источником
            return op.Amount > 10_000 ? "Review" : "Ok";
        }

        private static double StdDev(IEnumerable<decimal> values)
        {
            var arr = values.Select(Convert.ToDouble).ToArray();
            if (arr.Length == 0) return 0;
            var mean = arr.Average();
            var variance = arr.Sum(v => Math.Pow(v - mean, 2)) / arr.Length;
            return Math.Sqrt(variance);
        }
    }

    private sealed class FinanceOperation
    {
        public decimal Amount { get; set; }
        public string Category { get; set; } = "";
        public string? ExternalCheck { get; set; }
    }

    // -------- 17. Конструктор тестов производительности --------
    private static class PerfTestConstructor
    {
        public static void Run()
        {
            Console.WriteLine("Введите сценарии в DSL: threads=5;duration=100;msg=hello (пустая строка - старт)");
            var scenarios = new List<PerfScenario>();
            while (true)
            {
                var line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) break;
                scenarios.Add(ParseScenario(line));
            }

            foreach (var s in scenarios)
            {
                var stopwatch = Stopwatch.StartNew();
                var tasks = new List<System.Threading.Tasks.Task>();
                for (var i = 0; i < s.Threads; i++)
                {
                    tasks.Add(System.Threading.Tasks.Task.Run(() =>
                    {
                        System.Threading.Thread.Sleep(s.Duration);
                    }));
                }

                System.Threading.Tasks.Task.WaitAll(tasks.ToArray());
                stopwatch.Stop();
                s.ExecutionMs = stopwatch.ElapsedMilliseconds;
                Console.WriteLine($"Выполнение: {s.Threads} потоков, {s.Duration} мс, факт {s.ExecutionMs} мс, msg={s.Message}");
            }

            File.WriteAllText("perf_results.json", ToJson(scenarios));
            Console.WriteLine("Сценарии сохранены");

            Console.Write("Путь ко второму отчету для сравнения (пусто - пропуск): ");
            var cmp = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(cmp) && File.Exists(cmp))
            {
                var other = FromJson<List<PerfScenario>>(File.ReadAllText(cmp)) ?? new List<PerfScenario>();
                var comparison = scenarios.Zip(other, (a, b) => $"{a.Message}: {a.ExecutionMs} vs {b.ExecutionMs}");
                File.WriteAllLines("perf_comparison.txt", comparison);
                Console.WriteLine("Сравнение сохранено в perf_comparison.txt");
            }
        }

        private static PerfScenario ParseScenario(string line)
        {
            var parts = line.Split(';', StringSplitOptions.RemoveEmptyEntries);
            var scenario = new PerfScenario();
            foreach (var part in parts)
            {
                var kv = part.Split('=', 2);
                if (kv.Length != 2) continue;
                switch (kv[0])
                {
                    case "threads": scenario.Threads = int.Parse(kv[1]); break;
                    case "duration": scenario.Duration = int.Parse(kv[1]); break;
                    case "msg": scenario.Message = kv[1]; break;
                }
            }
            return scenario;
        }
    }

    private sealed class PerfScenario
    {
        public int Threads { get; set; }
        public int Duration { get; set; }
        public string Message { get; set; } = "";
        public long ExecutionMs { get; set; }
    }

    // -------- 18. Инструмент миграции данных --------
    private static class DataMigrationTool
    {
        public static void Run()
        {
            Console.Write("Исходный CSV (поля через ';'): ");
            var path = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                Console.WriteLine("Файл не найден");
                return;
            }

            var errors = new List<string>();
            var rows = File.ReadAllLines(path)
                .Select((line, index) => (line, index))
                .Select(tuple =>
                {
                    var parts = tuple.line.Split(';').Select(f => f.Trim()).ToArray();
                    if (parts.Length > 0) parts[0] = Mask(parts[0]);
                    if (parts.Any(string.IsNullOrWhiteSpace))
                    {
                        errors.Add($"Строка {tuple.index + 1}: пустые поля");
                    }
                    return parts;
                })
                .ToList();

            var json = ToJson(rows);
            File.WriteAllText("migration_output.json", json);
            File.WriteAllLines("migration_errors.txt", errors);
            Console.WriteLine($"Экспортировано в migration_output.json, ошибок: {errors.Count}");
        }

        private static string Mask(string value) => value.Length <= 2 ? "**" : value[..2] + new string('*', value.Length - 2);
    }

    // -------- 19. Расписание поездов --------
    private static class TrainSchedule
    {
        public static void Run()
        {
            var events = new SimpleMinPriorityQueue<TrainEvent, DateTime>();
            var start = DateTime.Now;
            events.Enqueue(new TrainEvent { Train = "A", Action = "Arrive", Time = start.AddMinutes(5) }, start.AddMinutes(5));
            events.Enqueue(new TrainEvent { Train = "B", Action = "Depart", Time = start.AddMinutes(2) }, start.AddMinutes(2));

            Console.Write("Задержка по поезду A (минуты, пусто - нет): ");
            if (int.TryParse(Console.ReadLine(), out var delay))
            {
                events.Enqueue(new TrainEvent { Train = "A", Action = "Delayed", Time = start.AddMinutes(delay) }, start.AddMinutes(delay));
            }

            var timeline = new StringBuilder();
            var delays = new List<string>();
            while (events.TryDequeue(out var ev, out _))
            {
                timeline.AppendLine($"{ev.Time:HH:mm}: {ev.Train} {ev.Action}");
                if (ev.Action == "Delayed")
                {
                    delays.Add($"{ev.Train} delayed to {ev.Time:HH:mm}");
                }
            }

            var gantt = BuildGantt(new[]
            {
                new TrainEvent { Train = "A", Action = "Route", Time = start.AddMinutes(0) },
                new TrainEvent { Train = "A", Action = "Route", Time = start.AddMinutes(10) },
                new TrainEvent { Train = "B", Action = "Route", Time = start.AddMinutes(0) },
                new TrainEvent { Train = "B", Action = "Route", Time = start.AddMinutes(8) }
            });

            File.WriteAllText("train_timeline.txt", timeline.ToString());
            File.WriteAllLines("train_gantt.txt", gantt);
            File.WriteAllLines("train_delays.txt", delays);
            Console.WriteLine("Расписание сохранено");
        }

        private static IEnumerable<string> BuildGantt(IEnumerable<TrainEvent> routes)
        {
            var grouped = routes.GroupBy(r => r.Train);
            foreach (var g in grouped)
            {
                var sb = new StringBuilder();
                sb.Append(g.Key).Append(" | ");
                foreach (var ev in g.OrderBy(e => e.Time))
                {
                    sb.Append(new string('-', ev.Time.Minute / 2));
                    sb.Append("|");
                }
                yield return sb.ToString();
            }
        }
    }

    private sealed class TrainEvent
    {
        public string Train { get; set; } = "";
        public string Action { get; set; } = "";
        public DateTime Time { get; set; }
    }

    private sealed class LogisticsEvent
    {
        public string Truck { get; set; } = "";
        public string Action { get; set; } = "";
        public string Location { get; set; } = "";
        public DateTime Time { get; set; }
    }

    // -------- 20. Система оповещений о погоде --------
    private static class WeatherAlerts
    {
        public static void Run()
        {
            Console.Write("Путь к JSON с прогнозами: ");
            var path = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                Console.WriteLine("Файл не найден, создается демо");
                File.WriteAllText("weather_demo.json", ToJson(new[]
                {
                    new WeatherForecast { Region = "RU-MOW", Temperature = -5, Alert = "Снег" }
                }));
                path = "weather_demo.json";
            }

            var forecasts = FromJson<List<WeatherForecast>>(File.ReadAllText(path)) ?? new List<WeatherForecast>();
            var cache = forecasts.ToDictionary(f => f.Region, f => f);

            Console.Write("Подписки регионов (через запятую): ");
            var subs = (Console.ReadLine() ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
            var notifications = new List<string>();
            foreach (var f in forecasts.Where(f => subs.Contains(f.Region)))
            {
                var msg = $"{f.Region}: {f.Temperature}C, {f.Alert}";
                Console.WriteLine(msg);
                notifications.Add(msg);
            }

            File.WriteAllLines("weather_notifications.log", notifications);
            // эмуляция фонового обновления кэша
            System.Threading.Tasks.Task.Run(async () =>
            {
                await System.Threading.Tasks.Task.Delay(500);
                foreach (var region in subs)
                {
                    if (cache.TryGetValue(region, out var fc))
                    {
                        fc.Temperature += 1; // обновление
                    }
                }
                File.WriteAllText("weather_cache.json", ToJson(cache.Values));
            });
        }
    }

    private sealed class WeatherForecast
    {
        public string Region { get; set; } = "";
        public double Temperature { get; set; }
        public string Alert { get; set; } = "";
    }

    // -------- 21. Каталог медиафайлов --------
    private static class MediaCatalog
    {
        public static void Run()
        {
            Console.Write("Каталог медиа: ");
            var dir = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(dir) || !Directory.Exists(dir))
            {
                Console.WriteLine("Нет каталога");
                return;
            }

            var files = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);
            var rnd = new Random(10);
            var items = files.Select(f => new MediaItem
            {
                Path = f,
                Size = new FileInfo(f).Length,
                DurationSeconds = rnd.Next(60, 400),
                Genre = GuessGenre(f)
            }).ToList();
            Console.WriteLine("Файлов: " + items.Count);

            Console.Write("Фильтр жанра (пусто - все): ");
            var genre = Console.ReadLine();
            Console.Write("Поиск по имени (пусто - без поиска): ");
            var query = Console.ReadLine();
            var filtered = items.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(genre))
            {
                filtered = filtered.Where(i => string.Equals(i.Genre, genre, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrWhiteSpace(query))
            {
                filtered = filtered.Where(i => Path.GetFileName(i.Path).Contains(query, StringComparison.OrdinalIgnoreCase));
            }
            File.WriteAllText("media_catalog.json", ToJson(items));
            File.WriteAllText("playlist.m3u", string.Join(Environment.NewLine, filtered.Select(i => i.Path)));
            Console.WriteLine("Каталог и плейлист сохранены");
        }

        private static string GuessGenre(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return ext switch
            {
                ".mp3" => "Music",
                ".mp4" => "Video",
                _ => "Other"
            };
        }
    }

    private sealed class MediaItem
    {
        public string Path { get; set; } = "";
        public long Size { get; set; }
        public int DurationSeconds { get; set; }
        public string Genre { get; set; } = "";
    }

    // -------- 22. Kanban --------
    private static class KanbanManager
    {
        public static void Run()
        {
            var board = new KanbanBoard();
            board.Columns.Add(new KanbanColumn { Name = "Todo", WipLimit = 3 });
            board.Columns.Add(new KanbanColumn { Name = "In Progress", WipLimit = 2 });
            board.Columns.Add(new KanbanColumn { Name = "Done", WipLimit = 5 });
            board.Columns[0].Tasks.Add(new KanbanTask { Title = "Сделать задание", Assignee = "QA", Due = DateTime.Today.AddDays(1) });
            board.Columns[0].Tasks.Add(new KanbanTask { Title = "Починить баг", Assignee = "Dev", Due = DateTime.Today.AddDays(2) });
            ApplyAutoRules(board);
            Console.WriteLine("Колонки: " + string.Join(", ", board.Columns.Select(c => c.Name)));
            File.WriteAllText("kanban.json", ToJson(board));
            File.WriteAllText("kanban.md", board.ToMarkdown());
            File.WriteAllText("kanban.html", board.ToHtml());
            Console.WriteLine("API заглушка GET /tasks возвращает количество: " + board.Columns.Sum(c => c.Tasks.Count));
        }

        private static void ApplyAutoRules(KanbanBoard board)
        {
            // Автоназначение ревьюера
            foreach (var task in board.Columns.SelectMany(c => c.Tasks))
            {
                if (string.IsNullOrWhiteSpace(task.Reviewer)) task.Reviewer = "Reviewer";
                if (task.Due < DateTime.Today) task.Overdue = true;
            }

            // WIP контроль
            foreach (var column in board.Columns)
            {
                if (column.Tasks.Count > column.WipLimit)
                {
                    Console.WriteLine($"WIP предупреждение: колонка {column.Name} превышает лимит");
                }
            }
        }
    }

    private sealed class KanbanBoard
    {
        public List<KanbanColumn> Columns { get; set; } = new();

        public string ToMarkdown()
        {
            var sb = new StringBuilder();
            foreach (var col in Columns)
            {
                sb.AppendLine($"## {col.Name} (WIP {col.Tasks.Count}/{col.WipLimit})");
                foreach (var t in col.Tasks)
                {
                    sb.AppendLine($"- {t.Title} [{t.Assignee}] Reviewer: {t.Reviewer} {(t.Overdue ? "**OVERDUE**" : "")}");
                }
            }
            return sb.ToString();
        }

        public string ToHtml()
        {
            var sb = new StringBuilder();
            sb.Append("<html><body>");
            foreach (var col in Columns)
            {
                sb.Append($"<h2>{col.Name} (WIP {col.Tasks.Count}/{col.WipLimit})</h2><ul>");
                foreach (var t in col.Tasks)
                {
                    sb.Append($"<li>{t.Title} [{t.Assignee}] Reviewer: {t.Reviewer} {(t.Overdue ? "(OVERDUE)" : "")}</li>");
                }
                sb.Append("</ul>");
            }
            sb.Append("</body></html>");
            return sb.ToString();
        }
    }

    private sealed class KanbanColumn
    {
        public string Name { get; set; } = "";
        public int WipLimit { get; set; }
        public List<KanbanTask> Tasks { get; set; } = new();
    }

    private sealed class KanbanTask
    {
        public string Title { get; set; } = "";
        public string Assignee { get; set; } = "";
        public string Reviewer { get; set; } = "";
        public DateTime Due { get; set; }
        public bool Overdue { get; set; }
    }

    // -------- 23. Платежный шлюз --------
    private static class PaymentGateway
    {
        public static void Run()
        {
            Console.WriteLine("Эмуляция платежа: валидируем, проводим, логируем.");
            var journal = new List<string>();
            var id = Guid.NewGuid().ToString();
            journal.Add($"[{DateTime.UtcNow:O}] Received {id}");
            var providers = new List<IPaymentProvider> { new FastPay(), new SafePay() };
            var amount = 100m;
            journal.Add($"[{DateTime.UtcNow:O}] Validated {id}");
            foreach (var provider in providers)
            {
                if (TryProcess(provider, id, amount, out var fee))
                {
                    journal.Add($"[{DateTime.UtcNow:O}] Provider={provider.Name} ok, fee={fee}");
                    break;
                }
                journal.Add($"[{DateTime.UtcNow:O}] Provider={provider.Name} failed, retry next");
            }

            File.WriteAllLines("payments.log", journal);
            Console.WriteLine("Лог сохранен в payments.log");
        }

        private static bool TryProcess(IPaymentProvider provider, string id, decimal amount, out decimal fee)
        {
            fee = provider.CalculateFee(amount);
            var attempts = 0;
            var rnd = new Random();
            while (attempts < 2)
            {
                attempts++;
                if (provider.Process(id, amount))
                {
                    return true;
                }
                if (rnd.NextDouble() < 0.3) System.Threading.Thread.Sleep(50); // эмуляция повторной попытки
            }
            return false;
        }
    }

    private interface IPaymentProvider
    {
        string Name { get; }
        bool Process(string id, decimal amount);
        decimal CalculateFee(decimal amount);
    }

    private sealed class FastPay : IPaymentProvider
    {
        public string Name => "FastPay";
        public bool Process(string id, decimal amount) => true;
        public decimal CalculateFee(decimal amount) => amount * 0.01m;
    }

    private sealed class SafePay : IPaymentProvider
    {
        public string Name => "SafePay";
        public bool Process(string id, decimal amount) => false;
        public decimal CalculateFee(decimal amount) => amount * 0.02m;
    }

    // -------- 24. Бронирование переговорных --------
    private static class RoomBooking
    {
        public static void Run()
        {
            var bookings = new List<Booking>
            {
                new() { Room = "101", Start = DateTime.Today.AddHours(10), End = DateTime.Today.AddHours(11) }
            };
            Console.Write("Импорт существующих броней из CSV? (путь/пусто): ");
            var import = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(import) && File.Exists(import))
            {
                foreach (var line in File.ReadLines(import))
                {
                    var parts = line.Split(';');
                    if (parts.Length == 3 &&
                        DateTime.TryParse(parts[1], out var s) &&
                        DateTime.TryParse(parts[2], out var e))
                    {
                        bookings.Add(new Booking { Room = parts[0], Start = s, End = e });
                    }
                }
            }

            Console.Write("Новая бронь (HH:mm-HH:mm): ");
            var raw = Console.ReadLine();
            if (raw != null && raw.Contains('-'))
            {
                var parts = raw.Split('-');
                if (TimeSpan.TryParse(parts[0], out var s) && TimeSpan.TryParse(parts[1], out var e))
                {
                    var start = DateTime.Today.Add(s);
                    var end = DateTime.Today.Add(e);
                    if (bookings.Any(b => b.Room == "101" && !(end <= b.Start || start >= b.End)))
                    {
                        Console.WriteLine("Конфликт бронирования");
                    }
                    else
                    {
                        bookings.Add(new Booking { Room = "101", Start = start, End = end });
                        Console.WriteLine("Бронь добавлена");
                    }
                }
            }

            Console.WriteLine("Календарь:");
            foreach (var b in bookings.OrderBy(b => b.Start))
            {
                Console.WriteLine($"{b.Room}: {b.Start:HH:mm}-{b.End:HH:mm}");
                if (b.Start - DateTime.Now < TimeSpan.FromHours(1) && b.Start > DateTime.Now)
                {
                    Console.WriteLine("Напоминание: скоро бронь " + b.Room);
                }
            }

            File.WriteAllLines("bookings.csv", bookings.Select(b => $"{b.Room};{b.Start:O};{b.End:O}"));
            File.WriteAllText("bookings.json", ToJson(bookings));
        }
    }

    public sealed class Booking
    {
        public string Room { get; set; } = "";
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

    // -------- 25. Платформа онлайн-курсов --------
    private static class OnlineCourses
    {
        public static void Run()
        {
            var course = new Course
            {
                Title = "C# Basics",
                Lessons = new List<string> { "Intro", "Variables", "Loops" },
                Students = new List<StudentProgress>
                {
                    new() { Student = "Ann", CompletedLessons = 2, Deadline = DateTime.Today.AddDays(3) },
                    new() { Student = "Bob", CompletedLessons = 1, Deadline = DateTime.Today.AddDays(-1) }
                }
            };
            Console.WriteLine($"Курс {course.Title}, студентов: {course.Students.Count}");
            foreach (var s in course.Students)
            {
                if (s.CompletedLessons >= course.Lessons.Count)
                {
                    s.Certificate = $"CERT-{course.Title}-{s.Student}";
                }
                if (s.Deadline < DateTime.Today)
                {
                    Console.WriteLine($"Дедлайн просрочен: {s.Student}");
                }
            }

            File.WriteAllText("course.json", ToJson(course));
            File.WriteAllText("course_stats.json", ToJson(new
            {
                course.Title,
                Completed = course.Students.Count(s => s.CompletedLessons >= course.Lessons.Count)
            }));
            Console.WriteLine("Статистика и сертификаты обновлены");

            Console.Write("Импортировать прогресс из JSON (путь/пусто): ");
            var imp = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(imp) && File.Exists(imp))
            {
                var imported = FromJson<List<StudentProgress>>(File.ReadAllText(imp));
                if (imported != null)
                {
                    course.Students = imported;
                    Console.WriteLine("Импортировано студентов: " + imported.Count);
                }
            }
            Console.WriteLine("Сертификаты выдано: " + course.Students.Count(s => !string.IsNullOrEmpty(s.Certificate)));
        }
    }

    private sealed class Course
    {
        public string Title { get; set; } = "";
        public List<string> Lessons { get; set; } = new();
        public List<StudentProgress> Students { get; set; } = new();
    }

    private sealed class StudentProgress
    {
        public string Student { get; set; } = "";
        public int CompletedLessons { get; set; }
        public DateTime Deadline { get; set; }
        public string? Certificate { get; set; }
    }

    // -------- 26. Симулятор биржевых торгов --------
    private static class TradingSimulator
    {
        public static void Run()
        {
            var book = new OrderBook();
            book.AddOrder(new Order { Id = "1", Type = OrderType.Buy, Price = 100, Quantity = 10 });
            book.AddOrder(new Order { Id = "2", Type = OrderType.Sell, Price = 99, Quantity = 5 });
            book.Match();
            File.WriteAllLines("trading.log", book.Log);
            File.WriteAllText("orderbook_state.json", ToJson(book.Snapshot()));
        }
    }

    private sealed class OrderBook
    {
        private readonly List<Order> _orders = new();
        public List<string> Log { get; } = new();

        public void AddOrder(Order order) => _orders.Add(order);

        public void Match()
        {
            var buys = _orders.Where(o => o.Type == OrderType.Buy).OrderByDescending(o => o.Price).ToList();
            var sells = _orders.Where(o => o.Type == OrderType.Sell).OrderBy(o => o.Price).ToList();
            foreach (var buy in buys)
            {
                foreach (var sell in sells.ToList())
                {
                    if (buy.Price < sell.Price || sell.Quantity == 0) continue;
                    var qty = Math.Min(buy.Quantity, sell.Quantity);
                    buy.Quantity -= qty;
                    sell.Quantity -= qty;
                    var line = $"Match {qty} @ {sell.Price}";
                    Console.WriteLine(line);
                    Log.Add(line);
                    if (sell.Quantity == 0) sells.Remove(sell);
                    if (buy.Quantity == 0) break;
                }
            }
        }

        public IEnumerable<Order> Snapshot() => _orders;
    }

    private sealed class Order
    {
        public string Id { get; set; } = "";
        public OrderType Type { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    private enum OrderType { Buy, Sell }

    // -------- 27. Генератор отчётов по качеству кода --------
    private static class CodeQualityReports
    {
        public static void Run()
        {
            Console.Write("Каталог проекта: ");
            var dir = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(dir) || !Directory.Exists(dir))
            {
                Console.WriteLine("Нет каталога");
                return;
            }

            var files = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
            var report = new CodeQualityReport
            {
                FileCount = files.Length,
                TotalSize = files.Sum(f => new FileInfo(f).Length),
                Timestamp = DateTime.UtcNow,
                Complexity = files.Length * 2,
                Coverage = 75
            };
            Console.WriteLine($"Файлов: {report.FileCount}, размер: {report.TotalSize}");
            SerializeXml(report, "code_quality.xml");
            File.WriteAllText("code_quality.json", ToJson(report));

            var chart = BuildAsciiChart(report);
            File.WriteAllLines("code_quality_chart.txt", chart);

            Console.Write("Файл предыдущего отчета для сравнения (пусто - пропуск): ");
            var prev = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(prev) && File.Exists(prev))
            {
                var old = FromJson<CodeQualityReport>(File.ReadAllText(prev));
                if (old != null)
                {
                    var diff = $"Delta Files: {report.FileCount - old.FileCount}, Delta Size: {report.TotalSize - old.TotalSize}";
                    File.WriteAllText("code_quality_diff.txt", diff);
                }
            }
        }

        private static IEnumerable<string> BuildAsciiChart(CodeQualityReport report)
        {
            return new[]
            {
                $"Files     | {new string('#', Math.Min(report.FileCount, 50))}",
                $"Complexity| {new string('#', Math.Min(report.Complexity, 50))}",
                $"Coverage  | {new string('#', report.Coverage / 2)} {report.Coverage}%"
            };
        }
    }

    public sealed class CodeQualityReport
    {
        public int FileCount { get; set; }
        public long TotalSize { get; set; }
        public DateTime Timestamp { get; set; }
        public int Complexity { get; set; }
        public int Coverage { get; set; }
    }

    // -------- 28. Система мониторинга IoT --------
    private static class IotMonitoring
    {
        public static void Run()
        {
            var rnd = new Random(42);
            var plugins = new List<IIotPlugin> { new TemperaturePlugin() };
            var devices = Enumerable.Range(1, 100).Select(i => new IotDevice { Id = "dev" + i }).ToList();
            var measurements = new List<IotMeasurement>();
            Parallel.ForEach(devices, d =>
            {
                foreach (var plugin in plugins)
                {
                    lock (measurements)
                    {
                        measurements.AddRange(plugin.Read(d, rnd));
                    }
                }
            });

            var alerts = measurements.Where(m => m.Value > 80).ToList();
            Console.WriteLine("Аварийных измерений: " + alerts.Count);

            var aggregated = measurements.GroupBy(m => m.DeviceId).Select(g => new
            {
                Device = g.Key,
                Avg = g.Average(x => x.Value),
                Max = g.Max(x => x.Value)
            }).ToList();

            File.WriteAllText("iot_state.json", ToJson(measurements));
            File.WriteAllText("iot_aggregated.json", ToJson(aggregated));
            Console.WriteLine("Панель:");
            foreach (var agg in aggregated.Take(5))
            {
                Console.WriteLine($"{agg.Device}: avg {agg.Avg:F1}, max {agg.Max:F1}");
            }

            Console.Write("Импорт нового типа устройства через плагин? (y/n): ");
            if (string.Equals(Console.ReadLine(), "y", StringComparison.OrdinalIgnoreCase))
            {
                plugins.Add(new TemperaturePlugin { Offset = 10 });
                Console.WriteLine("Новый плагин добавлен (Offset=10)");
            }
        }

    private sealed class IotDevice
    {
        public string Id { get; set; } = "";
    }

    private sealed class IotMeasurement
    {
        public string DeviceId { get; set; } = "";
        public double Value { get; set; }
        public DateTime Timestamp { get; set; }
    }

    private interface IIotPlugin
    {
        IEnumerable<IotMeasurement> Read(IotDevice device, Random rnd);
    }

    private sealed class TemperaturePlugin : IIotPlugin
    {
        public double Offset { get; set; }
        public IEnumerable<IotMeasurement> Read(IotDevice device, Random rnd)
        {
            for (var i = 0; i < 3; i++)
            {
                yield return new IotMeasurement
                {
                    DeviceId = device.Id,
                    Value = rnd.NextDouble() * 100 + Offset,
                    Timestamp = DateTime.UtcNow.AddSeconds(i)
                };
            }
        }
    }

}
}
