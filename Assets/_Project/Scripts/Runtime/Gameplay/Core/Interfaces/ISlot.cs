using System.Collections.Generic;
using _Project.Scripts.Runtime.Gameplay.Core.Models;
using _Project.Scripts.Runtime.Gameplay.Presentation.Grid.Controllers;

namespace _Project.Scripts.Runtime.Gameplay.Core.Interfaces {
    /// <summary>
    /// Represents a slot that can hold multiple hex stacks.
    /// </summary>
    public interface ISlot {
        /// <summary>
        /// Read-only collection of stacks currently in this slot.
        /// </summary>
        IReadOnlyList<IStack> Stacks { get; }

        /// <summary>
        /// Initializes the slot with its coordinates and grid controller.
        /// </summary>
        void Initialize(HexCoordinates coordinates, GridController gridController);

        /// <summary>
        /// Adds a hex stack to this slot.
        /// </summary>
        void SetStack(IStack hexStack, bool checkNeighbors = true);

        /// <summary>
        /// Removes all stacks from this slot.
        /// </summary>
        void ClearStacks();

        /// <summary>
        /// Checks if this slot has no stacks.
        /// </summary>
        bool IsEmpty();
    }
}

