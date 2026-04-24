namespace SimplexUI.Layers
{
    internal class GraphLayer((int width, int height) panelDimentions, Function function) : Layer(panelDimentions)
    {
        public Function Function { get; set; } = function;

        private const float DELTA_Y = 0.05f;
        private const float STEP_X = 2;
        private const int MAX_DIVISIONS = 6;


        public override void Draw(Graphics screen)
        {
            CalculateFunctionPoints();
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
            for (float windowX = 0; windowX <= PanelDimentions.Width; windowX += STEP_X)
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
            if (depth >= MAX_DIVISIONS)
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

            if (deviation > DELTA_Y)
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
}
