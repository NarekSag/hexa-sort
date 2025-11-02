using UnityEngine;

namespace _Project.Scripts.Runtime.Gameplay.Input.PositionCalculation {
    public interface IPositionCalculationService {
        Vector3 ScreenToWorldPosition(Vector3 screenPos);
        Vector3 ScreenToWorldPosition(Vector3 screenPos, float yHeight);
    }
}

