using UnityEngine;
using System.Collections.Generic;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain.Models;
using _Project.Scripts.Runtime.Gameplay.Grid.Animation;
using _Project.Scripts.Runtime.Gameplay.Input.Drag;
using VContainer;

namespace _Project.Scripts.Runtime.Gameplay.Grid.Presentation {
    public class HexStackSlot : MonoBehaviour, IPlacementTarget {
        private readonly List<HexStack> _hexStacks = new List<HexStack>();
        private HexGrid _grid;
        private IHexagonAnimationService _animationService;
        private HexCoordinates _coordinates;

        public void Initialize(HexCoordinates coordinates, HexGrid grid, IHexagonAnimationService animationService) {
            _coordinates = coordinates;
            _grid = grid;
            _animationService = animationService;
        }

        public void SetHexStack(HexStack hexStack) {
            if (hexStack == null) return;
            
            if (!_hexStacks.Contains(hexStack)) {
                _hexStacks.Add(hexStack);
                hexStack.transform.SetParent(transform);
                
                // Check neighbors and merge stacks
                CheckNeighborsAndMerge();
            }
        }

        private void CheckNeighborsAndMerge() {
            if (_grid == null) return;

            HexCoordinates[] neighbors = _coordinates.GetNeighbors();
            foreach (HexCoordinates neighborCoords in neighbors) {
                HexStackSlot neighborSlot = _grid.GetSlot(neighborCoords);
                if (neighborSlot != null && !neighborSlot.IsEmpty()) {
                    MergeWithSlot(neighborSlot);
                }
            }
        }

        public void MergeWithSlot(HexStackSlot otherSlot) {
            if (otherSlot == null || otherSlot == this) return;
            if (otherSlot.IsEmpty()) return;

            // Get all stacks from the neighbor slot
            var stacksToMerge = new List<HexStack>(otherSlot._hexStacks);
            
            // Move all hexagons from neighbor's stacks to this slot's first stack
            if (_hexStacks.Count > 0) {
                HexStack targetStack = _hexStacks[0];
                
                foreach (HexStack sourceStack in stacksToMerge) {
                    if (sourceStack != null) {
                        // Pass animation service if available
                        if (_animationService != null) {
                            targetStack.AddHexCellsFrom(sourceStack, animate: true, _animationService);
                        } else {
                            targetStack.AddHexCellsFrom(sourceStack, animate: false);
                        }
                        
                        // Destroy the empty stack (with delay if animating)
                        if (sourceStack.Hexagons.Count == 0) {
                            if (_animationService != null) {
                                // Delay destruction to allow animation to complete
                                Destroy(sourceStack.gameObject, 0.3f);
                            } else {
                                Destroy(sourceStack.gameObject);
                            }
                        }
                    }
                }
            } else {
                // If this slot is empty, move the entire stack
                foreach (HexStack stack in stacksToMerge) {
                    if (stack != null) {
                        _hexStacks.Add(stack);
                        stack.transform.SetParent(transform);
                    }
                }
            }

            // Clear the neighbor slot
            otherSlot._hexStacks.Clear();
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

