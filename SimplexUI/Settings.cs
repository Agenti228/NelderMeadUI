namespace SimplexUI
{
    public readonly struct Settings
    {
        public readonly int MaxIterations { get; }
        public readonly double Reflection { get; }
        public readonly double Compression { get; }
        public readonly double Stretching { get; }
        public readonly double Reduction { get; }

        /// <summary>
        /// Default values:
        /// <list type="bullet">
        /// <item><see cref="MaxIterations"/> = 1</item>
        /// <item><see cref="Reflection"/> = 1</item>
        /// <item><see cref="Compression"/> = 0.5</item>
        /// <item><see cref="Stretching"/> = 2</item>
        /// <item><see cref="Reduction"/> = 0.5</item>
        /// </list>
        /// </summary>
        public Settings()
        {
            MaxIterations = 1;
            Reflection = 1;
            Compression = 0.5;
            Stretching = 2;
            Reduction = 0.5;
        }

        public Settings(int maxIterations, double reflection = 1, double compression = 0.5, double stretching = 2, double reduction = 0.5)
        {
            MaxIterations = maxIterations;
            Reflection = reflection;
            Compression = compression;
            Stretching = stretching;
            Reduction = reduction;
        }
    }
}