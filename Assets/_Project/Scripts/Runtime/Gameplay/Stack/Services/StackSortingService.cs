using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Runtime.Gameplay.Cell;
using _Project.Scripts.Runtime.Gameplay.Stack.Domain;
using _Project.Scripts.Runtime.Gameplay.Grid.Animation;

namespace _Project.Scripts.Runtime.Gameplay.Stack.Services {
    /// <summary>
    /// Service that implements color sorting logic between hex stacks.
    /// Handles Pure→Pure merges, Mixed→Pure transfers, and enforces forbidden operations.
    /// </summary>
    public class StackSortingService {
        private readonly StackStateAnalyzer _stateAnalyzer;
        private readonly StackMergeService _mergeService;
        private readonly StackPositionService _positionService;
        private readonly StackColliderService _colliderService;

        public StackSortingService(
            StackStateAnalyzer stateAnalyzer,
            StackMergeService mergeService,
            StackPositionService positionService,
            StackColliderService colliderService) {
            _stateAnalyzer = stateAnalyzer;
            _mergeService = mergeService;
            _positionService = positionService;
            _colliderService = colliderService;
        }

        /// <summary>
        /// Checks if a cell can be transferred from source stack to target stack.
        /// </summary>
        public bool CanTransferCell(IStack sourceStack, IStack targetStack) {
            if (sourceStack == null || targetStack == null) {
                return false;
            }

            StackState sourceState = _stateAnalyzer.AnalyzeStackState(sourceStack);
            StackState targetState = _stateAnalyzer.AnalyzeStackState(targetStack);

            // Source must not be empty
            if (sourceState == StackState.Empty) {
                return false;
            }

            // Pure → Mixed is forbidden
            if (sourceState == StackState.Pure && targetState == StackState.Mixed) {
                return false;
            }

            // Mixed → Mixed is forbidden
            if (sourceState == StackState.Mixed && targetState == StackState.Mixed) {
                return false;
            }

            // Mixed → Pure: Only if top cell matches pure stack color
            if (sourceState == StackState.Mixed && targetState == StackState.Pure) {
                ColorType? topCellColor = _stateAnalyzer.GetTopCellColor(sourceStack);
                ColorType? pureStackColor = _stateAnalyzer.GetPureStackColor(targetStack);
                return topCellColor.HasValue && pureStackColor.HasValue && topCellColor.Value == pureStackColor.Value;
            }

            // Pure → Pure: Only if colors match
            if (sourceState == StackState.Pure && targetState == StackState.Pure) {
                ColorType? sourceColor = _stateAnalyzer.GetPureStackColor(sourceStack);
                ColorType? targetColor = _stateAnalyzer.GetPureStackColor(targetStack);
                return sourceColor.HasValue && targetColor.HasValue && sourceColor.Value == targetColor.Value;
            }

            // Pure → Empty: Allowed (empty stack becomes pure)
            if (sourceState == StackState.Pure && targetState == StackState.Empty) {
                return true;
            }

            // Mixed → Empty: Not allowed (empty should only accept pure stacks)
            if (sourceState == StackState.Mixed && targetState == StackState.Empty) {
                return false;
            }

            return false;
        }

        /// <summary>
        /// Checks if sorting should be triggered between two stacks.
        /// </summary>
        public bool ShouldTriggerSorting(IStack leftStack, IStack rightStack) {
            if (leftStack == null || rightStack == null) {
                return false;
            }

            StackState leftState = _stateAnalyzer.AnalyzeStackState(leftStack);
            StackState rightState = _stateAnalyzer.AnalyzeStackState(rightStack);

            // Pure → Pure merge: Check if same color
            if (leftState == StackState.Pure && rightState == StackState.Pure) {
                ColorType? leftColor = _stateAnalyzer.GetPureStackColor(leftStack);
                ColorType? rightColor = _stateAnalyzer.GetPureStackColor(rightStack);
                return leftColor.HasValue && rightColor.HasValue && leftColor.Value == rightColor.Value;
            }

            // Mixed → Pure transfer: Check if top cell matches pure color
            if (leftState == StackState.Mixed && rightState == StackState.Pure) {
                ColorType? topCellColor = _stateAnalyzer.GetTopCellColor(leftStack);
                ColorType? pureStackColor = _stateAnalyzer.GetPureStackColor(rightStack);
                return topCellColor.HasValue && pureStackColor.HasValue && topCellColor.Value == pureStackColor.Value;
            }

            // Pure ← Mixed transfer: Reverse direction
            if (leftState == StackState.Pure && rightState == StackState.Mixed) {
                ColorType? topCellColor = _stateAnalyzer.GetTopCellColor(rightStack);
                ColorType? pureStackColor = _stateAnalyzer.GetPureStackColor(leftStack);
                return topCellColor.HasValue && pureStackColor.HasValue && topCellColor.Value == pureStackColor.Value;
            }

            return false;
        }

