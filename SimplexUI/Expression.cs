namespace SimplexUI
{
    public class Function
    {
        struct Sign(char value, int priority)
        {
            public char Value = value;
            public int Priority = priority;
        }

        private List<Sign> SignsList { get; set; } = [];
        private Dictionary<char, Func<double, double, double>> BinaryFunctions { get; set; } = [];
        private Dictionary<string, Func<double, double>> UnaryFunctions { get; set; } = [];
        private string Expression { get; set; }
        public bool IsCorrect { get; set; }
        private string[] PostfixExpression { get; set; } = [];
        private static HashSet<string> ValidFunctions => ["sin", "cos", "tan", "log", "sqrt", "abs", "exp"];

        public Function(string input)
        {
            Initialize();
            Expression = input;
            PostfixParse();
        }

        private void Initialize()
        {
            IsCorrect = true;
            SignsList =
            [
                new Sign('+', 1),
                new Sign('-', 1),
                new Sign('*', 2),
                new Sign('/', 2),
                new Sign('^', 3),
                new Sign('(', 0),
                new Sign(')', 0)
            ];
            BinaryFunctions = new Dictionary<char, Func<double, double, double>>
            {
                { '+', (a, b) => a + b },
                { '-', (a, b) => a - b },
                { '*', (a, b) => a * b },
                { '/', (a, b) => a / b },
                { '^', Math.Pow }
            };
            UnaryFunctions = new Dictionary<string, Func<double, double>>
            {
                { "sin", Math.Sin },
                { "cos", Math.Cos },
                { "tan", Math.Tan },
                { "log", Math.Log },
                { "sqrt", Math.Sqrt },
                { "abs", Math.Abs },
                { "exp", Math.Exp }
            };
        }

        /// <summary>
        /// Блять почему если я меняю "?" на '?' все нахуй ломается
        /// </summary>
        private void PostfixParse()
        {
            int signStackTop = -1, argumentFlag = 0, priorityCorrector = 0;
            List<Sign> signStack = [];
            string notParsedExpression = string.Empty;

            Expression = Expression.Replace(" ", "");
            Expression = Expression.ToLower();

            for (int i = 0; i < Expression.Length; i++)
            {
                if (Expression[i] == '(')
                {
                    priorityCorrector += 10;
                }
                else if (Expression[i] == ')')
                {
                    priorityCorrector -= 10;
                }
                else if(Expression[i] == '[')
                {
                    argumentFlag++;
                    notParsedExpression += Expression[i];
                }
                else if (Expression[i] == ']')
                {
                    argumentFlag--;
                    notParsedExpression += Expression[i];
                }
                else if (argumentFlag < 1 && CheckSignOrNot(Expression[i], priorityCorrector, out Sign currentSign))
                {
                    if (currentSign.Value == '-' && (i == 0 || Expression[i - 1] == '(' || Expression[i - 1] == '['))
                    {
                        notParsedExpression += '0';
                    }

                    notParsedExpression += "?";

                    if (signStackTop == -1 || signStack[signStackTop].Priority < currentSign.Priority)
                    {
                        signStack.Add(currentSign);
                        signStackTop++;
                        continue;
                    }

                    for (int j = signStackTop; j > -1; j--)
                    {
                        notParsedExpression += signStack[j].Value + "?";
                        signStack.RemoveAt(j);
                        signStackTop--;
                    }

                    signStack.Add(currentSign);
                    signStackTop++;
                }
                else
                {
                    notParsedExpression += Expression[i];
                }
            }

            if (signStackTop > -1)
            {
                for (int j = signStackTop; j > -1; j--)
                {
                    notParsedExpression += "?" + signStack[j].Value;
                    signStack.RemoveAt(j);
                    signStackTop--;
                }
            }

            PostfixExpression = notParsedExpression.Split('?');
            if (argumentFlag == 0 && priorityCorrector == 0)
            {
                IsCorrect = TryCalculate(0, out _);
            }
            else
            {
                IsCorrect = false;
            }
        }

        public bool TryCalculate(double x, out double result)
        {
            if (!IsCorrect)
            {
                result = double.NaN;
                return false;
            }

            int indexCorrector = 0;
            double[] calculatingExpression = new double[PostfixExpression.Length];
            result = double.NaN;

            for (int i = 0; i < PostfixExpression.Length; i++)
            {
                if (double.TryParse(PostfixExpression[i], out calculatingExpression[i + indexCorrector]))
                {
                    continue;
                }

                if (CheckSignOrNot(PostfixExpression[i], 0, out Sign currentOperator))
                {
                    if (!BinaryFunctions.TryGetValue(currentOperator.Value, out var operation))
                    {
                        continue;
                    }
                    
                    indexCorrector -= 2;
                    calculatingExpression[i + indexCorrector] = operation(calculatingExpression[i + indexCorrector], calculatingExpression[i + indexCorrector + 1]);
                    continue;
                }

                if (PostfixExpression[i].Length == 1 && char.IsLetter(PostfixExpression[i][0]))
                {
                    calculatingExpression[i + indexCorrector] = x;
                    continue;
                }

                if (PostfixExpression[i].Contains('[') && ValidFunctions.Contains(PostfixExpression[i][..PostfixExpression[i].IndexOf('[')]))
                {
                    string arg = PostfixExpression[i].Substring(PostfixExpression[i].IndexOf('[') + 1, PostfixExpression[i].LastIndexOf(']') - PostfixExpression[i].IndexOf('[') - 1);
                    var argument = new Function(arg);
                    if (UnaryFunctions.TryGetValue(PostfixExpression[i][..PostfixExpression[i].IndexOf('[')], out var operation) && argument.TryCalculate(x, out calculatingExpression[i + indexCorrector]))
                    {
                        calculatingExpression[i + indexCorrector] = operation(calculatingExpression[i + indexCorrector]);
                        continue;
                    }
                    return false;
                }

                return false;
            }
            result = calculatingExpression[0];
            return true;
        }

        private bool CheckSignOrNot(string unit, int priorityCorrector, out Sign operation)
        {
            operation = new Sign(SignsList[0].Value, SignsList[0].Priority + priorityCorrector);

            if (unit.Length == 1)
            {
                foreach (Sign s in SignsList)
                {
                    if (s.Value == unit[0])
                    {
                        operation = new Sign(s.Value, s.Priority + priorityCorrector);
                        return true;
                    }
                }
            }

            return false;
        }

        private bool CheckSignOrNot(char unit, int priorityCorrector, out Sign operation)
        {
            operation = new Sign(SignsList[0].Value, SignsList[0].Priority + priorityCorrector);

            foreach (Sign s in SignsList)
            {
                if (s.Value == unit)
                {
                    operation = new Sign(s.Value, s.Priority + priorityCorrector);
                    return true;
                }
            }

            return false;
        }
    }
}
