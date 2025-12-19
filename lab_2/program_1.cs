using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

/// <summary>
/// Точка входа для лабораторной работы 2. Показывает меню и запускает выбранное задание.
/// </summary>
internal static class Program
{
    private const string Divider = "----------------------------------------";

    private static void Main()
    {
        Console.WriteLine("ЛР 2. Выберите задание:");
        while (true)
        {
            Console.WriteLine(Divider);
            Console.WriteLine("1 - Операции с матрицами");
            Console.WriteLine("2 - Поиск слов по длине");
            Console.WriteLine("3 - Удаление повторяющихся символов");
            Console.WriteLine("4 - Проверка прогрессии");
            Console.WriteLine("5 - Функция Аккермана");
            Console.WriteLine("6 - Группировка по делимости");
            Console.WriteLine("0 - Выход");
            Console.Write("Ваш выбор: ");

            var choice = Console.ReadLine();
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    RunMatrixTasks();
                    break;
                case "2":
                    RunWordLengthTasks();
                    break;
                case "3":
                    RunDuplicateRemovalTask();
                    break;
                case "4":
                    RunProgressionTask();
                    break;
                case "5":
                    RunAckermannTask();
                    break;
                case "6":
                    RunDivisibilityGroupingTask();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Неизвестная команда. Повторите ввод.");
                    break;
            }
        }
    }

    /// <summary>
    /// Меню для задач с матрицами.
    /// </summary>
    private static void RunMatrixTasks()
    {
        Console.WriteLine("Подзадача с матрицами:");
        Console.WriteLine("1 - Умножить матрицу на число");
        Console.WriteLine("2 - Сложить две матрицы");
        Console.WriteLine("3 - Перемножить две матрицы");
        Console.Write("Подзадача: ");

        var option = Console.ReadLine();
        try
        {
            switch (option)
            {
                case "1":
                    var matrix = ReadMatrixFromConsole("первой матрицы");
                    Console.Write("Число (int): ");
                    var scalar = int.Parse(Console.ReadLine() ?? "0", CultureInfo.InvariantCulture);
                    var scaled = MultiplyByScalar(matrix, scalar);
                    Console.WriteLine("Результат:");
                    PrintMatrix(scaled);
                    break;
                case "2":
                    var firstAdd = ReadMatrixFromConsole("первой матрицы");
                    var secondAdd = ReadMatrixFromConsole("второй матрицы");
                    var sum = AddMatrices(firstAdd, secondAdd);
                    Console.WriteLine("Сумма:");
                    PrintMatrix(sum);
                    break;
                case "3":
                    var firstMul = ReadMatrixFromConsole("левой матрицы");
                    var secondMul = ReadMatrixFromConsole("правой матрицы");
                    var product = MultiplyMatrices(firstMul, secondMul);
                    Console.WriteLine("Произведение:");
                    PrintMatrix(product);
                    break;
                default:
                    Console.WriteLine("Нет такой подзадачи.");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    /// <summary>
    /// Читает размеры и элементы матрицы из консоли.
    /// </summary>
    /// <param name="name">Имя матрицы для подсказки.</param>
    /// <returns>Двумерный массив целых чисел.</returns>
    private static int[,] ReadMatrixFromConsole(string name)
    {
        Console.WriteLine($"Введите размеры {name}:");
        Console.Write("Строк: ");
        var rows = int.Parse(Console.ReadLine() ?? "0", CultureInfo.InvariantCulture);
        Console.Write("Столбцов: ");
        var cols = int.Parse(Console.ReadLine() ?? "0", CultureInfo.InvariantCulture);

        if (rows <= 0 || cols <= 0)
        {
            throw new ArgumentException("Размеры матрицы должны быть положительными.");
        }

        var matrix = new int[rows, cols];
        Console.WriteLine("Введите элементы построчно, разделяя пробелами:");
        for (var r = 0; r < rows; r++)
        {
            var rowValues = ReadIntegerRow(cols, $"Строка {r + 1}: ");
            for (var c = 0; c < cols; c++)
            {
                matrix[r, c] = rowValues[c];
            }
        }

        return matrix;
    }

    /// <summary>
    /// Читает одну строку с фиксированным количеством целых чисел.
    /// </summary>
    private static int[] ReadIntegerRow(int expectedCount, string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var raw = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(raw))
            {
                continue;
            }

            var parts = raw.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != expectedCount)
            {
                Console.WriteLine("Неверное количество значений.");
                continue;
            }

            if (parts.All(p => int.TryParse(p, NumberStyles.Integer, CultureInfo.InvariantCulture, out _)))
            {
                return parts
                    .Select(p => int.Parse(p, CultureInfo.InvariantCulture))
                    .ToArray();
            }

            Console.WriteLine("Не удалось распознать числа. Повторите ввод.");
        }
    }

    /// <summary>
    /// Умножает матрицу на скаляр.
    /// </summary>
    private static int[,] MultiplyByScalar(int[,] matrix, int scalar)
    {
        var rows = matrix.GetLength(0);
        var cols = matrix.GetLength(1);
        var result = new int[rows, cols];
        // Перебираем каждую ячейку и умножаем на скаляр
        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                result[r, c] = matrix[r, c] * scalar;
            }
        }

        return result;
    }

    /// <summary>
    /// Складывает две матрицы поэлементно.
    /// </summary>
    private static int[,] AddMatrices(int[,] first, int[,] second)
    {
        var rows = first.GetLength(0);
        var cols = first.GetLength(1);
        if (rows != second.GetLength(0) || cols != second.GetLength(1))
        {
            throw new ArgumentException("Для сложения размеры матриц должны совпадать.");
        }

        var result = new int[rows, cols];
        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                result[r, c] = first[r, c] + second[r, c];
            }
        }

        return result;
    }

    /// <summary>
    /// Перемножает две матрицы классическим способом.
    /// </summary>
    private static int[,] MultiplyMatrices(int[,] left, int[,] right)
    {
        var leftRows = left.GetLength(0);
        var leftCols = left.GetLength(1);
        var rightRows = right.GetLength(0);
        var rightCols = right.GetLength(1);

        if (leftCols != rightRows)
        {
            throw new ArgumentException("Число столбцов левой матрицы должно совпадать с числом строк правой.");
        }

        var result = new int[leftRows, rightCols];
        // Стандартное тройное вложение для умножения матриц
        for (var r = 0; r < leftRows; r++)
        {
            for (var c = 0; c < rightCols; c++)
            {
                var sum = 0;
                for (var k = 0; k < leftCols; k++)
                {
                    sum += left[r, k] * right[k, c];
                }

                result[r, c] = sum;
            }
        }

        return result;
    }

    /// <summary>
    /// Печатает матрицу в консоль.
    /// </summary>
    private static void PrintMatrix(int[,] matrix)
    {
        var rows = matrix.GetLength(0);
        var cols = matrix.GetLength(1);
        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                Console.Write(matrix[r, c].ToString(CultureInfo.InvariantCulture).PadLeft(6));
            }

            Console.WriteLine();
        }
    }

    /// <summary>
    /// Задача 2: поиск самого короткого и самого длинного слова.
    /// </summary>
    private static void RunWordLengthTasks()
    {
        Console.WriteLine("Введите текст:");
        var text = Console.ReadLine() ?? string.Empty;

        var shortest = FindShortestWord(text);
        var longest = FindLongestWords(text);

        Console.WriteLine($"Минимальное слово: {shortest}");
        Console.WriteLine("Максимальные слова: " + string.Join(", ", longest));
    }

    /// <summary>
    /// Разбивает текст на слова, удаляя знаки препинания и пробелы.
    /// </summary>
    private static IEnumerable<string> SplitWords(string text)
    {
        var separators = new[]
        {
            ' ', '.', ',', ';', ':', '!', '?', '\t', '\n', '\r'
        };
        return text
            .Split(separators, StringSplitOptions.RemoveEmptyEntries)
            .Select(w => w.Trim());
    }

    /// <summary>
    /// Находит первое по порядку самое короткое слово.
    /// </summary>
    private static string FindShortestWord(string text)
    {
        var words = SplitWords(text).ToList();
        if (words.Count == 0)
        {
            return string.Empty;
        }

        // Сортируем по длине и берём первое слово
        return words.OrderBy(w => w.Length).First();
    }

    /// <summary>
    /// Находит все самые длинные слова.
    /// </summary>
    private static IReadOnlyCollection<string> FindLongestWords(string text)
    {
        var words = SplitWords(text).ToList();
        if (words.Count == 0)
        {
            return Array.Empty<string>();
        }

        var maxLength = words.Max(w => w.Length);
        // Фильтр по максимальной длине + Distinct по регистронезависимому сравнению
        return words
            .Where(w => w.Length == maxLength)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    /// <summary>
    /// Задача 3: удаляет подряд идущие одинаковые символы (без учёта регистра).
    /// </summary>
    private static void RunDuplicateRemovalTask()
    {
        Console.WriteLine("Введите текст для очистки:");
        var text = Console.ReadLine() ?? string.Empty;
        var cleaned = RemoveAdjacentDuplicates(text);
        Console.WriteLine("Результат:");
        Console.WriteLine(cleaned);
    }

    /// <summary>
    /// Схлопывает повторы символов: серия одинаковых букв превращается в одну нижнего регистра.
    /// </summary>
    private static string RemoveAdjacentDuplicates(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        var builder = new StringBuilder(text.Length);
        char? previous = null;
        foreach (var ch in text)
        {
            var lower = char.ToLowerInvariant(ch);
            if (previous.HasValue && previous.Value == lower)
            {
                // Пропускаем символ, если совпадает с предыдущим (в нижнем регистре)
                continue;
            }

            builder.Append(lower);
            previous = lower;
        }

        return builder.ToString();
    }

    /// <summary>
    /// Задача 4: определяет тип прогрессии.
    /// </summary>
    private static void RunProgressionTask()
    {
        Console.WriteLine("Введите числа через пробел:");
        var raw = Console.ReadLine() ?? string.Empty;
        var parts = raw.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var sequence = parts
            .Select(p => double.Parse(p, CultureInfo.InvariantCulture))
            .ToArray();

        var result = DetectProgression(sequence);
        Console.WriteLine($"Результат: {ProgressionTypeToText(result)}");
    }

    /// <summary>
    /// Варианты прогрессий.
    /// </summary>
    private enum ProgressionType
    {
        None,
        Arithmetic,
        Geometric,
        Both
    }

    /// <summary>
    /// Определяет, является ли последовательность арифметической, геометрической или обеих типов.
    /// </summary>
    private static ProgressionType DetectProgression(IReadOnlyList<double> sequence)
    {
        if (sequence.Count < 2)
        {
            return ProgressionType.None;
        }

        var tolerance = 1e-9;
        var isArithmetic = true;
        var isGeometric = true;

        var difference = sequence[1] - sequence[0];
        var ratio = Math.Abs(sequence[0]) < tolerance
            ? double.NaN
            : sequence[1] / sequence[0];

        for (var i = 2; i < sequence.Count; i++)
        {
            // Проверяем одинаковую разность
            if (Math.Abs(sequence[i] - sequence[i - 1] - difference) > tolerance)
            {
                isArithmetic = false;
            }

            // Проверяем одинаковое отношение
            if (Math.Abs(sequence[i - 1]) < tolerance)
            {
                isGeometric = false;
            }
            else if (Math.Abs(sequence[i] / sequence[i - 1] - ratio) > tolerance)
            {
                isGeometric = false;
            }
        }

        return (isArithmetic, isGeometric) switch
        {
            (true, true) => ProgressionType.Both,
            (true, false) => ProgressionType.Arithmetic,
            (false, true) => ProgressionType.Geometric,
            _ => ProgressionType.None
        };
    }

    /// <summary>
    /// Преобразует тип прогрессии в человекочитаемый текст.
    /// </summary>
    private static string ProgressionTypeToText(ProgressionType type) =>
        type switch
        {
            ProgressionType.Arithmetic => "арифметическая",
            ProgressionType.Geometric => "геометрическая",
            ProgressionType.Both => "арифметическая и геометрическая",
            _ => "не прогрессия"
        };

    /// <summary>
    /// Задача 5: запуск вычисления функции Аккермана.
    /// </summary>
    private static void RunAckermannTask()
    {
        Console.WriteLine("Функция Аккермана. Введите n:");
        var n = int.Parse(Console.ReadLine() ?? "0", CultureInfo.InvariantCulture);
        Console.WriteLine("Введите m:");
        var m = int.Parse(Console.ReadLine() ?? "0", CultureInfo.InvariantCulture);

        try
        {
            var value = Ackermann(n, m);
            Console.WriteLine($"A({n}, {m}) = {value}");
            Console.WriteLine($"Примеры: A(2, 5) = {Ackermann(2, 5)}, A(1, 2) = {Ackermann(1, 2)}");
        }
        catch (StackOverflowException)
        {
            Console.WriteLine("Введены слишком большие значения для рекурсивного расчёта.");
        }
    }

    /// <summary>
    /// Рекурсивно вычисляет функцию Аккермана.
    /// </summary>
    private static long Ackermann(long n, long m)
    {
        if (n == 0)
        {
            return m + 1;
        }

        if (m == 0)
        {
            // Ветка m = 0: уменьшаем n и переходим к A(n-1, 1)
            return Ackermann(n - 1, 1);
        }

        // Общий случай: A(n-1, A(n, m-1))
        return Ackermann(n - 1, Ackermann(n, m - 1));
    }

    /// <summary>
    /// Задача 6: группировка чисел по делимости.
    /// </summary>
    private static void RunDivisibilityGroupingTask()
    {
        Console.WriteLine("Группировка по делимости:");
        Console.Write("Путь к файлу с N: ");
        var inputPath = Console.ReadLine() ?? string.Empty;
        if (!File.Exists(inputPath))
        {
            Console.WriteLine("Файл не найден.");
            return;
        }

        if (!long.TryParse(File.ReadAllText(inputPath).Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var n) ||
            n < 1)
        {
            Console.WriteLine("В файле должно быть положительное целое N.");
            return;
        }

        Console.WriteLine("Режим работы:");
        Console.WriteLine("1 - Показать только количество групп");
        Console.WriteLine("2 - Сформировать группы и записать в файл");
        Console.Write("Выбор: ");
        var mode = Console.ReadLine();

        var stopwatch = Stopwatch.StartNew();
        var groupCount = CalculateMinimalGroupCount(n);

        if (mode == "1")
        {
            stopwatch.Stop();
            Console.WriteLine($"Необходимо групп: {groupCount}");
            Console.WriteLine($"Время: {stopwatch.Elapsed.TotalSeconds:F3} с");
            return;
        }

        var groups = BuildGroups(n);
        stopwatch.Stop();

        var outputPath = Path.Combine(Path.GetDirectoryName(inputPath) ?? ".", "groups_output.txt");
        WriteGroupsToFile(groups, outputPath);
        Console.WriteLine($"Группы сохранены в {outputPath}");
        Console.WriteLine($"Время: {stopwatch.Elapsed.TotalSeconds:F3} с");

        Console.Write("Заархивировать результат? (y/n): ");
        var answer = Console.ReadLine();
        if (string.Equals(answer, "y", StringComparison.OrdinalIgnoreCase))
        {
            var zipPath = ArchiveFile(outputPath);
            Console.WriteLine($"Архив создан: {zipPath}");
        }
    }

    /// <summary>
    /// Считает минимальное количество групп: равно длине максимальной цепочки делимости (степени двойки).
    /// </summary>
    private static int CalculateMinimalGroupCount(long n)
    {
        var groups = 1;
        var value = 1L;
        while (value * 2 <= n)
        {
            value *= 2;
            groups++;
        }

        return groups;
    }

    /// <summary>
    /// Формирует группы: номер группы = число простых множителей (с повторениями) + 1.
    /// </summary>
    private static Dictionary<int, List<long>> BuildGroups(long n)
    {
        var groups = new Dictionary<int, List<long>>();
        for (long number = 1; number <= n; number++)
        {
            var groupIndex = number == 1 ? 1 : CountPrimeFactors(number) + 1;
            if (!groups.TryGetValue(groupIndex, out var list))
            {
                list = new List<long>();
                groups[groupIndex] = list;
            }

            // Добавляем число в соответствующую группу
            list.Add(number);
        }

        return groups;
    }

    /// <summary>
    /// Подсчитывает количество простых множителей числа с учётом кратности.
    /// </summary>
    private static int CountPrimeFactors(long number)
    {
        if (number < 2)
        {
            return 0;
        }

        var count = 0;
        while (number % 2 == 0)
        {
            count++;
            number /= 2;
        }

        var divisor = 3L;
        while (divisor * divisor <= number)
        {
            while (number % divisor == 0)
            {
                count++;
                number /= divisor;
            }

            divisor += 2;
        }

        if (number > 1)
        {
            // Остался простой множитель > 1
            count++;
        }

        return count;
    }

    /// <summary>
    /// Записывает группы в текстовый файл.
    /// </summary>
    private static void WriteGroupsToFile(IDictionary<int, List<long>> groups, string path)
    {
        using var writer = new StreamWriter(path, false, Encoding.UTF8);
        foreach (var kvp in groups.OrderBy(k => k.Key))
        {
            writer.WriteLine($"Группа {kvp.Key}: {string.Join(' ', kvp.Value)}");
        }

        writer.WriteLine($"M = {groups.Keys.Max()}");
    }

    /// <summary>
    /// Архивирует файл в zip.
    /// </summary>
    /// <returns>Путь к созданному архиву.</returns>
    private static string ArchiveFile(string filePath)
    {
        var zipPath = Path.ChangeExtension(filePath, ".zip");
        using var archive = ZipFile.Open(zipPath, ZipArchiveMode.Create);
        archive.CreateEntryFromFile(filePath, Path.GetFileName(filePath), CompressionLevel.Optimal);
        return zipPath;
    }
}
