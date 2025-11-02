using UnityEngine;

namespace _Project.Scripts.Runtime.Gameplay.Input.PositionCalculation {
    public class PositionCalculationService : IPositionCalculationService {
        public Vector3 ScreenToWorldPosition(Vector3 screenPos) {
            return ScreenToWorldPosition(screenPos, 0f);
        }

        public Vector3 ScreenToWorldPosition(Vector3 screenPos, float yHeight) {
            Camera camera = Camera.main;
            if (camera == null) {
                Debug.LogError("Camera.main is not assigned!");
                return Vector3.zero;
            }

            // Create a drag plane at the specified Y height
            Plane dragPlane = new Plane(Vector3.up, new Vector3(0f, yHeight, 0f));

            // Raycast from camera through screen position to the drag plane
            Ray ray = camera.ScreenPointToRay(screenPos);
            
            if (dragPlane.Raycast(ray, out float distance)) {
                return ray.GetPoint(distance);
            }

            // Fallback: convert screen to world at the specified Y height
            Vector3 fallback = camera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, camera.nearClipPlane + 10f));
            fallback.y = yHeight;
            return fallback;
        }
    }
}

