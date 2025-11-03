using UnityEngine;
using System.Collections.Generic;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain.Models;
using _Project.Scripts.Runtime.Gameplay.Grid.Animation;
using _Project.Scripts.Runtime.Gameplay.Input.Drag;
using _Project.Scripts.Runtime.Gameplay.Grid.Presentation.Controllers;
using _Project.Scripts.Runtime.Gameplay.Stack;

namespace _Project.Scripts.Runtime.Gameplay.Grid.Presentation {
    public class HexStackSlot : MonoBehaviour, IPlacementTarget {
        private readonly List<HexStack> _hexStacks = new List<HexStack>();
        private HexGrid _grid;
        private IHexagonAnimationService _animationService;
        private HexCoordinates _coordinates;
        private GridController _gridController;

        public IReadOnlyList<HexStack> Stacks => _hexStacks.AsReadOnly();

        public void Initialize(HexCoordinates coordinates, HexGrid grid, IHexagonAnimationService animationService, GridController gridController) {
            _coordinates = coordinates;
            _grid = grid;
            _animationService = animationService;
            _gridController = gridController;
        }

        public void SetHexStack(HexStack hexStack) {
            SetHexStack(hexStack, checkNeighbors: true);
        }

        internal void SetHexStack(HexStack hexStack, bool checkNeighbors) {
            if (hexStack == null) return;
            
            if (!_hexStacks.Contains(hexStack)) {
                // If this slot was empty before adding the stack, make the stack non-draggable
                bool slotWasEmpty = _hexStacks.Count == 0;
                
                _hexStacks.Add(hexStack);
                hexStack.transform.SetParent(transform);
                
                // Notify the source board that this stack was placed (for refilling)
                hexStack.NotifyPlaced();
                
                // Disable manual dragging if placed on empty slot
                if (slotWasEmpty) {
                    hexStack.SetDraggable(false);
                }
                
                // Check neighbors and merge stacks via grid controller
                if (checkNeighbors && _gridController != null) {
                    _gridController.CheckNeighborsAndMerge(_coordinates);
                }
            }
        }
        
        public void ClearStacks() {
            _hexStacks.Clear();
        }

        public bool IsEmpty() {
            return _hexStacks.Count == 0;
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

