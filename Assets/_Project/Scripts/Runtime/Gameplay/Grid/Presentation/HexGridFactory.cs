using _Project.Scripts.Runtime.Gameplay.Grid.Animation;
using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain.Mappers;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain.Config;
using VContainer;
using _Project.Scripts.Runtime.Gameplay.Stack.Services;
using _Project.Scripts.Runtime.Gameplay.Stack.Controllers;

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
            
            // Create services for stack operations
            StackColliderService colliderService = new StackColliderService();
            StackPositionService positionService = new StackPositionService(colliderService);
            StackMergeService mergeService = new StackMergeService(colliderService, positionService);
            
            // Create controllers - StackController first
            StackController stackController = new StackController(mergeService, positionService, _animationService);
            
            // GridController needs the grid, but we'll create it and then set it
            Controllers.GridController gridController = new Controllers.GridController(grid, mapper, stackController);
            
            grid.Initialize(config, mapper, gridObject.transform, _animationService, gridController);

            return grid;
        }
    }
}

