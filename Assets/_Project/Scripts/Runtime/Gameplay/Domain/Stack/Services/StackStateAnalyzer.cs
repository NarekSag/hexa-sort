using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Runtime.Gameplay.Core.Interfaces;
using _Project.Scripts.Runtime.Gameplay.Core.Models;
using _Project.Scripts.Runtime.Gameplay.Domain.Stack.Models;

namespace _Project.Scripts.Runtime.Gameplay.Domain.Stack.Services {
    /// <summary>
    /// Analyzes the state of stacks (Pure, Mixed, Empty) based on their cell colors.
    /// </summary>
    public class StackStateAnalyzer {
        /// <summary>
        /// Analyzes a stack and determines its state.
        /// </summary>
        /// <param name="stack">The stack to analyze.</param>
        /// <returns>The state of the stack (Empty, Pure, or Mixed).</returns>
        public StackState AnalyzeStackState(IStack stack) {
            if (stack == null || stack.Cells == null) {
                return StackState.Empty;
            }

            IList<ICell> cells = stack.Cells;
            
            // Filter out null cells
            var validCells = cells.Where(c => c != null).ToList();
            
            if (validCells.Count == 0) {
                return StackState.Empty;
            }

            // Check if all cells have the same color
            ColorType firstColor = validCells[0].ColorType;
            bool allSameColor = validCells.All(cell => cell.ColorType == firstColor);

            return allSameColor ? StackState.Pure : StackState.Mixed;
        }

        /// <summary>
        /// Gets the color type of a pure stack. Returns null if stack is not pure.
        /// </summary>
        /// <param name="stack">The stack to check.</param>
        /// <returns>The color type if stack is pure, null otherwise.</returns>
        public ColorType? GetPureStackColor(IStack stack) {
            if (stack == null || stack.Cells == null) {
                return null;
            }

            var validCells = stack.Cells.Where(c => c != null).ToList();
            
            if (validCells.Count == 0) {
                return null;
            }

            // Check if all cells have the same color
            ColorType firstColor = validCells[0].ColorType;
            bool allSameColor = validCells.All(cell => cell.ColorType == firstColor);

            return allSameColor ? (ColorType?)firstColor : null;
        }

        /// <summary>
        /// Gets the color type of the topmost cell in a stack.
        /// </summary>
        /// <param name="stack">The stack to check.</param>
        /// <returns>The color type of the top cell, or null if stack is empty.</returns>
        public ColorType? GetTopCellColor(IStack stack) {
            if (stack == null || stack.Cells == null) {
                return null;
            }

            var validCells = stack.Cells.Where(c => c != null).ToList();
            
            if (validCells.Count == 0) {
                return null;
            }

            // Top cell is the last one in the list
            ICell topCell = validCells[validCells.Count - 1];
            return topCell.ColorType;
        }

        /// <summary>
        /// Checks if a stack is pure (all cells have the same color).
        /// </summary>
        public bool IsPure(IStack stack) {
            return AnalyzeStackState(stack) == StackState.Pure;
        }

        /// <summary>
        /// Checks if a stack is mixed (contains cells with different colors).
        /// </summary>
        public bool IsMixed(IStack stack) {
            return AnalyzeStackState(stack) == StackState.Mixed;
        }

        /// <summary>
        /// Checks if a stack is empty (has no cells).
        /// </summary>
        public bool IsEmpty(IStack stack) {
            return AnalyzeStackState(stack) == StackState.Empty;
        }
    }
}

