using UnityEngine;
using VContainer;
using _Project.Scripts.Runtime.Gameplay.Input.PositionCalculation;

namespace _Project.Scripts.Runtime.Gameplay.Input.Drag {
    /// <summary>
    /// Service responsible for handling drag interactions with draggable objects.
    /// Follows SOLID principles by depending on IDraggable abstraction.
    /// </summary>
    public class DragService : MonoBehaviour {
        [Inject] private IInputService _inputService;
        [Inject] private IPositionCalculationService _positionCalculationService;
        
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

            // Try to find an IDraggable object under the mouse
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                IDraggable draggable = hit.collider.GetComponent<IDraggable>();
                if (draggable != null) {
                    _currentDraggable = draggable;
                    // Store the original position
                    _originalPosition = draggable.GetPosition();
                }
            }
        }

        private void UpdateDragPosition() {
            if (_currentDraggable == null) return;

            Vector3 mousePosition = _inputService.GetMousePosition();
            Vector3 worldPosition = _positionCalculationService.ScreenToWorldPosition(mousePosition);
            _currentDraggable.SetPosition(worldPosition);
        }

        private void EndDrag() {
            if (_currentDraggable != null) {
                // Snap back to original position
                _currentDraggable.SetPosition(_originalPosition);
                _currentDraggable = null;
            }
        }

        public void SetDraggable(IDraggable draggable) {
            _currentDraggable = draggable;
            if (draggable != null) {
                _originalPosition = draggable.GetPosition();
            }
        }

        public IDraggable GetCurrentDraggable() {
            return _currentDraggable;
        }
    }
}

