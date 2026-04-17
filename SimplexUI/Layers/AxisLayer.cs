namespace SimplexUI.Layers
{
    internal class AxisLayer((int width, int height) panelDimentions) : Layer(panelDimentions)
    {
        internal interface IAxis
        {
            public PointF AxisStartWindow { get; }
            public PointF AxisEndWindow { get; }

            public abstract void DrawAxis(Graphics screen);
            public abstract void DrawMarks(Graphics screen);
        }

        internal readonly struct AxisX(PointF axisStart, PointF axisEnd) : IAxis
        {
            public PointF AxisStartWindow { get; } = axisStart;
            public PointF AxisEndWindow { get; } = axisEnd;

            public readonly void DrawAxis(Graphics screen)
            {
                screen.DrawLine(Pens.Black, 0, Center.Y, AxisEndWindow.X, Center.Y);
            }

            public readonly void DrawMarks(Graphics screen)
            {
                float minValue = ConvertWindowToGraph(AxisStartWindow).X;
                float maxValue = ConvertWindowToGraph(AxisEndWindow).X;

                if (minValue >= maxValue)
                {
                    return;
                }

                float firstMark = MathF.Ceiling(minValue);

                var markSize = new Size(1, 6);

                for (float value = firstMark; value <= maxValue; value++)
                {
                    float windowCoordinates = ValueToWindow(value);
                    if (windowCoordinates < 0 || windowCoordinates > AxisEndWindow.X || float.IsNaN(windowCoordinates))
                    {
                        continue;
                    }

                    screen.DrawLine(Pens.Red, windowCoordinates, Center.Y - markSize.Height / 2, windowCoordinates, Center.Y + markSize.Height / 2);

                    string label = value.ToString();
                    SizeF labelSize = screen.MeasureString(label, SystemFonts.DefaultFont);
                    screen.DrawString(label, SystemFonts.DefaultFont, Brushes.Black, windowCoordinates - labelSize.Width / 2, Center.Y + markSize.Height);
                }

                static float ValueToWindow(float x)
                {
                    return ConvertGraphToWindow(new PointF(x, 0)).X;
                }
            }
        }

        internal readonly struct AxisY(PointF axisStart, PointF axisEnd) : IAxis
        {
            public PointF AxisStartWindow { get; } = axisStart;
            public PointF AxisEndWindow { get; } = axisEnd;

            public readonly void DrawAxis(Graphics screen)
            {
                screen.DrawLine(Pens.Black, Center.X, 0, Center.X, AxisEndWindow.Y);
            }

            public readonly void DrawMarks(Graphics screen)
            {
                float minValue = ConvertWindowToGraph(AxisEndWindow).Y;
                float maxValue = ConvertWindowToGraph(AxisStartWindow).Y;

                if (minValue >= maxValue)
                {
                    return;
                }

                float firstMark = MathF.Ceiling(minValue);

                var markSize = new Size(1, 6);

                for (float value = firstMark; value <= maxValue; value++)
                {
                    float windowCoordinates = ValueToWindow(value);
                    if (windowCoordinates < 0 || windowCoordinates > AxisEndWindow.Y || float.IsNaN(windowCoordinates))
                    {
                        continue;
                    }

                    screen.DrawLine(Pens.Red, Center.X - markSize.Height / 2, windowCoordinates, Center.X + markSize.Height / 2, windowCoordinates);

                    string label = value.ToString();
                    SizeF labelSize = screen.MeasureString(label, SystemFonts.DefaultFont);
                    screen.DrawString(label, SystemFonts.DefaultFont, Brushes.Black, Center.X - (markSize.Height + labelSize.Width), windowCoordinates - labelSize.Height / 2);
                }

                static float ValueToWindow(float y)
                {
                    return ConvertGraphToWindow(new PointF(0, y)).Y;
                }
            }
        }

        private readonly AxisX _axisX = new(new PointF(0, panelDimentions.height / 2), new PointF(panelDimentions.width, panelDimentions.height / 2));
        private readonly AxisY _axisY = new(new PointF(panelDimentions.width / 2, 0), new PointF(panelDimentions.width / 2, panelDimentions.height));

        public override void Draw(Graphics screen)
        {
            DrawCoordinateAxes(screen);

            DrawAxisMarksX(screen);
            DrawAxisMarksY(screen);
        }

        private void DrawCoordinateAxes(Graphics screen)
        {
            screen.DrawLine(Pens.Black, Center.X, 0, Center.X, PanelDimentions.Height);
            screen.DrawLine(Pens.Black, 0, Center.Y, PanelDimentions.Width, Center.Y);
        }

        private void DrawAxisMarksX(Graphics screen)
        {
            float minValue = ConvertWindowToGraph(new PointF(0, 0)).X;
            float maxValue = ConvertWindowToGraph(new PointF(PanelDimentions.Width, 0)).X;

            if (minValue >= maxValue)
            {
                return;
            }

            float firstMark = MathF.Ceiling(minValue);

            var markSize = new Size(1, 6);
            var textFont = SystemFonts.DefaultFont;

            for (float value = firstMark; value <= maxValue; value++)
            {
                float windowCoordinates = ValueToWindow(value);
                if (windowCoordinates < 0 || windowCoordinates > PanelDimentions.Width || float.IsNaN(windowCoordinates))
                {
                    continue;
                }

                screen.DrawLine(Pens.Red, windowCoordinates, Center.Y - markSize.Height / 2, windowCoordinates, Center.Y + markSize.Height / 2);

                string label = value.ToString();
                SizeF labelSize = screen.MeasureString(label, textFont);

                if (value == 0)
                {
                    screen.DrawString(label, textFont, Brushes.Black, windowCoordinates - (markSize.Height + labelSize.Width), Center.Y + markSize.Height);
                }
                else
                {
                    screen.DrawString(label, textFont, Brushes.Black, windowCoordinates - labelSize.Width / 2, Center.Y + markSize.Height);
                }
            }

            static float ValueToWindow(float x)
            {
                return ConvertGraphToWindow(new PointF(x, 0)).X;
            }
        }

        private void DrawAxisMarksY(Graphics screen)
        {
            float minValue = ConvertWindowToGraph(new PointF(0, PanelDimentions.Height)).Y;
            float maxValue = ConvertWindowToGraph(new PointF(0, 0)).Y;

            if (minValue >= maxValue)
            {
                return;
            }

            float firstMark = MathF.Ceiling(minValue);

            var markSize = new Size(1, 6);
            var textFont = SystemFonts.DefaultFont;

            for (float value = firstMark; value <= maxValue; value++)
            {
                float windowCoordinates = ValueToWindow(value);
                if (windowCoordinates < 0 || windowCoordinates > PanelDimentions.Height || float.IsNaN(windowCoordinates))
                {
                    continue;
                }

                screen.DrawLine(Pens.Red, Center.X - markSize.Height / 2, windowCoordinates, Center.X + markSize.Height / 2, windowCoordinates);

                string label = value.ToString();
                SizeF labelSize = screen.MeasureString(label, textFont);

                if (value != 0)
                {
                    screen.DrawString(label, textFont, Brushes.Black, Center.X - (markSize.Height + labelSize.Width), windowCoordinates - labelSize.Height / 2);
                }
            }

            static float ValueToWindow(float y)
            {
                return ConvertGraphToWindow(new PointF(0, y)).Y;
            }
        }
    }
}
