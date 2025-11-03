using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain.Models;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain.Mappers;
using _Project.Scripts.Runtime.Gameplay.Stack.Controllers;
using _Project.Scripts.Runtime.Gameplay.Stack.Services;
using _Project.Scripts.Runtime.Gameplay.Stack.Domain;
using _Project.Scripts.Runtime.Gameplay.Grid.Animation;
using _Project.Scripts.Runtime.Gameplay.Stack;

namespace _Project.Scripts.Runtime.Gameplay.Grid.Presentation.Controllers {
    public class GridController {
        private readonly HexGrid _grid;
        private readonly IHexGridMapper _mapper;
        private readonly StackController _stackController;
        private readonly StackSortingService _sortingService;
        private readonly IHexagonAnimationService _animationService;
        
        // Maximum recursion depth to prevent infinite loops
        private const int MAX_RECURSION_DEPTH = 50;

        public GridController(
            HexGrid grid, 
            IHexGridMapper mapper, 
            StackController stackController,
            StackSortingService sortingService,
            IHexagonAnimationService animationService) {
            _grid = grid;
            _mapper = mapper;
            _stackController = stackController;
            _sortingService = sortingService;
            _animationService = animationService;
        }

        /// <summary>
        /// Checks all neighbors in all 6 directions and triggers sorting if applicable.
        /// Recursively re-checks stacks that had transfers until no more transfers are possible.
        /// </summary>
        public void CheckNeighborsAndSort(HexCoordinates slotCoordinates) {
            // Fire and forget async operation - the recursive method will handle the async chain
            CheckNeighborsAndSortRecursive(slotCoordinates, new HashSet<HexCoordinates>(), 0).Forget();
        }

        /// <summary>
        /// Internal recursive method that checks neighbors and re-checks affected slots after transfers.
        /// </summary>
        /// <param name="slotCoordinates">The slot coordinates to check</param>
        /// <param name="visitedInThisCycle">Set of coordinates already checked in this recursion cycle</param>
        /// <param name="depth">Current recursion depth (prevents infinite loops)</param>
        private async UniTask CheckNeighborsAndSortRecursive(HexCoordinates slotCoordinates, HashSet<HexCoordinates> visitedInThisCycle, int depth) {
            // Prevent infinite recursion
            if (depth > MAX_RECURSION_DEPTH) {
                return;
            }

            // Prevent checking the same slot multiple times in one cycle
            if (visitedInThisCycle.Contains(slotCoordinates)) {
                return;
            }

            if (_grid == null) {
                return;
            }

            HexStackSlot slot = _grid.GetSlot(slotCoordinates);
            if (slot == null || slot.IsEmpty()) {
                return;
            }

            // Mark this slot as visited in this cycle
            visitedInThisCycle.Add(slotCoordinates);

            // Track slots that were affected by transfers and need to be re-checked
            HashSet<HexCoordinates> slotsToRecheck = new HashSet<HexCoordinates>();
            
            // Track if any transfers occurred in this check - if not, we won't recurse
            bool anyTransfersOccurred = false;

            // Get all 6 neighbors in all directions
            HexCoordinates[] allNeighbors = GetAllNeighbors(slotCoordinates);
            
            foreach (HexCoordinates neighborCoords in allNeighbors) {
                // Skip neighbors that are already being checked in this cycle (prevents ping-pong)
                if (visitedInThisCycle.Contains(neighborCoords)) {
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
                        SortingResult result = await _sortingService.ProcessStackPair(currentStack, neighborStack, _animationService);
                        
                        // If transfers occurred, mark both slots for re-checking
                        if (result != null && result.WasSortingTriggered) {
                            anyTransfersOccurred = true;
                            
                            // Re-check the source slot (where cells came from) - this is crucial for cascading
                            slotsToRecheck.Add(slotCoordinates);
                            
                            // Re-check the destination slot (where cells went to) - in case it can now accept more
                            slotsToRecheck.Add(neighborCoords);
                        }
                    }
                }
            }
            
            // Only recurse if transfers actually occurred
            if (!anyTransfersOccurred) {
                return;
            }

            // Recursively re-check all slots that had transfers
            // This ensures that after cells move, we check if more cells can move
            // Important: We create a new visited set that excludes the slots we're about to re-check,
            // but includes all others to prevent cycles while still allowing re-checking of changed slots
            foreach (HexCoordinates coordsToRecheck in slotsToRecheck) {
                // Create a new visited set that excludes this coordinate (so we can re-check it)
                // but includes all others to prevent infinite loops
                HashSet<HexCoordinates> newVisitedSet = new HashSet<HexCoordinates>(visitedInThisCycle);
                newVisitedSet.Remove(coordsToRecheck); // Remove the slot we're re-checking
                await CheckNeighborsAndSortRecursive(coordsToRecheck, newVisitedSet, depth + 1);
            }
        }

        /// <summary>
        /// Legacy method kept for backward compatibility. Calls CheckNeighborsAndSort instead.
        /// </summary>
        public void CheckNeighborsAndMerge(HexCoordinates slotCoordinates) {
            CheckNeighborsAndSort(slotCoordinates);
        }

        /// <summary>
        /// Gets all 6 neighbors of the given coordinates in all directions.
        /// Includes: East, Northeast, Northwest, West, Southwest, Southeast.
        /// </summary>
        private HexCoordinates[] GetAllNeighbors(HexCoordinates coordinates) {
            return coordinates.GetNeighbors();
        }

        public bool IsValidCoordinate(HexCoordinates coordinates) {
            return _mapper != null && _mapper.IsValidCoordinate(coordinates);
        }

        public HexStackSlot GetSlot(HexCoordinates coordinates) {
            return _grid?.GetSlot(coordinates);
        }

        public HexCoordinates[] GetNeighbors(HexCoordinates coordinates) {
            return coordinates.GetNeighbors();
        }
    }
}

