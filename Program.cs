using System;
using System.IO;

namespace Lab03
{
    class Program
    {
        static void Main(string[] args)
        {
            int N = 8;
            int K = 17;

            bool running = true;
            while (running)
            {
                Console.WriteLine("\n================ ЛАБОРАТОРНА РОБОТА 3 ================");
                Console.WriteLine("Варіант: N = 8, K = 17, a = 2, b = 0, c = 3");
                Console.WriteLine("1. Завдання 1: Вкладена структура (Сумарний розмір усіх файлів у папці)");
                Console.WriteLine("2. Завдання 2: Розбиття навпіл (Перевірка чи всі елементи однакові)");
                Console.WriteLine("3. Завдання 3: Перебір із поверненням (Усі шляхи в матриці 5x5)");
                Console.WriteLine("4. Завдання 4: Трасування стеку викликів (для масиву з 4 елементів)");
                Console.WriteLine("5. Завдання 5: Порівняння рекурсивного та ітеративного алгоритмів");
                Console.WriteLine("6. Завдання 6: Демонстрація крайових випадків");
                Console.WriteLine("0. Вихід");
                Console.Write("Оберіть пункт: ");

                int choice = ReadInt();

                switch (choice)
                {
                    case 1:
                        Console.WriteLine("\n--- Завдання 1: Сумарний розмір файлів у папці ---");
                        Console.Write("Введіть шлях до папки (наприклад, C:\\Windows\\System32 або '.'): ");
                        string dirPath = Console.ReadLine();

                        if (!Directory.Exists(dirPath))
                        {
                            Console.WriteLine("Помилка: Вказана папка не існує!");
                        }
                        else
                        {
                            long totalBytes = GetDirectorySize(dirPath);
                            Console.WriteLine($"Сумарний розмір усіх файлів: {totalBytes} байт ({totalBytes / (1024.0 * 1024.0):F2} МБ)");
                        }
                        break;

                    case 2:
                        Console.WriteLine("\n--- Завдання 2: Розбиття масиву навпіл ---");
                        int[] arr16 = GenerateArray(N, 16, -20, 20);
                        PrintArray("Згенерований масив (16 елементів): ", arr16);

                        int maxDepth = 0;
                        bool isEqualRecursive = AreAllEqualRecursive(arr16, 0, arr16.Length - 1, ref maxDepth, 1);

                        Console.WriteLine($"Результат (чи всі елементи однакові): {isEqualRecursive}");
                        Console.WriteLine($"Максимальна глибина рекурсії: {maxDepth}");
                        Console.WriteLine($"Теоретичне значення log2(16): {Math.Log2(16)}");
                        break;

                    case 3:
                        Console.WriteLine("\n--- Завдання 3: Перебір шляхів у матриці 5x5 ---");
                        int M = K % 3 + 3; // 17 % 3 + 3 = 5
                        Console.WriteLine($"Розмірність матриці: {M}x{M}");
                        Console.WriteLine("Усі шляхи з (0,0) до (4,4) під час руху лише ВПРАВО та ВНИЗ:");

                        int pathCount = 0;
                        FindMatrixPaths(0, 0, M, "", ref pathCount);
                        Console.WriteLine($"\nЗагальна кількість знайдених шляхів: {pathCount}");
                        break;

                    case 4:
                        Console.WriteLine("\n--- Завдання 4: Трасування стеку викликів ---");
                        int[] arr4 = new int[] { 5, 5, 5, 5 };
                        PrintArray("Масив для трасування: ", arr4);
                        Console.WriteLine("Трасування:");
                        AreAllEqualTraced(arr4, 0, arr4.Length - 1, 0);
                        break;

                    case 5:
                        Console.WriteLine("\n--- Завдання 5: Порівняння рекурсії та циклу ---");
                        int[] testArr = GenerateArray(N, 16, -20, 20);
                        PrintArray("Вхідний масив: ", testArr);

                        int depthDummy = 0;
                        bool recResult = AreAllEqualRecursive(testArr, 0, testArr.Length - 1, ref depthDummy, 1);
                        bool iterResult = AreAllEqualIterative(testArr);

                        Console.WriteLine($"Рекурсивна версія повернула: {recResult}");
                        Console.WriteLine($"Ітеративна версія повернула:  {iterResult}");
                        Console.WriteLine($"Результати збігаються: {recResult == iterResult}");
                        break;

                    case 6:
                        Console.WriteLine("\n--- Завдання 6: Тестування крайових випадків ---");
                        RunEdgeCases();
                        break;

                    case 0:
                        running = false;
                        break;

                    default:
                        Console.WriteLine("Некоректний вибір.");
                        break;
                }
            }
        }

        // --- Завдання 1: Обхід вкладеної структури каталогів ---
        public static long GetDirectorySize(string dirPath)
        {
            long size = 0;

            try
            {
                string[] files = Directory.GetFiles(dirPath);
                for (int i = 0; i < files.Length; i++)
                {
                    try
                    {
                        FileInfo fi = new FileInfo(files[i]);
                        size += fi.Length;
                    }
                    catch (Exception) { }
                }

                string[] subdirs = Directory.GetDirectories(dirPath);
                for (int i = 0; i < subdirs.Length; i++)
                {
                    size += GetDirectorySize(subdirs[i]);
                }
            }
            catch (UnauthorizedAccessException) { }
            catch (Exception) { }

            return size;
        }

