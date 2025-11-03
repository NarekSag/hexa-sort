using _Project.Scripts.Runtime.Gameplay.Grid.Animation;
using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain.Mappers;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain.Config;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain.Services;
using VContainer;
using _Project.Scripts.Runtime.Gameplay.Stack.Services;
using _Project.Scripts.Runtime.Gameplay.Grid.Presentation.Controllers;

namespace _Project.Scripts.Runtime.Gameplay.Grid.Presentation {
    public class HexGridFactory 
    {
        [Inject] private IHexagonAnimationService _animationService;

        public HexGrid Create(HexGridConfig config, Transform parent = null) {
            if (config == null) {
                Debug.LogError("HexGridConfig is not assigned!");
                return null;
            }

            if (!config.IsValid()) {
                Debug.LogError("HexGridConfig is invalid! Check width, height, and cell prefab.");
                return null;
            }

            GameObject gridObject = new GameObject("HexGrid");
            if (parent != null) {
                gridObject.transform.SetParent(parent);
            }

            HexGrid grid = new HexGrid();
            IHexGridMapper mapper = new HexGridMapper(config.Width, config.Height);
            
            //TODO: Refactor this logic
            StackColliderService colliderService = new StackColliderService();
            StackPositionService positionService = new StackPositionService(colliderService);
            StackMergeService mergeService = new StackMergeService(colliderService, positionService);
            
            StackStateAnalyzer stateAnalyzer = new StackStateAnalyzer();
            StackSortingService sortingService = new StackSortingService(
                stateAnalyzer, 
                mergeService, 
                positionService, 
                colliderService);
            
            GridNeighborService neighborService = new GridNeighborService(grid, sortingService, _animationService);
            GridRecursionService recursionService = new GridRecursionService();
            GridCleanupService cleanupService = new GridCleanupService(grid, sortingService, _animationService);
            
            GridController gridController = new GridController(
                grid, 
                neighborService,
                recursionService,
                cleanupService);
            
            grid.Initialize(config, mapper, gridObject.transform, _animationService, gridController);

            return grid;
        }
    }
}

