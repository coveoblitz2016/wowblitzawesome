using CoveoBlitz;

namespace Coveo
{
    /// <summary>
    /// Interface for a module that can provide the "best choice" for the next move.
    /// Must return a value from the <see cref="CoveoBlitz.Direction"/> class.
    /// </summary>
    public interface IBestChoice
    {
        string BestMove(GameState gameState, IPathfinder pathfinder);
    }
}
