using UnityEngine;
using VContainer;
using _Project.Scripts.Runtime.Gameplay.Infrastructure.Input.Raycast;
using _Project.Scripts.Runtime.Gameplay.Core.Interfaces;
using _Project.Scripts.Runtime.Gameplay.Core.Models;
using _Project.Scripts.Runtime.Gameplay.Presentation.Grid.Controllers;
using _Project.Scripts.Runtime.Gameplay.Infrastructure.State;
using _Project.Scripts.Runtime.Gameplay.Domain.Boosters;

namespace _Project.Scripts.Runtime.Gameplay.Infrastructure.Input
{
    public class BoosterInputService
    {
        private readonly IRaycastService _raycastService;
        private readonly GameplayStateManager _stateManager;
        private readonly BoosterManager _boosterManager;
        private GridController _gridController;
        
        public BoosterInputService(
            IRaycastService raycastService,
            GameplayStateManager stateManager,
            BoosterManager boosterManager)
        {
            _raycastService = raycastService;
            _stateManager = stateManager;
            _boosterManager = boosterManager;
        }
        
        public void SetGridController(GridController gridController)
        {
            _gridController = gridController;
        }
        
        public void HandleBoosterClick(Ray ray)
        {
            if (_gridController == null || _stateManager.CurrentState.Value != GameplayState.BoosterActive)
            {
                return;
            }
            
            // Use RaycastService to find a slot
            if (_raycastService.RaycastToPlacementTarget(ray, out IPlacementTarget placementTarget))
            {
                if (placementTarget is ISlot slot)
                {
                    // Check if slot is not empty
                    if (!slot.IsEmpty())
                    {
                        // Get coordinates from slot
                        HexCoordinates coordinates = slot.GetCoordinates();
                        
                        // Destroy stack at slot (don't count towards progression)
                        _gridController.DestroyStackAtSlot(coordinates, countTowardsProgression: false);
                        
                        // Mark booster as used
                        var activeBooster = _boosterManager.ActiveBooster;
                        if (activeBooster != null)
                        {
                            _boosterManager.MarkBoosterAsUsed(activeBooster.Id);
                        }
                        
                        // Return to playing state
                        _stateManager.SetState(GameplayState.Playing);
                        _boosterManager.ClearActiveBooster();
                    }
                }
            }
        }
    }
}

