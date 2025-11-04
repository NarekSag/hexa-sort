using UnityEngine;
using VContainer;
using _Project.Scripts.Runtime.Gameplay.Input.PositionCalculation;
using _Project.Scripts.Runtime.Gameplay.Input.Raycast;
using _Project.Scripts.Runtime.Gameplay.Grid.Presentation;
using _Project.Scripts.Runtime.Gameplay.Stack;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain;

namespace _Project.Scripts.Runtime.Gameplay.Input.Drag {
    public class DragService : MonoBehaviour {
        [Inject] private IInputService _inputService;
        [Inject] private IPositionCalculationService _positionCalculationService;
        [Inject] private IRaycastService _raycastService;
        
        private IDraggable _currentDraggable;
        private Vector3 _originalPosition;

        private void Update() {
            // Check for mouse button down
            if (_inputService.GetMouseButtonDown(0)) {
                StartDrag();
            }

            // Update drag position
            if (_currentDraggable != null) {
                UpdateDragPosition();
            }

            // Check for mouse button up
            if (_inputService.GetMouseButtonUp(0)) {
                EndDrag();
            }
        }

        private void StartDrag() {
            Camera camera = Camera.main;
            if (camera == null) return;

            Vector3 mousePosition = _inputService.GetMousePosition();
            Ray ray = camera.ScreenPointToRay(mousePosition);

            // Use RaycastService to find a draggable object
            if (_raycastService.RaycastToDraggable(ray, out IDraggable draggable)) {
                _currentDraggable = draggable;
                _originalPosition = draggable.GetPosition();
            }
        }

        private void UpdateDragPosition() {
            if (_currentDraggable == null) return;

            Vector3 mousePosition = _inputService.GetMousePosition();
            // Use the original Y position to create a drag plane at the stack's height
            // _originalPosition.y / 2 - so it's lower than the stack's height but higher than the ground level
            Vector3 worldPosition = _positionCalculationService.ScreenToWorldPosition(mousePosition, _originalPosition.y / 2);
            
            _currentDraggable.SetPosition(worldPosition);
        }

        private void EndDrag() {
            if (_currentDraggable == null) return;

            Camera camera = Camera.main;
            if (camera != null) 
            {
                Vector3 mousePosition = _inputService.GetMousePosition();
                Ray ray = camera.ScreenPointToRay(mousePosition);

                // Get colliders from the current draggable to ignore them in the raycast
                Collider[] collidersToIgnore = null;
                if (_currentDraggable is MonoBehaviour draggableMono) 
                {
                    collidersToIgnore = draggableMono.GetComponentsInChildren<Collider>();
                }

                // Use RaycastService to find a placement target, ignoring current draggable's colliders
                if (_raycastService.RaycastToPlacementTarget(ray, collidersToIgnore, out IPlacementTarget placementTarget))
                {
                    if (placementTarget.CanAccept(_currentDraggable, out Vector3 targetPosition))
                    {
                        _currentDraggable.SetPosition(targetPosition);
                        
                        // Register the stack with the slot if it's a HexStackSlot
                        if (placementTarget is ISlot slot && _currentDraggable is IStack stack)
                        {
                            slot.SetStack(stack);
                        }
                    }
                    else
                    {
                        // Cannot accept this draggable, snap back to original position
                        _currentDraggable.SetPosition(_originalPosition);
                    }
                } 
                else 
                {
                    // Not over a placement target, snap back to original position
                    _currentDraggable.SetPosition(_originalPosition);
                }
            } 
            else 
            {
                // No camera, just snap back
                _currentDraggable.SetPosition(_originalPosition);
            }

            _currentDraggable = null;
        }
    }
}

