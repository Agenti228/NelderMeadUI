using SimplexUI.Exceptions;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using static System.Windows.Forms.AxHost;

namespace SimplexUI
{
    /// <summary>
    /// Представляет математическую функцию от одной переменной x.
    /// Поддерживает операции +, -, *, /, ^, скобки (), унарный минус,
    /// а также функции: sin, cos, tan, log, sqrt, abs, exp.
    /// </summary>
    public class Function
    {
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

        private static bool IsOperator(string item) => _operatorPriority.ContainsKey(item) && item != "(" && item != ")";
        private static bool IsLetter(char item) => (item >= 'a' && item <= 'z') || (item >= 'A' && item <= 'Z');

        private readonly string _infixExpression;
        private string[] _postfixTokens = []; 
        private readonly List<string> _variables = [];

        public int GetVariablesCount => _variables.Count;
        public bool IsCorrect;
        public string Message;

        public Function(string input)
        {
            _infixExpression = input.Replace(" ", "").ToLower();

            try
            {
                ParseToPostfix();
                IsCorrect = true;
                Message = string.Empty;
            }
            catch (Exception ex) when (ex is EmptyExpressionException or UndefinedParseException or InvalidOperatorLocationException or InvalidVariableOrOperatorException or InvalidParenthesisInputException or InvalidDigitInputException)
            {
                Message = ex.Message;
                IsCorrect = false;
            }
            catch (Exception ex)
            {
                Message = $"Internal error: {ex.Message}";
                IsCorrect = false;
            }
        }

        private void ParseToPostfix()
        {
            var output = new StringBuilder();
            var operatorStack = new Stack<string>();
            int parenthesesDepth = 0;

            if (_infixExpression.Length == 0)
            {
                throw new EmptyExpressionException("Error: Empty exception");
            }

            for (int i = 0; i < _infixExpression.Length; i++)
            {
                char ch = _infixExpression[i];

                if (char.IsDigit(ch))
                {
                    HandleDigit(ref i, ref output);
                }
                else if (ch == '(')
                {
                    HandleOpenParentheses(i, ref operatorStack, ref parenthesesDepth);
                }
                else if (ch == ')')
                {
                    HandleCloseParentheses(ref output, ref operatorStack, ref parenthesesDepth);
                }
                else if (IsLetter(ch) || IsOperator(ch.ToString()))
                {
                    HandleLetters(ref i, ref output, ref operatorStack, parenthesesDepth);
                }
                else
                {
                    throw new UndefinedParseException("Error: Undefined");
                }
            }

            while (operatorStack.Count > 0 && operatorStack.Peek() != "(")
            {
                _ = output.Append('?').Append(operatorStack.Pop());
            }

            string postfixString = output.ToString();
            _postfixTokens = postfixString.Split('?', StringSplitOptions.RemoveEmptyEntries);
            if (parenthesesDepth != 0)
            {
                throw new InvalidParenthesisInputException("Error: Invalid parenthesis");
            }
        }

        private void HandleDigit(ref int index, ref StringBuilder output)
        {
            int start = index;
            while (index < _infixExpression.Length && (char.IsDigit(_infixExpression[index]) || _infixExpression[index] == '.'))
            {
                index++;
            }
            string digit = _infixExpression[start..index];
            if (!IsNumber(digit, out _) || (index < _infixExpression.Length && _infixExpression[index] != ')' && !IsOperator(_infixExpression[index].ToString())))
            {
                throw new InvalidDigitInputException("Error: Invalid digit");
            }
            _ = output.Append(digit);
            index--;
        }

        private void HandleOpenParentheses(int index, ref Stack<string> operatorStack, ref int parenthesesDepth)
        {
            if (index > 0 && (char.IsDigit(_infixExpression[index - 1]) || _infixExpression[index - 1] == 'x'))
            {
                throw new InvalidParenthesisInputException("Error: Invalid parenthesis");
            }
            parenthesesDepth += 10;
            operatorStack.Push(_infixExpression[index].ToString());
        }

        private static void HandleCloseParentheses(ref StringBuilder output, ref Stack<string> operatorStack, ref int parenthesesDepth)
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
        }

        private void HandleLetters(ref int index, ref StringBuilder output, ref Stack<string> operatorStack, int parenthesesDepth)
        {
            char ch = _infixExpression[index];
            string op = ch.ToString();
            if (IsOperator(op))
            {
                if (ch == '-' && (index == 0 || _infixExpression[index - 1] == '('))
                {
                    _ = output.Append('0');

                }
                else if (index == 0 || index + 1 == _infixExpression.Length || (index > 0 && IsOperator(_infixExpression[index - 1].ToString())))
                {
                    throw new InvalidOperatorLocationException("Error: Wrong operator location");
                }
            }
            else if (IsLetter(ch))
            {
                int start = index;
                while (index < _infixExpression.Length && (char.IsLetter(_infixExpression[index]) || char.IsDigit(_infixExpression[index])))
                {
                    index++;
                }
                op = _infixExpression[start..index].ToLower();
                index--;
                if (Regex.IsMatch(op, @"x\d+") || op == "x")
                {
                    _ = output.Append('?');
                    _ = output.Append(op);
                    if (!_variables.Contains(op))
                    {
                        _variables.Add(op);
                    }
                    return;
                }
                else if (!IsOperator(op))
                {
                    throw new InvalidVariableOrOperatorException("Error: Invalid variable or operator");
                }
            }
            else
            {
                throw new InvalidVariableOrOperatorException("Error: Invalid variable or operator");
            }

            _ = output.Append('?');
            int currentPriority = _operatorPriority[op.ToString()] + parenthesesDepth;

            while (operatorStack.Count > 0 && operatorStack.Peek() != "(" &&
                   _operatorPriority[operatorStack.Peek()] + parenthesesDepth >= currentPriority)
            {
                _ = output.Append(operatorStack.Pop()).Append('?');
            }
            operatorStack.Push(op);
        }

        /// <summary>
        /// Внутренний метод вычисления по постфиксной записи
        /// </summary>
        public double Calculate(double[] X)
        {
            double result = double.NaN; 

            if (!IsCorrect || X.Length != _variables.Count)
            {
                return double.NaN;
            }

            var stack = new Stack<double>();

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
                        return double.NaN;
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

                if (Regex.IsMatch(token, @"x\d+") || token == "x")
                {
                    stack.Push(X[_variables.IndexOf(token)]);
                    continue;
                }

                return double.NaN;
            }

            if (stack.Count != 1)
            {
                return double.NaN;
            }

            result = stack.Pop();
            return result;
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