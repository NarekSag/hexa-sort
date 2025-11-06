using System;
using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using _Project.Scripts.Runtime.Gameplay.Core.Models;
using _Project.Scripts.Runtime.Gameplay.Domain.Grid.Services;
using _Project.Scripts.Runtime.Gameplay.Core.Interfaces;
using _Project.Scripts.Runtime.Gameplay.Presentation.Grid.Slot;

namespace _Project.Scripts.Runtime.Gameplay.Presentation.Grid.Controllers
{
    public class GridController
    {
        private readonly HexSlotRegistry _slotRegistry;
        private readonly GridNeighborService _neighborService;
        private readonly GridRecursionService _recursionService;
        private readonly GridCleanupService _cleanupService;

        // Queue system to prevent concurrent operations
        private readonly Queue<HexCoordinates> _operationQueue = new Queue<HexCoordinates>();
        private bool _isProcessing = false;

        public Transform GridTransform { get; private set; }
        public GridCleanupService CleanupService => _cleanupService;

        public event Action OnOperationsComplete;

        public GridController(
            HexSlotRegistry slotRegistry,
            GridNeighborService neighborService,
            GridRecursionService recursionService,
            GridCleanupService cleanupService)
        {
            _slotRegistry = slotRegistry;
            _neighborService = neighborService;
            _recursionService = recursionService;
            _cleanupService = cleanupService;
        }

        public void SetGridTransform(Transform gridTransform)
        {
            GridTransform = gridTransform;
        }

        public void CheckNeighborsAndSort(HexCoordinates slotCoordinates)
        {
            // Add to queue and start processing if not already running
            _operationQueue.Enqueue(slotCoordinates);

            if (!_isProcessing)
            {
                ProcessQueue().Forget();
            }
        }

        private async UniTask ProcessQueue()
        {
            if (_isProcessing)
            {
                return;
            }

            _isProcessing = true;

            try
            {
                while (_operationQueue.Count > 0)
                {
                    HexCoordinates coordinates = _operationQueue.Dequeue();
                    await CheckNeighborsAndSortRecursive(coordinates, new HashSet<HexCoordinates>(), 0);
                }
            }
            finally
            {
                _isProcessing = false;
            }
        }

