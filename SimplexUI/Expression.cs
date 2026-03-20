using System;
using System.Collections.Generic;
using System.Text;

namespace SimplexUI
{
    /// <summary>
    /// Представляет математическую функцию от одной переменной x.
    /// Поддерживает операции +, -, *, /, ^, скобки (), унарный минус,
    /// а также функции: sin, cos, tan, log, sqrt, abs, exp.
    /// </summary>
    public class Function
    {
        /// <summary>
        /// Проверяет, является ли символ бинарным оператором (не скобкой)
        /// </summary>
        private static bool IsOperator(char ch) => OperatorPriority.ContainsKey(ch) && ch != '(' && ch != ')';
        private static readonly Dictionary<char, int> OperatorPriority = new()
        {
            { '+', 1 },
            { '-', 1 },
            { '*', 2 },
            { '/', 2 },
            { '^', 3 },
            { '(', 0 },
            { ')', 0 }
        };

        private static readonly Dictionary<char, Func<double, double, double>> BinaryOps = new()
        {
            { '+', (a, b) => a + b },
            { '-', (a, b) => a - b },
            { '*', (a, b) => a * b },
            { '/', (a, b) => a / b },
            { '^', Math.Pow }
        };

        private static readonly Dictionary<string, Func<double, double>> UnaryOps = new()
        {
            { "sin", Math.Sin },
            { "cos", Math.Cos },
            { "tan", Math.Tan },
            { "log", Math.Log },
            { "sqrt", Math.Sqrt },
            { "abs", Math.Abs },
            { "exp", Math.Exp }
        };

        private readonly string _infixExpression;
        private string[] _postfixTokens;

        public bool IsCorrect { get; private set; }

        public Function(string input)//Андрей, почини пж
        {
            _infixExpression = input.Replace(" ", "").ToLower();
            ParseToPostfix();
        }

        /// <summary>
        /// Преобразование инфиксного выражения в постфиксную запись (алгоритм сортировочной станции)
        /// </summary>
        private void ParseToPostfix()
        {
            var output = new StringBuilder();
            var operatorStack = new Stack<char>();
            int parenthesesDepth = 0;
            int bracketDepth = 0;

            for (int i = 0; i < _infixExpression.Length; i++)
            {
                char ch = _infixExpression[i];

                if (ch == '(')
                {
                    parenthesesDepth += 10;
                    operatorStack.Push(ch);
                }
                else if (ch == ')')
                {
                    parenthesesDepth -= 10;
                    while (operatorStack.Count > 0 && operatorStack.Peek() != '(')
                    {
                        output.Append('?').Append(operatorStack.Pop());
                    }
                    if (operatorStack.Count > 0 && operatorStack.Peek() == '(')
                        operatorStack.Pop();
                }
                else if (ch == '[')
                {
                    bracketDepth++;
                    output.Append(ch);
                }
                else if (ch == ']')
                {
                    bracketDepth--;
                    output.Append(ch);
                }
                else if (bracketDepth == 0 && IsOperator(ch))
                {
                    if (ch == '-' && (i == 0 || _infixExpression[i - 1] == '(' || _infixExpression[i - 1] == '['))
                    {
                        output.Append('0');
                    }

                    output.Append('?');
                    int currentPriority = OperatorPriority[ch] + parenthesesDepth;

                    while (operatorStack.Count > 0 && operatorStack.Peek() != '(' &&
                           OperatorPriority[operatorStack.Peek()] + parenthesesDepth >= currentPriority)
                    {
                        output.Append('?').Append(operatorStack.Pop());
                    }
                    operatorStack.Push(ch);
                }
                else
                {
                    output.Append(ch);
                }
            }

            while (operatorStack.Count > 0)
            {
                output.Append('?').Append(operatorStack.Pop());
            }

            string postfixString = output.ToString();
            _postfixTokens = postfixString.Split('?', StringSplitOptions.RemoveEmptyEntries);

            if (parenthesesDepth == 0 && bracketDepth == 0)
            {
                IsCorrect = TryCalculateInternal(0, out _);
            }
            else
            {
                IsCorrect = false;
            }
        }

        /// <summary>
        /// Публичный метод вычисления значения функции при заданном x
        /// </summary>
        public bool TryCalculate(double x, out double result)
        {
            if (!IsCorrect)
            {
                result = double.NaN;
                return false;
            }
            return TryCalculateInternal(x, out result);
        }

        /// <summary>
        /// Внутренний метод вычисления по постфиксной записи
        /// </summary>
        private bool TryCalculateInternal(double x, out double result)
        {
            var stack = new Stack<double>();

            foreach (string token in _postfixTokens)
            {
                if (IsNumber(token, out double number))
                {
                    stack.Push(number);
                    continue;
                }

                if (token == "x")
                {
                    stack.Push(x);
                    continue;
                }

                if (token.Length == 1 && BinaryOps.ContainsKey(token[0]))
                {
                    if (stack.Count < 2)
                    {
                        result = double.NaN;
                        return false;
                    }

                    double b = stack.Pop();
                    double a = stack.Pop();
                    char op = token[0];

                    if (op == '/' && Math.Abs(b) == 0)
                    {
                        result = double.NaN;
                        return false;
                    }

                    double value = BinaryOps[op](a, b);
                    stack.Push(value);
                    continue;
                }

                if (token.Contains('[') && token.EndsWith(']'))
                {
                    int openBracket = token.IndexOf('[');
                    string funcName = token.Substring(0, openBracket);

                    if (!UnaryOps.ContainsKey(funcName))
                    {
                        result = double.NaN;
                        return false;
                    }

                    string argExpr = token.Substring(openBracket + 1, token.Length - openBracket - 2);
                    var innerFunc = new Function(argExpr);

                    if (!innerFunc.TryCalculate(x, out double argValue))
                    {
                        result = double.NaN;
                        return false;
                    }

                    double funcResult = UnaryOps[funcName](argValue);
                    stack.Push(funcResult);
                    continue;
                }

                result = double.NaN;
                return false;
            }

            if (stack.Count != 1)
            {
                result = double.NaN;
                return false;
            }

            result = stack.Pop();
            return true;
        }

        /// <summary>
        /// Проверяет, является ли строка корректным числом
        /// </summary>
        private static bool IsNumber(string token, out double value)
        {
            value = 0;
            if (string.IsNullOrEmpty(token))
                return false;

            int start = 0;
            bool hasMinus = false;

            if (token[0] == '-')
            {
                if (token.Length == 1)
                    return false;
                start = 1;
                hasMinus = true;
            }

            bool hasDecimalPoint = false;
            int integerPart = 0;
            double fractionalPart = 0;
            int fractionalDigits = 0;

            for (int i = start; i < token.Length; i++)
            {
                char c = token[i];

                if (c == '.')
                {
                    if (hasDecimalPoint)
                        return false;
                    hasDecimalPoint = true;
                    continue;
                }

                if (c < '0' || c > '9')
                    return false;

                if (!hasDecimalPoint)
                {
                    integerPart = integerPart * 10 + (c - '0');
                }
                else
                {
                    fractionalPart = fractionalPart * 10 + (c - '0');
                    fractionalDigits++;
                }
            }

            double result = integerPart + fractionalPart / Math.Pow(10, fractionalDigits);
            if (hasMinus)
                result = -result;

            value = result;
            return true;
        }
    }
}