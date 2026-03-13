namespace SimplexUI.Tests
{
    public class SimplexTests
    {
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
            // Точки: (0,0)=0, (2,0)=2, (0,4)=4 – после сортировки худшая (0,4)
            double[][] coords = { [0.0, 0.0], [2.0, 0.0], [0.0, 4.0] };
            static double func(double[] x) => x[0] + x[1];
            var simplex = new Simplex(settings, coords, func);
            simplex.SortPoints();

            // Act
            var center = simplex.GetCenter;

            // Assert
            // Центр должен быть средним арифметическим первых двух точек: ((0+2)/2, (0+0)/2) = (1,0)
            Assert.Equal(1.0, center.Coordinates[0]);
            Assert.Equal(0.0, center.Coordinates[1]);
            // Значение функции в центре: 1+0 = 1
            Assert.Equal(1.0, center.Value);
        }

        // Тест метода Reflect – проверка формулы отражения
        [Theory]
        [InlineData(1.0)] // стандартное отражение
        [InlineData(0.8)] // другой коэффициент
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

            // Ожидаемая формула: center + (center - worst) * reflectionCoeff
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

            // Формула: center + (expandedPoint - center) * stretchCoeff
            // expandedPoint - center = 2, умноженное на coeff, плюс center = 1 + 2*coeff
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
            var originalPoints = simplex.ClonePoints(); // сохраняем копию до замены

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

        // Тест метода ReduceSimplex – все точки, кроме лучшей, сжимаются к лучшей
        [Fact]
        public void ReduceSimplex_ShouldShrinkAllPointsTowardsBest()
        {
            // Arrange
            var settings = new Settings { Reduction = 0.5 };
            double[][] coords = [[0.0], [4.0], [8.0]];
            var simplex = new Simplex(settings, coords, TestFunction);
            simplex.SortPoints(); // после сортировки: best=0, second=4, worst=8

            // Act
            simplex.ReduceSimplex();
            var points = simplex.ClonePoints();

            // Assert
            Assert.Equal(0.0, points[0].Coordinates[0]); // лучшая не меняется
            // Остальные: best + (old - best) * reduction
            Assert.Equal(0.0 + (4.0 - 0.0) * 0.5, points[1].Coordinates[0]); // 2.0
            Assert.Equal(0.0 + (8.0 - 0.0) * 0.5, points[2].Coordinates[0]); // 4.0
        }

        // Тест метода Iteration – один из возможных сценариев !!!!!!
        

        //Тест исключения при несовместимых точках в методе Reflect
        [Fact]
        public void Reflect_WhenPointsHaveDifferentFunctions_ThrowsException()
        {
            // Arrange
            var simplex = new Simplex(new Settings(), new double[0][], x => x[0]);
            var point1 = new Point(new[] { 1.0 }, x => x[0]);
            var point2 = new Point(new[] { 2.0 }, x => x[0] * 2); // другая функция

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => simplex.Reflect(point1, point2));
            Assert.Equal("left and right part have different functions", ex.Message);
        }
    }
}