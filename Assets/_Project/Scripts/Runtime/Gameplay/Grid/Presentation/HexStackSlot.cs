using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain.Models;
using _Project.Scripts.Runtime.Gameplay.Input.Drag;

namespace _Project.Scripts.Runtime.Gameplay.Grid.Presentation {
    public class HexStackSlot : MonoBehaviour, IPlacementTarget {
        [SerializeField] private HexCoordinates _coordinates;
        private HexStack _hexStack;

        public void SetCoordinates(HexCoordinates coordinates) {
            _coordinates = coordinates;
        }

        public void SetHexStack(HexStack hexStack) {
            _hexStack = hexStack;
        }

        public HexStack GetHexStack() {
            return _hexStack;
        }

        public bool IsEmpty() {
            return _hexStack == null;
        }

        private Vector3 GetPlacementPosition() {
            return transform.position;
        }

        public bool CanAccept(IDraggable draggable, out Vector3 targetPosition) {
            targetPosition = GetPlacementPosition();
            return IsEmpty() && draggable is HexStack;
        }
    }
}

