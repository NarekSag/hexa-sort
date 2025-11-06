namespace _Project.Scripts.Runtime.Gameplay.Core.Models
{
    public class LevelData
    {
        public int LevelNumber;
        public int GridWidth;
        public int GridHeight;
        public int MinColors;
        public int MaxColors;
        public int MinStackHeight;
        public int MaxStackHeight;
        public ColorType[] AvailableColors;
        public int CellsToClear;

        public override string ToString()
        {
            return $"Level {LevelNumber}: {GridWidth}x{GridHeight} grid, " +
                   $"{MinColors}-{MaxColors} colors, " +
                   $"stacks {MinStackHeight}-{MaxStackHeight}, " +
                   $"clear {CellsToClear} cells";
        }
    }
}

