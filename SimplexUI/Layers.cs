namespace SimplexUI
{
    public enum Layers
    {
        Graph,
        Axis,
        Simplex
    }

    internal abstract class Layer((int width, int height) panelDimentions)
    {
        public (int Width, int Height) PanelDimentions { get; set; } = panelDimentions;
        protected List<PointF> WindowPoints { get; set; } = [];
        public static float Scale { get; set; } = 40;
        public static PointF Center { get; set; } = default;
        public static PointF Offset { get; set; } = default;

        public abstract void Draw(Graphics screen);

        public static bool IsValidPoint(PointF graphPoint)
        {
            return !float.IsNaN(graphPoint.Y);
        }

        public static PointF ConvertWindowToGraph(PointF point)
        {
            if (point.Y > Center.Y)
            {
                point.Y = -Math.Abs(point.Y - Center.Y) / Scale;
            }
            else
            {
                point.Y = Math.Abs(Center.Y - point.Y) / Scale;
            }

            if (point.X > Center.X)
            {
                point.X = Math.Abs(point.X - Center.X) / Scale;
            }
            else
            {
                point.X = -Math.Abs(Center.X - point.X) / Scale;
            }

            return point;
        }

        public static PointF ConvertGraphToWindow(PointF point)
        {
            point.X = (Scale * point.X) + Center.X;
            point.Y = (-Scale * point.Y) + Center.Y;
            return point;
        }
    }

    internal class GraphLayer((int width, int height) panelDimentions, Function function) : Layer(panelDimentions)
    {
        public Function Function { get; set; } = function;

        private const float _deltaY = 0.05f;
        private const float _stepX = 2;
        private const int _maxDivisions = 6;


        public override void Draw(Graphics screen)
        {
            CalculateFunctionPoints(); // move to some Update method
            DrawFunctionPoints(screen);
        }

        private void CalculateFunctionPoints()
        {
            if (!Function.IsCorrect)
            {
                return;
            }

            WindowPoints.Clear();

            List<PointF> baseWindowPoints = [];
            for (float windowX = 0; windowX <= PanelDimentions.Width; windowX += _stepX)
            {
                PointF graphPoint = GetPointAsGraph(windowX);
                if (IsValidPoint(graphPoint))
                {
                    baseWindowPoints.Add(ConvertGraphToWindow(graphPoint));
                }
            }

            if (baseWindowPoints.Count < 2)
            {
                return;
            }

            WindowPoints.Add(baseWindowPoints[0]);
            for (int i = 0; i < baseWindowPoints.Count - 1; i++)
            {
                PointF leftWindow = baseWindowPoints[i];
                PointF rightWindow = baseWindowPoints[i + 1];

                PointF leftGraph = ConvertWindowToGraph(leftWindow);
                PointF rightGraph = ConvertWindowToGraph(rightWindow);

                RefineInterval(leftGraph, rightGraph, leftWindow, rightWindow, 0);

                WindowPoints.Add(rightWindow);
            }
        }

        private void RefineInterval(PointF leftGraph, PointF rightGraph, PointF leftWindow, PointF rightWindow, int depth)
        {
            if (depth >= _maxDivisions)
            {
                return;
            }

            float middleWindowX = (leftWindow.X + rightWindow.X) / 2f;
            PointF middleGraph = GetPointAsGraph(middleWindowX);

            if (!IsValidPoint(middleGraph))
            {
                return;
            }

            float interpolatedY = (leftGraph.Y + rightGraph.Y) / 2f;
            float deviation = Math.Abs(middleGraph.Y - interpolatedY);

            if (deviation > _deltaY)
            {
                RefineInterval(leftGraph, middleGraph, leftWindow, ConvertGraphToWindow(middleGraph), depth + 1);
                WindowPoints.Add(ConvertGraphToWindow(middleGraph));
                RefineInterval(middleGraph, rightGraph, ConvertGraphToWindow(middleGraph), rightWindow, depth + 1);
            }
        }

        private void DrawFunctionPoints(Graphics screen)
        {
            for (int i = 0; i < WindowPoints.Count - 1; i++)
            {
                PointF currentWindowPoint = WindowPoints[i];
                PointF nextWindowPoint = WindowPoints[i + 1];

                if (float.IsNaN(currentWindowPoint.Y))
                {
                    return;
                }

                if (currentWindowPoint.Y < -PanelDimentions.Height || currentWindowPoint.Y > PanelDimentions.Height * 2)
                {
                    continue;
                }

                screen.DrawLine(Pens.Gray, currentWindowPoint, nextWindowPoint);
                //screen.DrawRectangle(Pens.Blue, currentWindowPoint.X - 2, currentWindowPoint.Y - 2, 4, 4);
            }
        }


        private PointF GetPointAsGraph(float windowX)
        {
            PointF graphPoint = ConvertWindowToGraph(new PointF(windowX, 0));
            if (Function.TryCalculate(graphPoint.X, out double value))
            {
                return new PointF(graphPoint.X, (float)value);
            }

            return new PointF(graphPoint.X, float.NaN);
        }
    }

    internal class AxisLayer((int width, int height) panelDimentions) : Layer(panelDimentions)
    {
        public override void Draw(Graphics screen)
        {
            DrawCoordinateAxes(screen);

            DrawAxisMarks(screen, true);
            DrawAxisMarks(screen, false);
        }

        private void DrawCoordinateAxes(Graphics screen)
        {
            screen.DrawLine(Pens.Black, Center.X, 0, Center.X, PanelDimentions.Height);
            screen.DrawLine(Pens.Black, 0, Center.Y, PanelDimentions.Width, Center.Y);
        }

        private void DrawAxisMarks(Graphics screen, bool isXAxis)
        {
            float minValue;
            float maxValue;
            float axisCoordinate;
            Func<float, float> valueToWindow;

            if (isXAxis)
            {
                minValue = ConvertWindowToGraph(new PointF(0, 0)).X;
                maxValue = ConvertWindowToGraph(new PointF(PanelDimentions.Width, 0)).X;
                axisCoordinate = Center.Y;
                valueToWindow = x => ConvertGraphToWindow(new PointF(x, 0)).X;
            }
            else
            {
                minValue = ConvertWindowToGraph(new PointF(0, PanelDimentions.Height)).Y;
                maxValue = ConvertWindowToGraph(new PointF(0, 0)).Y;
                axisCoordinate = Center.X;
                valueToWindow = y => ConvertGraphToWindow(new PointF(0, y)).Y;
            }

            if (minValue >= maxValue)
            {
                return;
            }

            float firstMark = MathF.Ceiling(minValue);

            var markSize = new Size(1, 6);
            var markPen = Pens.Red;
            var textFont = SystemFonts.DefaultFont;
            var textBrush = Brushes.Black;

            for (float value = firstMark; value <= maxValue; value++)
            {
                float windowCoordinates = valueToWindow(value);
                if (windowCoordinates < 0 || windowCoordinates > (isXAxis ? PanelDimentions.Width : PanelDimentions.Height) || float.IsNaN(windowCoordinates))
                {
                    continue;
                }

                if (value == 0)
                {
                    if (isXAxis)
                    {
                        string label = value.ToString();
                        SizeF labelSize = screen.MeasureString(label, textFont);
                        screen.DrawString(label, textFont, textBrush, windowCoordinates - (markSize.Height + labelSize.Width), axisCoordinate + markSize.Height);
                    }

                    continue;
                }

                if (isXAxis)
                {
                    screen.DrawLine(markPen, windowCoordinates, axisCoordinate - markSize.Height / 2, windowCoordinates, axisCoordinate + markSize.Height / 2);

                    string label = value.ToString();
                    SizeF labelSize = screen.MeasureString(label, textFont);
                    screen.DrawString(label, textFont, textBrush, windowCoordinates - labelSize.Width / 2, axisCoordinate + markSize.Height);
                }
                else
                {
                    screen.DrawLine(markPen, axisCoordinate - markSize.Height / 2, windowCoordinates, axisCoordinate + markSize.Height / 2, windowCoordinates);

                    string label = value.ToString();
                    SizeF labelSize = screen.MeasureString(label, textFont);
                    screen.DrawString(label, textFont, textBrush, axisCoordinate - (markSize.Height + labelSize.Width), windowCoordinates - labelSize.Height / 2);
                }
            }
        }
    }

    internal class SimplexLayer((int width, int height) panelDimentions, Function function) : Layer(panelDimentions)
    {
        public Function Function { get; set; } = function;
        public List<EvaluateableVector[]> Simplexes { get; set; } = [];

        public override void Draw(Graphics screen)
        {
            DrawSimplex(screen);
        }

        private void DrawSimplex(Graphics screen)
        {
            if (Function?.IsCorrect ?? false)
            {
                return;
            }

            for (int i = 0; i < Simplexes.Count; i++)
            {
                for (int j = 1; j < Simplexes[i].Length; j++)
                {
                    PointF frstPnt = ConvertGraphToWindow(new PointF((float)Simplexes[i][j - 1][0], (float)Simplexes[i][j - 1].Value));
                    PointF scndPnt = ConvertGraphToWindow(new PointF((float)Simplexes[i][j][0], (float)Simplexes[i][j].Value));
                    if (frstPnt.X < 0 || scndPnt.X < 0 || frstPnt.X > PanelDimentions.Width || scndPnt.X > PanelDimentions.Width)
                    {
                        continue;
                    }

                    if (frstPnt.Y < 0 || scndPnt.Y < 0 || frstPnt.Y > PanelDimentions.Height || scndPnt.Y > PanelDimentions.Height)
                    {
                        continue;
                    }

                    screen.DrawLine(Pens.BlueViolet, frstPnt, scndPnt);
                    screen.DrawRectangle(Pens.Blue, frstPnt.X - 1, frstPnt.Y - 1, 2, 2);
                    screen.DrawRectangle(Pens.Blue, scndPnt.X - 1, scndPnt.Y - 1, 2, 2);
                }
            }
        }
    }
}
