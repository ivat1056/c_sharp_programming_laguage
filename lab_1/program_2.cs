using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;

/// <summary>
/// Программа с набором учебных заданий: математика, строки, рекурсия, алгоритмы.
/// Организована через меню по блокам, каждое задание сопровождается примерами.
/// </summary>
class Program
{
    /// <summary>
    /// Точка входа: отображает главное меню и запускает выбранный блок.
    /// </summary>
    /// <param name="args">Аргументы командной строки (не используются).</param>
    static void Main(string[] args)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Меню заданий ===");
            Console.WriteLine("1 - Блок 1: Матрицы и векторы");
            Console.WriteLine("2 - Блок 2: Работа со строками и текстом");
            Console.WriteLine("3 - Блок 3: Числовые последовательности и прогрессии");
            Console.WriteLine("4 - Блок 4: Рекурсия и факториальные задачи");
            Console.WriteLine("5 - Блок 5: Алгоритмы и дополнительные возможности");
            Console.WriteLine("0 - Выход");
            Console.Write("Ваш выбор: ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    RunBlock1();
                    break;
                case "2":
                    RunBlock2();
                    break;
                case "3":
                    RunBlock3();
                    break;
                case "4":
                    RunBlock4();
                    break;
                case "5":
                    RunBlock5();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Неизвестная команда. Нажмите любую клавишу...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    /// <summary>
    /// Демонстрации блока 1: матрицы и векторы (задания 1-3).
    /// </summary>
    static void RunBlock1()
    {
        Console.Clear();
        Console.WriteLine("=== Блок 1: Матрицы и векторы ===");

        double[] vector = { 1, 2, 3 };
        double[] scaled = MultiplyVector(vector, 3);
        Console.WriteLine("1.1 Вектор * 3: [" + string.Join(", ", scaled) + "]");

        int[,] m1 = { { 3, 5 }, { 7, 9 } };
        int[,] m2 = { { 1, 2 }, { 3, 4 } };
        int[,] diff = SubtractMatrices(m1, m2);
        Console.WriteLine("1.2 Разность матриц 2x2:");
        PrintMatrix(diff);

        int[,] square = { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } };
        int[,] transposed = TransposeSquareMatrix(square);
        Console.WriteLine("1.3 Транспонированная матрица 3x3:");
        PrintMatrix(transposed);

        double mean = MeanOfMatrix(square);
        Console.WriteLine($"2.1 Среднее арифметическое элементов: {mean:0.00}");

        int diagSum = MainDiagonalSum(square);
        Console.WriteLine($"2.2 Сумма главной диагонали: {diagSum}");

        bool equal = MatricesEqual(m1, m2);
        Console.WriteLine($"3 Равенство матриц m1 и m2: {equal}");

        Pause();
    }

    /// <summary>
    /// Демонстрации блока 2: работа со строками и текстом (задания 4-7).
    /// </summary>
    static void RunBlock2()
    {
        Console.Clear();
        Console.WriteLine("=== Блок 2: Строки и текст ===");

        string sample = "А роза упала на лапу Азора. Привет, мир! Привет?";

        string maxVowels = FirstWordWithMaxVowels(sample);
        Console.WriteLine($"4.1 Первое слово с максимумом гласных: {maxVowels}");

        List<string> unique = UniqueWordsSorted(sample);
        Console.WriteLine("4.2 Уникальные слова по алфавиту: " + string.Join(", ", unique));

        bool palindrome = IsPalindrome(sample);
        Console.WriteLine($"5 Палиндром ли текст: {palindrome}");

        string reversedWords = ReverseWords(sample);
        Console.WriteLine($"6 Слова в тексте перевёрнуты: {reversedWords}");

        Dictionary<string, int> freq = WordFrequencies(sample);
        Console.WriteLine("7 Частоты слов:");
        foreach (var pair in freq.OrderBy(p => p.Key))
        {
            Console.WriteLine($"   {pair.Key}: {pair.Value}");
        }

        Pause();
    }

