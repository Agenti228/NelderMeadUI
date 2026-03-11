using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nelder_Mead_method
{
    struct Sign
    {
        public char Value;
        public int Priority;
        public Sign(char value, int priority) { Value = value; Priority = priority; }
    }

    internal class myFunc
    {
        private List<Sign> signsList { get; set; }
        private Dictionary<char, Func<double, double, double>> binaryFunctions { get; set; }
        private Dictionary<string, Func<double, double>> unaryFunctions { get; set; }
        private string expression { get; set; }
        public bool isCorrect { get; set; }
        private string[] postfixExpression { get; set; }

        public myFunc(string input)
        {
            Initialize();
            expression = input;
            PostfixParse();

        }

        private void Initialize()
        {
            isCorrect = false;
            signsList = new List<Sign>()
            {
                new Sign('+', 1),
                new Sign('-', 1),
                new Sign('*', 2),
                new Sign('/', 2),
                new Sign('(', 0),
                new Sign(')', 0)
            };
            binaryFunctions = new Dictionary<char, Func<double, double, double>>
            {
                { '+', (a, b) => a + b },
                { '-', (a, b) => a - b },
                { '*', (a, b) => a * b },
                { '/', (a, b) => a / b }
            };
            unaryFunctions = new Dictionary<string, Func<double, double>>
            {
                { "sin", a => Math.Sin(a) },
                { "cos", a => Math.Cos(a) },
                { "tan", a => Math.Tan(a) },
                { "log", a => Math.Log(a) },
                { "sqrt", a => Math.Sqrt(a) },
                { "abs", a => Math.Abs(a) }
            };
        }

        private void PostfixParse()
        {
            int signStackTop = -1, argumentFlag = 0, priorityCorrector = 0;
            List<Sign> signStack = new List<Sign>();
            Sign currentSign;
            string notParsedExpression = "";

            expression = expression.Replace(" ", "");
            expression = expression.ToLower();

            for (int i = 0; i < expression.Length; i++)
            {
                if (expression[i] == '(') priorityCorrector += 10;
                else if (expression[i] == ')') priorityCorrector -= 10;
                else
                {
                    if (expression[i] == '[') argumentFlag++;
                    else if (expression[i] == ']') argumentFlag--;
                    if (argumentFlag < 1 && checkSignOrNot(expression[i], priorityCorrector, out currentSign))
                    {
                        notParsedExpression += "?";
                        if (!(signStackTop == -1 || signStack[signStackTop].Priority < currentSign.Priority))
                        {
                            for (int j = signStackTop; j > -1; j--)
                            {
                                notParsedExpression += signStack[j].Value + "?";
                                signStack.RemoveAt(j);
                                signStackTop--;
                            }
                        }
                        signStack.Add(currentSign);
                        signStackTop++;
                    }
                    else notParsedExpression += expression[i];
                }
            }
            if (signStackTop > -1) for (int j = signStackTop; j > -1; j--)
            {
                notParsedExpression += "?" + signStack[j].Value;
                signStack.RemoveAt(j);
                signStackTop--;
            }
            postfixExpression = notParsedExpression.Split('?');
            if (argumentFlag == 0 && priorityCorrector == 0) isCorrect = checkRelevance();
            else isCorrect = false;
        }

        public double Calculate(double x)
        {
            if (!isCorrect) return 0;

            HashSet<string> validFunctions = new HashSet<string> { "sin", "cos", "tan", "log", "sqrt", "abs" };

            int indexCorrector = 0;
            Sign currentOperator;
            double[] result = new double[postfixExpression.Length];

            for (int i = 0; i < postfixExpression.Length; i++)
            {
                if (double.TryParse(postfixExpression[i], out result[i + indexCorrector])) continue;
                else if (postfixExpression[i].Length == 1 && checkSignOrNot(postfixExpression[i][0], 0, out currentOperator))
                {
                    if (binaryFunctions.TryGetValue(currentOperator.Value, out var operation))
                    {
                        indexCorrector -= 2;
                        result[i + indexCorrector] = operation(result[i + indexCorrector], result[i + indexCorrector + 1]);
                    }
                }
                else if (postfixExpression[i].Length == 1 && char.IsLetter(postfixExpression[i][0])) result[i + indexCorrector] = x; //переменные!!!
                else if (postfixExpression[i].Contains('[') &&
                    validFunctions.Contains(postfixExpression[i].Substring(0, postfixExpression[i].IndexOf('['))))
                {
                    string arg = postfixExpression[i].Substring(postfixExpression[i].IndexOf('[') + 1,
                       postfixExpression[i].LastIndexOf(']') - postfixExpression[i].IndexOf('[') - 1);
                    myFunc argument = new myFunc(arg);
                    result[i + indexCorrector] = argument.Calculate(x);
                    if (unaryFunctions.TryGetValue(postfixExpression[i].Substring(0, postfixExpression[i].IndexOf('[')), out var operation))
                    {
                        result[i + indexCorrector] = operation(result[i + indexCorrector]);
                    }
                }
                else return 0;
            }
            return result[0];
        }

        private bool checkSignOrNot(char c, int priorityCorrector, out Sign op)
        {
            foreach (Sign s in signsList) if (s.Value == c)
            {
                op = new Sign(s.Value, s.Priority + priorityCorrector);
                return true;
            }
            op = new Sign(signsList[0].Value, signsList[0].Priority + priorityCorrector);
            return false;
        }

        private bool checkRelevance()
        {
            HashSet<string> validFunctions = new HashSet<string> { "sin", "cos", "tan", "log", "sqrt", "abs" };
            Sign currentOperator;

            for (int i = 0; i < postfixExpression.Length; i++)
            {
                if (double.TryParse(postfixExpression[i], out _)) continue;
                else if (postfixExpression[i].Length == 1 && checkSignOrNot(postfixExpression[i][0], 0, out currentOperator) 
                    && binaryFunctions.TryGetValue(currentOperator.Value, out _)) continue;
                else if (postfixExpression[i].Length == 1 && char.IsLetter(postfixExpression[i][0])) continue;
               // else if (char.IsLetter(postfixExpression[i][0]) && double.TryParse(postfixExpression[i].Substring(1), out _)) continue;
                else if (postfixExpression[i].Contains('[') &&
                    validFunctions.Contains(postfixExpression[i].Substring(0, postfixExpression[i].IndexOf('['))))
                {
                    string arg = postfixExpression[i].Substring(postfixExpression[i].IndexOf('[') + 1,
                       postfixExpression[i].LastIndexOf(']') - postfixExpression[i].IndexOf('[') - 1);
                    myFunc argument = new myFunc(arg);
                    if (unaryFunctions.TryGetValue(postfixExpression[i].Substring(0, postfixExpression[i].IndexOf('[')), out _) && argument.isCorrect) continue;
                }
                else return false;
            }
            return true;
        }
    }
}
