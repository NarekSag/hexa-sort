using UnityEngine;
using System.Collections.Generic;
using _Project.Scripts.Runtime.Gameplay.Input.Drag;

namespace _Project.Scripts.Runtime.Gameplay.Grid.Presentation {
    public class HexStack : MonoBehaviour, IDraggable {
        [SerializeField] private List<HexCell> _hexagons = new List<HexCell>();

        public void SetPosition(Vector3 position) {
            transform.position = position;
        }

        public Vector3 GetPosition() {
            return transform.position;
        }

        public List<HexCell> Hexagons => _hexagons;
    }
}

