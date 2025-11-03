using UnityEngine;

namespace _Project.Scripts.Runtime.Gameplay.Input.Drag {
    public interface IDraggable {
        /// <summary>
        /// Sets the world position of the draggable object.
        /// </summary>
        void SetPosition(Vector3 position);

        /// <summary>
        /// Gets the current world position of the draggable object.
        /// </summary>
        Vector3 GetPosition();
        /// <summary>
        /// Sets the parent transform of the draggable object.
        /// </summary>
        void SetParent(Transform parent);
        
        /// <summary>
        /// Gets whether this object can be manually dragged by the player.
        /// </summary>
        bool CanBeDragged();
    }
}

