using UnityEngine;
using System.Collections.Generic;
using _Project.Scripts.Runtime.Gameplay.Grid.Animation;
using _Project.Scripts.Runtime.Gameplay.Cell;

namespace _Project.Scripts.Runtime.Gameplay.Stack {
    /// <summary>
    /// Interface for a stack of hexagon cells.
    /// Abstracts the stack from its Unity-specific implementation.
    /// </summary>
    public interface IStack {
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
        /// Adds cells from another stack to this stack.
        /// </summary>
        /// <param name="sourceStack">The source stack to take cells from.</param>
        /// <param name="animate">Whether to animate the merge operation.</param>
        /// <param name="animationService">Optional animation service to use for animating.</param>
        void AddCellsFrom(IStack sourceStack, bool animate = true, IHexagonAnimationService animationService = null);

        /// <summary>
        /// Updates the collider size of the stack.
        /// </summary>
        void UpdateColliderSize();
    }
}

