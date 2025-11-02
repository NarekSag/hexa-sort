using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Grid.Presentation;

namespace _Project.Scripts.Runtime.Gameplay.Input {
    public interface IRaycastService {
        bool RaycastToHexStackSlot(Ray ray, out HexStackSlot hexStackSlot);
        bool RaycastToHexStackContainer(Ray ray, out HexStackContainer hexStackContainer);
    }

    public interface HexStackContainer {
        // Marker interface for containers that can hold HexStacks
    }
}

