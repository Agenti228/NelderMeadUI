namespace SimplexUI.SimplexIterationStrategies
{
    public interface ISimplexIterationStrategy
    {
        IEnumerable<Simplex> Iterate();
    }
}
