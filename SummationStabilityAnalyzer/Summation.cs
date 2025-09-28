namespace SummationStabilityAnalyzer;

/// <summary>
/// Класс содержащий алгоритмы суммирования чисел с плавающей точкой
/// </summary>
public static class Summation
{
    /// <summary>
    /// Наивное последовательное суммирование слева направо
    /// </summary>
    /// <param name="values">Последовательность слагаемых для суммирования</param>
    /// <returns>Сумма элементов последовательности в типе double</returns>
    /// <exception cref="ArgumentNullException">Проверка на null</exception>
    public static double NaiveSum(IEnumerable<double> values)
    {
        if (values == null)
        {
            throw new ArgumentNullException(nameof(values), "Коллекция значений не должна быть null");
        }

        double sum = 0.0;
        foreach (double x in values)
        {
            sum += x;
        }
        return sum;
    }

    /// <summary>
    /// Алгоритм Кэхана с компенсацией потерянных разрядов
    /// </summary>
    /// <param name="values">Последовательность слагаемых для суммирования</param>
    /// <returns>Сумма элементов последовательности в типе double</returns>
    /// <exception cref="ArgumentNullException">Проверка на null</exception>
    public static double KahanSum(IEnumerable<double> values)
    {
        if (values == null)
        {
            throw new ArgumentNullException(nameof(values), "Коллекция значений не должна быть null");
        }

        double sum = 0.0;
        double c = 0.0;
        foreach (double x in values)
        {
            double y = x - c;
            double t = sum + y;
            c = (t - sum) - y;
            sum = t;
        }
        return sum;
    }

    /// <summary>
    ///  Алгоритм Ноймайера, улучшение Кэхана для случаев, когда модуль слагаемого больше модуля текущей суммы
    /// </summary>
    /// <param name="values">Последовательность слагаемых для суммирования</param>
    /// <returns>Сумма элементов последовательности в типе double</returns>
    /// <exception cref="ArgumentNullException">Проверка на null</exception>
    public static double NeumaierSum(IEnumerable<double> values)
    {
        if (values == null)
        {
            throw new ArgumentNullException(nameof(values), "Коллекция значений не должна быть null");
        }

        double sum = 0.0;
        double c = 0.0;
        foreach (double x in values)
        {
            double t = sum + x;
            if (Math.Abs(sum) >= Math.Abs(x))
            {
                c += (sum - t) + x;
            }
            else
            {
                c += (x - t) + sum;
            }
            sum = t;
        }
        return sum + c;
    }

    /// <summary>
    /// Попарное (рекурсивное) суммирование: делит массив на части и складывает результаты
    /// </summary>
    /// <param name="values">Массив слагаемых для суммирования</param>
    /// <returns>Сумма элементов массива в типе <see cref="double"/>.</returns>
    /// <exception cref="ArgumentNullException">Проверка на null</exception>
    public static double PairwiseSum(double[] values)
    {
        if (values == null)
        {
            throw new ArgumentNullException(nameof(values), "Массив значений не должен быть null");
        }

        if (values.Length == 0)
        {
            return 0.0;
        }

        return PairwiseSumRecursive(values, 0, values.Length);
    }

    /// <summary>
    /// Рекурсивный помощник для попарного суммирования
    /// </summary>
    /// <param name="a">Массив слагаемых</param>
    /// <param name="start">Начальный индекс диапазона (включительно)</param>
    /// <param name="length">Длина диапазона (количество элементов)</param>
    /// <returns>Сумма элементов указанного диапазона массива</returns>
    private static double PairwiseSumRecursive(double[] a, int start, int length)
    {
        if (length == 1)
        {
            return a[start];
        }

        if (length == 2)
        {
            return a[start] + a[start + 1];
        }

        int mid = length / 2;
        double left = PairwiseSumRecursive(a, start, mid);
        double right = PairwiseSumRecursive(a, start + mid, length - mid);
        return left + right;
    }
}