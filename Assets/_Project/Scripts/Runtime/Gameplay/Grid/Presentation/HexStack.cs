using UnityEngine;
using System.Collections.Generic;
using _Project.Scripts.Runtime.Gameplay.Input.Drag;
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

        private void RepositionAllHexagons() {
            for (int i = 0; i < _hexagons.Count; i++) {
                if (_hexagons[i] != null) {
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

        public void AddHexCellsFrom(HexStack sourceStack) {
            if (sourceStack == null || sourceStack == this) return;

            // Store the starting index for positioning new hexagons
            int startingIndex = _hexagons.Count;

            // Move all hexagons from source to this stack and position them
            int cellIndex = startingIndex;
            foreach (HexCell hexCell in sourceStack._hexagons) {
                if (hexCell != null && !_hexagons.Contains(hexCell)) {
                    _hexagons.Add(hexCell);
                    hexCell.transform.SetParent(transform);
                    
                    // Position the hexagon vertically stacked on top of existing ones
                    float yOffset = _cellColliderSize.y * cellIndex;
                    hexCell.transform.localPosition = new Vector3(0f, yOffset, 0f);
                    
                    cellIndex++;
                }
            }
            
            // Update collider sizes
            UpdateColliderSize();
            sourceStack.UpdateColliderSize();

            // Reposition all hexagons to ensure they're stacked correctly
            RepositionAllHexagons();

            // Clear the source stack
            sourceStack._hexagons.Clear();
        }
    }
}

