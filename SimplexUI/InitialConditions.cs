using SimplexUI.Exceptions;

namespace SimplexUI
{
    public readonly struct InitialConditions
    {
        public readonly EvaluateableVector[] InitialVectors { get; } = [];
        public readonly int SimplexDimention => InitialVectors.Length;
        public readonly Func<double[], double> VectorFunction { get; }

        /// <summary>
        /// Initializes simplex based on passed coordinates
        /// </summary>
        /// <param name="initialVectorCoordinates">initial simplex</param>
        /// <param name="vectorFunction">function on which simplex defined</param>
        /// <exception cref="MismatchingDimentionsException"></exception>
        public InitialConditions(double[][] initialVectorCoordinates, Func<double[], double> vectorFunction)
        {
            VectorFunction = vectorFunction;

            if (initialVectorCoordinates.Length == 0)
            {
                return;
            }

            int simplexDimention = initialVectorCoordinates[0].Length + 1;

            if (initialVectorCoordinates.Length != simplexDimention)
            {
                throw new MismatchingDimentionsException("Number of vectors must be greater than number of vector coordinates by 1");
            }

            InitialVectors = new EvaluateableVector[simplexDimention];

            for (int i = 0; i < simplexDimention; i++)
            {
                InitialVectors[i] = new EvaluateableVector(initialVectorCoordinates[i], VectorFunction);
            }
        }

        /// <summary>
        /// Initializes simplex automatically, based on one vector
        /// </summary>
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
