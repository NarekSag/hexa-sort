using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Runtime.Gameplay.Input.Drag;
using _Project.Scripts.Runtime.Gameplay.Grid.Animation;
using _Project.Scripts.Runtime.Gameplay.Cell;
using _Project.Scripts.Runtime.Gameplay.Stack.Services;
using _Project.Scripts.Runtime.Utilities.Logging;

namespace _Project.Scripts.Runtime.Gameplay.Stack {
    public class HexStack : MonoBehaviour, IDraggable, IStack {
        private List<HexCell> _hexagons = new List<HexCell>();
        private BoxCollider _collider;
        private StackColliderService _colliderService;
        private StackMergeService _mergeService;
        private StackPositionService _positionService;
        private bool _isDraggable = true;
        
        public event Action<HexStack> OnPlaced;

        // IStack implementation
        public Transform Transform => transform;
        
        public Vector3 Position {
            get => transform.position;
            set => transform.position = value;
        }
        
        public IList<ICell> Cells => _hexagons.Cast<ICell>().ToList();

        public void Initialize() 
        {
            _collider = GetComponent<BoxCollider>();
            if (_collider == null) {
                _collider = gameObject.AddComponent<BoxCollider>();
            }
            
            // Initialize services
            _colliderService = new StackColliderService();
            _positionService = new StackPositionService(_colliderService);
            _mergeService = new StackMergeService(_colliderService, _positionService);
            
            // Initialize collider if we have hexagons
            if (_hexagons != null && _hexagons.Count > 0) 
            {
                if (_colliderService.CalculateCellColliderSize(_hexagons[0])) 
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
            
            _colliderService.UpdateCollider(_collider, _hexagons.Count);
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

        public List<HexCell> Hexagons => _hexagons;

        // IStack implementation
        public void AddCellsFrom(IStack sourceStack, bool animate = true, IHexagonAnimationService animationService = null) {
            if (_mergeService == null) {
                return;
            }

            // Store the starting index for positioning
            int startingIndex = _hexagons.Count;

            // Delegate merge logic to service
            _mergeService.MergeStacks(this, sourceStack, animate, animationService);

            // Update collider sizes
            UpdateColliderSize();
            if (sourceStack != null) {
                sourceStack.UpdateColliderSize();
            }

            if (!animate) {
                _positionService.RepositionAllHexagons(_hexagons);
            } else {
                _positionService.RepositionAllHexagons(_hexagons, excludeFromIndex: startingIndex);
            }
        }
    }
}

