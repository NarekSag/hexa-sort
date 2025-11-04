using UnityEngine;
using _Project.Scripts.Runtime.Utilities.Logging;
using _Project.Scripts.Runtime.Gameplay.Core.Interfaces;

namespace _Project.Scripts.Runtime.Gameplay.Domain.Stack.Services {
    public class StackColliderService {
        private Vector3 _cellColliderSize;

        public bool CalculateCellColliderSize(ICell cell) {
            if (cell == null || cell.Transform == null) {
                CustomDebug.LogError(LogCategory.Gameplay, "Cell or Transform is null");
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

        public void UpdateCollider(BoxCollider collider, int hexagonCount) {
            if (collider == null) {
                CustomDebug.LogError(LogCategory.Gameplay, "Collider is null");
                return;
            }

            Vector3 currentSize = collider.size;
            
            // Handle empty stack gracefully - set minimum size
            if (hexagonCount == 0) {
                collider.size = new Vector3(currentSize.x, 0f, currentSize.z);
                collider.center = Vector3.zero;
                return;
            }

            float totalHeight = _cellColliderSize.y * hexagonCount;
            collider.size = new Vector3(currentSize.x, totalHeight, currentSize.z);
            
            // Adjust center to position collider bottom at the base
            collider.center = new Vector3(0f, totalHeight * 0.5f, 0f);
        }

        public float CalculateYOffset(int index) {
            return _cellColliderSize.y * index;
        }
    }
}
