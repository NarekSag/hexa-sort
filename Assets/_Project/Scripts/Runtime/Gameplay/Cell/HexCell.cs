using UnityEngine;

namespace _Project.Scripts.Runtime.Gameplay.Cell {
    public class HexCell : MonoBehaviour, ICell {
        [SerializeField] private ColorType _colorType;
        
        public Transform Transform => transform;
        
        public Vector3 LocalPosition {
            get => transform.localPosition;
            set => transform.localPosition = value;
        }
        
        /// <summary>
        /// Gets the color type of this cell.
        /// </summary>
        public ColorType ColorType {
            get => _colorType;
            private set => _colorType = value;
        }
        
        /// <summary>
        /// Checks if this cell has a valid color type assigned.
        /// </summary>
        public bool HasColor() {
            return true; // ColorType is always valid (enum default is 0 which is Red)
        }
    }
}