        /// <summary>
        /// Executes a pure-to-pure merge. All cells from right stack move to left stack.
        /// </summary>
        public void ExecutePureToPureMerge(IStack leftStack, IStack rightStack, IHexagonAnimationService animationService) {
            if (leftStack == null || rightStack == null) {
                return;
            }

            bool animate = animationService != null;
            _mergeService.MergeStacks(leftStack, rightStack, animate, animationService);

            // Update colliders if stacks are HexStack
            if (leftStack is HexStack leftHexStack) {
                leftHexStack.UpdateColliderSize();
            }
            if (rightStack is HexStack rightHexStack) {
                rightHexStack.UpdateColliderSize();
            }
        }

        /// <summary>
        /// Executes mixed-to-pure transfer with cascading behavior.
        /// Transfers top cells from mixed stack to pure stack until no more transfers are possible.
        /// </summary>
        /// <returns>True if any transfers occurred, false otherwise.</returns>
        public bool ExecuteMixedToPureTransfer(IStack mixedStack, IStack pureStack, IHexagonAnimationService animationService) {
            if (mixedStack == null || pureStack == null) {
                return false;
            }

            bool anyTransfersOccurred = false;
            bool animate = animationService != null;

            // Cascading transfer: Continue until no more transfers possible
            while (true) {
                // Check if we can still transfer
                if (!CanTransferCell(mixedStack, pureStack)) {
                    break;
                }

                // Get the top cell from mixed stack
                HexCell topCell = RemoveTopCell(mixedStack);
                if (topCell == null) {
                    break;
                }

                // Transfer to pure stack
                AddCellToTop(pureStack, topCell, animate, animationService);
                anyTransfersOccurred = true;

                // Update colliders
                if (mixedStack is HexStack mixedHexStack) {
                    mixedHexStack.UpdateColliderSize();
                }
                if (pureStack is HexStack pureHexStack) {
                    pureHexStack.UpdateColliderSize();
                }

                // If mixed stack is now empty or pure, we're done
                StackState newMixedState = _stateAnalyzer.AnalyzeStackState(mixedStack);
                if (newMixedState == StackState.Empty || newMixedState == StackState.Pure) {
                    break;
                }
            }

            // Reposition cells if not animating
            if (!animate) {
                if (mixedStack is HexStack mixedHexStack) {
                    _positionService.RepositionAllHexagons(mixedHexStack.Hexagons);
                }
                if (pureStack is HexStack pureHexStack) {
                    _positionService.RepositionAllHexagons(pureHexStack.Hexagons);
                }
            }

            return anyTransfersOccurred;
        }

