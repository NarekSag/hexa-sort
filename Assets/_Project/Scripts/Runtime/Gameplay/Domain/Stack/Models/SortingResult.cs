namespace _Project.Scripts.Runtime.Gameplay.Domain.Stack.Models
{
    public class SortingResult
    {
        public bool WasSortingTriggered { get; set; }

        public SortingAction Action { get; set; }

        public int CellsMoved { get; set; }

        public string Description { get; set; }

        public static SortingResult None()
        {
            return new SortingResult
            {
                WasSortingTriggered = false,
                Action = SortingAction.None,
                CellsMoved = 0,
                Description = "No sorting action occurred"
            };
        }

        public static SortingResult PureMerge(int cellsMoved)
        {
            return new SortingResult
            {
                WasSortingTriggered = true,
                Action = SortingAction.PureMerge,
                CellsMoved = cellsMoved,
                Description = $"Pure merge: {cellsMoved} cells moved"
            };
        }

        public static SortingResult MixedTransfer(int cellsMoved)
        {
            return new SortingResult
            {
                WasSortingTriggered = true,
                Action = SortingAction.MixedTransfer,
                CellsMoved = cellsMoved,
                Description = $"Mixed to pure transfer: {cellsMoved} cells moved"
            };
        }
    }

    public enum SortingAction
    {
        None,
        PureMerge,
        MixedTransfer
    }
}

