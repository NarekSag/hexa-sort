using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain.Models;
using _Project.Scripts.Runtime.Gameplay.Grid.Presentation;
using _Project.Scripts.Runtime.Gameplay.Stack.Services;
using _Project.Scripts.Runtime.Gameplay.Stack.Domain;
using _Project.Scripts.Runtime.Gameplay.Grid.Animation;
using _Project.Scripts.Runtime.Gameplay.Stack;

namespace _Project.Scripts.Runtime.Gameplay.Grid.Domain.Services {
    public class GridNeighborService {
        private readonly HexGrid _grid;
        private readonly StackSortingService _sortingService;
        private readonly IHexagonAnimationService _animationService;

        public GridNeighborService(
            HexGrid grid,
            StackSortingService sortingService,
            IHexagonAnimationService animationService) {
            _grid = grid;
            _sortingService = sortingService;
            _animationService = animationService;
        }

        public async UniTask<NeighborProcessingResult> ProcessNeighbors(
            HexCoordinates slotCoordinates,
            HashSet<HexCoordinates> visitedSlots,
            HashSet<HexCoordinates> slotsThatReceivedCells) {
            
            HexStackSlot slot = _grid?.GetSlot(slotCoordinates);
            if (slot == null || slot.IsEmpty()) {
                return NeighborProcessingResult.None();
            }

            var result = new NeighborProcessingResult();
            HexCoordinates[] allNeighbors = slotCoordinates.GetNeighbors();

            foreach (HexCoordinates neighborCoords in allNeighbors) {
                // Skip neighbors that are already being checked, UNLESS they received cells
                if (visitedSlots.Contains(neighborCoords) && !slotsThatReceivedCells.Contains(neighborCoords)) {
                    continue;
                }

                HexStackSlot neighborSlot = _grid.GetSlot(neighborCoords);
                if (neighborSlot == null || neighborSlot.IsEmpty()) {
                    continue;
                }

                // Process each stack in current slot with each stack in neighbor slot
                foreach (HexStack currentStack in slot.Stacks) {
                    if (currentStack == null || currentStack.Cells == null || currentStack.Cells.Count == 0) {
                        continue;
                    }

                    foreach (HexStack neighborStack in neighborSlot.Stacks) {
                        if (neighborStack == null || neighborStack.Cells == null || neighborStack.Cells.Count == 0) {
                            continue;
                        }

                        // Process sorting between the two stacks and await animation completion
                        SortingResult sortingResult = await _sortingService.ProcessStackPair(currentStack, neighborStack, _animationService);

                        // If transfers occurred, mark both slots for re-checking
                        if (sortingResult != null && sortingResult.WasSortingTriggered) {
                            result.AnyTransfersOccurred = true;

                            // Re-check the source slot (where cells came from)
                            result.SlotsToRecheck.Add(slotCoordinates);

                            // Re-check the destination slot (where cells went to)
                            result.SlotsToRecheck.Add(neighborCoords);

                            // Mark the destination as having received cells
                            result.SlotsThatReceivedCells.Add(neighborCoords);
                        }
                    }
                }
            }

            return result;
        }

        public bool HasPotentialPureMerges(
            HexCoordinates slotCoordinates,
            HashSet<HexCoordinates> visitedSlots,
            HashSet<HexCoordinates> slotsThatReceivedCells) {
            
            HexStackSlot slot = _grid?.GetSlot(slotCoordinates);
            if (slot == null || slot.IsEmpty()) {
                return false;
            }

            HexCoordinates[] allNeighbors = slotCoordinates.GetNeighbors();

            foreach (HexStack stack in slot.Stacks) {
                if (stack == null || stack.Cells == null || stack.Cells.Count == 0) {
                    continue;
                }

                foreach (HexCoordinates neighborCoords in allNeighbors) {
                    // Skip if already visited (unless it received cells)
                    if (visitedSlots.Contains(neighborCoords) && !slotsThatReceivedCells.Contains(neighborCoords)) {
                        continue;
                    }

                    HexStackSlot neighborSlot = _grid.GetSlot(neighborCoords);
                    if (neighborSlot == null || neighborSlot.IsEmpty()) {
                        continue;
                    }

                    foreach (HexStack neighborStack in neighborSlot.Stacks) {
                        if (neighborStack == null || neighborStack.Cells == null || neighborStack.Cells.Count == 0) {
                            continue;
                        }

                        // Check if any sorting is possible (including pure-to-pure merge)
                        if (_sortingService.ShouldTriggerSorting(stack, neighborStack)) {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }

    public class NeighborProcessingResult {
        public bool AnyTransfersOccurred { get; set; }
        public HashSet<HexCoordinates> SlotsToRecheck { get; set; } = new HashSet<HexCoordinates>();
        public HashSet<HexCoordinates> SlotsThatReceivedCells { get; set; } = new HashSet<HexCoordinates>();

        public static NeighborProcessingResult None() {
            return new NeighborProcessingResult();
        }
    }
}

