using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Core.Interfaces;
using _Project.Scripts.Runtime.Utilities.Logging;

namespace _Project.Scripts.Runtime.Gameplay.Presentation.Stack {
    /// <summary>
    /// Handles collider management for a hex stack.
    /// Each stack has its own collider component that manages the stack's BoxCollider.
    /// </summary>
    public class HexStackCollider : MonoBehaviour {
        private BoxCollider _collider;
        private Vector3 _cellColliderSize;
        private bool _isInitialized = false;

        /// <summary>
        /// Initializes the collider component with the stack's BoxCollider.
        /// </summary>
        public void Initialize(BoxCollider collider) {
            if (collider == null) {
                CustomDebug.LogError(LogCategory.Gameplay, "Collider is null in HexStackCollider.Initialize");
                return;
            }

            _collider = collider;
            _isInitialized = true;
        }

        /// <summary>
        /// Calculates the cell collider size from a sample cell.
        /// Should be called once when the first cell is added to the stack.
        /// </summary>
        public bool CalculateCellColliderSize(ICell cell) {
            if (cell == null || cell.Transform == null) {
                CustomDebug.LogError(LogCategory.Gameplay, "Cell or Transform is null in CalculateCellColliderSize");
                return false;
            }

            Collider cellCollider = cell.Transform.GetComponent<Collider>();
            if (cellCollider != null) {
                _cellColliderSize = cellCollider.bounds.size;
                return true;
            }

            CustomDebug.LogError(LogCategory.Gameplay, "Cell has no collider");
            return false;
        }

        /// <summary>
        /// Updates the stack collider based on the number of cells.
        /// </summary>
        public void UpdateCollider(int cellCount) {
            if (!_isInitialized || _collider == null) {
                CustomDebug.LogError(LogCategory.Gameplay, "HexStackCollider not initialized or collider is null");
                return;
            }

            Vector3 currentSize = _collider.size;
            
            // Handle empty stack gracefully - set minimum size
            if (cellCount == 0) {
                _collider.size = new Vector3(currentSize.x, 0f, currentSize.z);
                _collider.center = Vector3.zero;
                return;
            }

            float totalHeight = _cellColliderSize.y * cellCount;
            _collider.size = new Vector3(currentSize.x, totalHeight, currentSize.z);
            
            // Adjust center to position collider bottom at the base
            _collider.center = new Vector3(0f, totalHeight * 0.5f, 0f);
        }

        /// <summary>
        /// Calculates the Y offset for a cell at the given index.
        /// </summary>
        public float CalculateYOffset(int index) {
            return _cellColliderSize.y * index;
        }

        /// <summary>
        /// Gets the cell collider size that was calculated.
        /// </summary>
        public Vector3 CellColliderSize => _cellColliderSize;
    }
}

