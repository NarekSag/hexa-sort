using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Runtime.Gameplay.Input.Drag;
using _Project.Scripts.Runtime.Gameplay.Grid.Animation;
using _Project.Scripts.Runtime.Utilities.Logging;

namespace _Project.Scripts.Runtime.Gameplay.Grid.Presentation {
    public class HexStack : MonoBehaviour, IDraggable {
        [SerializeField] private List<HexCell> _hexagons = new List<HexCell>();
        private BoxCollider _collider;
        private Vector3 _cellColliderSize;
        

        private void Awake() {
            _collider = GetComponent<BoxCollider>();
            if (_collider == null) {
                _collider = gameObject.AddComponent<BoxCollider>();
            }
            
            CalculateSingleHexCellHeight();
            UpdateColliderSize();
        }

        private void CalculateSingleHexCellHeight() {
            if (_hexagons == null || _hexagons.Count == 0) {
                CustomDebug.LogError(LogCategory.Gameplay, "HexStack has no hexagons");
                return;
            }

            Collider cellCollider = _hexagons[0].GetComponent<Collider>();
            if (cellCollider != null) {
                _cellColliderSize = cellCollider.bounds.size;
            } else {
                CustomDebug.LogError(LogCategory.Gameplay, "HexCell has no collider");
            }
        }

        public void UpdateColliderSize() {
            if (_collider == null)
            {
                CustomDebug.LogError(LogCategory.Gameplay, "HexStack has no collider");
                return;
            }
            
            if (_hexagons.Count == 0) {
                CustomDebug.LogError(LogCategory.Gameplay, "HexStack has no hexagons");
                return;
            }
            
            Vector3 currentSize = _collider.size;
            float totalHeight = _cellColliderSize.y * _hexagons.Count;

            _collider.size = new Vector3(currentSize.x, totalHeight, currentSize.z);
            
            // Adjust center to position collider bottom at the base
            _collider.center = new Vector3(0f, totalHeight * 0.5f, 0f);
        }

        private void RepositionAllHexagons(int? excludeFromIndex = null) {
            for (int i = 0; i < _hexagons.Count; i++) {
                if (_hexagons[i] != null) {
                    // Skip repositioning if this index should be excluded (being animated)
                    if (excludeFromIndex.HasValue && i >= excludeFromIndex.Value) {
                        continue;
                    }
                    
                    float yOffset = _cellColliderSize.y * i;
                    _hexagons[i].transform.localPosition = new Vector3(0f, yOffset, 0f);
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

        public List<HexCell> Hexagons => _hexagons;

        public void AddHexCellsFrom(HexStack sourceStack, bool animate = true, IHexagonAnimationService animationService = null) {
            if (sourceStack == null || sourceStack == this) return;

            // Store the starting index for positioning new hexagons
            int startingIndex = _hexagons.Count;

            // Collect hexagons to merge and their target positions
            var hexagonsToMerge = new List<HexCell>();
            var targetLocalPositions = new List<Vector3>();
            int cellIndex = startingIndex;

            foreach (HexCell hexCell in sourceStack._hexagons) {
                if (hexCell != null && !_hexagons.Contains(hexCell)) {
                    hexagonsToMerge.Add(hexCell);
                    
                    // Calculate target local position
                    float yOffset = _cellColliderSize.y * cellIndex;
                    Vector3 targetLocalPosition = new Vector3(0f, yOffset, 0f);
                    targetLocalPositions.Add(targetLocalPosition);
                    
                    cellIndex++;
                }
            }

            if (hexagonsToMerge.Count == 0) return;

            // Animate or immediately move hexagons
            if (animate && animationService != null) {
                // Use animation service
                var hexagonTransforms = hexagonsToMerge.Select(cell => cell.transform).ToList();
                animationService.AnimateHexagonStackMerge(
                    hexagonTransforms,
                    sourceStack.transform,
                    transform,
                    targetLocalPositions
                );
            } else {
                // Immediate positioning (no animation)
                for (int i = 0; i < hexagonsToMerge.Count; i++) {
                    HexCell hexCell = hexagonsToMerge[i];
                    hexCell.transform.SetParent(transform);
                    hexCell.transform.localPosition = targetLocalPositions[i];
                }
            }

            // Add all hexagons to this stack
            foreach (HexCell hexCell in hexagonsToMerge) {
                _hexagons.Add(hexCell);
            }
            
            // Update collider sizes
            UpdateColliderSize();
            sourceStack.UpdateColliderSize();

            // Reposition all hexagons to ensure they're stacked correctly
            // Only do this if not animating, as animation handles positioning
            if (!animate) {
                RepositionAllHexagons();
            } else {
                // When animating, we still need to ensure existing hexagons are positioned correctly
                // but skip the ones being animated
                RepositionAllHexagons(excludeFromIndex: startingIndex);
            }

            // Clear the source stack
            sourceStack._hexagons.Clear();
        }
    }
}

