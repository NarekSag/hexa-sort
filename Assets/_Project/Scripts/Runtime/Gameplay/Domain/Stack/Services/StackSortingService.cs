using UnityEngine;
using Cysharp.Threading.Tasks;
using _Project.Scripts.Runtime.Gameplay.Core.Interfaces;
using _Project.Scripts.Runtime.Gameplay.Domain.Stack.Models;
using _Project.Scripts.Runtime.Gameplay.Presentation.Cell;
using _Project.Scripts.Runtime.Gameplay.Presentation.Stack;
using _Project.Scripts.Runtime.Gameplay.Core.Models;

namespace _Project.Scripts.Runtime.Gameplay.Domain.Stack.Services
{
    public class StackSortingService
    {
        private readonly StackStateAnalyzer _stateAnalyzer;
        private readonly StackMergeService _mergeService;

        public StackSortingService(
            StackStateAnalyzer stateAnalyzer,
            StackMergeService mergeService)
        {
            _stateAnalyzer = stateAnalyzer;
            _mergeService = mergeService;
        }

        public bool CanTransferCell(IStack sourceStack, IStack targetStack)
        {
            if (sourceStack == null || targetStack == null)
            {
                return false;
            }

            StackState sourceState = _stateAnalyzer.AnalyzeStackState(sourceStack);
            StackState targetState = _stateAnalyzer.AnalyzeStackState(targetStack);

            // Source must not be empty
            if (sourceState == StackState.Empty)
            {
                return false;
            }

            // Pure → Mixed is forbidden
            if (sourceState == StackState.Pure && targetState == StackState.Mixed)
            {
                return false;
            }

            // Mixed → Mixed: Only allowed if top cell colors match
            if (sourceState == StackState.Mixed && targetState == StackState.Mixed)
            {
                ColorType? topCellColor1 = _stateAnalyzer.GetTopCellColor(sourceStack);
                ColorType? topCellColor2 = _stateAnalyzer.GetTopCellColor(targetStack);
                return topCellColor1.HasValue && topCellColor2.HasValue && topCellColor1.Value == topCellColor2.Value;
            }

            // Mixed → Pure: Only if top cell matches pure stack color
            if (sourceState == StackState.Mixed && targetState == StackState.Pure)
            {
                ColorType? topCellColor = _stateAnalyzer.GetTopCellColor(sourceStack);
                ColorType? pureStackColor = _stateAnalyzer.GetPureStackColor(targetStack);
                return topCellColor.HasValue && pureStackColor.HasValue && topCellColor.Value == pureStackColor.Value;
            }

            // Pure → Pure: Only if colors match
            if (sourceState == StackState.Pure && targetState == StackState.Pure)
            {
                ColorType? sourceColor = _stateAnalyzer.GetPureStackColor(sourceStack);
                ColorType? targetColor = _stateAnalyzer.GetPureStackColor(targetStack);
                return sourceColor.HasValue && targetColor.HasValue && sourceColor.Value == targetColor.Value;
            }

            // Pure → Empty: Allowed (empty stack becomes pure)
            if (sourceState == StackState.Pure && targetState == StackState.Empty)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if sorting should be triggered between two stacks.
        /// </summary>
        public bool ShouldTriggerSorting(IStack leftStack, IStack rightStack)
        {
            if (leftStack == null || rightStack == null)
            {
                return false;
            }

            StackState leftState = _stateAnalyzer.AnalyzeStackState(leftStack);
            StackState rightState = _stateAnalyzer.AnalyzeStackState(rightStack);

            // Pure → Pure merge: Check if same color
            if (leftState == StackState.Pure && rightState == StackState.Pure)
            {
                ColorType? leftColor = _stateAnalyzer.GetPureStackColor(leftStack);
                ColorType? rightColor = _stateAnalyzer.GetPureStackColor(rightStack);
                return leftColor.HasValue && rightColor.HasValue && leftColor.Value == rightColor.Value;
            }

            // Mixed → Pure transfer: Check if top cell matches pure color
            if (leftState == StackState.Mixed && rightState == StackState.Pure)
            {
                ColorType? topCellColor = _stateAnalyzer.GetTopCellColor(leftStack);
                ColorType? pureStackColor = _stateAnalyzer.GetPureStackColor(rightStack);
                return topCellColor.HasValue && pureStackColor.HasValue && topCellColor.Value == pureStackColor.Value;
            }

            // Pure ← Mixed transfer: Reverse direction
            if (leftState == StackState.Pure && rightState == StackState.Mixed)
            {
                ColorType? topCellColor = _stateAnalyzer.GetTopCellColor(rightStack);
                ColorType? pureStackColor = _stateAnalyzer.GetPureStackColor(leftStack);
                return topCellColor.HasValue && pureStackColor.HasValue && topCellColor.Value == pureStackColor.Value;
            }

            // Mixed → Mixed transfer: Check if top cells match
            if (leftState == StackState.Mixed && rightState == StackState.Mixed)
            {
                ColorType? topCellColor1 = _stateAnalyzer.GetTopCellColor(leftStack);
                ColorType? topCellColor2 = _stateAnalyzer.GetTopCellColor(rightStack);
                return topCellColor1.HasValue && topCellColor2.HasValue && topCellColor1.Value == topCellColor2.Value;
            }

            return false;
        }

        /// <summary>
        /// Executes a pure-to-pure merge. All cells from right stack move to left stack.
        /// </summary>
        public async UniTask ExecutePureToPureMerge(IStack leftStack, IStack rightStack, bool animate = true)
        {
            if (leftStack == null || rightStack == null)
            {
                return;
            }

            // Use merge service for merge (with or without animation)
            await _mergeService.MergeStacks(leftStack, rightStack, animate);

            leftStack.UpdateColliderSize();
            rightStack.UpdateColliderSize();
        }


        /// <summary>
        /// Executes mixed-to-pure transfer step-by-step (one cell at a time).
        /// Transfers the top cell from mixed stack to pure stack if colors match.
        /// The recursive re-checking system will handle subsequent transfers.
        /// </summary>
        /// <returns>True if a transfer occurred, false otherwise.</returns>
        public async UniTask<bool> ExecuteMixedToPureTransfer(IStack mixedStack, IStack pureStack, bool animate = true)
        {
            if (mixedStack == null || pureStack == null)
            {
                return false;
            }

            // Check if we can transfer
            if (!CanTransferCell(mixedStack, pureStack))
            {
                return false;
            }

            // Transfer only ONE cell (step-by-step behavior)
            ICell topCell = RemoveTopCell(mixedStack);
            if (topCell == null)
            {
                return false;
            }

            // Transfer to pure stack and await animation
            await AddCellToTop(pureStack, topCell, animate);

            mixedStack.UpdateColliderSize();
            pureStack.UpdateColliderSize();

            // Reposition cells if not animating
            if (!animate)
            {
                mixedStack.RepositionAllCells();
                pureStack.RepositionAllCells();
            }

            return true;
        }

        /// <summary>
        /// Executes mixed-to-mixed transfer step-by-step (one cell at a time).
        /// Transfers the top cell from mixedStack1 to mixedStack2 if top cell colors match.
        /// The recursive re-checking system will handle subsequent transfers.
        /// </summary>
        /// <returns>True if a transfer occurred, false otherwise.</returns>
        public async UniTask<bool> ExecuteMixedToMixedTransfer(IStack mixedStack1, IStack mixedStack2,
            bool animate = true)
        {
            if (mixedStack1 == null || mixedStack2 == null)
            {
                return false;
            }

            // Check if we can transfer
            if (!CanTransferCell(mixedStack1, mixedStack2))
            {
                return false;
            }

            // Transfer only ONE cell (step-by-step behavior)
            ICell topCell = RemoveTopCell(mixedStack1);
            if (topCell == null)
            {
                return false;
            }

            // Transfer to mixedStack2 and await animation
            await AddCellToTop(mixedStack2, topCell, animate);

            // Update colliders
            mixedStack1.UpdateColliderSize();
            mixedStack2.UpdateColliderSize();

            // Reposition cells if not animating
            if (!animate)
            {
                mixedStack1.RepositionAllCells();
                mixedStack2.RepositionAllCells();
            }

            return true;
        }

        /// <summary>
        /// Main entry point for processing two stacks. Analyzes states and executes appropriate sorting logic.
        /// </summary>
        public async UniTask<SortingResult> ProcessStackPair(IStack leftStack, IStack rightStack, bool animate = true)
        {
            if (leftStack == null || rightStack == null)
            {
                return SortingResult.None();
            }

            StackState leftState = _stateAnalyzer.AnalyzeStackState(leftStack);
            StackState rightState = _stateAnalyzer.AnalyzeStackState(rightStack);

            // Pure → Pure merge
            if (leftState == StackState.Pure && rightState == StackState.Pure)
            {
                ColorType? leftColor = _stateAnalyzer.GetPureStackColor(leftStack);
                ColorType? rightColor = _stateAnalyzer.GetPureStackColor(rightStack);

                if (leftColor.HasValue && rightColor.HasValue && leftColor.Value == rightColor.Value)
                {
                    int cellsBefore = rightStack.Cells.Count;
                    await ExecutePureToPureMerge(leftStack, rightStack, animate);
                    int cellsMoved = cellsBefore; // All cells moved
                    return SortingResult.PureMerge(cellsMoved);
                }
            }

            // Mixed → Pure transfer (left is mixed, right is pure)
            if (leftState == StackState.Mixed && rightState == StackState.Pure)
            {
                ColorType? topCellColor = _stateAnalyzer.GetTopCellColor(leftStack);
                ColorType? pureStackColor = _stateAnalyzer.GetPureStackColor(rightStack);

                if (topCellColor.HasValue && pureStackColor.HasValue && topCellColor.Value == pureStackColor.Value)
                {
                    int cellsBefore = leftStack.Cells.Count;
                    bool transferred = await ExecuteMixedToPureTransfer(leftStack, rightStack, animate);
                    if (transferred)
                    {
                        int cellsMoved = cellsBefore - leftStack.Cells.Count;
                        return SortingResult.MixedTransfer(cellsMoved);
                    }
                }
            }

            // Pure ← Mixed transfer (left is pure, right is mixed)
            if (leftState == StackState.Pure && rightState == StackState.Mixed)
            {
                ColorType? topCellColor = _stateAnalyzer.GetTopCellColor(rightStack);
                ColorType? pureStackColor = _stateAnalyzer.GetPureStackColor(leftStack);

                if (topCellColor.HasValue && pureStackColor.HasValue && topCellColor.Value == pureStackColor.Value)
                {
                    int cellsBefore = rightStack.Cells.Count;
                    bool transferred = await ExecuteMixedToPureTransfer(rightStack, leftStack, animate);
                    if (transferred)
                    {
                        int cellsMoved = cellsBefore - rightStack.Cells.Count;
                        return SortingResult.MixedTransfer(cellsMoved);
                    }
                }
            }

            // Mixed → Mixed transfer
            if (leftState == StackState.Mixed && rightState == StackState.Mixed)
            {
                ColorType? topCellColor1 = _stateAnalyzer.GetTopCellColor(leftStack);
                ColorType? topCellColor2 = _stateAnalyzer.GetTopCellColor(rightStack);

                if (topCellColor1.HasValue && topCellColor2.HasValue && topCellColor1.Value == topCellColor2.Value)
                {
                    // Track cells before transfer (cells are removed from leftStack)
                    int cellsBefore = leftStack.Cells.Count;
                    bool transferred = await ExecuteMixedToMixedTransfer(leftStack, rightStack, animate);
                    if (transferred)
                    {
                        int cellsMoved = cellsBefore - leftStack.Cells.Count;
                        return SortingResult.MixedTransfer(cellsMoved);
                    }
                }
            }

            return SortingResult.None();
        }

        /// <summary>
        /// Removes and returns the topmost cell from a stack.
        /// </summary>
        private ICell RemoveTopCell(IStack stack)
        {
            if (stack.Cells == null || stack.Cells.Count == 0)
            {
                return null;
            }

            // Top cell is the last one in the list
            int lastIndex = stack.Cells.Count - 1;
            ICell topCell = stack.Cells[lastIndex];
            stack.Cells.RemoveAt(lastIndex);

            return topCell;
        }

        /// <summary>
        /// Adds a cell to the top of a stack.
        /// </summary>
        private async UniTask AddCellToTop(IStack stack, ICell cell, bool animate)
        {
            if (stack == null || cell == null)
            {
                return;
            }

            // Ensure Hexagons list exists
            if (stack.Cells == null)
            {
                return;
            }

            // Ensure stack collider is initialized with cell size before calculating offsets
            // Use the cell being added, or an existing cell from the stack
            ICell cellForSize = cell;
            if (stack.Cells.Count > 0)
            {
                cellForSize = stack.Cells[0];
            }

            // Initialize cell collider size on stack if needed
            if (cellForSize != null && stack is HexStack hexStack)
            {
                HexStackCollider stackCollider = hexStack.GetStackCollider();
                if (stackCollider != null && stackCollider.CellColliderSize == Vector3.zero)
                {
                    stackCollider.CalculateCellColliderSize(cellForSize);
                }
            }

            // Ensure cell is parented to stack
            cell.Transform.SetParent(stack.Transform);

            // Add to list (top is end of list)
            stack.Cells.Add(cell);

            // Calculate position using stack's collider
            int index = stack.Cells.Count - 1;
            float yOffset = stack.CalculateYOffset(index);
            Vector3 targetLocalPosition = new Vector3(0f, yOffset, 0f);

            if (animate)
            {
                // Get animator from cell (if it's a HexCell)
                HexCellAnimator animator = null;
                if (cell is HexCell hexCell)
                {
                    animator = hexCell.GetAnimator();
                }

                if (animator != null)
                {
                    // Capture source position and rotation before changing parent
                    Vector3 sourceWorldPosition = cell.Transform.position;

                    Vector3 destinationWorldPosition = stack.Transform.TransformPoint(targetLocalPosition);

                    // Calculate direction for flip axis
                    Vector3 direction = (destinationWorldPosition - sourceWorldPosition).normalized;
                    bool flipOnZAxis = Mathf.Abs(direction.x) > Mathf.Abs(direction.z);
                    Vector3 flipAxis = flipOnZAxis ? Vector3.forward : Vector3.right;

                    // Animate the cell to its new position and await completion
                    await animator.AnimateMerge(sourceWorldPosition, destinationWorldPosition, flipAxis,
                        animator.BaseDelay);
                }
                else
                {
                    // No animator available, set position immediately
                    cell.LocalPosition = targetLocalPosition;
                }
            }
            else
            {
                // Immediate positioning
                cell.LocalPosition = targetLocalPosition;
            }
        }
    }
}

