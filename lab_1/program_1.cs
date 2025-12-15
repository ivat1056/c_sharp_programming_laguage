using System;

class Program
{
    /// <summary>
    /// Точка входа: показывает меню и запускает выбранное задание.
    /// </summary>
    /// <param name="args">Аргументы командной строки (не используются).</param>
    static void Main(string[] args)
    {
        // Главный цикл: отображаем меню и обрабатываем выбор пользователя.
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Выберите задание:");
            Console.WriteLine("1 - Записная книжка");
            Console.WriteLine("2 - Игра для двух игроков");
            Console.WriteLine("0 - Выход");
            Console.Write("Ваш выбор: ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    RunNotebook();
                    break;
                case "2":
                    RunGame();
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
    /// Задание 1: вывод персональных данных и среднего балла.
    /// Демонстрирует разные способы вывода и центрирование текста.
    /// </summary>
    static void RunNotebook()
    {
        Console.Clear();
        Console.WriteLine("=== Задание 1. Записная книжка ===");

        // Переменные, которые можно менять для персонализации.
        string name = "Алексей";
        int age = 23;
        double height = 1.82; // рост в метрах
        int historyScore = 87;
        int mathScore = 92;
        int russianScore = 78;

        // Расчёт среднего балла по трём предметам.
        double averageScore = (historyScore + mathScore + russianScore) / 3.0;

        // Обычный вывод конкатенацией строк.
        Console.WriteLine("\nОбычный вывод:");
        Console.WriteLine("Имя: " + name + ", возраст: " + age + ", рост: " + height + " м");
        Console.WriteLine("История: " + historyScore + ", Математика: " + mathScore + ", Русский: " + russianScore);
        Console.WriteLine("Средний балл: " + averageScore);

        // Форматированный вывод через string.Format.
        Console.WriteLine("\nФорматированный вывод:");
        Console.WriteLine(
            string.Format("Имя: {0}, возраст: {1}, рост: {2:0.00} м", name, age, height));
        Console.WriteLine(
            string.Format("История: {0}, Математика: {1}, Русский: {2}", historyScore, mathScore, russianScore));
        Console.WriteLine(string.Format("Средний балл: {0:0.00}", averageScore));

        // Интерполяция строк.
        Console.WriteLine("\nИнтерполяция строк:");
        Console.WriteLine($"Имя: {name}, возраст: {age}, рост: {height:0.00} м");
        Console.WriteLine($"История: {historyScore}, Математика: {mathScore}, Русский: {russianScore}");
        Console.WriteLine($"Средний балл: {averageScore:0.00}");

        // Бонус: вывод блока текста по центру консоли.
        Console.WriteLine("\nБонус: центрированный вывод:");
        string[] notebookLines =
        {
            $"Имя: {name}",
            $"Возраст: {age}",
            $"Рост: {height:0.00} м",
            $"История: {historyScore}, Математика: {mathScore}, Русский: {russianScore}",
            $"Средний балл: {averageScore:0.00}"
        };
        PrintCenteredBlock(notebookLines);

        Console.WriteLine("\nНажмите любую клавишу, чтобы вернуться в меню...");
        Console.ReadKey();
    }

    /// Задание 2: игра на вычитание числа для двух игроков.
    static void RunGame()
    {
        Console.Clear();
        Console.WriteLine("=== Задание 2. Игра \"Вычти число\" ===");
        Console.WriteLine("1 - Стандарт (число 12..120, ход 1..4)");
        Console.WriteLine("2 - Пользовательские настройки");
        Console.WriteLine("3 - Одиночная игра против бота");
        Console.Write("Выберите режим: ");

        string mode = Console.ReadLine();
        bool vsBot = mode == "3"; // true, если выбран бот

        // Настройки по умолчанию из условия.
        int minStart = 12;
        int maxStart = 120;
        int minTake = 1;
        int maxTake = 4;

        if (mode == "2")
        {
            Console.WriteLine("\nНастройка диапазона начального числа:");
            minStart = ReadInt("Минимум (>= 5): ", 5, 1_000);
            maxStart = ReadInt($"Максимум (>= {minStart}): ", minStart, 5_000);

            Console.WriteLine("\nНастройка хода:");
            minTake = ReadInt("Минимальный ход (>=1): ", 1, 100);
            maxTake = ReadInt($"Максимальный ход (>= {minTake}): ", minTake, 100);
        }

        // Получаем имена игроков; бот получает имя по умолчанию.
        Console.Write("\nВведите никнейм игрока 1: ");
        string player1 = NonEmpty(Console.ReadLine(), "Игрок1");
        string player2;
        if (vsBot)
        {
            player2 = "Bot";
            Console.WriteLine($"Никнейм бота: {player2}");
        }
        else
        {
            Console.Write("Введите никнейм игрока 2: ");
            player2 = NonEmpty(Console.ReadLine(), "Игрок2");
        }

        // Подготовка генератора случайных чисел и флага для повторной игры.
        var random = new Random();
        bool playAgain = true;

        while (playAgain)
        {
            // Генерируем стартовое число и начинаем с первого игрока.
            int gameNumber = random.Next(minStart, maxStart + 1);
            int currentPlayerIndex = 0; // 0 - player1, 1 - player2

            Console.WriteLine($"\nСтартовое число: {gameNumber}");

            // Игровой цикл до тех пор, пока число не станет нулём.
            while (gameNumber > 0)
            {
                string currentPlayer = currentPlayerIndex == 0 ? player1 : player2;
                Console.WriteLine($"\nЧисло: {gameNumber}");
                int userTry;

                if (vsBot && currentPlayer == player2)
                {
                    // Простейшая стратегия бота: стремится оставить число кратным (maxTake + 1).
                    int target = (maxTake + 1);
                    int optimal = gameNumber % target;
                    userTry = optimal == 0 ? minTake : Math.Min(optimal, maxTake);
                    userTry = Math.Max(minTake, Math.Min(userTry, maxTake));
                    Console.WriteLine($"Ход {currentPlayer} (бот): {userTry}");
                }
                else
                {
                    userTry = ReadInt(
                        $"Ход {currentPlayer} (число {minTake}-{maxTake}): ",
                        minTake,
                        maxTake);
                }

                gameNumber -= userTry;
                if (gameNumber <= 0)
                {
                    Console.WriteLine($"\n{currentPlayer} победил! Поздравляем!");
                    break;
                }

                currentPlayerIndex = 1 - currentPlayerIndex; // переключаем игрока
            }

            Console.Write("\nСыграть ещё раз? (y/n): ");
            string answer = Console.ReadLine();
            playAgain = answer != null && answer.Trim().ToLower() == "y";
        }

        Console.WriteLine("Спасибо за игру! Нажмите любую клавишу, чтобы вернуться в меню...");
        Console.ReadKey();
    }

    static int ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine();
            if (int.TryParse(input, out int value) && value >= min && value <= max)
            {
                return value;
            }

            Console.WriteLine($"Введите целое число от {min} до {max}.");
        }
    }

    /// Возвращает строку или подставляет запасное значение, если строка пустая.
    static string NonEmpty(string? value, string fallback)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return fallback;
        }

        return value.Trim();
    }
    
    static void PrintCenteredBlock(string[] lines)
    {
        int width;
        try
        {
            width = Console.WindowWidth;
        }
        catch
        {
            width = 80; // запасной вариант для окружений без консоли
        }

        foreach (var line in lines)
        {
            int padding = Math.Max(0, (width - line.Length) / 2);
            Console.WriteLine(new string(' ', padding) + line);
        }
    }
}
