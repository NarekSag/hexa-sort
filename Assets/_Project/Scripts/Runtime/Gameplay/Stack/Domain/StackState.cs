namespace _Project.Scripts.Runtime.Gameplay.Stack.Domain {
    /// <summary>
    /// Represents the state of a hex stack based on its cell colors.
    /// </summary>
    public enum StackState {
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

