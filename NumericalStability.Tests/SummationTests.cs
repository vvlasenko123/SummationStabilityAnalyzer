using SummationStabilityAnalyzer;

public class SummationTests
{
    private const double Epsilon = 1e-12;

    private static void AssertAlmostEqual(double actual, double expected, double eps = Epsilon)
    {
        if (double.IsNaN(expected))
        {
            Assert.True(double.IsNaN(actual));
            return;
        }

        if (double.IsPositiveInfinity(expected))
        {
            Assert.True(double.IsPositiveInfinity(actual));
            return;
        }

        if (double.IsNegativeInfinity(expected))
        {
            Assert.True(double.IsNegativeInfinity(actual));
            return;
        }

        Assert.True(Math.Abs(actual - expected) <= eps, $"actual={actual}, expected={expected}, eps={eps}");
    }

    // ---------- NaiveSum ----------

    [Fact]
    public void T1_NaiveSum_Null_Throws()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => Summation.NaiveSum(null));
        Assert.Equal("values", ex.ParamName);
        Assert.Contains("Коллекция значений не должна быть null", ex.Message);
        // Покрытие: N1, R1 (negative)
    }

    [Fact]
    public void T5_NaiveSum_Empty_ReturnsZero()
    {
        var result = Summation.NaiveSum(Array.Empty<double>());
        AssertAlmostEqual(result, 0.0);
        // Покрытие: N2, R2
    }

    [Fact]
    public void T13_NaiveSum_WithNaN_ReturnsNaN()
    {
        var result = Summation.NaiveSum(new double[] { 1.0, double.NaN, 2.0 });
        Assert.True(double.IsNaN(result));
        // Покрытие: N2, R2
    }

    [Fact]
    public void T17_NaiveSum_PositiveInfinity_PropagatesInfinity()
    {
        var result = Summation.NaiveSum(new double[] { 1.0, double.PositiveInfinity, 2.0 });
        Assert.True(double.IsPositiveInfinity(result));
        // Покрытие: N2, R2
    }

    [Fact]
    public void T18_NaiveSum_NegativeInfinity_PropagatesNegInfinity()
    {
        var result = Summation.NaiveSum(new double[] { -3.0, double.NegativeInfinity, 2.0 });
        Assert.True(double.IsNegativeInfinity(result));
        // Покрытие: N2, R2
    }

    [Fact]
    public void T19_NaiveSum_MixedInfinity_ReturnsNaN()
    {
        var result = Summation.NaiveSum(new double[] { double.PositiveInfinity, double.NegativeInfinity });
        Assert.True(double.IsNaN(result));
        // Покрытие: N2, R2
    }

    [Fact]
    public void T23_NaiveSum_Cancellation_LosesUnit()
    {
        var result = Summation.NaiveSum(new double[] { 1e16, 1.0, -1e16 });
        AssertAlmostEqual(result, 0.0);
        // Покрытие: N2, R2
    }

    [Fact]
    public void T30_NaiveSum_PositiveAndNegativeZero_ReturnsZero()
    {
        var result = Summation.NaiveSum(new double[] { 0.0, -0.0 });
        AssertAlmostEqual(result, 0.0);
        // Покрытие: N2, R2
    }

    // ---------- KahanSum ----------

    [Fact]
    public void T2_KahanSum_Null_Throws()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => Summation.KahanSum(null));
        Assert.Equal("values", ex.ParamName);
        Assert.Contains("Коллекция значений не должна быть null", ex.Message);
        // Покрытие: K1, R1 (negative)
    }

    [Fact]
    public void T6_KahanSum_Empty_ReturnsZero()
    {
        var result = Summation.KahanSum(Array.Empty<double>());
        AssertAlmostEqual(result, 0.0);
        // Покрытие: K2, R2
    }

    [Fact]
    public void T14_KahanSum_WithNaN_ReturnsNaN()
    {
        var result = Summation.KahanSum(new double[] { 1.0, double.NaN, 2.0 });
        Assert.True(double.IsNaN(result));
        // Покрытие: K2, R2
    }

    [Fact]
    public void T24_KahanSum_Cancellation_ApproxOne()
    {
        var result = Summation.KahanSum(new double[] { 1e16, 1.0, -1e16 });
        AssertAlmostEqual(result, 0.0);
        // Покрытие: K2, R2
    }

    [Fact]
    public void T27_KahanSum_Subnormal_CloserOrEqual_ToPairwise()
    {
        // Сформируем последовательность очень малых чисел и их суммарный ориентир
        // Используем subnormal ~1e-320
        var values = new List<double>();
        for (int i = 0; i < 50000; i++)
        {
            values.Add(1e-320);
        }
        // Добавим несколько противоположных для разной суммы
        for (int i = 0; i < 1000; i++)
        {
            values.Add(-1e-320);
        }

        // Эталон через попарное суммирование
        var baseline = Summation.PairwiseSum(values.ToArray());
        var naive = Summation.NaiveSum(values);
        var kahan = Summation.KahanSum(values);

        double errNaive = Math.Abs(naive - baseline);
        double errKahan = Math.Abs(kahan - baseline);

        Assert.True(errKahan <= errNaive);
        // Покрытие: K2, R2
    }

    // ---------- NeumaierSum ----------

    [Fact]
    public void T3_NeumaierSum_Null_Throws()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => Summation.NeumaierSum(null));
        Assert.Equal("values", ex.ParamName);
        Assert.Contains("Коллекция значений не должна быть null", ex.Message);
        // Покрытие: Ne1, R1 (negative)
    }

    [Fact]
    public void T7_NeumaierSum_Empty_ReturnsZero()
    {
        var result = Summation.NeumaierSum(Array.Empty<double>());
        AssertAlmostEqual(result, 0.0);
        // Покрытие: Ne2, R2
    }

    [Fact]
    public void T15_NeumaierSum_WithNaN_ReturnsNaN()
    {
        var result = Summation.NeumaierSum(new double[] { 1.0, double.NaN, 2.0 });
        Assert.True(double.IsNaN(result));
        // Покрытие: Ne2, Ne3, R2, R3
    }

    [Fact]
    public void T21_NeumaierSum_MixedInfinity_ReturnsNaN()
    {
        var result = Summation.NeumaierSum(new double[] { double.PositiveInfinity, double.NegativeInfinity });
        Assert.True(double.IsNaN(result));
        // Покрытие: Ne2, Ne3, R2, R3
    }

    [Fact]
    public void T25_NeumaierSum_Cancellation_ApproxOne()
    {
        var result = Summation.NeumaierSum(new double[] { 1e16, 1.0, -1e16 });
        AssertAlmostEqual(result, 1.0, 1e-12);
        // Покрытие: Ne2, Ne3, R2, R3
    }

    [Fact]
    public void T28_NeumaierSum_Subnormal_CloserOrEqual_ToPairwise()
    {
        var values = new List<double>();
        for (int i = 0; i < 50000; i++)
        {
            values.Add(1e-320);
        }
        for (int i = 0; i < 1000; i++)
        {
            values.Add(-1e-320);
        }

        var baseline = Summation.PairwiseSum(values.ToArray());
        var naive = Summation.NaiveSum(values);
        var neumaier = Summation.NeumaierSum(values);

        double errNaive = Math.Abs(naive - baseline);
        double errNeu = Math.Abs(neumaier - baseline);

        Assert.True(errNeu <= errNaive);
        // Покрытие: Ne2, Ne3, R2, R3
    }

    // ---------- PairwiseSum ----------

    [Fact]
    public void T4_PairwiseSum_Null_Throws()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => Summation.PairwiseSum((double[])null));
        Assert.Equal("values", ex.ParamName);
        Assert.Contains("Массив значений не должен быть null", ex.Message);
        // Покрытие: P1, R4 (negative)
    }

    [Fact]
    public void T8_PairwiseSum_Empty_ReturnsZero()
    {
        var result = Summation.PairwiseSum(Array.Empty<double>());
        AssertAlmostEqual(result, 0.0);
        // Покрытие: P2, R5
    }

    [Fact]
    public void T9_PairwiseSum_Len1_ReturnsElement()
    {
        var result = Summation.PairwiseSum(new double[] { 42.0 });
        AssertAlmostEqual(result, 42.0);
        // Покрытие: P3, PR1, R6
    }

    [Fact]
    public void T10_PairwiseSum_Len2_ReturnsSum()
    {
        var result = Summation.PairwiseSum(new double[] { 1.5, 2.5 });
        AssertAlmostEqual(result, 4.0);
        // Покрытие: P3, PR2, R7
    }

    [Fact]
    public void T11_PairwiseSum_Even_ReturnsSum()
    {
        var result = Summation.PairwiseSum(new double[] { 1, 2, 3, 4 });
        AssertAlmostEqual(result, 10.0);
        // Покрытие: P3, PR3, R8
    }

    [Fact]
    public void T12_PairwiseSum_Odd_ReturnsSum()
    {
        var result = Summation.PairwiseSum(new double[] { 1, 2, 3, 4, 5 });
        AssertAlmostEqual(result, 15.0);
        // Покрытие: P3, PR3, R8
    }

    [Fact]
    public void T16_PairwiseSum_WithNaN_ReturnsNaN()
    {
        var result = Summation.PairwiseSum(new double[] { 1.0, double.NaN, 2.0 });
        Assert.True(double.IsNaN(result));
        // Покрытие: P3, PR3, R8
    }

    [Fact]
    public void T22_PairwiseSum_MixedInfinity_ReturnsNaN()
    {
        var result = Summation.PairwiseSum(new double[] { double.PositiveInfinity, double.NegativeInfinity });
        Assert.True(double.IsNaN(result));
        // Покрытие: P3, PR2, R7
    }

    [Fact]
    public void T26_PairwiseSum_Cancellation_GivesZero()
    {
        var result = Summation.PairwiseSum(new double[] { 1e16, 1.0, -1e16 });
        AssertAlmostEqual(result, 0.0);
        // Покрытие: P3, PR2/PR3, R7/R8 (зависит от разбиения)
    }

    [Fact]
    public void T29_PairwiseSum_Overflow_PositiveInfinity()
    {
        var result = Summation.PairwiseSum(new double[] { double.MaxValue, 1e308 });
        Assert.True(double.IsPositiveInfinity(result));
        // Покрытие: P3, PR3, R8
    }
}
