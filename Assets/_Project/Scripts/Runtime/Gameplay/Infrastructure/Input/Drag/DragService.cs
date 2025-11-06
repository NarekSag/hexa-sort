using UnityEngine;
using VContainer;
using _Project.Scripts.Runtime.Gameplay.Infrastructure.Input.PositionCalculation;
using _Project.Scripts.Runtime.Gameplay.Infrastructure.Input.Raycast;
using _Project.Scripts.Runtime.Gameplay.Core.Interfaces;
using _Project.Scripts.Runtime.Gameplay.Infrastructure.State;
using _Project.Scripts.Runtime.Gameplay.Presentation.Grid.Slot;

namespace _Project.Scripts.Runtime.Gameplay.Infrastructure.Input.Drag
{
    public class DragService : MonoBehaviour
    {
        [Inject] private IInputService _inputService;
        [Inject] private IPositionCalculationService _positionCalculationService;
        [Inject] private IRaycastService _raycastService;
        [Inject] private GameplayStateManager _stateManager;
        [Inject] private BoosterInputService _boosterInputService;
        [Inject] private DropService _dropService;

        private IDraggable _currentDraggable;
        private Vector3 _originalPosition;
        private HexSlot _currentlyHighlightedSlot;

        private void Update()
        {
            // Check if we're in booster active mode - route input to booster service
            if (_stateManager.CurrentState.Value == GameplayState.BoosterActive)
            {
                HandleBoosterInput();
                return;
            }

            // Normal drag/drop logic
            // Check for mouse button down
            if (_inputService.GetMouseButtonDown(0))
            {
                StartDrag();
            }

            // Update drag position
            if (_currentDraggable != null)
            {
                UpdateDragPosition();
            }

            // Check for mouse button up
            if (_inputService.GetMouseButtonUp(0))
            {
                EndDrag();
            }
        }

        private void HandleBoosterInput()
        {
            // Check for mouse button down in booster mode
            if (_inputService.GetMouseButtonDown(0))
            {
                Camera mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    Vector3 mousePosition = _inputService.GetMousePosition();
                    Ray ray = mainCamera.ScreenPointToRay(mousePosition);
                    _boosterInputService.HandleBoosterClick(ray);
                }
            }
        }

        private void StartDrag()
        {
            // Clear any existing highlight when starting a new drag
            ClearHighlight();

            Camera mainCamera = Camera.main;
            if (mainCamera == null) return;

            Vector3 mousePosition = _inputService.GetMousePosition();
            Ray ray = mainCamera.ScreenPointToRay(mousePosition);

            // Use RaycastService to find a draggable object
            if (_raycastService.RaycastToDraggable(ray, out IDraggable draggable))
            {
                _currentDraggable = draggable;
                _originalPosition = draggable.GetPosition();
            }
        }

        private void UpdateDragPosition()
        {
            if (_currentDraggable == null) return;

            Vector3 mousePosition = _inputService.GetMousePosition();
            // Use the original Y position to create a drag plane at the stack's height
            // _originalPosition.y / 2 - so it's lower than the stack's height but higher than the ground level
            Vector3 worldPosition =
                _positionCalculationService.ScreenToWorldPosition(mousePosition, _originalPosition.y / 2);

            _currentDraggable.SetPosition(worldPosition);

            // Check for hovered slots and highlight empty ones
            UpdateSlotHighlight();
        }

        private void UpdateSlotHighlight()
        {
            if (_currentDraggable == null)
            {
                ClearHighlight();
                return;
            }

            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                ClearHighlight();
                return;
            }

            Vector3 mousePosition = _inputService.GetMousePosition();
            Ray ray = mainCamera.ScreenPointToRay(mousePosition);

            // Get colliders from the current draggable to ignore them in the raycast
            Collider[] collidersToIgnore = null;
            if (_currentDraggable is MonoBehaviour draggableMono)
            {
                collidersToIgnore = draggableMono.GetComponentsInChildren<Collider>();
            }

            // Check if we're hovering over a slot
            if (_raycastService.RaycastToPlacementTarget(ray, collidersToIgnore, out ISlot slot))
            {
                // Check if it's empty and can accept the current draggable
                if (slot.IsEmpty() && slot.CanAccept(_currentDraggable, out Vector3 _))
                {
                    // Check if it's a HexSlot for highlighting
                    if (slot is HexSlot hexSlot)
                    {
                        // Highlight this slot if it's different from the currently highlighted one
                        if (_currentlyHighlightedSlot != hexSlot)
                        {
                            ClearHighlight();
                            _currentlyHighlightedSlot = hexSlot;
                            hexSlot.SetHighlighted(true);
                        }
                    }

                    return;
                }
            }

            // Not hovering over a valid empty slot, clear highlight
            ClearHighlight();
        }

        private void ClearHighlight()
        {
            if (_currentlyHighlightedSlot != null)
            {
                _currentlyHighlightedSlot.RemoveHighlight();
                _currentlyHighlightedSlot = null;
            }
        }

        private void EndDrag()
        {
            if (_currentDraggable == null) return;

            // Use DropService to handle the drop logic
            _dropService.TryDrop(_currentDraggable, _originalPosition, out Vector3 _);

            // Clear any highlight when drag ends
            ClearHighlight();
            _currentDraggable = null;
        }
    }
}

