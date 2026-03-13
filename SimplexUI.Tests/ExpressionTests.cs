using SimplexUI;
using System;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace SimplexUI.Tests
{
    public class ExpressionTests
    {
        [Fact]
        public void Constructor_ValidExpression_ShouldSetIsCorrectTrue()
        {
            //Arrange
            string expression = "2+3*x";

            //Act
            var function = new Function(expression);

            //Assert
            Assert.True(function.IsCorrect);
        }
        [Fact]
        public void Constructor_InvalidExpression_UnbalancedParentheses_ShouldSetIsCorrectFalse()
        {
            //Arrange
            string expression = "(2+3*x";

            //Act
            var func = new Function(expression);

            //Assert
            Assert.False(func.IsCorrect);
        }
        [Fact]
        public void Constructor_InvalidExpression_UnknownSymbol_ShouldSetIsCorrectFalse()
        {
            //Arrange
            string expression = "2+3@x";

            //Act
            var func = new Function(expression);

            //Assert
            Assert.False(func.IsCorrect);
        }
        [Fact]
        public void Constructor_InvalidExpression_UnbalancedBracketsForFunction_ShouldSetIsCorrectFalse()
        {
            //Arrange
            string expression = "sin(x";

            //Act
            var func = new Function(expression);

            //Assert
            Assert.False(func.IsCorrect);
        }
        [Fact]
        public void Constructor_EmptyExpression_ShouldSetIsCorrectFalse()
        {
            //Arrange
            string expression = "";

            //Act
            var func = new Function(expression);

            //Assert
            Assert.False(func.IsCorrect);
        }
        [Fact]
        public void TryCalculate_SimpleConstantExpression_ReturnsCorrectValue()
        {
            //Arrange
            var func = new Function("2+3*4");
            double expected = 2 + 3 * 4; // 14

            //Act
            bool success = func.TryCalculate(0, out double result);

            //Assert
            Assert.True(success);
            Assert.Equal(expected, result, precision: 10);
        }
        [Fact]
        public void TryCalculate_ExpressionWithVariableX_ReturnsCorrectValue()
        {
            //Arrange
            var func = new Function("x*x + 2*x + 1");
            double x = 3;
            double expected = x * x + 2 * x + 1;

            //Act
            bool success = func.TryCalculate(x, out double result);

            //Assert
            Assert.True(success);
            Assert.Equal(expected, result, precision: 10);
        }
        [Fact]
        public void TryCalculate_ExpressionWithParentheses_RespectsPriority()
        {
            //Arrange
            var func = new Function("(2+3)*4");
            double expected = (2 + 3) * 4; // 20

            //Act
            bool success = func.TryCalculate(0, out double result);

            //Assert
            Assert.True(success);
            Assert.Equal(expected, result, precision: 10);
        }

        [Fact]
        public void TryCalculate_NegativeNumber_HandledCorrectly()
        {
            //Arrange
            var func = new Function("-5+3");
            double expected = -5 + 3; // -2

            //Act
            bool success = func.TryCalculate(0, out double result);

            //Assert
            Assert.True(success);
            Assert.Equal(expected, result, precision: 10);
        }

        [Fact]
        public void TryCalculate_NegativeVariable_HandledCorrectly()
        {
            //Arrange
            var func = new Function("-x");
            double x = 2;
            double expected = -2;

            //Act
            bool success = func.TryCalculate(x, out double result);

            //Assert
            Assert.True(success);
            Assert.Equal(expected, result, precision: 10);
        }
        [Fact]
        public void TryCalculate_UnaryFunctionSin_ReturnsCorrectValue()
        {
            //Arrange
            var func = new Function("sin[x]");
            double x = Math.PI / 2;
            double expected = Math.Sin(x); // 1

            //Act
            bool success = func.TryCalculate(x, out double result);

            //Assert
            Assert.True(success);
            Assert.Equal(expected, result, precision: 10);
        }
        [Fact]
        public void TryCalculate_UnaryFunctionCos_ReturnsCorrectValue()
        {
            //Arrange
            var func = new Function("cos[x]");
            double x = 0;
            double expected = Math.Cos(x); // 1

            //Act
            bool success = func.TryCalculate(x, out double result);

            //Assert
            Assert.True(success);
            Assert.Equal(expected, result, precision: 10);
        }
        [Fact]
        public void TryCalculate_UnaryFunctionTan_ReturnsCorrectValue()
        {
            //Arrange
            var func = new Function("tan[x]");
            double x = Math.PI / 4;
            double expected = Math.Tan(x); // 1

            //Act
            bool success = func.TryCalculate(x, out double result);

            //Assert
            Assert.True(success);
            Assert.Equal(expected, result, precision: 10);
        }
        [Fact]
        public void TryCalculate_UnaryFunctionLog_ReturnsCorrectValue()
        {
            //Arrange
            var func = new Function("log[x]");
            double x = Math.E;
            double expected = Math.Log(x); // 1

            //Act
            bool success = func.TryCalculate(x, out double result);

            //Assert
            Assert.True(success);
            Assert.Equal(expected, result, precision: 10);
        }
        [Fact]
        public void TryCalculate_UnaryFunctionAbs_ReturnsCorrectValue()
        {
            //Arrange
            var func = new Function("abs[x]");
            double x = -5;
            double expected = Math.Abs(x); // 5

            //Act
            bool success = func.TryCalculate(x, out double result);

            //Assert
            Assert.True(success);
            Assert.Equal(expected, result, precision: 10);
        }
        [Fact]
        public void TryCalculate_NestedFunctions_ReturnsCorrectValue()
        {
            //Arrange
            var func = new Function("sin[cos[x]]");
            double x = 0;
            double expected = Math.Sin(Math.Cos(0)); // sin(1) ~ 0.84147

            //Act
            bool success = func.TryCalculate(x, out double result);

            //Assert
            Assert.True(success);
            Assert.Equal(expected, result, precision: 5);
        }
        [Fact]
        public void TryCalculate_FunctionWithExpressionInside_ReturnsCorrectValue()
        {
            //Arrange
            var func = new Function("sin[x+1]");
            double x = 0;
            double expected = Math.Sin(1); // ~ 0.84147

            //Act
            bool success = func.TryCalculate(x, out double result);

            //Assert
            Assert.True(success);
            Assert.Equal(expected, result, precision: 5);
        }
        [Fact]
        public void TryCalculate_DivisionByZero_ReturnsInfinity()
        {
            //Arrange
            var func = new Function("1/0");

            //Act
            bool success = func.TryCalculate(0, out double result);

            //Assert
            Assert.True(success); // вычисление не должно вернуть false, просто результат бесконечность
            Assert.Equal(double.PositiveInfinity, result);
        }
        [Fact]
        public void TryCalculate_InvalidExpression_ReturnsFalse()
        {
            //Arrange
            var func = new Function("2+*3");

            //Act
            bool success = func.TryCalculate(0, out double result);

            //Assert
            Assert.False(success);
            Assert.Equal(double.NaN, result);
        }
        [Fact]
        public void TryCalculate_UnknownFunction_ReturnsFalse()
        {
            //Arrange
            var func = new Function("unknown(x)");

            //Act
            bool success = func.TryCalculate(0, out double result);

            //Assert
            Assert.False(success);
            Assert.Equal(double.NaN, result);
        }
        [Fact]
        public void TryCalculate_WhenIsCorrectFalse_ReturnsFalse()
        {
            //Arrange
            var func = new Function("2+*3");

            //Act
            bool success = func.TryCalculate(0, out double result);

            //Assert
            Assert.False(success);
            Assert.Equal(double.NaN, result);
        }
        [Fact]
        public void TryCalculate_ComplexExpression_ReturnsCorrectValue()
        {
            //Arrange
            var func = new Function("2*sin[x] + sqrt[x+1] - abs[-3]");
            double x = 2;
            double expected = 2 * Math.Sin(2) + Math.Sqrt(3) - 3; // примерно 2*0.9093 + 1.732 - 3 = 1.8186+1.732-3=0.5506

            //Act
            bool success = func.TryCalculate(x, out double result);

            //Assert
            Assert.True(success);
            Assert.Equal(expected, result, precision: 4);
        }
        [Fact]
        public void TryCalculate_ExpressionWithSpaces_IgnoresSpaces()
        {
            //Arrange
            var func = new Function("  2  +  3  *  x  ");
            double x = 4;
            double expected = 2 + 3 * 4; // 14

            //Act
            bool success = func.TryCalculate(x, out double result);

            //Assert
            Assert.True(success);
            Assert.Equal(expected, result, precision: 10);
        }
        [Fact]
        public void TryCalculate_ExpressionWithUnaryMinusAndFunction_ReturnsCorrectValue()
        {
            // Arrange
            var func = new Function("-sin[x]");
            double x = 0;
            double expected = -Math.Sin(0); // 0

            // Act
            bool success = func.TryCalculate(x, out double result);

            // Assert
            Assert.True(success);
            Assert.Equal(expected, result, precision: 10);
        }
    }
}
