namespace SimplexUI.Layers
{
    public enum LayerNames
    {
        Graph,
        Axis,
        Simplex
    }

    internal abstract class Layer((int width, int height) panelDimentions)
    {
        private const float DEFAULT_SCALE = 40;
        public (int Width, int Height) PanelDimentions { get; set; } = panelDimentions;
        protected List<PointF> WindowPoints { get; set; } = [];
        public static float Scale { get; set; } = DEFAULT_SCALE;
        public static PointF Center { get; set; } = default;

        public static void ResetView()
        {
            Scale = DEFAULT_SCALE;
            Center = default;
        }

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
}
