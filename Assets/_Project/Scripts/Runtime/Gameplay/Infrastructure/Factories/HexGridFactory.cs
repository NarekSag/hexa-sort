using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Core.Models;
using _Project.Scripts.Runtime.Gameplay.Domain.Grid.Services;
using _Project.Scripts.Runtime.Gameplay.Domain.Stack.Services;
using _Project.Scripts.Runtime.Gameplay.Presentation.Grid.Controllers;
using _Project.Scripts.Runtime.Gameplay.Presentation.Grid.Slot;
using _Project.Scripts.Runtime.Gameplay.Domain.Grid.Models;

namespace _Project.Scripts.Runtime.Gameplay.Infrastructure.Factories
{
    public class HexGridFactory
    {
        public GridController Create(HexSlot slotPrefab, LevelData levelData, Transform parent = null)
        {
            if (slotPrefab == null)
            {
                Debug.LogError("SlotPrefab is not assigned!");
                return null;
            }

            if (levelData == null)
            {
                Debug.LogError("LevelData is not provided!");
                return null;
            }

            int gridWidth = levelData.GridWidth;
            int gridHeight = levelData.GridHeight;

            // Create grid container GameObject
            GameObject gridObject = new GameObject("HexGrid");
            if (parent != null)
            {
                gridObject.transform.SetParent(parent);
            }

            // Create registry and mapper
            HexSlotRegistry slotRegistry = new HexSlotRegistry();
            IHexGridMapper mapper = new HexGridMapper(gridWidth, gridHeight);

            // TODO: Refactor this stack service creation logic
            StackMergeService mergeService = new StackMergeService();
            StackStateAnalyzer stateAnalyzer = new StackStateAnalyzer();
            StackSortingService sortingService = new StackSortingService(
                stateAnalyzer,
                mergeService);

            // Create grid services
            GridNeighborService neighborService = new GridNeighborService(slotRegistry, sortingService);
            GridRecursionService recursionService = new GridRecursionService();
            GridCleanupService cleanupService = new GridCleanupService(slotRegistry, sortingService);

            GridController gridController = new GridController(
                slotRegistry,
                neighborService,
                recursionService,
                cleanupService);

            // Set the grid transform reference
            gridController.SetGridTransform(gridObject.transform);

            // Create all slots
            CreateAllSlots(slotPrefab, mapper, gridObject.transform, slotRegistry, gridController);

            return gridController;
        }

        private void CreateAllSlots(
            HexSlot slotPrefab,
            IHexGridMapper mapper,
            Transform gridTransform,
            HexSlotRegistry slotRegistry,
            GridController gridController)
        {
            // Use mapper dimensions
            for (int z = 0; z < mapper.Height; z++)
            {
                for (int x = 0; x < mapper.Width; x++)
                {
                    HexCoordinates coordinates = mapper.GetCoordinateFromOffset(x, z);
                    Vector3 position = mapper.GetWorldPositionFromOffset(x, z);

                    HexSlot slot = CreateSlot(slotPrefab, coordinates, position, gridTransform, gridController);
                    if (slot != null)
                    {
                        slotRegistry.Register(coordinates, slot);
                    }
                }
            }
        }

        private HexSlot CreateSlot(
            HexSlot slotPrefab,
            HexCoordinates coordinates,
            Vector3 position,
            Transform gridTransform,
            GridController gridController)
        {
            HexSlot slot = Object.Instantiate(slotPrefab, gridTransform);
            slot.transform.localPosition = position;
            slot.Initialize(coordinates, gridController);
            return slot;
        }
    }
}

