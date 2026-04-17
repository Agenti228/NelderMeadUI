namespace SimplexUI.Tests
{
    public class ExpressionTests
    {
        [Fact]
        public void Constructor_ValidExpression_ShouldSetIsCorrectTrue()
        {
            string expression = "2+3*x";

            var function = new Function(expression);

            Assert.True(function.IsCorrect);
        }

        [Fact]
        public void Constructor_InvalidExpression_UnbalancedParentheses_ShouldSetIsCorrectFalse()
        {
            string expression = "(2+3*x";

            var func = new Function(expression);

            Assert.False(func.IsCorrect);
        }

        [Fact]
        public void Constructor_InvalidExpression_UnknownSymbol_ShouldSetIsCorrectFalse()
        {
            string expression = "2+3@x";

            var func = new Function(expression);

            Assert.False(func.IsCorrect);
        }

        [Fact]
        public void Constructor_InvalidExpression_UnbalancedBracketsForFunction_ShouldSetIsCorrectFalse()
        {
            string expression = "sin(x";

            var func = new Function(expression);

            Assert.False(func.IsCorrect);
        }

        [Fact]
        public void Constructor_EmptyExpression_ShouldSetIsCorrectFalse()
        {
            string expression = "";

            var func = new Function(expression);

            Assert.False(func.IsCorrect);
        }

        [Fact]
        public void TryCalculate_SimpleConstantExpression_ReturnsCorrectValue()
        {
            var func = new Function("2+3*4");
            double expected = 2 + 3 * 4; // 14

            double result = func.Calculate([0]);

            Assert.True(func.IsCorrect);
            Assert.Equal(expected, result, precision: 10);
        }

        [Fact]
        public void TryCalculate_ExpressionWithVariableX_ReturnsCorrectValue()
        {
            var func = new Function("x*x + 2*x + 1");
            double x = 3;
            double expected = x * x + 2 * x + 1;

            double result = func.Calculate([x]);

            Assert.True(func.IsCorrect);
            Assert.Equal(expected, result, precision: 10);
        }

        [Fact]
        public void TryCalculate_ExpressionWithParentheses_RespectsPriority()
        {
            var func = new Function("(2+3)*4");
            double expected = (2 + 3) * 4; // 20

            double result = func.Calculate([0]);

            Assert.True(func.IsCorrect);
            Assert.Equal(expected, result, precision: 10);
        }

        [Fact]
        public void TryCalculate_NegativeNumber_HandledCorrectly()
        {
            var func = new Function("-5+3");
            double expected = -5 + 3; // -2

            double result = func.Calculate([0]);

            Assert.True(func.IsCorrect);
            Assert.Equal(expected, result, precision: 10);
        }

        [Fact]
        public void TryCalculate_NegativeVariable_HandledCorrectly()
        {
            var func = new Function("-x");
            double x = 2;
            double expected = -2;

            double result = func.Calculate([x]);

            Assert.True(func.IsCorrect);
            Assert.Equal(expected, result, precision: 10);
        }

        [Fact]
        public void TryCalculate_UnaryFunctionSin_ReturnsCorrectValue()
        {
            var func = new Function("sin(x)");
            double x = Math.PI / 2;
            double expected = Math.Sin(x); // 1

            double result = func.Calculate([x]);

            Assert.True(func.IsCorrect);
            Assert.Equal(expected, result, precision: 10);
        }

        [Fact]
        public void TryCalculate_UnaryFunctionCos_ReturnsCorrectValue()
        {
            var func = new Function("cos(x)");
            double x = 0;
            double expected = Math.Cos(x); // 1

            double result = func.Calculate([x]);

            Assert.True(func.IsCorrect);
            Assert.Equal(expected, result, precision: 10);
        }

        [Fact]
        public void TryCalculate_UnaryFunctionTan_ReturnsCorrectValue()
        {
            var func = new Function("tan(x)");
            double x = Math.PI / 4;
            double expected = Math.Tan(x); // 1

            double result = func.Calculate([x]);

            Assert.True(func.IsCorrect);
            Assert.Equal(expected, result, precision: 10);
        }

        [Fact]
        public void TryCalculate_UnaryFunctionLog_ReturnsCorrectValue()
        {
            var func = new Function("log(x)");
            double x = Math.E;
            double expected = Math.Log(x); // 1

            double result = func.Calculate([x]);

            Assert.True(func.IsCorrect);
            Assert.Equal(expected, result, precision: 10);
        }

        [Fact]
        public void TryCalculate_UnaryFunctionAbs_ReturnsCorrectValue()
        {
            var func = new Function("abs(x)");
            double x = -5;
            double expected = Math.Abs(x); // 5

            double result = func.Calculate([x]);

            Assert.True(func.IsCorrect);
            Assert.Equal(expected, result, precision: 10);
        }

        [Fact]
        public void TryCalculate_NestedFunctions_ReturnsCorrectValue()
        {
            var func = new Function("sin(cos(x))");
            double x = 0;
            double expected = Math.Sin(Math.Cos(0)); // sin(1) ~ 0.84147

            double result = func.Calculate([x]);

            Assert.True(func.IsCorrect);
            Assert.Equal(expected, result, precision: 5);
        }

        [Fact]
        public void TryCalculate_FunctionWithExpressionInside_ReturnsCorrectValue()
        {
            var func = new Function("sin(x+1)");
            double x = 0;
            double expected = Math.Sin(1); // ~ 0.84147

            double result = func.Calculate([x]);

            Assert.True(func.IsCorrect);
            Assert.Equal(expected, result, precision: 5);
        }

        [Fact]
        public void TryCalculate_DivisionByZero_ReturnsInfinity()
        {
            var func = new Function("1/0");

            double result = func.Calculate([0]);

            Assert.True(func.IsCorrect); // вычисление не должно вернуть false, просто результат бесконечность
            Assert.Equal(double.PositiveInfinity, result);
        }

        [Fact]
        public void TryCalculate_InvalidExpression_ReturnsFalse()
        {
            var func = new Function("2+*3");

            double result = func.Calculate([0]);

            Assert.False(func.IsCorrect);
            Assert.Equal(double.NaN, result);
        }

        [Fact]
        public void TryCalculate_UnknownFunction_ReturnsFalse()
        {
            var func = new Function("unknown(x)");

            double result = func.Calculate([0]);

            Assert.False(func.IsCorrect);
            Assert.Equal(double.NaN, result);
        }

        [Fact]
        public void TryCalculate_WhenIsCorrectFalse_ReturnsFalse()
        {
            var func = new Function("2+*3");

            double result = func.Calculate([0]);

            Assert.False(func.IsCorrect);
            Assert.Equal(double.NaN, result);
        }

        [Fact]
        public void TryCalculate_ComplexExpression_ReturnsCorrectValue()
        {
            var func = new Function("2*sin(x) + sqrt(x+1) - abs(-3)");
            double x = 2;
            double expected = 2 * Math.Sin(2) + Math.Sqrt(3) - 3; // примерно 2*0.9093 + 1.732 - 3 = 1.8186+1.732-3=0.5506

            double result = func.Calculate([x]);

            Assert.True(func.IsCorrect);
            Assert.Equal(expected, result, precision: 4);
        }

        [Fact]
        public void TryCalculate_ExpressionWithSpaces_IgnoresSpaces()
        {
            var func = new Function("  2  +  3  *  x  ");
            double x = 4;
            double expected = 2 + 3 * 4; // 14

            double result = func.Calculate([x]);

            Assert.True(func.IsCorrect);
            Assert.Equal(expected, result, precision: 10);
        }

        [Fact]
        public void TryCalculate_ExpressionWithUnaryMinusAndFunction_ReturnsCorrectValue()
        {
            var func = new Function("-sin(x)");
            double x = 0;
            double expected = -Math.Sin(0); // 0

            double result = func.Calculate([x]);

            Assert.True(func.IsCorrect);
            Assert.Equal(expected, result, precision: 10);
        }
    }
}
