using UnityEngine;
using System.Collections.Generic;
using _Project.Scripts.Runtime.Gameplay.Core.Interfaces;
using _Project.Scripts.Runtime.Gameplay.Core.Models;
using _Project.Scripts.Runtime.Gameplay.Presentation.Grid.Controllers;

namespace _Project.Scripts.Runtime.Gameplay.Presentation.Grid.Slot {
    public class HexSlot : MonoBehaviour, ISlot, IPlacementTarget {
        private readonly List<IStack> _hexStacks = new List<IStack>();
        private HexCoordinates _coordinates;
        private GridController _gridController;

        public IReadOnlyList<IStack> Stacks => _hexStacks.AsReadOnly();
        
        public HexCoordinates GetCoordinates() {
            return _coordinates;
        }

        public void Initialize(HexCoordinates coordinates, GridController gridController) {
            _coordinates = coordinates;
            _gridController = gridController;
        }

        public void SetStack(IStack hexStack, bool checkNeighbors) {
            if (hexStack == null) return;
            
            if (!_hexStacks.Contains(hexStack)) {
                // If this slot was empty before adding the stack, make the stack non-draggable
                bool slotWasEmpty = _hexStacks.Count == 0;
                
                _hexStacks.Add(hexStack);
                hexStack.SetParent(transform);
                
                // Notify the source board that this stack was placed (for refilling)
                hexStack.NotifyPlaced();
                
                // Disable manual dragging if placed on empty slot
                if (slotWasEmpty) {
                    hexStack.SetDraggable(false);
                }
                
                // Check neighbors and merge stacks via grid controller
                if (checkNeighbors && _gridController != null) {
                    _gridController.CheckNeighborsAndSort(_coordinates);
                }
            }
        }
        
        public void ClearStacks() {
            _hexStacks.Clear();
        }

        public bool IsEmpty() {
            // Clean up null or empty stacks before checking
            _hexStacks.RemoveAll(stack => stack == null || stack.Cells.Count == 0);
            return _hexStacks.Count == 0;
        }

        private Vector3 GetPlacementPosition(int stackIndex) {
            Vector3 basePosition = transform.position;
            
            // Get slot height from collider
            float slotHeight = 0f;
            Collider slotCollider = GetComponent<Collider>();
            if (slotCollider != null) {
                slotHeight = slotCollider.bounds.size.y;
            }
            
            if (stackIndex == 0) {
                // First stack should be placed on top of the slot
                return basePosition + Vector3.up * slotHeight;
            }

            // Calculate Y offset based on stacked stacks above
            // Only count non-empty stacks
            float totalHeight = slotHeight; // Start with slot height
            for (int i = 0; i < stackIndex; i++) {
                if (i < _hexStacks.Count && _hexStacks[i] != null && _hexStacks[i].Cells.Count > 0) {
                    totalHeight += _hexStacks[i].Height;
                }
            }

            return basePosition + Vector3.up * totalHeight;
        }

        public bool CanAccept(IDraggable draggable, out Vector3 targetPosition) {
            targetPosition = transform.position;

            if (!(draggable is IStack hexStack)) {
                return false;
            }

            // Check if this stack is already in this slot (to prevent re-adding)
            if (_hexStacks.Contains(hexStack)) {
                return false;
            }

            // Clean up null or empty stacks before calculating position
            _hexStacks.RemoveAll(stack => stack == null || stack.Cells.Count == 0);

            // Only allow placing stacks on empty slots
            if (!IsEmpty()) {
                return false;
            }

            // Calculate position for the new stack
            targetPosition = GetPlacementPosition(0);
            
            return true;
        }
    }
}

