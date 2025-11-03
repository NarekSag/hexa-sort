using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
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

            // Mixed → Mixed: Only allowed if top cell colors match
            if (sourceState == StackState.Mixed && targetState == StackState.Mixed) {
                ColorType? topCellColor1 = _stateAnalyzer.GetTopCellColor(sourceStack);
                ColorType? topCellColor2 = _stateAnalyzer.GetTopCellColor(targetStack);
                return topCellColor1.HasValue && topCellColor2.HasValue && topCellColor1.Value == topCellColor2.Value;
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

            // Mixed → Mixed transfer: Check if top cells match
            if (leftState == StackState.Mixed && rightState == StackState.Mixed) {
                ColorType? topCellColor1 = _stateAnalyzer.GetTopCellColor(leftStack);
                ColorType? topCellColor2 = _stateAnalyzer.GetTopCellColor(rightStack);
                return topCellColor1.HasValue && topCellColor2.HasValue && topCellColor1.Value == topCellColor2.Value;
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
        /// Executes mixed-to-pure transfer step-by-step (one cell at a time).
        /// Transfers the top cell from mixed stack to pure stack if colors match.
        /// The recursive re-checking system will handle subsequent transfers.
        /// </summary>
        /// <returns>True if a transfer occurred, false otherwise.</returns>
        public async UniTask<bool> ExecuteMixedToPureTransfer(IStack mixedStack, IStack pureStack, IHexagonAnimationService animationService) {
            if (mixedStack == null || pureStack == null) {
                return false;
            }

            // Check if we can transfer
            if (!CanTransferCell(mixedStack, pureStack)) {
                return false;
            }

            bool animate = animationService != null;

            // Transfer only ONE cell (step-by-step behavior)
            HexCell topCell = RemoveTopCell(mixedStack);
            if (topCell == null) {
                return false;
            }

            // Transfer to pure stack and await animation
            await AddCellToTop(pureStack, topCell, animate, animationService);

            // Update colliders
            HexStack mixedHexStack = mixedStack as HexStack;
            HexStack pureHexStack = pureStack as HexStack;
            
            if (mixedHexStack != null) {
                mixedHexStack.UpdateColliderSize();
            }
            if (pureHexStack != null) {
                pureHexStack.UpdateColliderSize();
            }

            // Reposition cells if not animating
            if (!animate) {
                if (mixedHexStack != null) {
                    _positionService.RepositionAllHexagons(mixedHexStack.Hexagons);
                }
                if (pureHexStack != null) {
                    _positionService.RepositionAllHexagons(pureHexStack.Hexagons);
                }
            }

            return true;
        }

        /// <summary>
        /// Executes mixed-to-mixed transfer step-by-step (one cell at a time).
        /// Transfers the top cell from mixedStack1 to mixedStack2 if top cell colors match.
        /// The recursive re-checking system will handle subsequent transfers.
        /// </summary>
        /// <returns>True if a transfer occurred, false otherwise.</returns>
        public async UniTask<bool> ExecuteMixedToMixedTransfer(IStack mixedStack1, IStack mixedStack2, IHexagonAnimationService animationService) {
            if (mixedStack1 == null || mixedStack2 == null) {
                return false;
            }

            // Check if we can transfer
            if (!CanTransferCell(mixedStack1, mixedStack2)) {
                return false;
            }

            bool animate = animationService != null;

            // Transfer only ONE cell (step-by-step behavior)
            HexCell topCell = RemoveTopCell(mixedStack1);
            if (topCell == null) {
                return false;
            }

            // Transfer to mixedStack2 and await animation
            await AddCellToTop(mixedStack2, topCell, animate, animationService);

            // Update colliders
            HexStack hexStack1 = mixedStack1 as HexStack;
            HexStack hexStack2 = mixedStack2 as HexStack;
            
            if (hexStack1 != null) {
                hexStack1.UpdateColliderSize();
            }
            if (hexStack2 != null) {
                hexStack2.UpdateColliderSize();
            }

            // Reposition cells if not animating
            if (!animate) {
                if (hexStack1 != null) {
                    _positionService.RepositionAllHexagons(hexStack1.Hexagons);
                }
                if (hexStack2 != null) {
                    _positionService.RepositionAllHexagons(hexStack2.Hexagons);
                }
            }

            return true;
        }

        /// <summary>
        /// Main entry point for processing two stacks. Analyzes states and executes appropriate sorting logic.
        /// </summary>
        public async UniTask<SortingResult> ProcessStackPair(IStack leftStack, IStack rightStack, IHexagonAnimationService animationService) {
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
                    bool transferred = await ExecuteMixedToPureTransfer(leftStack, rightStack, animationService);
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
                    bool transferred = await ExecuteMixedToPureTransfer(rightStack, leftStack, animationService);
                    if (transferred) {
                        int cellsMoved = cellsBefore - rightStack.Cells.Count;
                        return SortingResult.MixedTransfer(cellsMoved);
                    }
                }
            }
            
            // Mixed → Mixed transfer
            if (leftState == StackState.Mixed && rightState == StackState.Mixed) {
                ColorType? topCellColor1 = _stateAnalyzer.GetTopCellColor(leftStack);
                ColorType? topCellColor2 = _stateAnalyzer.GetTopCellColor(rightStack);
                
                if (topCellColor1.HasValue && topCellColor2.HasValue && topCellColor1.Value == topCellColor2.Value) {
                    // Track cells before transfer (cells are removed from leftStack)
                    int cellsBefore = leftStack.Cells.Count;
                    bool transferred = await ExecuteMixedToMixedTransfer(leftStack, rightStack, animationService);
                    if (transferred) {
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
        private async UniTask AddCellToTop(IStack stack, HexCell cell, bool animate, IHexagonAnimationService animationService) {
            if (stack == null || cell == null) {
                return;
            }

            if (stack is not HexStack hexStack) {
                return;
            }

            // Ensure Hexagons list exists
            if (hexStack.Hexagons == null) {
                return;
            }

            // Ensure collider service is initialized with cell size before calculating offsets
            // Use the cell being added, or an existing cell from the stack
            ICell cellForSize = cell;
            if (hexStack.Hexagons.Count > 0) {
                cellForSize = hexStack.Hexagons[0];
            }
            if (cellForSize != null) {
                _colliderService.CalculateCellColliderSize(cellForSize);
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
                // Animate the cell to its new position and await completion
                var cellTransforms = new List<Transform> { cell.Transform };
                var targetPositions = new List<Vector3> { targetLocalPosition };
                await animationService.AnimateHexagonStackMerge(
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