        /// <summary>
        /// Main entry point for processing two stacks. Analyzes states and executes appropriate sorting logic.
        /// </summary>
        public SortingResult ProcessStackPair(IStack leftStack, IStack rightStack, IHexagonAnimationService animationService) {
            if (leftStack == null || rightStack == null) {
                return SortingResult.None();
            }

            StackState leftState = _stateAnalyzer.AnalyzeStackState(leftStack);
            StackState rightState = _stateAnalyzer.AnalyzeStackState(rightStack);

            // Pure → Pure merge
            if (leftState == StackState.Pure && rightState == StackState.Pure) {
                ColorType? leftColor = _stateAnalyzer.GetPureStackColor(leftStack);
                ColorType? rightColor = _stateAnalyzer.GetPureStackColor(rightStack);
                
                if (leftColor.HasValue && rightColor.HasValue && leftColor.Value == rightColor.Value) {
                    int cellsBefore = rightStack.Cells.Count;
                    ExecutePureToPureMerge(leftStack, rightStack, animationService);
                    int cellsMoved = cellsBefore; // All cells moved
                    return SortingResult.PureMerge(cellsMoved);
                }
            }

            // Mixed → Pure transfer (left is mixed, right is pure)
            if (leftState == StackState.Mixed && rightState == StackState.Pure) {
                ColorType? topCellColor = _stateAnalyzer.GetTopCellColor(leftStack);
                ColorType? pureStackColor = _stateAnalyzer.GetPureStackColor(rightStack);
                
                if (topCellColor.HasValue && pureStackColor.HasValue && topCellColor.Value == pureStackColor.Value) {
                    int cellsBefore = leftStack.Cells.Count;
                    bool transferred = ExecuteMixedToPureTransfer(leftStack, rightStack, animationService);
                    if (transferred) {
                        int cellsMoved = cellsBefore - leftStack.Cells.Count;
                        return SortingResult.MixedTransfer(cellsMoved);
                    }
                }
            }

            // Pure ← Mixed transfer (left is pure, right is mixed)
            if (leftState == StackState.Pure && rightState == StackState.Mixed) {
                ColorType? topCellColor = _stateAnalyzer.GetTopCellColor(rightStack);
                ColorType? pureStackColor = _stateAnalyzer.GetPureStackColor(leftStack);
                
                if (topCellColor.HasValue && pureStackColor.HasValue && topCellColor.Value == pureStackColor.Value) {
                    int cellsBefore = rightStack.Cells.Count;
                    bool transferred = ExecuteMixedToPureTransfer(rightStack, leftStack, animationService);
                    if (transferred) {
                        int cellsMoved = cellsBefore - rightStack.Cells.Count;
                        return SortingResult.MixedTransfer(cellsMoved);
                    }
                }
            }

            return SortingResult.None();
        }

        /// <summary>
        /// Removes and returns the topmost cell from a stack.
        /// </summary>
        private HexCell RemoveTopCell(IStack stack) {
            if (stack is not HexStack hexStack) {
                return null;
            }

            if (hexStack.Hexagons == null || hexStack.Hexagons.Count == 0) {
                return null;
            }

            // Top cell is the last one in the list
            int lastIndex = hexStack.Hexagons.Count - 1;
            HexCell topCell = hexStack.Hexagons[lastIndex];
            hexStack.Hexagons.RemoveAt(lastIndex);

            return topCell;
        }

        /// <summary>
        /// Adds a cell to the top of a stack.
        /// </summary>
        private void AddCellToTop(IStack stack, HexCell cell, bool animate, IHexagonAnimationService animationService) {
            if (stack == null || cell == null) {
                return;
            }

            if (stack is not HexStack hexStack) {
                return;
            }

            // Ensure cell is parented to stack
            cell.Transform.SetParent(hexStack.Transform);

            // Add to list (top is end of list)
            hexStack.Hexagons.Add(cell);

            // Calculate position
            int index = hexStack.Hexagons.Count - 1;
            float yOffset = _colliderService.CalculateYOffset(index);
            Vector3 targetLocalPosition = new Vector3(0f, yOffset, 0f);

            if (animate && animationService != null) {
                // Animate the cell to its new position
                var cellTransforms = new List<Transform> { cell.Transform };
                var targetPositions = new List<Vector3> { targetLocalPosition };
                animationService.AnimateHexagonStackMerge(
                    cellTransforms,
                    hexStack.Transform, // source (cell's current parent)
                    hexStack.Transform, // target
                    targetPositions
                );
            } else {
                // Immediate positioning
                cell.LocalPosition = targetLocalPosition;
            }
        }
    }
}

