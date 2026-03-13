namespace SimplexUI.Tests
{
    public class SimplexTests
    {
        const double _tolerance = 0.00000001;
        static double TestFunction(double[] x)
        {
            return x[0];
        }

        // Тест свойства GetBest – должно возвращать первую точку после сортировки
        [Fact]
        public void GetBest_ShouldReturnFirstPointAfterSort()
        {
            // Arrange
            var settings = new Settings();
            double[][] coords = { [5.0], [1.0], [3.0] };
            var simplex = new Simplex(settings, coords, TestFunction);
            simplex.SortPoints();

            // Act
            var best = simplex.GetBest;

            // Assert
            Assert.Equal(1.0, best.Coordinates[0]);
        }

        // Тест конструктора
        [Fact]
        public void Constructor_ShoukdInitializationAllFields()
        {
            //Arrange
            Settings settings = new()
            {
                Reflection = 1,
                Compression = 0.5,
                Stretching = 2,
                Reduction = 0.5,
                Step = 0,
                MaxIterations = 200
            };
            double[][] coords = [[1, 2], [3, 4], [5, 6]];
            static double func(double[] x) => x[0] + x[1];
            //Act
            var simplex = new Simplex(settings, coords, func);
            var points = simplex.ClonePoints();
            //Assert

            Assert.Equal(3, points.Length);
            Assert.Equal(1, points[0][0]);
            Assert.Equal(2, points[0][1]);
            Assert.Equal(3, points[1][0]);
            Assert.Equal(3, points[0].Value);
        }

        // Тест свойства GetSecondBest
        [Fact]
        public void GetSecondBest_ShouldReturnSecondPointAfterSort()
        {
            // Arrange
            var settings = new Settings();
            double[][] coords = [[5.0], [1.0], [3.0]];
            var simplex = new Simplex(settings, coords, TestFunction);
            simplex.SortPoints();

            // Act
            var second = simplex.GetSecondBest;

            // Assert
            Assert.Equal(3.0, second.Coordinates[0]);
        }

        // Тест свойства GetWorst
        [Fact]
        public void GetWorst_ShouldReturnLastPointAfterSort()
        {
            // Arrange
            var settings = new Settings();
            double[][] coords = { [5.0], [1.0], [3.0] };
            var simplex = new Simplex(settings, coords, TestFunction);
            simplex.SortPoints();

            // Act
            var worst = simplex.GetWorst;

            // Assert
            Assert.Equal(5.0, worst.Coordinates[0]);
        }

        // Тест свойства GetCenter – центр тяжести всех точек, кроме худшей
        [Fact]
        public void GetCenter_ShouldExcludeWorstPoint()
        {
            // Arrange
            var settings = new Settings();
            double[][] coords = { [0.0, 0.0], [2.0, 0.0], [0.0, 4.0] };
            static double func(double[] x) => x[0] + x[1];
            var simplex = new Simplex(settings, coords, func);
            simplex.SortPoints();

            // Act
            var center = simplex.GetCenter;

            // Assert
            Assert.Equal(1.0, center.Coordinates[0]);
            Assert.Equal(0.0, center.Coordinates[1]);
            Assert.Equal(1.0, center.Value);
        }

        // Тест метода Reflect – проверка формулы отражения
        [Theory]
        [InlineData(1.0)]
        [InlineData(0.8)]
        public void Reflect_ShouldReturnCorrectPoint(double reflectionCoeff)
        {
            // Arrange
            var settings = new Settings { Reflection = reflectionCoeff };
            // Создаём Simplex с фиктивными координатами – для вызова Reflect точки не нужны
            var simplex = new Simplex(settings, [], TestFunction);
            var worst = new Point([3.0], TestFunction);
            var center = new Point([1.0], TestFunction);

            // Act
            var reflected = simplex.Reflect(worst, center);

            // Ожидаемая формула:
            double expected = 1 + (1 - 3) * reflectionCoeff;//reflectionPoint + (reflectionPoint - reflectedPoint) * _settings.Reflection;

            // Assert
            Assert.Equal(expected, reflected.Coordinates[0], precision: 10);
            Assert.Equal(worst.Function, reflected.Function); // функция должна быть той же
        }

        // Тест метода Expand
        [Theory]
        [InlineData(2.0)]
        [InlineData(1.5)]
        public void Expand_ShouldReturnCorrectPoint(double stretchCoeff)
        {
            // Arrange
            var settings = new Settings { Stretching = stretchCoeff };
            var simplex = new Simplex(settings, new double[0][], TestFunction);
            var expandedPoint = new Point([3.0], TestFunction);
            var center = new Point([1.0], TestFunction);

            // Act
            var expanded = simplex.Expand(expandedPoint, center);
            double expected = 1.0 + 2.0 * stretchCoeff;

            // Assert
            Assert.Equal(expected, expanded.Coordinates[0], precision: 10);
        }


        // Тест метода ReplaceWorst – худшая точка заменяется на новую
        [Fact]
        public void ReplaceWorst_ShouldReplaceLastPoint()
        {
            // Arrange
            var settings = new Settings();
            double[][] coords = [[0.0], [1.0], [2.0]];
            var simplex = new Simplex(settings, coords, TestFunction);
            var newPoint = new Point([5.0], TestFunction);
            var originalPoints = simplex.ClonePoints();

            // Act
            simplex.ReplaceWorst(newPoint);
            var points = simplex.ClonePoints();

            // Assert
            Assert.Equal(originalPoints.Length, points.Length);
            // Первые две точки не изменились
            Assert.Equal(originalPoints[0].Coordinates[0], points[0].Coordinates[0]);
            Assert.Equal(originalPoints[1].Coordinates[0], points[1].Coordinates[0]);
            // Последняя точка стала новой
            Assert.Equal(5.0, points[2].Coordinates[0]);
        }

        [Fact]
        public void ReduceSimplex_ShouldShrinkAllPointsTowardsBest()
        {
            // Arrange
            var settings = new Settings { Reduction = 0.5 };
            double[][] coords = [[0.0], [4.0], [8.0]];
            var simplex = new Simplex(settings, coords, TestFunction);
            simplex.SortPoints();

            // Act
            simplex.ReduceSimplex();
            var points = simplex.ClonePoints();

            // Assert
            Assert.Equal(0.0, points[0].Coordinates[0]); // лучшая не меняется
            Assert.Equal(0.0 + (4.0 - 0.0) * 0.5, points[1].Coordinates[0]); // 2.0
            Assert.Equal(0.0 + (8.0 - 0.0) * 0.5, points[2].Coordinates[0]); // 4.0
        }

        [Fact]
        public void Reflect_WhenPointsHaveDifferentFunctions_ThrowsException()
        {
            // Arrange
            var simplex = new Simplex(new Settings(), new double[0][], x => x[0]);
            var point1 = new Point(new[] { 1.0 }, x => x[0]);
            var point2 = new Point(new[] { 2.0 }, x => x[0] * 2); // другая функция

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => simplex.Reflect(point1, point2));
            Assert.Equal("Left and right part have different pointers to functions", ex.Message);
        }

        [Fact]
        public void Iteration_WhenReflectedisBestAndExpandedBetter_ShouldReplaceWorstWithExpanded()
        {
            //Arrange
            var settings = new Settings { Reflection = 1.0, Stretching = 2.0 };
            double[][] coords = [[0, 0], [3, 0], [0, 4]];
            Func<double[], double> func = x => x[0] + x[1];
            var simplex = new Simplex(settings, coords, func);
            simplex.SortPoints();

            //Act
            simplex.Iteration();
            var points = simplex.ClonePoints();
            var worst = points[^1];

            //Assert
            Assert.Equal(4.5, worst.Coordinates[0]);
            Assert.Equal(-8.0, worst.Coordinates[1]);
            Assert.Equal(-3.5, worst.Value);
        }

        [Fact]
        public void Iteration_WhenReflectedIsBestAndExpandedNotBetter_ShouldReplaceWorstWithReflected()
        {
            //Arrange
            var settings = new Settings { Reflection = 1.0, Stretching = 2.0 };
            double[][] coords = [[3, 0], [4, 0], [5, 0]];
            Func<double[], double> func = x => Math.Pow(x[0] - 2,2);
            var simplex = new Simplex(settings, coords, func);
            simplex.SortPoints();

            //Act
            simplex.Iteration();
            var points = simplex.ClonePoints();
            var worst = points[^1];

            //Assert
            Assert.Equal(2.0, worst.Coordinates[0]);
            Assert.Equal(0.0, worst.Coordinates[1]);
            Assert.Equal(0.0, worst.Value);
        }

        [Fact]
        public void Iteration_WhenReflectedIsBetweenBestAndSecondBest_ShouldReplaceWorstWithReflected()
        {
            //Arrange
            var settings = new Settings { Reflection = 1.0 };
            double[][] coords = [[2, 0], [4, 0], [5, 0]];
            Func<double[], double> func = x => Math.Pow(x[0] - 2, 2);
            var simplex = new Simplex(settings, coords, func);
            simplex.SortPoints();

            //Act
            simplex.Iteration();
            var points = simplex.ClonePoints();
            var worst = points[^1];

            //Assert
            Assert.Equal(1.0, worst.Coordinates[0]);
            Assert.Equal(0.0, worst.Coordinates[1]);
            Assert.Equal(1.0, worst.Value);
        }

        [Fact]
        public void Iteration_WhenReflectedWorseThanSecondButBetterThanWorstAndOutsideContractionIsBetter_ShouldReplaceWorstWithContractedOutside()
        {
            //Arrange
            var settings = new Settings { Reflection = 1.0, Compression = 0.5 };
            double[][] coords = [[2, 0], [3.2, 0], [5, 0]];
            Func<double[], double> func = x => Math.Pow(x[0] - 2, 2);
            var simplex = new Simplex(settings, coords, func);
            simplex.SortPoints();

            //Act
            simplex.Iteration();
            var points = simplex.ClonePoints();
            var worst = points[^1];

            //Assert
            Assert.Equal(1.4, worst.Coordinates[0], _tolerance);
            Assert.Equal(0.0, worst.Coordinates[1]);
            Assert.Equal(0.36, worst.Value, _tolerance);
        }

        [Fact]
        public void Iteration_WhenReflectedIsWorseThanWorstAndInsideContractionIsBetter_ShouldReplaceWorstWithContractedInside()
        {
            //Arrange
            var settings = new Settings { Reflection = 1.0, Compression = 0.5 };
            double[][] coords = [[2, 0], [3, 0], [0.5, 0]];
            Func<double[], double> func = x => Math.Pow(x[0] - 2, 2);
            var simplex = new Simplex(settings, coords, func);
            simplex.SortPoints();

            //Act
            simplex.Iteration();
            var points = simplex.ClonePoints();
            var worst = points[^1];

            //Assert
            Assert.Equal(1.5, worst.Coordinates[0]);
            Assert.Equal(0.0, worst.Coordinates[1]);
            Assert.Equal(0.25, worst.Value);
        }

        [Fact]
        public void Iteration_WhenReflectedIsWorseThanSecondButBetterThanWorstAndOutsideContractionIsNotBetter_ShouldReduceSimplex()
        {
            //Arrange
            var settings = new Settings { Reflection = 1.0, Compression = 0.5, Reduction = 0.5 };
            double[][] coords = [[2.0, 0.0], [3.2, 0.0], [5.0, 0.0]];
            Func<double[], double> func = (x) =>
            {
                double x0 = x[0];
                if (Math.Abs(x0 - 2.6) < 1e-6) return 0.36;   // центр
                if (Math.Abs(x0 - 0.2) < 1e-6) return 3.24;   // отражённая
                if (Math.Abs(x0 - 1.4) < 1e-6) return 5.0;    // внешнее сжатие (хуже)
                return Math.Pow(x0 - 2, 2);
            };
            var simplex = new Simplex(settings, coords, func);
            simplex.SortPoints();

            //Act
            simplex.Iteration();
            var points = simplex.ClonePoints();

            //Assert
            Assert.Equal(2.0, points[0].Coordinates[0], 2);
            Assert.Equal(2.6, points[1].Coordinates[0], 2);
            Assert.Equal(3.5, points[2].Coordinates[0], 2);
        }

        [Fact]
        public void Iteration_WhenReflectedIsWorseThanWorstAndInsideContractionIsNotBetter_ShouldReduceSimplex()
        {
            //Arrange
            var settings = new Settings { Reflection = 1.0, Compression = 0.5, Reduction = 0.5 };
            double[][] coords = [[2.0, 0.0], [3.0, 0.0], [0.5, 0.0]];
            Func<double[], double> func = (x) =>
            {
                double x0 = x[0];
                if (Math.Abs(x0 - 2.5) < 1e-6) return 0.25;   // центр
                if (Math.Abs(x0 - 4.5) < 1e-6) return 6.25;   // отражённая
                if (Math.Abs(x0 - 1.5) < 1e-6) return 3.0;    // внутреннее сжатие (хуже)
                return Math.Pow(x0 - 2, 2);
            };
            var simplex = new Simplex(settings, coords, func);
            simplex.SortPoints();

            //Act
            simplex.Iteration();
            var points = simplex.ClonePoints();

            //Assert
            Assert.Equal(2.0, points[0].Coordinates[0], 2);
            Assert.Equal(2.5, points[1].Coordinates[0], 2);
            Assert.Equal(1.25, points[2].Coordinates[0], 2);
        }
    }
}