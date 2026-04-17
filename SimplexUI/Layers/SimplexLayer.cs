namespace SimplexUI.Layers
{
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
                    PointF firstPoint = ConvertGraphToWindow(new PointF((float)Simplexes[i][j - 1][0], (float)Simplexes[i][j - 1].Value));
                    PointF secondPoint = ConvertGraphToWindow(new PointF((float)Simplexes[i][j][0], (float)Simplexes[i][j].Value));

                    if (!IsValidPoint(firstPoint) || !IsValidPoint(secondPoint))
                    {
                        continue;
                    }

                    if (firstPoint.X < 0 || secondPoint.X < 0 || firstPoint.X > PanelDimentions.Width || secondPoint.X > PanelDimentions.Width)
                    {
                        continue;
                    }

                    if (firstPoint.Y < 0 || secondPoint.Y < 0 || firstPoint.Y > PanelDimentions.Height || secondPoint.Y > PanelDimentions.Height)
                    {
                        continue;
                    }

                    screen.DrawLine(Pens.BlueViolet, firstPoint, secondPoint);
                    screen.DrawRectangle(Pens.Blue, firstPoint.X - 1, firstPoint.Y - 1, 2, 2);
                    screen.DrawRectangle(Pens.Blue, secondPoint.X - 1, secondPoint.Y - 1, 2, 2);
                }
            }
        }
    }
}
