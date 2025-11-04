using UnityEngine;
using System;
using System.Collections.Generic;
using _Project.Scripts.Runtime.Gameplay.Core.Interfaces;
using _Project.Scripts.Runtime.Gameplay.Presentation.Animation;
using _Project.Scripts.Runtime.Gameplay.Domain.Stack.Services;
using _Project.Scripts.Runtime.Utilities.Logging;

namespace _Project.Scripts.Runtime.Gameplay.Presentation.Stack {
    public class HexStack : MonoBehaviour, IDraggable, IStack {
        private List<ICell> _cells = new List<ICell>();
        private BoxCollider _collider;
        private StackColliderService _colliderService;
        private StackMergeService _mergeService;
        private StackPositionService _positionService;
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
            
            // Initialize services
            _colliderService = new StackColliderService();
            _positionService = new StackPositionService(_colliderService);
            _mergeService = new StackMergeService(_colliderService, _positionService);
            
            // Initialize collider if we have hexagons
            if (_cells != null && _cells.Count > 0) 
            {
                if (_colliderService.CalculateCellColliderSize(_cells[0])) 
                {
                    UpdateColliderSize();
                }
            }
        }

        public void UpdateColliderSize() {
            if (_colliderService == null || _collider == null) {
                CustomDebug.LogError(LogCategory.Gameplay, "Collider or collider service is null");
                return;
            }
            
            _colliderService.UpdateCollider(_collider, _cells.Count);
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
        public void AddCellsFrom(IStack sourceStack, bool animate = true, HexAnimationService animationService = null) {
            if (_mergeService == null) {
                return;
            }

            // Store the starting index for positioning
            int startingIndex = _cells.Count;

            // Delegate merge logic to service
            _mergeService.MergeStacks(this, sourceStack, animate, animationService);

            // Update collider sizes
            UpdateColliderSize();
            if (sourceStack != null) {
                sourceStack.UpdateColliderSize();
            }

            if (!animate) {
                _positionService.RepositionAllCells(_cells);
            } else {
                _positionService.RepositionAllCells(_cells, excludeFromIndex: startingIndex);
            }
        }
    }
}

