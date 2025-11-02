using UnityEngine;

namespace _Project.Scripts.Runtime.Gameplay.Input {
    public class InputService : IInputService {
        public bool GetMouseButtonDown(int button) {
            return UnityEngine.Input.GetMouseButtonDown(button);
        }

        public bool GetMouseButtonUp(int button) {
            return UnityEngine.Input.GetMouseButtonUp(button);
        }

        public Vector3 GetMousePosition() {
            return UnityEngine.Input.mousePosition;
        }
    }
}

