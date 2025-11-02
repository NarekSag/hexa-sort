using UnityEngine;

namespace _Project.Scripts.Runtime.Gameplay.Input.PositionCalculation {
    public class PositionCalculationService : IPositionCalculationService {
        private readonly Plane _dragPlane;

        public PositionCalculationService() {
            // Default drag plane at Y=0 (ground level)
            _dragPlane = new Plane(Vector3.up, Vector3.zero);
        }

        public Vector3 ScreenToWorldPosition(Vector3 screenPos) {
            Camera camera = Camera.main;
            if (camera == null) {
                Debug.LogError("Camera.main is not assigned!");
                return Vector3.zero;
            }

            // Raycast from camera through screen position to the drag plane
            Ray ray = camera.ScreenPointToRay(screenPos);
            
            if (_dragPlane.Raycast(ray, out float distance)) {
                return ray.GetPoint(distance);
            }

            // Fallback: convert screen to world at a fixed depth
            return camera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, camera.nearClipPlane + 10f));
        }
    }
}

