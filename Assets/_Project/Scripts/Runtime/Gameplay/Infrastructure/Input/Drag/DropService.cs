using UnityEngine;
using VContainer;
using _Project.Scripts.Runtime.Gameplay.Infrastructure.Input.Raycast;
using _Project.Scripts.Runtime.Gameplay.Core.Interfaces;

namespace _Project.Scripts.Runtime.Gameplay.Infrastructure.Input.Drag
{
    public class DropService
    {
        [Inject] private IInputService _inputService;
        [Inject] private IRaycastService _raycastService;

        /// <summary>
        /// Attempts to drop a draggable object at the current mouse position.
        /// Returns true if the drop was successful, false otherwise.
        /// </summary>
        public bool TryDrop(IDraggable draggable, Vector3 originalPosition, out Vector3 finalPosition)
        {
            finalPosition = originalPosition;

            if (draggable == null)
            {
                return false;
            }

            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                // No camera, snap back to original position
                draggable.SetPosition(originalPosition);
                return false;
            }

            Vector3 mousePosition = _inputService.GetMousePosition();
            Ray ray = mainCamera.ScreenPointToRay(mousePosition);

            // Get colliders from the current draggable to ignore them in the raycast
            Collider[] collidersToIgnore = null;
            if (draggable is MonoBehaviour draggableMono)
            {
                collidersToIgnore = draggableMono.GetComponentsInChildren<Collider>();
            }

            // Use RaycastService to find a slot, ignoring current draggable's colliders
            if (_raycastService.RaycastToPlacementTarget(ray, collidersToIgnore, out ISlot slot))
            {
                if (slot.CanAccept(draggable, out Vector3 targetPosition))
                {
                    draggable.SetPosition(targetPosition);
                    finalPosition = targetPosition;

                    // Register the stack with the slot
                    if (draggable is IStack stack)
                    {
                        slot.SetStack(stack);
                    }

                    return true;
                }
                else
                {
                    // Cannot accept this draggable, snap back to original position
                    draggable.SetPosition(originalPosition);
                    return false;
                }
            }
            else
            {
                // Not over a slot, snap back to original position
                draggable.SetPosition(originalPosition);
                return false;
            }
        }
    }
}

