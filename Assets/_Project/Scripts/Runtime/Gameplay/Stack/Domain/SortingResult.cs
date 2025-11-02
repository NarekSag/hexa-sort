namespace _Project.Scripts.Runtime.Gameplay.Stack.Domain {
    /// <summary>
    /// Represents the result of a sorting operation between two stacks.
    /// </summary>
    public class SortingResult {
        /// <summary>
        /// Whether any sorting action was triggered.
        /// </summary>
        public bool WasSortingTriggered { get; set; }
        
        /// <summary>
        /// The type of sorting action that occurred.
        /// </summary>
        public SortingAction Action { get; set; }
        
        /// <summary>
        /// Number of cells that were moved.
        /// </summary>
        public int CellsMoved { get; set; }
        
        /// <summary>
        /// Human-readable description of what happened (for debugging).
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Creates a result indicating no sorting occurred.
        /// </summary>
        public static SortingResult None() {
            return new SortingResult {
                WasSortingTriggered = false,
                Action = SortingAction.None,
                CellsMoved = 0,
                Description = "No sorting action occurred"
            };
        }

        /// <summary>
        /// Creates a result indicating a pure-to-pure merge occurred.
        /// </summary>
        public static SortingResult PureMerge(int cellsMoved) {
            return new SortingResult {
                WasSortingTriggered = true,
                Action = SortingAction.PureMerge,
                CellsMoved = cellsMoved,
                Description = $"Pure merge: {cellsMoved} cells moved"
            };
        }

        /// <summary>
        /// Creates a result indicating mixed-to-pure transfers occurred.
        /// </summary>
        public static SortingResult MixedTransfer(int cellsMoved) {
            return new SortingResult {
                WasSortingTriggered = true,
                Action = SortingAction.MixedTransfer,
                CellsMoved = cellsMoved,
                Description = $"Mixed to pure transfer: {cellsMoved} cells moved"
            };
        }
    }

    /// <summary>
    /// Types of sorting actions that can occur.
    /// </summary>
    public enum SortingAction {
        None,
        PureMerge,
        MixedTransfer
    }
}

