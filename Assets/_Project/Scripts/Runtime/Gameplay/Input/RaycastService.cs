using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Grid.Presentation;

namespace _Project.Scripts.Runtime.Gameplay.Input {
    public class RaycastService : IRaycastService {
        private const float MaxRaycastDistance = 1000f;

        public bool RaycastToHexStackSlot(Ray ray, out HexStackSlot hexStackSlot) {
            hexStackSlot = null;

            if (Physics.Raycast(ray, out RaycastHit hit, MaxRaycastDistance)) {
                hexStackSlot = hit.collider.GetComponent<HexStackSlot>();
                return hexStackSlot != null;
            }

            return false;
        }

        public bool RaycastToHexStackContainer(Ray ray, out HexStackContainer hexStackContainer) {
            hexStackContainer = null;

            if (Physics.Raycast(ray, out RaycastHit hit, MaxRaycastDistance)) {
                var container = hit.collider.GetComponent<MonoBehaviour>() as HexStackContainer;
                if (container != null) {
                    hexStackContainer = container;
                    return true;
                }
            }

            return false;
        }
    }
}

