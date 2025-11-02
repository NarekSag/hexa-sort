using UnityEngine;

namespace _Project.Scripts.Runtime.Gameplay.Cell {
    public class HexCell : MonoBehaviour, ICell {
        public Transform Transform => transform;
        
        public Vector3 LocalPosition {
            get => transform.localPosition;
            set => transform.localPosition = value;
        }
    }
}

