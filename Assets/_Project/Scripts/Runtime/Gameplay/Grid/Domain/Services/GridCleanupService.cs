using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain.Models;
using _Project.Scripts.Runtime.Gameplay.Grid.Presentation;
using _Project.Scripts.Runtime.Gameplay.Stack.Services;
using _Project.Scripts.Runtime.Gameplay.Grid.Animation;
using _Project.Scripts.Runtime.Gameplay.Stack;
using _Project.Scripts.Runtime.Gameplay.Cell;

namespace _Project.Scripts.Runtime.Gameplay.Grid.Domain.Services {
    public class GridCleanupService {
        private readonly HexGrid _grid;
        private readonly StackSortingService _sortingService;
        private readonly IHexagonAnimationService _animationService;

        public GridCleanupService(
            HexGrid grid,
            StackSortingService sortingService,
            IHexagonAnimationService animationService) {
            _grid = grid;
            _sortingService = sortingService;
            _animationService = animationService;
        }

        public async UniTask ProcessPureMerges(HashSet<HexCoordinates> affectedSlots) {
            if (_grid == null || affectedSlots == null || affectedSlots.Count == 0) {
                return;
            }

            // Wait one frame to ensure all animations have fully completed
            await UniTask.Yield();

            bool anyPureMergesOccurred;
            do {
                anyPureMergesOccurred = false;

                foreach (HexCoordinates coordsToCheck in affectedSlots) {
                    HexStackSlot slotToCheck = _grid.GetSlot(coordsToCheck);
                    if (slotToCheck == null || slotToCheck.IsEmpty()) {
                        continue;
                    }

                    // Get neighbors of this slot
                    HexCoordinates[] neighbors = coordsToCheck.GetNeighbors();

                    foreach (HexStack stack in slotToCheck.Stacks) {
                        if (stack == null || stack.Cells == null || stack.Cells.Count == 0) {
                            continue;
                        }

                        // Check each neighbor
                        foreach (HexCoordinates neighborCoords in neighbors) {
                            HexStackSlot neighborSlot = _grid.GetSlot(neighborCoords);
                            if (neighborSlot == null || neighborSlot.IsEmpty()) {
                                continue;
                            }

                            foreach (HexStack neighborStack in neighborSlot.Stacks) {
                                if (neighborStack == null || neighborStack.Cells == null || neighborStack.Cells.Count == 0) {
                                    continue;
                                }

                                // Check if pure merge is possible
                                if (_sortingService.ShouldTriggerSorting(stack, neighborStack)) {
                                    // Execute the merge
                                    var result = await _sortingService.ProcessStackPair(stack, neighborStack, _animationService);
                                    if (result != null && result.WasSortingTriggered) {
                                        anyPureMergesOccurred = true;
                                        // Add both slots to affected slots for next iteration
                                        affectedSlots.Add(coordsToCheck);
                                        affectedSlots.Add(neighborCoords);
                                        // Wait for animation to complete
                                        await UniTask.Yield();
                                        break;
                                    }
                                }
                            }

                            if (anyPureMergesOccurred) break;
                        }

                        if (anyPureMergesOccurred) break;
                    }

                    if (anyPureMergesOccurred) break;
                }

                // Continue checking until no more pure merges are found
            } while (anyPureMergesOccurred);
        }

        public async UniTask CheckAndClearStacksWithTenPlusCells(HexCoordinates slotCoordinates) {
            if (_grid == null) {
                return;
            }

            HexStackSlot slot = _grid.GetSlot(slotCoordinates);
            if (slot == null || slot.IsEmpty()) {
                return;
            }

            // Wait one more frame to ensure all animations are fully complete
            await UniTask.Yield();

            // Check all stacks in the slot
            var stacksToClear = new List<HexStack>();

            foreach (HexStack stack in slot.Stacks) {
                if (stack == null) {
                    continue;
                }

                // Check if stack has 10+ cells
                if (stack.Cells != null && stack.Cells.Count >= 10) {
                    stacksToClear.Add(stack);
                }
            }

            // If no stacks to clear, return early
            if (stacksToClear.Count == 0) {
                return;
            }

            // Destroy all cells in stacks with 10+ cells, then destroy the stack GameObjects
            foreach (HexStack stackToClear in stacksToClear) {
                // Destroy all cells in the stack
                if (stackToClear.Hexagons != null) {
                    foreach (HexCell cell in stackToClear.Hexagons) {
                        if (cell != null && cell.gameObject != null) {
                            Object.Destroy(cell.gameObject);
                        }
                    }
                    stackToClear.Hexagons.Clear();
                }

                // Destroy the stack GameObject
                if (stackToClear.gameObject != null) {
                    Object.Destroy(stackToClear.gameObject);
                }
            }

            // Clear all stacks from the slot
            slot.ClearStacks();
        }

        public async UniTask CheckAndClearStacksWithTenPlusCells(HashSet<HexCoordinates> slotCoordinates) {
            if (slotCoordinates == null) {
                return;
            }

            foreach (HexCoordinates coords in slotCoordinates) {
                await CheckAndClearStacksWithTenPlusCells(coords);
            }
        }
    }
}