        // --- Завдання 2: Розбиття навпіл ---
        public static bool AreAllEqualRecursive(int[] arr, int lo, int hi, ref int maxDepth, int currentDepth)
        {
            if (currentDepth > maxDepth)
            {
                maxDepth = currentDepth;
            }

            if (lo == hi)
            {
                return true;
            }

            int mid = lo + (hi - lo) / 2;

            bool leftEqual = AreAllEqualRecursive(arr, lo, mid, ref maxDepth, currentDepth + 1);
            bool rightEqual = AreAllEqualRecursive(arr, mid + 1, hi, ref maxDepth, currentDepth + 1);

            return leftEqual && rightEqual && (arr[lo] == arr[mid + 1]);
        }

        // --- Завдання 3: Перебір із поверненням (Backtracking) ---
        public static void FindMatrixPaths(int r, int c, int M, string path, ref int count)
        {
            string currentCell = $"({r},{c})";
            string newPath = string.IsNullOrEmpty(path) ? currentCell : $"{path}->{currentCell}";

            if (r == M - 1 && c == M - 1)
            {
                Console.WriteLine(newPath);
                count++;
                return;
            }

            if (r + 1 < M)
            {
                FindMatrixPaths(r + 1, c, M, newPath, ref count);
            }

            if (c + 1 < M)
            {
                FindMatrixPaths(r, c + 1, M, newPath, ref count);
            }
        }

        // --- Завдання 4: Трасування ---
        public static bool AreAllEqualTraced(int[] arr, int lo, int hi, int depth)
        {
            string indent = new string(' ', depth * 2);
            Console.WriteLine($"{indent}enter AreAllEqual({lo}..{hi})");

            bool result;
            if (lo == hi)
            {
                result = true;
                Console.WriteLine($"{indent}  enter AreAllEqual({lo}..{hi}) -> {result}");
            }
            else
            {
                int mid = lo + (hi - lo) / 2;
                bool left = AreAllEqualTraced(arr, lo, mid, depth + 1);
                bool right = AreAllEqualTraced(arr, mid + 1, hi, depth + 1);
                result = left && right && (arr[lo] == arr[mid + 1]);
                Console.WriteLine($"{indent}exit  AreAllEqual({lo}..{hi}) -> {result}");
            }

            return result;
        }

        // --- Завдання 5: Ітеративна версія ---
        public static bool AreAllEqualIterative(int[] arr)
        {
            if (arr == null || arr.Length == 0) return true;

            for (int i = 1; i < arr.Length; i++)
            {
                if (arr[i] != arr[0])
                {
                    return false;
                }
            }
            return true;
        }

        // --- Допоміжні методи ---
        static int[] GenerateArray(int seed, int size, int min, int max)
        {
            Random rnd = new Random(seed);
            int[] arr = new int[size];
            for (int i = 0; i < size; i++)
            {
                arr[i] = rnd.Next(min, max + 1);
            }
            return arr;
        }

        static void PrintArray(string title, int[] array)
        {
            Console.Write(title);
            if (array == null || array.Length == 0)
            {
                Console.WriteLine("[]");
                return;
            }
            Console.Write("[ ");
            for (int i = 0; i < array.Length; i++)
            {
                Console.Write($"{array[i]}{(i < array.Length - 1 ? ", " : "")}");
            }
            Console.WriteLine(" ]");
        }

        static int ReadInt()
        {
            int val;
            while (!int.TryParse(Console.ReadLine(), out val))
            {
                Console.Write("Помилка! Введіть ціле число: ");
            }
            return val;
        }

        static void RunEdgeCases()
        {
            Console.WriteLine("\n--- 1. Порожня папка ---");
            string tempDir = Path.Combine(Path.GetTempPath(), "EmptyTestDir_" + Guid.NewGuid());
            Directory.CreateDirectory(tempDir);
            long emptySize = GetDirectorySize(tempDir);
            Console.WriteLine($"Розмір порожньої папки: {emptySize} байт");
            Directory.Delete(tempDir);

            Console.WriteLine("\n--- 2. Папка без доступу ---");
            long protectedSize = GetDirectorySize("C:\\System Volume Information");
            Console.WriteLine($"Розмір папки з обмеженим доступом: {protectedSize} байт");

            Console.WriteLine("\n--- 3. Масив з 1 елемента ---");
            int[] single = new int[] { 42 };
            int d = 0;
            bool resSingle = AreAllEqualRecursive(single, 0, 0, ref d, 1);
            Console.WriteLine($"Чи всі елементи однакові для [42]: {resSingle}");

            Console.WriteLine("\n--- 4. Усі елементи однакові ---");
            int[] same = new int[] { 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7 };
            d = 0;
            bool resSame = AreAllEqualRecursive(same, 0, same.Length - 1, ref d, 1);
            Console.WriteLine($"Чи всі елементи однакові для 16 тіньових сімок: {resSame}");

            Console.WriteLine("\n--- 5. Варіантів не існує (матриця 1x1) ---");
            int cCount = 0;
            FindMatrixPaths(0, 0, 1, "", ref cCount);
            Console.WriteLine($"Кількість шляхів для 1x1: {cCount}");
        }
    }
}