        private async UniTask CheckNeighborsAndSortRecursive(
            HexCoordinates slotCoordinates,
            HashSet<HexCoordinates> visitedInThisCycle,
            int depth,
            HashSet<HexCoordinates> slotsThatReceivedCells = null)
        {
            slotsThatReceivedCells ??= new HashSet<HexCoordinates>();

            // Check if recursion should continue
            if (!_recursionService.ShouldContinueRecursion(slotCoordinates, visitedInThisCycle, depth))
            {
                return;
            }

            ISlot slot = _slotRegistry?.GetSlot(slotCoordinates);
            if (slot == null || slot.IsEmpty())
            {
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
            foreach (HexCoordinates receivedCoords in neighborResult.SlotsThatReceivedCells)
            {
                slotsThatReceivedCells.Add(receivedCoords);
            }

            // If no transfers occurred, check for potential pure merges
            if (!neighborResult.AnyTransfersOccurred)
            {
                bool hasPotentialPureMerges = _neighborService.HasPotentialPureMerges(
                    slotCoordinates,
                    visitedInThisCycle,
                    slotsThatReceivedCells);

                if (!hasPotentialPureMerges)
                {
                    // If no potential merges and we're at depth 0, do cleanup
                    if (depth == 0)
                    {
                        await _cleanupService.CheckAndClearStacksWithTenPlusCells(slotCoordinates);
                        // Notify that operations are complete
                        OnOperationsComplete?.Invoke();
                    }

                    return;
                }

                // If there are potential pure merges, mark slots for re-checking
                neighborResult.SlotsToRecheck.Add(slotCoordinates);
                HexCoordinates[] allNeighbors = slotCoordinates.GetNeighbors();
                foreach (HexCoordinates neighborCoords in allNeighbors)
                {
                    ISlot neighborSlot = _slotRegistry.GetSlot(neighborCoords);
                    if (neighborSlot != null && !neighborSlot.IsEmpty())
                    {
                        neighborResult.SlotsToRecheck.Add(neighborCoords);
                    }
                }
            }

            // Recursively re-check all slots that had transfers or potential merges
            foreach (HexCoordinates coordsToRecheck in neighborResult.SlotsToRecheck)
            {
                HashSet<HexCoordinates> newVisitedSet = _recursionService.CreateVisitedSetForRecursion(
                    visitedInThisCycle,
                    coordsToRecheck,
                    slotsThatReceivedCells);

                await CheckNeighborsAndSortRecursive(coordsToRecheck, newVisitedSet, depth + 1, slotsThatReceivedCells);
            }

            // After all recursive transfers complete (at depth 0), do final cleanup
            if (depth == 0)
            {
                // Collect all affected slots
                HashSet<HexCoordinates> allAffectedSlots = new HashSet<HexCoordinates>();
                foreach (HexCoordinates coords in neighborResult.SlotsToRecheck)
                {
                    allAffectedSlots.Add(coords);
                }

                allAffectedSlots.Add(slotCoordinates);

                // Process pure merges and clear stacks with 10+ cells
                await _cleanupService.ProcessPureMerges(allAffectedSlots);
                await _cleanupService.CheckAndClearStacksWithTenPlusCells(allAffectedSlots);

                // Notify that operations are complete (allows external systems to check for failure)
                OnOperationsComplete?.Invoke();
            }
        }

        public bool IsLevelFailed()
        {
            if (_slotRegistry == null)
            {
                return false;
            }

            // Check if all slots are not empty (all cells on grid are filled)
            foreach (ISlot slot in _slotRegistry.GetAllSlots())
            {
                if (slot == null || slot.IsEmpty())
                {
                    return false; // Found an empty slot, level hasn't failed
                }
            }

            // All slots are filled - level has failed
            return true;
        }

        /// <summary>
        /// Destroys all stacks at the specified slot coordinates.
        /// Used by boosters to destroy stacks.
        /// </summary>
        /// <param name="coordinates">The coordinates of the slot to destroy stacks at</param>
        /// <param name="countTowardsProgression">Whether cleared cells should count towards level progression (default: true)</param>
        public void DestroyStackAtSlot(HexCoordinates coordinates, bool countTowardsProgression = true)
        {
            if (_slotRegistry == null)
            {
                return;
            }

            ISlot slot = _slotRegistry.GetSlot(coordinates);
            if (slot == null || slot.IsEmpty())
            {
                return;
            }

            // Count total cells before destroying
            int totalCellsCleared = 0;

            // Create a copy of stacks list to avoid modification during iteration
            var stacksToDestroy = new System.Collections.Generic.List<IStack>(slot.Stacks);

            // Destroy all cells in stacks, then destroy the stack GameObjects
            foreach (IStack stackToDestroy in stacksToDestroy)
            {
                if (stackToDestroy == null)
                {
                    continue;
                }

                // Count cells in this stack
                if (stackToDestroy.Cells != null)
                {
                    totalCellsCleared += stackToDestroy.Cells.Count;

                    // Destroy all cells in the stack
                    foreach (var cell in stackToDestroy.Cells)
                    {
                        if (cell != null && cell.Transform.gameObject != null)
                        {
                            UnityEngine.Object.Destroy(cell.Transform.gameObject);
                        }
                    }

                    stackToDestroy.Cells.Clear();
                }

                // Destroy the stack GameObject
                if (stackToDestroy.Transform.gameObject != null)
                {
                    UnityEngine.Object.Destroy(stackToDestroy.Transform.gameObject);
                }
            }

            // Clear all stacks from the slot
            slot.ClearStacks();

            // Notify cleanup service about cells cleared (for level progression)
            // Only if countTowardsProgression is true (boosters should not add points)
            if (countTowardsProgression && totalCellsCleared > 0)
            {
                _cleanupService.NotifyCellsCleared(totalCellsCleared);
            }

            // Trigger operations complete event
            OnOperationsComplete?.Invoke();
        }
    }
}


