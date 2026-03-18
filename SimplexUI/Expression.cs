using System;
using System.Collections.Generic;
using System.Text;

namespace SimplexUI
{
    /// <summary>
    /// Представляет математическую функцию от одной переменной x.
    /// Поддерживает операции +, -, *, /, ^, скобки (), унарный минус,
    /// а также функции: sin, cos, tan, log, sqrt, abs, exp.
    /// Выражение может содержать пробелы.
    /// </summary>
    public class Function
    {
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
        /// <summary>
        /// 
        /// </summary>
        private readonly string _infixExpression;

        private string[] _postfixTokens;

        /// <summary>
        /// Признак корректности выражения.
        /// true, если выражение успешно разобрано и может быть вычислено.
        /// </summary>
        public bool IsCorrect { get; private set; }

        /// <summary>
        /// Создаёт функцию по заданному строковому выражению.
        /// </summary>
        /// <param name="input">Строка с математическим выражением (переменная обозначается буквой 'x')</param>
        public Function(string input)
        {
            _infixExpression = input.Replace(" ", "").ToLower();
            ParseToPostfix();
        }
        /// <summary>
        /// Проверяет, является ли символ бинарным оператором.
        /// </summary>
        private static bool IsOperator(char ch) => OperatorPriority.ContainsKey(ch) && ch != '(' && ch != ')';

        /// <summary>
        /// Преобразует инфиксное выражение в постфиксную запись (алгоритм сортировочной станции).
        /// Результат сохраняется в _postfixTokens в виде массива токенов.
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
        /// Пытается вычислить значение функции при заданном x.
        /// </summary>
        /// <param name="x">Значение переменной</param>
        /// <param name="result">Результат вычисления (double.NaN при ошибке)</param>
        /// <returns>true, если вычисление успешно, иначе false</returns>
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
        /// Внутренний метод вычисления по постфиксной записи с использованием стека значений.
        /// </summary>
        private bool TryCalculateInternal(double x, out double result)
        {
            var stack = new Stack<double>();

            foreach (string token in _postfixTokens)
            {
                // Если токен - число
                if (double.TryParse(token, out double number))
                {
                    stack.Push(number);
                }
                // Если токен - бинарный оператор
                else if (token.Length == 1 && BinaryOps.ContainsKey(token[0]))
                {
                    if (stack.Count < 2)
                    {
                        result = double.NaN;
                        return false;
                    }
                    double b = stack.Pop();
                    double a = stack.Pop();
                    try
                    {
                        double value = BinaryOps[token[0]](a, b);
                        stack.Push(value);
                    }
                    catch
                    {
                        result = double.NaN;
                        return false;
                    }
                }
                // Если токен - переменная 'x'
                else if (token == "x")
                {
                    stack.Push(x);
                }
                else if (token.Contains('[') && token.EndsWith(']'))
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
                    try
                    {
                        double value = UnaryOps[funcName](argValue);
                        stack.Push(value);
                    }
                    catch
                    {
                        result = double.NaN;
                        return false;
                    }
                }
                else
                {
                    result = double.NaN;
                    return false;
                }
            }

            if (stack.Count != 1)
            {
                result = double.NaN;
                return false;
            }

            result = stack.Pop();
            return true;
        }
    }
}