using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Runtime.Gameplay.Core.Interfaces;
using _Project.Scripts.Runtime.Gameplay.Core.Models;
using _Project.Scripts.Runtime.Gameplay.Domain.Stack.Models;

namespace _Project.Scripts.Runtime.Gameplay.Domain.Stack.Services
{
    public class StackStateAnalyzer
    {
        public StackState AnalyzeStackState(IStack stack)
        {
            if (stack == null || stack.Cells == null)
            {
                return StackState.Empty;
            }

            IList<ICell> cells = stack.Cells;

            // Filter out null cells
            var validCells = cells.Where(c => c != null).ToList();

            if (validCells.Count == 0)
            {
                return StackState.Empty;
            }

            // Check if all cells have the same color
            ColorType firstColor = validCells[0].ColorType;
            bool allSameColor = validCells.All(cell => cell.ColorType == firstColor);

            return allSameColor ? StackState.Pure : StackState.Mixed;
        }

        public ColorType? GetPureStackColor(IStack stack)
        {
            if (stack == null || stack.Cells == null)
            {
                return null;
            }

            var validCells = stack.Cells.Where(c => c != null).ToList();

            if (validCells.Count == 0)
            {
                return null;
            }

            // Check if all cells have the same color
            ColorType firstColor = validCells[0].ColorType;
            bool allSameColor = validCells.All(cell => cell.ColorType == firstColor);

            return allSameColor ? (ColorType?)firstColor : null;
        }

        public ColorType? GetTopCellColor(IStack stack)
        {
            if (stack == null || stack.Cells == null)
            {
                return null;
            }

            var validCells = stack.Cells.Where(c => c != null).ToList();

            if (validCells.Count == 0)
            {
                return null;
            }

            // Top cell is the last one in the list
            ICell topCell = validCells[validCells.Count - 1];
            return topCell.ColorType;
        }
    }
}

