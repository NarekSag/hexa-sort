using _Project.Scripts.Runtime.Gameplay.Grid.Animation;
using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain.Mappers;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain.Config;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain.Services;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain.Models;
using _Project.Scripts.Runtime.Gameplay.Stack.Services;
using _Project.Scripts.Runtime.Gameplay.Grid.Presentation.Controllers;

namespace _Project.Scripts.Runtime.Gameplay.Grid.Presentation {
    public class HexGridFactory 
    {
        public GridController Create(HexGridConfig config, HexAnimationConfig hexagonAnimationConfig, Transform parent = null) {
            if (config == null) {
                Debug.LogError("HexGridConfig is not assigned!");
                return null;
            }

            if (!config.IsValid()) {
                Debug.LogError("HexGridConfig is invalid! Check width, height, and cell prefab.");
                return null;
            }

            // Create grid container GameObject
            GameObject gridObject = new GameObject("HexGrid");
            if (parent != null) {
                gridObject.transform.SetParent(parent);
            }

            // Create registry and mapper
            HexSlotRegistry slotRegistry = new HexSlotRegistry();
            IHexGridMapper mapper = new HexGridMapper(config.Width, config.Height);
            
            // TODO: Refactor this stack service creation logic
            StackColliderService colliderService = new StackColliderService();
            StackPositionService positionService = new StackPositionService(colliderService);
            StackMergeService mergeService = new StackMergeService(colliderService, positionService);
            
            StackStateAnalyzer stateAnalyzer = new StackStateAnalyzer();
            StackSortingService sortingService = new StackSortingService(
                stateAnalyzer, 
                mergeService, 
                positionService, 
                colliderService);

            HexAnimationService animationService = new HexAnimationService(hexagonAnimationConfig);
            
            // Create grid services
            GridNeighborService neighborService = new GridNeighborService(slotRegistry, sortingService, animationService);
            GridRecursionService recursionService = new GridRecursionService();
            GridCleanupService cleanupService = new GridCleanupService(slotRegistry, sortingService, animationService);
            
            GridController gridController = new GridController(
                slotRegistry, 
                neighborService,
                recursionService,
                cleanupService);
            
            // Create all slots
            CreateAllSlots(config, mapper, gridObject.transform, slotRegistry, gridController);

            return gridController;
        }

        private void CreateAllSlots(
            HexGridConfig config, 
            IHexGridMapper mapper, 
            Transform gridTransform,
            HexSlotRegistry slotRegistry,
            GridController gridController) {
            
            for (int z = 0; z < config.Height; z++) {
                for (int x = 0; x < config.Width; x++) {
                    HexCoordinates coordinates = mapper.GetCoordinateFromOffset(x, z);
                    Vector3 position = mapper.GetWorldPositionFromOffset(x, z);
                    
                    HexSlot slot = CreateSlot(config, coordinates, position, gridTransform, gridController);
                    if (slot != null) {
                        slotRegistry.Register(coordinates, slot);
                    }
                }
            }
        }

        private HexSlot CreateSlot(
            HexGridConfig config,
            HexCoordinates coordinates, 
            Vector3 position, 
            Transform gridTransform,
            GridController gridController) {
            
            if (config.SlotPrefab == null) {
                Debug.LogError("HexStackSlot prefab is not assigned!");
                return null;
            }

            HexSlot slot = Object.Instantiate(config.SlotPrefab, gridTransform);
            slot.transform.localPosition = position;
            slot.Initialize(coordinates, gridController);
            return slot;
        }
    }
}

