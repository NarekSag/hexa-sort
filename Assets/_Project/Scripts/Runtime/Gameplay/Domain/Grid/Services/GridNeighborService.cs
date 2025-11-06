using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using _Project.Scripts.Runtime.Gameplay.Core.Models;
using _Project.Scripts.Runtime.Gameplay.Presentation.Grid.Slot;
using _Project.Scripts.Runtime.Gameplay.Domain.Stack.Services;
using _Project.Scripts.Runtime.Gameplay.Core.Interfaces;
using _Project.Scripts.Runtime.Gameplay.Domain.Stack.Models;

namespace _Project.Scripts.Runtime.Gameplay.Domain.Grid.Services {
    public class GridNeighborService {
        private readonly HexSlotRegistry _slotRegistry;
        private readonly StackSortingService _sortingService;

        public GridNeighborService(
            HexSlotRegistry slotRegistry,
            StackSortingService sortingService) {
            _slotRegistry = slotRegistry;
            _sortingService = sortingService;
        }

        public async UniTask<NeighborProcessingResult> ProcessNeighbors(
            HexCoordinates slotCoordinates,
            HashSet<HexCoordinates> visitedSlots,
            HashSet<HexCoordinates> slotsThatReceivedCells) {
            
            ISlot slot = _slotRegistry?.GetSlot(slotCoordinates);
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

                ISlot neighborSlot = _slotRegistry.GetSlot(neighborCoords);
                if (neighborSlot == null || neighborSlot.IsEmpty()) {
                    continue;
                }

                // Create snapshots of stacks to avoid collection modification during async operations
                var currentStacks = new List<IStack>(slot.Stacks);
                var neighborStacks = new List<IStack>(neighborSlot.Stacks);

                // Process each stack in current slot with each stack in neighbor slot
                foreach (IStack currentStack in currentStacks) {
                    if (currentStack == null || currentStack.Cells == null || currentStack.Cells.Count == 0) {
                        continue;
                    }

                    foreach (IStack neighborStack in neighborStacks) {
                        if (neighborStack == null || neighborStack.Cells == null || neighborStack.Cells.Count == 0) {
                            continue;
                        }

                        // Process sorting between the two stacks and await animation completion
                        SortingResult sortingResult = await _sortingService.ProcessStackPair(currentStack, neighborStack, animate: true);

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
            
            ISlot slot = _slotRegistry?.GetSlot(slotCoordinates);
            if (slot == null || slot.IsEmpty()) {
                return false;
            }

            HexCoordinates[] allNeighbors = slotCoordinates.GetNeighbors();

            // Create snapshot of stacks to avoid collection modification issues
            var currentStacks = new List<IStack>(slot.Stacks);

            foreach (IStack stack in currentStacks) {
                if (stack == null || stack.Cells == null || stack.Cells.Count == 0) {
                    continue;
                }

                foreach (HexCoordinates neighborCoords in allNeighbors) {
                    // Skip if already visited (unless it received cells)
                    if (visitedSlots.Contains(neighborCoords) && !slotsThatReceivedCells.Contains(neighborCoords)) {
                        continue;
                    }

                    ISlot neighborSlot = _slotRegistry.GetSlot(neighborCoords);
                    if (neighborSlot == null || neighborSlot.IsEmpty()) {
                        continue;
                    }

                    // Create snapshot of neighbor stacks
                    var neighborStacks = new List<IStack>(neighborSlot.Stacks);

                    foreach (IStack neighborStack in neighborStacks) {
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

