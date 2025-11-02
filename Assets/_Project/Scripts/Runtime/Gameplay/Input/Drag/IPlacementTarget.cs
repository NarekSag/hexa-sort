using UnityEngine;

namespace _Project.Scripts.Runtime.Gameplay.Input.Drag {
    public interface IPlacementTarget {
        bool CanAccept(IDraggable draggable, out Vector3 targetPosition);
    }
}

