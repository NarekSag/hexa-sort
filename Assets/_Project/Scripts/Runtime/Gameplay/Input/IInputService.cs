using UnityEngine;

namespace _Project.Scripts.Runtime.Gameplay.Input {
    public interface IInputService {
        bool GetMouseButtonDown(int button);
        bool GetMouseButtonUp(int button);
        Vector3 GetMousePosition();
    }
}

