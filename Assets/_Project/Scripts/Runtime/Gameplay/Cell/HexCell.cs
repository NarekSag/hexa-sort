using UnityEngine;

namespace _Project.Scripts.Runtime.Gameplay.Cell {
    public class HexCell : MonoBehaviour, ICell {
        [SerializeField] private ColorType _colorType;
        [SerializeField] private ColorMaterialConfig _colorMaterialConfig;
        
        private HexCellMaterialController _materialController;
        
        public Transform Transform => transform;
        
        public Vector3 LocalPosition {
            get => transform.localPosition;
            set => transform.localPosition = value;
        }
        
        /// <summary>
        /// Gets the color type of this cell.
        /// </summary>
        public ColorType ColorType {
            get => _colorType;
            private set => _colorType = value;
        }
        
        private void Awake() {
            InitializeMaterialController();
        }
        
        private void InitializeMaterialController() {
            _materialController = new HexCellMaterialController(gameObject, _colorMaterialConfig);
            _materialController.Initialize();
            _materialController.UpdateMaterial(_colorType);
        }
    }
}

