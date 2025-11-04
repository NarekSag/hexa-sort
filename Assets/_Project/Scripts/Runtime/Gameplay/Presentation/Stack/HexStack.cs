using UnityEngine;
using System;
using System.Collections.Generic;
using _Project.Scripts.Runtime.Gameplay.Core.Interfaces;
using _Project.Scripts.Runtime.Gameplay.Domain.Stack.Services;
using _Project.Scripts.Runtime.Utilities.Logging;

namespace _Project.Scripts.Runtime.Gameplay.Presentation.Stack {
    public class HexStack : MonoBehaviour, IDraggable, IStack {
        private List<ICell> _cells = new List<ICell>();
        private BoxCollider _collider;
        private HexStackCollider _stackCollider;
        private StackMergeService _mergeService;
        private bool _isDraggable = true;

        private float _height = 0f;
        
        public event Action<IStack> OnPlaced;

        // IStack implementation
        public Transform Transform => transform;
        
        public Vector3 Position {
            get => transform.position;
            set => transform.position = value;
        }
        
        public IList<ICell> Cells => _cells;

        public float Height => _height;

        public void Initialize() 
        {
            _collider = GetComponent<BoxCollider>();
            if (_collider == null) {
                _collider = gameObject.AddComponent<BoxCollider>();
            }
            
            _height = _collider.bounds.size.y;
            
            // Initialize stack collider component
            _stackCollider = GetComponent<HexStackCollider>();
            if (_stackCollider == null) {
                _stackCollider = gameObject.AddComponent<HexStackCollider>();
            }
            _stackCollider.Initialize(_collider);
            
            // Initialize services (no longer need position or collider services)
            _mergeService = new StackMergeService();
            
            // Initialize collider if we have cells
            if (_cells != null && _cells.Count > 0) 
            {
                if (_stackCollider.CalculateCellColliderSize(_cells[0])) 
                {
                    UpdateColliderSize();
                }
            }
        }

        public void UpdateColliderSize() {
            if (_stackCollider == null) {
                CustomDebug.LogError(LogCategory.Gameplay, "Stack collider component is null");
                return;
            }
            
            _stackCollider.UpdateCollider(_cells.Count);
        }

        /// <summary>
        /// Gets the stack collider component.
        /// </summary>
        public HexStackCollider GetStackCollider() {
            return _stackCollider;
        }

        /// <summary>
        /// Calculates the Y offset for a cell at the given index.
        /// </summary>
        public float CalculateYOffset(int index) {
            if (_stackCollider == null) {
                return 0f;
            }
            return _stackCollider.CalculateYOffset(index);
        }

        /// <summary>
        /// Repositions all cells in this stack based on their index.
        /// </summary>
        /// <param name="excludeFromIndex">Optional index to exclude from repositioning (for cells being animated).</param>
        public void RepositionAllCells(int? excludeFromIndex = null) {
            if (_cells == null) {
                return;
            }

            for (int i = 0; i < _cells.Count; i++) {
                if (_cells[i] != null) {
                    // Skip repositioning if this index should be excluded (being animated)
                    if (excludeFromIndex.HasValue && i >= excludeFromIndex.Value) {
                        continue;
                    }
                    
                    float yOffset = CalculateYOffset(i);
                    _cells[i].LocalPosition = new Vector3(0f, yOffset, 0f);
                }
            }
        }

        public void SetPosition(Vector3 position) {
            transform.position = position;
        }

        public Vector3 GetPosition() {
            return transform.position;
        }

        public void SetParent(Transform parent) {
            transform.SetParent(parent);
        }
        
        public bool CanBeDragged() {
            return _isDraggable;
        }
        
        public void SetDraggable(bool draggable) {
            _isDraggable = draggable;
        }
        
        public void NotifyPlaced() {
            OnPlaced?.Invoke(this);
        }

        // IStack implementation
        public async void AddCellsFrom(IStack sourceStack, bool animate = true) {
            if (_mergeService == null) {
                return;
            }

            // Store the starting index for positioning
            int startingIndex = _cells.Count;

            // Delegate merge logic to service (now async)
            await _mergeService.MergeStacks(this, sourceStack, animate);

            // Update collider sizes
            UpdateColliderSize();
            if (sourceStack != null) {
                sourceStack.UpdateColliderSize();
            }

            if (!animate) {
                RepositionAllCells();
            } else {
                RepositionAllCells(excludeFromIndex: startingIndex);
            }
        }
    }
}

