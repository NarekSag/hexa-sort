namespace _Project.Scripts.Runtime.Gameplay.Domain.Stack.Models
{
    public enum StackState
    {
        /// <summary>
        /// Stack has no cells.
        /// </summary>
        Empty,

        /// <summary>
        /// Stack contains cells, all of which have the same color.
        /// </summary>
        Pure,

        /// <summary>
        /// Stack contains cells with different colors.
        /// </summary>
        Mixed
    }
}

