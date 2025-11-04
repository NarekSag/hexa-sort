using UnityEngine;
using System.Collections.Generic;

namespace _Project.Scripts.Runtime.Gameplay.Core.Interfaces {
    /// <summary>
    /// Interface for a stack of hexagon cells.
    /// Abstracts the stack from its Unity-specific implementation.
    /// </summary>
    public interface IStack {
        /// <summary>
        /// Gets the height of the stack in world units.
        /// </summary>
        float Height { get; }

        /// <summary>
        /// Gets the list of cells in this stack.
        /// </summary>
        IList<ICell> Cells { get; }
        
        /// <summary>
        /// Gets the transform component of the stack.
        /// </summary>
        Transform Transform { get; }
        
        /// <summary>
        /// Gets or sets the world position of the stack.
        /// </summary>
        Vector3 Position { get; set; }

        /// <summary>
        /// Event invoked when the stack has been placed.
        /// </summary>
        event System.Action<IStack> OnPlaced;

        /// <summary>
        /// Initializes the stack and its contained cells.
        /// </summary>
        void Initialize();
        
        /// <summary>
        /// Adds cells from another stack to this stack.
        /// </summary>
        /// <param name="sourceStack">The source stack to take cells from.</param>
        /// <param name="animate">Whether to animate the merge operation.</param>
        void AddCellsFrom(IStack sourceStack, bool animate = true);

        /// <summary>
        /// Updates the collider size of the stack.
        /// </summary>
        void UpdateColliderSize();

        /// <summary>
        /// Sets the parent transform of the stack.
        /// </summary>
        void SetParent(Transform parent);

        /// <summary>
        /// Notifies the stack that it has been placed.
        /// </summary>
        void NotifyPlaced();

        /// <summary>
        /// Gets whether the stack can be manually dragged by the player.
        /// </summary>
        bool CanBeDragged();

        /// <summary>
        /// Sets the draggable state of the stack.
        /// </summary>
        void SetDraggable(bool draggable);
    }
}

