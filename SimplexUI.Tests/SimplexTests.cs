namespace SimplexUI.Tests
{
    /// <summary>
    /// Блять ПЕРЕСИЧИТАТЬ значение для Assert
    /// </summary>
    public class SimplexTests
    {
        const double _tolerance = 0.00000001;
        static double TestFunction(double[] x) => x[0];
        static double TestSumFunction(double[] x) => x[0] + x[1];
        static double TestPowFunction(double[] x) => Math.Pow(x[0] - 2, 2);


        [Fact]
        public void GetBest_ShouldReturnFirstPointAfterSort()
        {
            var settings = new Settings();
            var initialConditions = new InitialConditions([[5], [1]], TestFunction);
            var simplex = new Simplex(settings, initialConditions);

            simplex.SortPoints();
            var best = simplex.GetBestInSorted;

            Assert.Equal(1, best[0], _tolerance);
        }

        [Fact]
        public void Constructor_ShoukdInitializationAllFields()
        {
            var settings = new Settings();
            var initialConditions = new InitialConditions([[1, 2], [3, 4], [5, 6]], TestSumFunction);
            var simplex = new Simplex(settings, initialConditions);

            var points = simplex.ClonePoints();

            Assert.Equal(3, points.Length, _tolerance);
            Assert.Equal(1, points[0][0], _tolerance);
            Assert.Equal(2, points[0][1], _tolerance);
            Assert.Equal(3, points[1][0], _tolerance);
            Assert.Equal(3, points[0].Value, _tolerance);
        }

        [Fact]
        public void GetSecondBest_ShouldReturnSecondPointAfterSort()
        {
            var settings = new Settings();
            var initialConditions = new InitialConditions([[5], [1]], TestFunction);
            var simplex = new Simplex(settings, initialConditions);

            simplex.SortPoints();
            var second = simplex.GetSecondBestInSorted;

            Assert.Equal(5, second[0], _tolerance);
        }

        [Fact]
        public void GetWorst_ShouldReturnLastPointAfterSort()
        {
            var settings = new Settings();
            var initialConditions = new InitialConditions([[5], [1]], TestFunction);
            var simplex = new Simplex(settings, initialConditions);

            simplex.SortPoints();
            var worst = simplex.GetWorstInSorted;

            Assert.Equal(5, worst[0], _tolerance);
        }

        [Fact]
        public void GetCenter_ShouldExcludeWorstPoint()
        {
            var settings = new Settings();
            var initialConditions = new InitialConditions([[0, 0], [2, 0], [0, 4]], TestSumFunction);
            var simplex = new Simplex(settings, initialConditions);

            simplex.SortPoints();
            var center = simplex.GetCenterInSorted;

            Assert.Equal(1, center[0], _tolerance);
            Assert.Equal(0, center[1], _tolerance);
            Assert.Equal(1, center.Value, _tolerance);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(0.8)]
        public void Reflect_ShouldReturnCorrectPoint(double reflectionCoeff)
        {
            var settings = new Settings(1, reflection: reflectionCoeff);
            var initialConditions = new InitialConditions([], TestFunction); // Создаём Simplex с фиктивными координатами – для вызова Reflect точки не нужны
            var simplex = new Simplex(settings, initialConditions);
            var worst = new EvaluateableVector([3], TestFunction);
            var center = new EvaluateableVector([1], TestFunction);

            var reflected = simplex.Reflect(worst, center);

            double expected = 1 + (1 - 3) * reflectionCoeff; // Ожидаемая формула
            Assert.Equal(expected, reflected[0], _tolerance);
            Assert.Equal(worst.Function, reflected.Function); // функция должна быть той же
        }

        [Theory]
        [InlineData(2)]
        [InlineData(1.5)]
        public void Expand_ShouldReturnCorrectPoint(double stretchCoeff)
        {
            var settings = new Settings(1, stretching: stretchCoeff);
            var initialConditions = new InitialConditions([], TestFunction);
            var simplex = new Simplex(settings, initialConditions);
            var expandedPoint = new EvaluateableVector([3], TestFunction);
            var center = new EvaluateableVector([1], TestFunction);

            var expanded = simplex.Expand(expandedPoint, center);
            double expected = 1 + 2 * stretchCoeff;

            Assert.Equal(expected, expanded[0], _tolerance);
        }

        [Fact]
        public void ReplaceWorst_ShouldReplaceLastPoint()
        {
            var settings = new Settings();
            var initialConditions = new InitialConditions([[0], [2]], TestFunction);
            var simplex = new Simplex(settings, initialConditions);
            var newPoint = new EvaluateableVector([5], TestFunction);
            var originalPoints = simplex.ClonePoints();

            simplex.ReplaceWorst(newPoint);
            var points = simplex.ClonePoints();

            Assert.Equal(originalPoints.Length, points.Length);
            Assert.Equal(originalPoints[0][0], points[0][0]); // Первая точка не изменились
            Assert.Equal(5, points[1][0]); // Последняя точка стала новой
        }

        [Fact]
        public void ReduceSimplex_ShouldShrinkAllPointsTowardsBest()
        {
            var settings = new Settings();
            var initialConditions = new InitialConditions([[0], [8]], TestFunction);
            var simplex = new Simplex(settings, initialConditions);
            simplex.SortPoints();

            simplex.ReduceSimplex();
            var points = simplex.ClonePoints();

            Assert.Equal(0, points[0][0], _tolerance); // лучшая не меняется
            Assert.Equal(0 + (8 - 0) * 0.5, points[1][0], _tolerance); // 4
        }

        [Fact]
        public void Reflect_WhenPointsHaveDifferentFunctions_ThrowsException()
        {
            var settings = new Settings();
            var initialConditions = new InitialConditions([], TestFunction);
            var simplex = new Simplex(settings, initialConditions);
            var point1 = new EvaluateableVector([1], x => x[0]);
            var point2 = new EvaluateableVector([2], x => x[0] * 2); // другая функция

            var ex = Assert.Throws<Exception>(() => simplex.Reflect(point1, point2));

            Assert.Equal("Left and right part have different pointers to functions", ex.Message);
        }

        [Fact]
        public void Iteration_WhenReflectedIsBestAndExpandedBetter_ShouldReplaceWorstWithExpanded()
        {
            var settings = new Settings();
            var initialConditions = new InitialConditions([[0, 0], [3, 0], [0, 4]], TestSumFunction);
            var simplex = new Simplex(settings, initialConditions);

            simplex.Iteration();
            simplex.SortPoints();
            var best = simplex.GetBestInSorted;

            Assert.Equal(4.5, best[0], _tolerance);
            Assert.Equal(-8, best[1], _tolerance);
            Assert.Equal(-3.5, best.Value, _tolerance);
        }

        [Fact]
        public void Iteration_WhenReflectedIsBestAndExpandedNotBetter_ShouldReplaceWorstWithReflected()
        {
            var settings = new Settings();
            var initialConditions = new InitialConditions([[3, 0], [4, 0], [5, 0]], TestPowFunction);
            var simplex = new Simplex(settings, initialConditions);

            simplex.Iteration();
            simplex.SortPoints();
            var best = simplex.GetBestInSorted;

            Assert.Equal(2, best[0], _tolerance);
            Assert.Equal(0, best[1], _tolerance);
            Assert.Equal(0, best.Value, _tolerance);
        }

        [Fact]
        public void Iteration_WhenReflectedIsBetweenBestAndSecondBest_ShouldReplaceWorstWithReflected()
        {
            var settings = new Settings();
            var initialConditions = new InitialConditions([[2, 0], [4, 0], [5, 0]], TestPowFunction);
            var simplex = new Simplex(settings, initialConditions);

            simplex.Iteration();
            simplex.SortPoints();
            var second = simplex.GetSecondBestInSorted;

            Assert.Equal(1, second[0], _tolerance);
            Assert.Equal(0, second[1], _tolerance);
            Assert.Equal(1, second.Value, _tolerance);
        }

        [Fact]
        public void Iteration_WhenReflectedWorseThanSecondButBetterThanWorstAndOutsideContractionIsBetter_ShouldReplaceWorstWithContractedOutside()
        {
            var settings = new Settings();
            var initialConditions = new InitialConditions([[2, 0], [3.2, 0], [5, 0]], TestPowFunction);
            var simplex = new Simplex(settings, initialConditions);

            simplex.Iteration();
            simplex.SortPoints();
            var second = simplex.GetSecondBestInSorted;

            Assert.Equal(1.4, second[0], _tolerance);
            Assert.Equal(0, second[1], _tolerance);
            Assert.Equal(0.36, second.Value, _tolerance);
        }

        [Fact]
        public void Iteration_WhenReflectedIsWorseThanWorstAndInsideContractionIsBetter_ShouldReplaceWorstWithContractedInside()
        {
            var settings = new Settings();
            var initialConditions = new InitialConditions([[2, 0], [3, 0], [0.5, 0]], TestPowFunction);
            var simplex = new Simplex(settings, initialConditions);

            simplex.Iteration();
            simplex.SortPoints();
            var worst = simplex.GetSecondBestInSorted;

            Assert.Equal(1.5, worst[0], _tolerance);
            Assert.Equal(0, worst[1], _tolerance);
            Assert.Equal(0.25, worst.Value, _tolerance);
        }

        [Fact]
        public void Iteration_WhenReflectedIsWorseThanSecondButBetterThanWorstAndOutsideContractionIsNotBetter_ShouldReduceSimplex()
        {
            var settings = new Settings();
            var initialConditions = new InitialConditions([[2, 0], [3.2, 0], [5, 0]], Function);
            var simplex = new Simplex(settings, initialConditions);
            simplex.SortPoints();

            simplex.Iteration();
            var points = simplex.ClonePoints();

            Assert.Equal(2, points[0][0], _tolerance);
            Assert.Equal(2.6, points[1][0], _tolerance);
            Assert.Equal(3.5, points[2][0], _tolerance);

            static double Function(double[] x)
            {
                double x0 = x[0];
                if (Math.Abs(x0 - 2.6) < 1e-6)
                {
                    return 0.36;   // центр
                }
                else if (Math.Abs(x0 - 0.2) < 1e-6)
                {
                    return 3.24;   // отражённая
                }
                else if (Math.Abs(x0 - 1.4) < 1e-6)
                {
                    return 5;    // внешнее сжатие (хуже)
                }

                return Math.Pow(x0 - 2, 2);
            }
        }

        [Fact]
        public void Iteration_WhenReflectedIsWorseThanWorstAndInsideContractionIsNotBetter_ShouldReduceSimplex()
        {
            var settings = new Settings();
            var initialConditions = new InitialConditions([[2, 0], [3, 0], [0.5, 0]], Function);
            var simplex = new Simplex(settings, initialConditions);
            simplex.SortPoints();

            simplex.Iteration();
            var points = simplex.ClonePoints();

            Assert.Equal(2, points[0][0], _tolerance);
            Assert.Equal(2.5, points[1][0], _tolerance);
            Assert.Equal(1.25, points[2][0], _tolerance);

            static double Function(double[] x)
            {
                double x0 = x[0];
                if (Math.Abs(x0 - 2.5) < 1e-6)
                {
                    return 0.25;   // центр
                }
                else if (Math.Abs(x0 - 4.5) < 1e-6)
                {
                    return 6.25;   // отражённая
                }
                else if (Math.Abs(x0 - 1.5) < 1e-6)
                {
                    return 3;    // внутреннее сжатие (хуже)
                }

                return Math.Pow(x0 - 2, 2);
            }
        }
    }
}