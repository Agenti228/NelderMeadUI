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
        private static bool IsOperator(string item) => _operatorPriority.ContainsKey(item) && item != "(" && item != ")";
        private static bool IsLetter(char item) => (item >= 'a' && item <= 'z') || (item >= 'A' && item <= 'Z');
        private static readonly Dictionary<string, int> _operatorPriority = new()
        {
            { "+", 1 },
            { "-", 1 },
            { "*", 2 },
            { "/", 2 },
            { "^", 3 },
            { "(", 0 },
            { ")", 0 },
            { "sin", 4 },
            { "cos", 4 },
            { "tan", 4 },
            { "log", 4 },
            { "sqrt", 4 },
            { "abs", 4 },
            { "exp", 4 }

        };

        private static readonly Dictionary<char, Func<double, double, double>> _binaryOperations = new()
        {
            { '+', (a, b) => a + b },
            { '-', (a, b) => a - b },
            { '*', (a, b) => a * b },
            { '/', (a, b) => a / b },
            { '^', Math.Pow }
        };

        private static readonly Dictionary<string, Func<double, double>> _unaryOperations = new()
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
        private string[] _postfixTokens = []; //Андрей, почини пж. Check if it has some strange interractions

        public bool IsCorrect { get; private set; }

        public Function(string input)
        {
            _infixExpression = input.Replace(" ", "").ToLower();
            IsCorrect = TryParseToPostfix();
        }

        /// <summary>
        /// Преобразование инфиксного выражения в постфиксную запись (алгоритм сортировочной станции)
        /// </summary>
        private bool TryParseToPostfix()
        {
            var output = new StringBuilder();
            var operatorStack = new Stack<string>();
            int parenthesesDepth = 0;

            for (int i = 0; i < _infixExpression.Length; i++)
            {
                char ch = _infixExpression[i];

                if (char.IsDigit(ch) || ch == '.')
                {
                    int start = i;
                    while (i < _infixExpression.Length && (char.IsDigit(_infixExpression[i]) || _infixExpression[i] == '.'))
                    {
                        i++;
                    }
                    _ = output.Append(_infixExpression[start..i]);
                    i--;
                    continue;
                }

                if (ch == '(' || ch == '[')
                {
                    if (i > 0 && (char.IsDigit(_infixExpression[i - 1]) || _infixExpression[i - 1] == 'x'))
                    {
                        return false;
                    }
                    parenthesesDepth += 10;
                    operatorStack.Push(ch.ToString());
                    continue;
                }

                if (ch == ')' || ch == ']')
                {
                    parenthesesDepth -= 10;
                    while (operatorStack.Count > 0 && operatorStack.Peek() != "(")
                    {
                        _ = output.Append('?').Append(operatorStack.Pop());
                    }
                    if (operatorStack.Count > 0 && operatorStack.Peek() == "(")
                    {
                        _ = operatorStack.Pop();
                    }
                    continue;
                }

                if (!IsNumber(ch.ToString(), out _))
                {
                    string op = ch.ToString();
                    if (IsOperator(op))
                    {
                        if ((i > 0 && IsOperator(_infixExpression[i - 1].ToString())))
                        {
                            return false;
                        }
                        if (ch == '-' && (i == 0 || _infixExpression[i - 1] == '(' || _infixExpression[i - 1] == '['))
                        {
                            _ = output.Append('0');
                        }
                    }
                    else if (op == "x")
                    {
                        _ = output.Append('?');
                        _ = output.Append(op);
                        continue;
                    }
                    else if (IsLetter(ch))
                    {
                        int start = i;
                        while (i < _infixExpression.Length && char.IsLetter(_infixExpression[i]))
                        {
                            i++;
                        }
                        op = _infixExpression[start..i].ToLower();
                        i--;
                        if (!IsOperator(op))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }

                    _ = output.Append('?');
                    int currentPriority = _operatorPriority[op.ToString()] + parenthesesDepth;

                    while (operatorStack.Count > 0 && operatorStack.Peek() != "(" &&
                           _operatorPriority[operatorStack.Peek()] + parenthesesDepth >= currentPriority)
                    {
                        _ = output.Append(operatorStack.Pop()).Append('?');
                    }
                    operatorStack.Push(op);
                    continue;
                }
            }

            while (operatorStack.Count > 0)
            {
                _ = output.Append('?').Append(operatorStack.Pop());
            }

            string postfixString = output.ToString();
            _postfixTokens = postfixString.Split('?', StringSplitOptions.RemoveEmptyEntries);

            if (parenthesesDepth == 0)
            {
                return true;
            }
            return false;
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

            List<string> variables = [];

            foreach (string token in _postfixTokens)
            {
                if (IsNumber(token, out double number))
                {
                    stack.Push(number);
                    continue;
                }

                if (token.Length == 1 && _binaryOperations.ContainsKey(token[0]))
                {
                    if (stack.Count < 2)
                    {
                        result = double.NaN;
                        return false;
                    }

                    double b = stack.Pop();
                    double a = stack.Pop();
                    char op = token[0];

                    double value = _binaryOperations[op](a, b);
                    stack.Push(value);
                    continue;
                }

                if (_unaryOperations.ContainsKey(token))
                {
                    double a = stack.Pop();
                    double funcResult = _unaryOperations[token](a);
                    stack.Push(funcResult);
                    continue;
                }

                if (token == "x")
                {
                    stack.Push(x);
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
            {
                return false;
            }

            int start = 0;
            bool hasMinus = false;

            if (token[0] == '-')
            {
                if (token.Length == 1)
                {
                    return false;
                }

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
                    {
                        return false;
                    }

                    hasDecimalPoint = true;
                    continue;
                }

                if (c < '0' || c > '9')
                {
                    return false;
                }

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
            {
                result = -result;
            }

            value = result;
            return true;
        }
    }
}