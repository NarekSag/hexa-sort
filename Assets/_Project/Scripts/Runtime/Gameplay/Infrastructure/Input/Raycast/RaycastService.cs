using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Core.Interfaces;

namespace _Project.Scripts.Runtime.Gameplay.Infrastructure.Input.Raycast {
    public class RaycastService : IRaycastService {
        private const float MaxRaycastDistance = 1000f;

        public bool RaycastToPlacementTarget(Ray ray, out IPlacementTarget placementTarget) {
            return RaycastToPlacementTarget(ray, null, out placementTarget);
        }

        public bool RaycastToPlacementTarget(Ray ray, Collider[] ignoreColliders, out IPlacementTarget placementTarget) {
            placementTarget = null;

            // Use RaycastAll to get all hits, allowing us to ignore specific colliders
            RaycastHit[] hits = Physics.RaycastAll(ray, MaxRaycastDistance);
            
            foreach (RaycastHit hit in hits) {
                // Skip if this collider should be ignored
                if (ignoreColliders != null && System.Array.IndexOf(ignoreColliders, hit.collider) >= 0) {
                    continue;
                }
                
                IPlacementTarget target = hit.collider.GetComponent<IPlacementTarget>();
                if (target != null) {
                    placementTarget = target;
                    return true;
                }
            }

            return false;
        }

        public bool RaycastToDraggable(Ray ray, out IDraggable draggable) {
            draggable = null;

            if (Physics.Raycast(ray, out RaycastHit hit, MaxRaycastDistance)) {
                IDraggable potentialDraggable = hit.collider.GetComponent<IDraggable>();
                if (potentialDraggable != null && potentialDraggable.CanBeDragged()) {
                    draggable = potentialDraggable;
                    return true;
                }
            }

            return false;
        }
    }
}

