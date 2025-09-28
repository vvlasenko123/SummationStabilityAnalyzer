using System.Diagnostics;

namespace SummationStabilityAnalyzer;

class Program
{
    private static void Main(string[] args)
    {
        // Кейс 1: катастрофическое вычитание
        double[] case1 = [1e16, 1.0, -1e16];
        double truth1 = 1.0;
        RunCase("Кейс 1: [1e16, 1.0, -1e16], истина = 1.0", case1, truth1);

        // Кейс 2: длинная серия малых слагаемых
        int n = 2_000_000;
        double term = 1e-8;
        double[] case2 = new double[n];
        for (int i = 0; i < n; i++)
        {
            case2[i] = term;
        }

        double truth2 = n * term;
        RunCase($"Кейс 2: {n:N0} раз по {term:R}, истина = {truth2:R}", case2, truth2);

        // Кейс 3: чередующиеся по знаку малые значения (фиксированный seed)
        int m = 1_000_000;
        double[] case3 = MakeAlternating(m, scale: 1e-8, seed: 42);
        // В качестве референса берём более устойчивый Neumaier
        double truth3 = Summation.NeumaierSum(case3);
        RunCase($"Кейс 3: {m:N0} попеременных малых значений, референс = Neumaier", case3, truth3);
    }

    private static void RunCase(string title, double[] data, double truth)
    {
        Console.WriteLine(new string('=', 80));
        Console.WriteLine(title);

        Measure(() => Summation.NaiveSum(data), out double naive, out TimeSpan tNaive);
        Measure(() => Summation.KahanSum(data), out double kahan, out TimeSpan tKahan);
        Measure(() => Summation.NeumaierSum(data), out double neumaier, out TimeSpan tNeum);
        Measure(() => Summation.PairwiseSum(data), out double pairwise, out TimeSpan tPair);

        var (naAbs, naRel) = Errors(naive, truth);
        var (kaAbs, kaRel) = Errors(kahan, truth);
        var (neAbs, neRel) = Errors(neumaier, truth);
        var (pwAbs, pwRel) = Errors(pairwise, truth);

        Console.WriteLine();
        Console.WriteLine("Результаты:");
        Console.WriteLine($"Наивная сумма     : {naive:R}   время: {tNaive.TotalMilliseconds:N1} мс");
        Console.WriteLine($"Кэхан сумма       : {kahan:R}   время: {tKahan.TotalMilliseconds:N1} мс");
        Console.WriteLine($"Ноймайер сумма    : {neumaier:R}   время: {tNeum.TotalMilliseconds:N1} мс");
        Console.WriteLine($"Попарное сумма    : {pairwise:R}   время: {tPair.TotalMilliseconds:N1} мс");

        Console.WriteLine();
        Console.WriteLine("Ошибки (abs | rel):");
        Console.WriteLine($"Наивная   : {naAbs:E3} | {naRel:E3}");
        Console.WriteLine($"Кэхан     : {kaAbs:E3} | {kaRel:E3}");
        Console.WriteLine($"Ноймайер  : {neAbs:E3} | {neRel:E3}");
        Console.WriteLine($"Попарное  : {pwAbs:E3} | {pwRel:E3}");
        Console.WriteLine();
    }

    private static void Measure(Func<double> f, out double result, out TimeSpan elapsed)
    {
        Stopwatch sw = Stopwatch.StartNew();
        result = f();
        sw.Stop();
        elapsed = sw.Elapsed;
    }

    private static (double abs, double rel) Errors(double value, double truth)
    {
        double abs = Math.Abs(value - truth);
        double denom = Math.Max(Math.Abs(truth), double.Epsilon);
        double rel = abs / denom;
        return (abs, rel);
    }

    private static double[] MakeAlternating(int n, double scale, int seed)
    {
        Random rnd = new Random(seed);
        double[] a = new double[n];
        for (int i = 0; i < n; i++)
        {
            int sign = (i % 2 == 0) ? 1 : -1;
            a[i] = sign * scale * rnd.NextDouble();
        }

        return a;
    }
}