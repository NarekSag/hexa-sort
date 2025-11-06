using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Core.Interfaces;

namespace _Project.Scripts.Runtime.Gameplay.Infrastructure.Input.Raycast
{
    public interface IRaycastService
    {
        /// <summary>
        /// Raycasts to find a slot at the ray position.
        /// </summary>
        bool RaycastToPlacementTarget(Ray ray, out ISlot slot);

        /// <summary>
        /// Raycasts to find a slot, ignoring specified colliders.
        /// </summary>
        bool RaycastToPlacementTarget(Ray ray, Collider[] ignoreColliders, out ISlot slot);

        /// <summary>
        /// Raycasts to find a draggable object at the ray position.
        /// </summary>
        bool RaycastToDraggable(Ray ray, out IDraggable draggable);
    }
}

