using UnityEngine;

namespace _Project.Scripts.Runtime.Gameplay.Grid.Presentation {
    public class HexStackSlot : MonoBehaviour {
        private HexStack _hexStack;

        public void SetHexStack(HexStack hexStack) {
            _hexStack = hexStack;
        }

        public HexStack GetHexStack() {
            return _hexStack;
        }

        public bool IsEmpty() {
            return _hexStack == null;
        }
    }
}

