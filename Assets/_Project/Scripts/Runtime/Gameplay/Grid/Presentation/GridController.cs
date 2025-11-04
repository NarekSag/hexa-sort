using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain.Models;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain.Services;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain;

namespace _Project.Scripts.Runtime.Gameplay.Grid.Presentation.Controllers {
    public class GridController {
        private readonly HexSlotRegistry _slotRegistry;
        private readonly GridNeighborService _neighborService;
        private readonly GridRecursionService _recursionService;
        private readonly GridCleanupService _cleanupService;

        public GridController(
            HexSlotRegistry slotRegistry, 
            GridNeighborService neighborService,
            GridRecursionService recursionService,
            GridCleanupService cleanupService) {
            _slotRegistry = slotRegistry;
            _neighborService = neighborService;
            _recursionService = recursionService;
            _cleanupService = cleanupService;
        }

        public void CheckNeighborsAndSort(HexCoordinates slotCoordinates) {
            CheckNeighborsAndSortRecursive(slotCoordinates, new HashSet<HexCoordinates>(), 0).Forget();
        }

        private async UniTask CheckNeighborsAndSortRecursive(
            HexCoordinates slotCoordinates,
            HashSet<HexCoordinates> visitedInThisCycle,
            int depth,
            HashSet<HexCoordinates> slotsThatReceivedCells = null) {
            
            slotsThatReceivedCells ??= new HashSet<HexCoordinates>();

            // Check if recursion should continue
            if (!_recursionService.ShouldContinueRecursion(slotCoordinates, visitedInThisCycle, depth)) {
                return;
            }

            ISlot slot = _slotRegistry?.GetSlot(slotCoordinates);
            if (slot == null || slot.IsEmpty()) {
                return;
            }

            // Mark this slot as visited
            visitedInThisCycle.Add(slotCoordinates);

            // Process neighbors and get result
            NeighborProcessingResult neighborResult = await _neighborService.ProcessNeighbors(
                slotCoordinates,
                visitedInThisCycle,
                slotsThatReceivedCells);

            // Merge slots that received cells into the accumulated set
            foreach (HexCoordinates receivedCoords in neighborResult.SlotsThatReceivedCells) {
                slotsThatReceivedCells.Add(receivedCoords);
            }

            // If no transfers occurred, check for potential pure merges
            if (!neighborResult.AnyTransfersOccurred) {
                bool hasPotentialPureMerges = _neighborService.HasPotentialPureMerges(
                    slotCoordinates,
                    visitedInThisCycle,
                    slotsThatReceivedCells);

                if (!hasPotentialPureMerges) {
                    // If no potential merges and we're at depth 0, do cleanup
                    if (depth == 0) {
                        await _cleanupService.CheckAndClearStacksWithTenPlusCells(slotCoordinates);
                    }
                    return;
                }

                // If there are potential pure merges, mark slots for re-checking
                neighborResult.SlotsToRecheck.Add(slotCoordinates);
                HexCoordinates[] allNeighbors = slotCoordinates.GetNeighbors();
                foreach (HexCoordinates neighborCoords in allNeighbors) {
                    ISlot neighborSlot = _slotRegistry.GetSlot(neighborCoords);
                    if (neighborSlot != null && !neighborSlot.IsEmpty()) {
                        neighborResult.SlotsToRecheck.Add(neighborCoords);
                    }
                }
            }

            // Recursively re-check all slots that had transfers or potential merges
            foreach (HexCoordinates coordsToRecheck in neighborResult.SlotsToRecheck) {
                HashSet<HexCoordinates> newVisitedSet = _recursionService.CreateVisitedSetForRecursion(
                    visitedInThisCycle,
                    coordsToRecheck,
                    slotsThatReceivedCells);

                await CheckNeighborsAndSortRecursive(coordsToRecheck, newVisitedSet, depth + 1, slotsThatReceivedCells);
            }

            // After all recursive transfers complete (at depth 0), do final cleanup
            if (depth == 0) {
                // Collect all affected slots
                HashSet<HexCoordinates> allAffectedSlots = new HashSet<HexCoordinates>();
                foreach (HexCoordinates coords in neighborResult.SlotsToRecheck) {
                    allAffectedSlots.Add(coords);
                }
                allAffectedSlots.Add(slotCoordinates);

                // Process pure merges and clear stacks with 10+ cells
                await _cleanupService.ProcessPureMerges(allAffectedSlots);
                await _cleanupService.CheckAndClearStacksWithTenPlusCells(allAffectedSlots);
            }
        }
    }
}

