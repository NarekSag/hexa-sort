using UnityEngine;

namespace _Project.Scripts.Runtime.Gameplay.Cell {
    /// <summary>
    /// Interface for a hexagon cell in the grid.
    /// Abstracts the cell from its Unity-specific implementation.
    /// </summary>
    public interface ICell {
        /// <summary>
        /// Gets the transform component of the cell.
        /// </summary>
        Transform Transform { get; }
        
        /// <summary>
        /// Gets or sets the local position of the cell within its parent stack.
        /// </summary>
        Vector3 LocalPosition { get; set; }
        
        /// <summary>
        /// Gets the color type of this cell.
        /// </summary>
        ColorType ColorType { get; }
    }
}

