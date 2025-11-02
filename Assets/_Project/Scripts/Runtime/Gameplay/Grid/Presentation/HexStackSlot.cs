using UnityEngine;
using System.Collections.Generic;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain.Models;
using _Project.Scripts.Runtime.Gameplay.Input.Drag;

namespace _Project.Scripts.Runtime.Gameplay.Grid.Presentation {
    public class HexStackSlot : MonoBehaviour, IPlacementTarget {
        [SerializeField] private HexCoordinates _coordinates;
        private readonly List<HexStack> _hexStacks = new List<HexStack>();

        public void SetCoordinates(HexCoordinates coordinates) {
            _coordinates = coordinates;
        }

        public void SetHexStack(HexStack hexStack) {
            if (hexStack == null) return;
            
            if (!_hexStacks.Contains(hexStack)) {
                _hexStacks.Add(hexStack);
                hexStack.transform.SetParent(transform);
            }
        }

        private Vector3 GetPlacementPosition(int stackIndex) {
            Vector3 basePosition = transform.position;
            
            if (stackIndex == 0) {
                return basePosition;
            }

            // Calculate Y offset based on stacked stacks above
            float totalHeight = 0f;
            for (int i = 0; i < stackIndex; i++) {
                if (i < _hexStacks.Count) {
                    float stackHeight = GetStackHeight(_hexStacks[i]);
                    totalHeight += stackHeight;
                }
            }

            return basePosition + Vector3.up * totalHeight;
        }

        private float GetStackHeight(HexStack stack) {
            // Use collider size to get the height of the stack (size is in local space, not world)
            Collider stackCollider = stack.GetComponent<Collider>();
            if (stackCollider != null) {
                return stackCollider.bounds.size.y;
            }

            // Fallback: use a default height if no collider
            return 1f;
        }

        public bool CanAccept(IDraggable draggable, out Vector3 targetPosition) {
            targetPosition = transform.position;

            if (!(draggable is HexStack hexStack)) {
                return false;
            }

            // Check if this stack is already in this slot (to prevent re-adding)
            if (_hexStacks.Contains(hexStack)) {
                return false;
            }

            // Calculate position for the new stack (on top of existing stacks)
            int newStackIndex = _hexStacks.Count;
            targetPosition = GetPlacementPosition(newStackIndex);
            
            return true;
        }
    }
}

