using UnityEngine;

namespace _Project.Scripts.Runtime.Gameplay.Input.Drag {
    /// <summary>
    /// Interface for objects that can be dragged by the DragService.
    /// Follows Dependency Inversion Principle (SOLID).
    /// </summary>
    public interface IDraggable {
        /// <summary>
        /// Sets the world position of the draggable object.
        /// </summary>
        void SetPosition(Vector3 position);

        /// <summary>
        /// Gets the current world position of the draggable object.
        /// </summary>
        Vector3 GetPosition();
    }
}

