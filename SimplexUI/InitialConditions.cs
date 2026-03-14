namespace SimplexUI
{
    public readonly struct InitialConditions
    {
        public readonly EvaluateableVector[] InitialVectors { get; } = [];
        public readonly int SimplexDimention => InitialVectors.Length;
        public readonly Func<double[], double> VectorFunction { get; }

        public InitialConditions(double[][] initialVectorCoordinates, Func<double[], double> vectorFunction)
        {
            VectorFunction = vectorFunction;

            if (initialVectorCoordinates.Length == 0)
            {
                return;
            }

            int simplexDimention = initialVectorCoordinates[0].Length + 1;

            InitialVectors = new EvaluateableVector[simplexDimention];

            for (int i = 0; i < simplexDimention; i++)
            {
                InitialVectors[i] = new EvaluateableVector(initialVectorCoordinates[i], VectorFunction);
            }
        }

        public InitialConditions(EvaluateableVector initialVector, double edgeLength)
        {
            VectorFunction = initialVector.Function;

            if (initialVector.Coordinates.Length == 0)
            {
                return;
            }

            int simplexDimention = initialVector.Coordinates.Length + 1;
            
            InitialVectors = new EvaluateableVector[simplexDimention];
            InitialVectors[0] = (EvaluateableVector)initialVector.Clone();

            for (int i = 1; i < simplexDimention; i++)
            {
                InitialVectors[i] = (EvaluateableVector)initialVector.Clone();
                InitialVectors[i][i - 1] += edgeLength;
            }
        }
    }
}
