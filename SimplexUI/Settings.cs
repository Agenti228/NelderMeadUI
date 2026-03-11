namespace SimplexUI
{
    public struct Settings
    {
        public double Step { get; set; }
        public int MaxIterations { get; set; }
        public double Reflection { get; set; }
        public double Compression { get; set; }
        public double Stretching { get; set; }
        public double Reduction { get; set; }

        public Settings()
        {
            Reflection = 1;
            Compression = 0.5;
            Stretching = 2;
            Reduction = 0.5;
        }

        public Settings(double step, int maxIterations)
        {
            Step = step;
            MaxIterations = maxIterations;
            Reflection = 1;
            Stretching = 0.5;
            Compression = 2;
            Reduction = 0.5;
        }

        public Settings(double step, int maxIterations, double reflection, double stretching, double compression, double reduction)
        {
            Step = step;
            MaxIterations = maxIterations;
            Reflection = reflection;
            Stretching = stretching;
            Compression = compression;
            Reduction = reduction;
        }
    }
}