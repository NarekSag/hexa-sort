using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain.Models;

namespace _Project.Scripts.Runtime.Gameplay.Grid.Presentation {
    public class HexGrid : MonoBehaviour {

        [SerializeField] private int _width = 6;
        [SerializeField] private int _height = 6;

        [SerializeField] private HexCell _cellPrefab;

        private HexCell[] _cells;

        void Awake () {
            _cells = new HexCell[_height * _width];

            for (int z = 0, i = 0; z < _height; z++) {
                for (int x = 0; x < _width; x++) {
                    CreateCell(x, z, i++);
                }
            }
        }
        
        void CreateCell (int x, int z, int i) {
            Vector3 position;
            position.x = (x + z * 0.5f - z / 2) * (HexMetrics.InnerRadius * 2f);
		    position.y = 0f;
		    position.z = z * (HexMetrics.OuterRadius * 1.5f);

            HexCell cell = _cells[i] = Instantiate(_cellPrefab, position, Quaternion.identity);
            cell.transform.SetParent(transform, false);
            cell.transform.localPosition = position;
        }
    }
}

