using UnityEngine;

namespace _Project.Scripts.Runtime.Gameplay.Input {
    public interface IPositionCalculationService {
        Vector3 ScreenToWorldPosition(Vector3 screenPos);
    }
}

