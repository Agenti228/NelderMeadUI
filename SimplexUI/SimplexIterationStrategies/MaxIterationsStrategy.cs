namespace SimplexUI.SimplexIterationStrategies
{
    public class MaxIterationsStrategy : ISimplexIterationStrategy
    {
        private Simplex _simplex;

        public MaxIterationsStrategy(double[] coordinates, Func<double[], double> function, double edgeLength = 2, int maxIterations = 100)
        {
            var initialVector = new EvaluateableVector(coordinates, function);
            var initialConditions = new InitialConditions(initialVector, edgeLength);
            var settings = new Settings(maxIterations);
            _simplex = new Simplex(settings, initialConditions);
        }

        public IEnumerable<Simplex> Iterate()
        {
            for (int i = 0; i < _simplex.Settings.MaxIterations; i++)
            {
                _simplex.SortVectors();
                _simplex.IterationOnSorted();
                yield return (Simplex)_simplex.Clone();
            }
        }
    }
}
