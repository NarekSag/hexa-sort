namespace _Project.Scripts.Runtime.Gameplay.Core.Models {
    /// <summary>
    /// Contains all data needed to configure and run a specific level.
    /// Generated from LevelProgressionConfig based on level number.
    /// </summary>
    public class LevelData {
        public int LevelNumber;
        public int GridWidth;
        public int GridHeight;
        public int MinColors;
        public int MaxColors;
        public int MinStackHeight;
        public int MaxStackHeight;
        public ColorType[] AvailableColors;
        public int CellsToClear;
        
        public override string ToString() {
            return $"Level {LevelNumber}: {GridWidth}x{GridHeight} grid, " +
                   $"{MinColors}-{MaxColors} colors, " +
                   $"stacks {MinStackHeight}-{MaxStackHeight}, " +
                   $"clear {CellsToClear} cells";
        }
    }
}

