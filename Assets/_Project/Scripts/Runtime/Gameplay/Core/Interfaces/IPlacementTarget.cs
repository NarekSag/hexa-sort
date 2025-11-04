using UnityEngine;

namespace _Project.Scripts.Runtime.Gameplay.Core.Interfaces {
    public interface IPlacementTarget {
        bool CanAccept(IDraggable draggable, out Vector3 targetPosition);
    }
}