    /// <summary>
    /// Демонстрации блока 3: числовые последовательности (задания 8-10).
    /// </summary>
    static void RunBlock3()
    {
        Console.Clear();
        Console.WriteLine("=== Блок 3: Числовые последовательности и прогрессии ===");

        int[] fibSeq = { 1, 1, 2, 3, 5, 8 };
        Console.WriteLine($"8 Последовательность {string.Join(",", fibSeq)} фибоначчиева: {IsFibonacciSequence(fibSeq)}");

        double arithSum = ProgressionSum(1, 2, 5, true);
        double geomSum = ProgressionSum(2, 3, 4, false);
        Console.WriteLine($"9 Сумма арифметической прогрессии (a1=1,d=2,n=5): {arithSum}");
        Console.WriteLine($"9 Сумма геометрической прогрессии (a1=2,q=3,n=4): {geomSum}");

        long kth = CustomSequenceElement(3, 4);
        Console.WriteLine($"10 4-й элемент последовательности с N=3: {kth}");

        Pause();
    }

    /// <summary>
    /// Демонстрации блока 4: рекурсия и связанные задачи (задания 11-14).
    /// </summary>
    static void RunBlock4()
    {
        Console.Clear();
        Console.WriteLine("=== Блок 4: Рекурсия ===");

        Console.WriteLine($"11 F_5 (рекурсия): {FibonacciRecursive(5)}");
        Console.WriteLine($"12 Сумма цифр 1234: {DigitSumRecursive(1234)}");
        Console.WriteLine($"13 2^10 (рекурсия): {PowerRecursive(2, 10)}");
        Console.WriteLine($"14 НОД(54, 24): {GcdRecursive(54, 24)}");

        Pause();
    }

    /// <summary>
    /// Демонстрации блока 5: алгоритмы и доп. задания (15-20).
    /// </summary>
    static void RunBlock5()
    {
        Console.Clear();
        Console.WriteLine("=== Блок 5: Алгоритмы ===");

        // 15. Решето Эратосфена и измерение времени.
        int n = 100;
        var primes = SievePrimes(n);
        Console.WriteLine($"15.1 Простые числа до {n}: {string.Join(", ", primes)}");
        var watch = Stopwatch.StartNew();
        var primesLarge = SievePrimes(100_000);
        watch.Stop();
        Console.WriteLine($"15.2 Время для N=100000: {watch.ElapsedMilliseconds} мс (найдено {primesLarge.Count} простых)");

        // 16. Сортировка с режимами.
        int[] arr = { 5, 2, 9, 1, 5, 6 };
        Console.WriteLine("16 Исходный массив: " + string.Join(", ", arr));
        int[] asc = (int[])arr.Clone();
        MergeSort(asc, true);
        int[] desc = (int[])arr.Clone();
        MergeSort(desc, false);
        Console.WriteLine("16 По возрастанию: " + string.Join(", ", asc));
        Console.WriteLine("16 По убыванию: " + string.Join(", ", desc));

        // 17. Файловый менеджер: копирование и архивация (демо без архивации).
        string source = Path.Combine(Path.GetTempPath(), "demo_source.txt");
        string dest = Path.Combine(Path.GetTempPath(), "demo_copy.txt");
        File.WriteAllText(source, "Пример содержимого файла");
        CopyFileWithOptionalArchive(source, dest, archiveSource: false);
        Console.WriteLine($"17 Файл скопирован в {dest}");

        // 18. Степень двойки.
        Console.WriteLine($"18 Наибольшая степень двойки, делящая 48: {LargestPowerOfTwoDivisor(48)}");

        // 19. Даты.
        var date1 = new DateTime(2024, 1, 1);
        var date2 = new DateTime(2024, 2, 15);
        Console.WriteLine($"19.1 Дней между {date1:d} и {date2:d}: {FullDaysBetween(date1, date2)}");
        Console.WriteLine($"19.2 Високосный 2024: {IsLeapYear(2024)}");

        // 20. Рюкзак (упрощённый).
        var items = new List<(int weight, int value)>
        {
            (2, 6),
            (2, 4),
            (6, 5),
            (5, 3),
            (4, 6)
        };
        int capacity = 10;
        int best = KnapsackMaxValue(items, capacity);
        Console.WriteLine($"20 Максимальная ценность для вместимости {capacity}: {best}");

        Pause();
    }

    /// <summary>
    /// Умножает вектор на число.
    /// </summary>
    /// <param name="vector">Исходный вектор.</param>
    /// <param name="scalar">Число, на которое умножаем.</param>
    /// <returns>Новый вектор с умноженными элементами.</returns>
    static double[] MultiplyVector(double[] vector, double scalar)
    {
        double[] result = new double[vector.Length];
        for (int i = 0; i < vector.Length; i++)
        {
            result[i] = vector[i] * scalar;
        }

        return result;
    }

