using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Input.Drag;

namespace _Project.Scripts.Runtime.Gameplay.Input.Raycast {
    /// <summary>
    /// Service for raycasting to game-specific objects.
    /// Uses abstractions to follow Dependency Inversion Principle (SOLID).
    /// </summary>
    public interface IRaycastService {
        /// <summary>
        /// Raycasts to find a placement target (e.g., slot) at the ray position.
        /// </summary>
        bool RaycastToPlacementTarget(Ray ray, out IPlacementTarget placementTarget);

        /// <summary>
        /// Raycasts to find a placement target, ignoring specified colliders.
        /// </summary>
        bool RaycastToPlacementTarget(Ray ray, Collider[] ignoreColliders, out IPlacementTarget placementTarget);

        /// <summary>
        /// Raycasts to find a draggable object at the ray position.
        /// </summary>
        bool RaycastToDraggable(Ray ray, out IDraggable draggable);
    }
}

