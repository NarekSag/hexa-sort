using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using _Project.Scripts.Runtime.Gameplay.Core.Interfaces;
using _Project.Scripts.Runtime.Gameplay.Presentation.Cell;
using _Project.Scripts.Runtime.Gameplay.Presentation.Stack;

namespace _Project.Scripts.Runtime.Gameplay.Domain.Stack.Services
{
    public class StackMergeService
    {
        public async UniTask MergeStacks(IStack targetStack, IStack sourceStack, bool animate = true)
        {
            if (targetStack == null || sourceStack == null || targetStack == sourceStack)
            {
                return;
            }

            IList<ICell> targetCells = targetStack.Cells;
            IList<ICell> sourceCells = sourceStack.Cells;

            if (sourceCells == null || sourceCells.Count == 0)
            {
                return;
            }

            // Ensure stack collider is initialized with cell size before calculating offsets
            // Try target stack first, then source stack
            ICell cellForSize = null;
            if (targetCells != null && targetCells.Count > 0)
            {
                cellForSize = targetCells[0];
            }
            else if (sourceCells != null && sourceCells.Count > 0)
            {
                cellForSize = sourceCells[0];
            }

            // Initialize cell collider size on target stack if needed
            if (cellForSize != null && targetStack is HexStack targetHexStack)
            {
                HexStackCollider targetCollider = targetHexStack.GetStackCollider();
                if (targetCollider != null && targetCollider.CellColliderSize == Vector3.zero)
                {
                    targetCollider.CalculateCellColliderSize(cellForSize);
                }
            }

            // Store the starting index for positioning new cells
            int startingIndex = targetCells != null ? targetCells.Count : 0;

            // Collect cells to merge and their target positions (LIFO - process from last to first)
            var cellsToMerge = new List<ICell>();
            var targetLocalPositions = new List<Vector3>();
            int cellIndex = startingIndex;

            // Process cells in reverse order (LIFO - last in, first out)
            for (int i = sourceCells.Count - 1; i >= 0; i--)
            {
                ICell cell = sourceCells[i];
                if (cell != null && !targetCells.Contains(cell))
                {
                    cellsToMerge.Add(cell);

                    // Calculate target local position using target stack's collider
                    float yOffset = targetStack.CalculateYOffset(cellIndex);
                    Vector3 targetLocalPosition = new Vector3(0f, yOffset, 0f);
                    targetLocalPositions.Add(targetLocalPosition);

                    cellIndex++;
                }
            }

            if (cellsToMerge.Count == 0)
            {
                return;
            }

            // Animate or immediately move cells
            if (animate)
            {
                // Use cell animators
                await AnimateCellsToStack(cellsToMerge, sourceStack.Transform, targetStack.Transform,
                    targetLocalPositions);
            }
            else
            {
                // Immediate positioning (no animation)
                for (int i = 0; i < cellsToMerge.Count; i++)
                {
                    ICell cell = cellsToMerge[i];
                    cell.Transform.SetParent(targetStack.Transform);
                    cell.LocalPosition = targetLocalPositions[i];
                }
            }

            foreach (ICell cell in cellsToMerge)
            {
                if (!targetStack.Cells.Contains(cell))
                {
                    targetStack.Cells.Add(cell);
                }
            }

            if (sourceStack is IStack sourceHexStack)
            {
                sourceHexStack.Cells.Clear();
            }
        }

        private async UniTask AnimateCellsToStack(
            List<ICell> cells,
            Transform sourceStackTransform,
            Transform destinationStackTransform,
            List<Vector3> targetLocalPositions)
        {
            if (cells == null || cells.Count == 0)
            {
                return;
            }

            if (sourceStackTransform == null || destinationStackTransform == null)
            {
                return;
            }

            if (targetLocalPositions == null || targetLocalPositions.Count != cells.Count)
            {
                return;
            }

            // Create list of tasks for all animations
            var animationTasks = new List<UniTask>();

            for (int i = 0; i < cells.Count; i++)
            {
                ICell cell = cells[i];
                if (cell == null) continue;

                // Get animator from cell (if it's a HexCell)
                HexCellAnimator animator = null;
                if (cell is HexCell hexCell)
                {
                    animator = hexCell.GetAnimator();
                }

                // Capture source world position and original rotation BEFORE changing parent
                Vector3 sourceWorldPosition = cell.Transform.position;
                Quaternion originalRotation = cell.Transform.rotation;

                Vector3 targetLocalPosition = targetLocalPositions[i];
                Vector3 destinationWorldPosition = destinationStackTransform.TransformPoint(targetLocalPosition);

                // Calculate direction from source to destination
                Vector3 direction = (destinationWorldPosition - sourceWorldPosition).normalized;

                // Determine flip axis based on direction (X or Z axis)
                // If movement is more along X axis, flip on Z axis (forward/backward flip)
                // If movement is more along Z axis, flip on X axis (left/right flip)
                bool flipOnZAxis = Mathf.Abs(direction.x) > Mathf.Abs(direction.z);
                Vector3 flipAxis = flipOnZAxis ? Vector3.forward : Vector3.right;

                // Calculate delay with stagger (using config values from animator if available)
                float baseDelay = animator?.BaseDelay ?? 0f;
                float staggerDelay = animator?.StaggerDelay ?? 0.15f;
                float delay = baseDelay + (i * staggerDelay);

                // Set parent immediately so local position calculations are correct
                cell.Transform.SetParent(destinationStackTransform);

                // Restore original rotation before animating (parent change might have affected it)
                cell.Transform.rotation = originalRotation;

                // Animate if animator is available, otherwise just set position immediately
                if (animator != null)
                {
                    animationTasks.Add(animator.AnimateMerge(sourceWorldPosition, destinationWorldPosition, flipAxis,
                        delay));
                }
                else
                {
                    // No animator available, set position immediately
                    cell.Transform.position = destinationWorldPosition;
                    cell.LocalPosition = targetLocalPosition;
                }
            }

            // Wait for all animations to complete
            await UniTask.WhenAll(animationTasks);
        }
    }
}
