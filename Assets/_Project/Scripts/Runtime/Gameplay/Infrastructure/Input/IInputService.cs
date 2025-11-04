using UnityEngine;

namespace _Project.Scripts.Runtime.Gameplay.Infrastructure.Input {
    public interface IInputService {
        bool GetMouseButtonDown(int button);
        bool GetMouseButtonUp(int button);
        Vector3 GetMousePosition();
    }
}

