using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Runtime.Gameplay.Grid.Animation;
using _Project.Scripts.Runtime.Gameplay.Cell;

namespace _Project.Scripts.Runtime.Gameplay.Stack.Services {
    public class StackMergeService {
        private readonly StackColliderService _colliderService;
        private readonly StackPositionService _positionService;

        public StackMergeService(StackColliderService colliderService, StackPositionService positionService) {
            _colliderService = colliderService;
            _positionService = positionService;
        }

        public void MergeStacks(IStack targetStack, IStack sourceStack, bool animate = true, HexAnimationService animationService = null) {
            if (targetStack == null || sourceStack == null || targetStack == sourceStack) {
                return;
            }

            IList<ICell> targetCells = targetStack.Cells;
            IList<ICell> sourceCells = sourceStack.Cells;

            if (sourceCells == null || sourceCells.Count == 0) {
                return;
            }

            // Ensure collider service is initialized with cell size before calculating offsets
            // Try target stack first, then source stack
            ICell cellForSize = null;
            if (targetCells != null && targetCells.Count > 0) {
                cellForSize = targetCells[0];
            } else if (sourceCells != null && sourceCells.Count > 0) {
                cellForSize = sourceCells[0];
            }
            
            if (cellForSize != null) {
                _colliderService.CalculateCellColliderSize(cellForSize);
            }

            // Store the starting index for positioning new cells
            int startingIndex = targetCells != null ? targetCells.Count : 0;

            // Collect cells to merge and their target positions (LIFO - process from last to first)
            var cellsToMerge = new List<ICell>();
            var targetLocalPositions = new List<Vector3>();
            int cellIndex = startingIndex;

            // Process cells in reverse order (LIFO - last in, first out)
            for (int i = sourceCells.Count - 1; i >= 0; i--) {
                ICell cell = sourceCells[i];
                if (cell != null && !targetCells.Contains(cell)) {
                    cellsToMerge.Add(cell);
                    
                    // Calculate target local position
                    float yOffset = _colliderService.CalculateYOffset(cellIndex);
                    Vector3 targetLocalPosition = new Vector3(0f, yOffset, 0f);
                    targetLocalPositions.Add(targetLocalPosition);
                    
                    cellIndex++;
                }
            }

            if (cellsToMerge.Count == 0) {
                return;
            }

            // Animate or immediately move cells
            if (animate && animationService != null) {
                // Use animation service
                var cellTransforms = cellsToMerge.Select(cell => cell.Transform).ToList();
                animationService.AnimateHexagonStackMerge(
                    cellTransforms,
                    sourceStack.Transform,
                    targetStack.Transform,
                    targetLocalPositions
                );
            } else {
                // Immediate positioning (no animation)
                for (int i = 0; i < cellsToMerge.Count; i++) {
                    ICell cell = cellsToMerge[i];
                    cell.Transform.SetParent(targetStack.Transform);
                    cell.LocalPosition = targetLocalPositions[i];
                }
            }

            foreach (ICell cell in cellsToMerge) 
            {
                if (!targetStack.Cells.Contains(cell)) {
                        targetStack.Cells.Add(cell);
                }
            }

            if (sourceStack is IStack sourceHexStack) {
                sourceHexStack.Cells.Clear();
            }
        }
    }
}