    /// <summary>
    /// Вычитает матрицу b из матрицы a (a - b).
    /// </summary>
    static int[,] SubtractMatrices(int[,] a, int[,] b)
    {
        int rows = a.GetLength(0);
        int cols = a.GetLength(1);
        if (rows != b.GetLength(0) || cols != b.GetLength(1))
        {
            throw new ArgumentException("Матрицы должны быть одинакового размера");
        }

        int[,] result = new int[rows, cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                result[i, j] = a[i, j] - b[i, j];
            }
        }

        return result;
    }

    /// <summary>
    /// Транспонирует квадратную матрицу.
    /// </summary>
    static int[,] TransposeSquareMatrix(int[,] matrix)
    {
        int size = matrix.GetLength(0);
        if (size != matrix.GetLength(1))
        {
            throw new ArgumentException("Матрица должна быть квадратной");
        }

        int[,] result = new int[size, size];
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                result[j, i] = matrix[i, j];
            }
        }

        return result;
    }

    /// <summary>
    /// Среднее арифметическое всех элементов матрицы.
    /// </summary>
    static double MeanOfMatrix(int[,] matrix)
    {
        double sum = 0;
        int count = matrix.Length;
        foreach (var value in matrix)
        {
            sum += value;
        }

        return sum / count;
    }

    /// <summary>
    /// Сумма элементов главной диагонали квадратной матрицы.
    /// </summary>
    static int MainDiagonalSum(int[,] matrix)
    {
        int size = matrix.GetLength(0);
        if (size != matrix.GetLength(1))
        {
            throw new ArgumentException("Матрица должна быть квадратной");
        }

        int sum = 0;
        for (int i = 0; i < size; i++)
        {
            sum += matrix[i, i];
        }

        return sum;
    }

    /// <summary>
    /// Проверяет равенство двух матриц по размерам и значениям.
    /// </summary>
    static bool MatricesEqual(int[,] a, int[,] b)
    {
        int rows = a.GetLength(0);
        int cols = a.GetLength(1);
        if (rows != b.GetLength(0) || cols != b.GetLength(1))
        {
            return false;
        }

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (a[i, j] != b[i, j])
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Находит первое слово с максимальным количеством гласных.
    /// </summary>
    static string FirstWordWithMaxVowels(string text)
    {
        string[] words = SplitWords(text);
        string vowels = "аеёиоуыэюяAEIOUYaeiouyАЕЁИОУЫЭЮЯ";
        string result = string.Empty;
        int best = -1;

        foreach (var word in words)
        {
            int count = word.Count(ch => vowels.Contains(ch));
            if (count > best)
            {
                best = count;
                result = word;
            }
        }

        return result;
    }

    /// <summary>
    /// Возвращает список уникальных слов, отсортированных по алфавиту.
    /// </summary>
    static List<string> UniqueWordsSorted(string text)
    {
        string[] words = SplitWords(text);
        return words.Select(w => w.ToLower()).Distinct().OrderBy(w => w).ToList();
    }

    /// <summary>
    /// Проверяет, является ли текст палиндромом (без регистра и знаков).
    /// </summary>
    static bool IsPalindrome(string text)
    {
        string cleaned = new string(text.Where(char.IsLetterOrDigit).Select(char.ToLower).ToArray());
        string reversed = new string(cleaned.Reverse().ToArray());
        return cleaned == reversed;
    }

    /// <summary>
    /// Переворачивает все слова, сохраняя порядок.
    /// </summary>
    static string ReverseWords(string text)
    {
        string[] words = SplitWordsPreserveSeparators(text, out List<string> separators);
        for (int i = 0; i < words.Length; i++)
        {
            words[i] = new string(words[i].Reverse().ToArray());
        }

        // Собираем обратно, используя сохранённые разделители.
        var parts = new List<string>();
        for (int i = 0; i < words.Length; i++)
        {
            parts.Add(words[i]);
            if (i < separators.Count)
            {
                parts.Add(separators[i]);
            }
        }

        return string.Concat(parts);
    }

    /// <summary>
    /// Считает частоты слов в тексте.
    /// </summary>
    static Dictionary<string, int> WordFrequencies(string text)
    {
        string[] words = SplitWords(text);
        var dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var word in words)
        {
            if (dict.ContainsKey(word))
            {
                dict[word]++;
            }
            else
            {
                dict[word] = 1;
            }
        }

        return dict;
    }

    /// <summary>
    /// Проверяет, является ли последовательность фибоначчиевой (начиная с 3-го элемента).
    /// </summary>
    static bool IsFibonacciSequence(int[] numbers)
    {
        if (numbers.Length < 3)
        {
            return true; // с двух элементов условие не нарушается
        }

        for (int i = 2; i < numbers.Length; i++)
        {
            if (numbers[i] != numbers[i - 1] + numbers[i - 2])
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Сумма первых n элементов арифметической или геометрической прогрессии.
    /// </summary>
    /// <param name="a1">Первый элемент.</param>
    /// <param name="step">Разность (для арифм.) или знаменатель (для геом.).</param>
    /// <param name="n">Количество элементов.</param>
    /// <param name="isArithmetic">true для арифметической, false для геометрической.</param>
    static double ProgressionSum(double a1, double step, int n, bool isArithmetic)
    {
        if (n <= 0)
        {
            return 0;
        }

        if (isArithmetic)
        {
            double an = a1 + (n - 1) * step;
            return n * (a1 + an) / 2.0;
        }
        else
        {
            if (step == 1)
            {
                return a1 * n;
            }

            return a1 * (1 - Math.Pow(step, n)) / (1 - step);
        }
    }

    /// <summary>
    /// Возвращает k-й элемент последовательности, где первые два равны N, остальные — сумма двух предыдущих.
    /// </summary>
    static long CustomSequenceElement(long n, int k)
    {
        if (k <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(k));
        }

        if (k == 1 || k == 2)
        {
            return n;
        }

        long a = n;
        long b = n;
        for (int i = 3; i <= k; i++)
        {
            long c = a + b;
            a = b;
            b = c;
        }

        return b;
    }

    /// <summary>
    /// Рекурсивно вычисляет число Фибоначчи.
    /// </summary>
    static long FibonacciRecursive(int n)
    {
        if (n < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(n));
        }

        if (n <= 1)
        {
            return n;
        }

        return FibonacciRecursive(n - 1) + FibonacciRecursive(n - 2);
    }

    /// <summary>
    /// Рекурсивно суммирует цифры числа.
    /// </summary>
    static int DigitSumRecursive(int number)
    {
        if (number < 10)
        {
            return number;
        }

        return number % 10 + DigitSumRecursive(number / 10);
    }

    /// <summary>
    /// Возведение числа в степень через рекурсию.
    /// </summary>
    static long PowerRecursive(long @base, int exp)
    {
        if (exp < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(exp));
        }

        if (exp == 0)
        {
            return 1;
        }

        return @base * PowerRecursive(@base, exp - 1);
    }

    /// <summary>
    /// Рекурсивный НОД по алгоритму Евклида.
    /// </summary>
    static int GcdRecursive(int a, int b)
    {
        if (b == 0)
        {
            return a;
        }

        return GcdRecursive(b, a % b);
    }

    /// <summary>
    /// Решето Эратосфена: список простых чисел до N включительно.
    /// </summary>
    static List<int> SievePrimes(int n)
    {
        if (n < 2)
        {
            return new List<int>();
        }

        bool[] isPrime = Enumerable.Repeat(true, n + 1).ToArray();
        isPrime[0] = isPrime[1] = false;

        for (int p = 2; p * p <= n; p++)
        {
            if (isPrime[p])
            {
                for (int multiple = p * p; multiple <= n; multiple += p)
                {
                    isPrime[multiple] = false;
                }
            }
        }

        var primes = new List<int>();
        for (int i = 2; i <= n; i++)
        {
            if (isPrime[i])
            {
                primes.Add(i);
            }
        }

        return primes;
    }

    /// <summary>
    /// Сортирует массив слиянием. Режим ascending определяет порядок.
    /// </summary>
    static void MergeSort(int[] array, bool ascending)
    {
        if (array.Length <= 1)
        {
            return;
        }

        int mid = array.Length / 2;
        int[] left = array.Take(mid).ToArray();
        int[] right = array.Skip(mid).ToArray();

        MergeSort(left, ascending);
        MergeSort(right, ascending);

        Merge(array, left, right, ascending);
    }

    /// <summary>
    /// Копирует содержимое одного файла в другой и опционально архивирует исходный файл.
    /// </summary>
    /// <param name="sourcePath">Путь к исходному файлу.</param>
    /// <param name="destPath">Путь назначения.</param>
    /// <param name="archiveSource">Архивировать ли исходный файл после копирования.</param>
    static void CopyFileWithOptionalArchive(string sourcePath, string destPath, bool archiveSource)
    {
        File.Copy(sourcePath, destPath, overwrite: true);

        if (archiveSource)
        {
            string zipPath = sourcePath + ".zip";
            using (var archive = ZipFile.Open(zipPath, ZipArchiveMode.Create))
            {
                archive.CreateEntryFromFile(sourcePath, Path.GetFileName(sourcePath));
            }
        }
    }

    /// <summary>
    /// Определяет наибольшую степень двойки, на которую делится N.
    /// </summary>
    static int LargestPowerOfTwoDivisor(int n)
    {
        if (n == 0)
        {
            return 0;
        }

        int power = 1;
        while (n % 2 == 0)
        {
            power *= 2;
            n /= 2;
        }

        return power;
    }

    /// <summary>
    /// Разница полных дней между двумя датами.
    /// </summary>
    static int FullDaysBetween(DateTime a, DateTime b)
    {
        return (int)Math.Abs((b.Date - a.Date).TotalDays);
    }

    /// <summary>
    /// Проверка года на високосность.
    /// </summary>
    static bool IsLeapYear(int year)
    {
        return DateTime.IsLeapYear(year);
    }

    /// <summary>
    /// Решение задачи о рюкзаке (0/1) динамическим программированием.
    /// </summary>
    static int KnapsackMaxValue(List<(int weight, int value)> items, int capacity)
    {
        int[,] dp = new int[items.Count + 1, capacity + 1];
        for (int i = 1; i <= items.Count; i++)
        {
            int w = items[i - 1].weight;
            int v = items[i - 1].value;
            for (int c = 0; c <= capacity; c++)
            {
                if (w > c)
                {
                    dp[i, c] = dp[i - 1, c];
                }
                else
                {
                    dp[i, c] = Math.Max(dp[i - 1, c], dp[i - 1, c - w] + v);
                }
            }
        }

        return dp[items.Count, capacity];
    }

    /// <summary>
    /// Вспомогательная печать матрицы в консоль.
    /// </summary>
    static void PrintMatrix(int[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        for (int i = 0; i < rows; i++)
        {
            var row = new int[cols];
            for (int j = 0; j < cols; j++)
            {
                row[j] = matrix[i, j];
            }

            Console.WriteLine("   [" + string.Join(", ", row) + "]");
        }
    }

    /// <summary>
    /// Разбивает текст на слова по базовым разделителям.
    /// </summary>
    static string[] SplitWords(string text)
    {
        char[] separators = { ' ', '.', ',', '!', '?', '\t', '\r', '\n' };
        return text.Split(separators, StringSplitOptions.RemoveEmptyEntries);
    }

    /// <summary>
    /// Разбивает текст на слова, дополнительно сохраняя разделители для сборки.
    /// </summary>
    static string[] SplitWordsPreserveSeparators(string text, out List<string> separators)
    {
        separators = new List<string>();
        var words = new List<string>();
        var current = new List<char>();

        foreach (char c in text)
        {
            if (char.IsLetterOrDigit(c))
            {
                current.Add(c);
            }
            else
            {
                if (current.Count > 0)
                {
                    words.Add(new string(current.ToArray()));
                    current.Clear();
                }

                separators.Add(c.ToString());
            }
        }

        if (current.Count > 0)
        {
            words.Add(new string(current.ToArray()));
        }

        return words.ToArray();
    }

    /// <summary>
    /// Сливает две половины массива для сортировки.
    /// </summary>
    static void Merge(int[] target, int[] left, int[] right, bool ascending)
    {
        int i = 0, j = 0, k = 0;
        while (i < left.Length && j < right.Length)
        {
            bool takeLeft = ascending ? left[i] <= right[j] : left[i] >= right[j];
            if (takeLeft)
            {
                target[k++] = left[i++];
            }
            else
            {
                target[k++] = right[j++];
            }
        }

        while (i < left.Length)
        {
            target[k++] = left[i++];
        }

        while (j < right.Length)
        {
            target[k++] = right[j++];
        }
    }

    /// <summary>
    /// Пауза до нажатия клавиши.
    /// </summary>
    static void Pause()
    {
        Console.WriteLine("\nНажмите любую клавишу, чтобы вернуться в меню...");
        Console.ReadKey();
    }
}
