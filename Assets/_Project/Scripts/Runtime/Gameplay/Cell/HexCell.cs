using UnityEngine;

namespace _Project.Scripts.Runtime.Gameplay.Cell 
{
    public class HexCell : MonoBehaviour, ICell 
    {
        [SerializeField] private ColorType _colorType;
        [SerializeField] private ColorMaterialConfig _colorMaterialConfig;
            
        private HexCellMaterialController _materialController;
            
        public Transform Transform => transform;
            
        public Vector3 LocalPosition 
        {
            get => transform.localPosition;
            set => transform.localPosition = value;
        }

        private Collider _collider;

        public float Height => _collider?.bounds.size.y ?? 0.1f;
            
        public ColorType ColorType {
            get => _colorType;
            private set => _colorType = value;
        }
        
        public void Initialize(ColorType colorType, int index) 
        {
            _colorType = colorType;
            _collider = GetComponent<Collider>();

            InitializeMaterialController(_colorType);

            transform.localPosition = new Vector3(0, index * Height, 0);
        }
        
        private void InitializeMaterialController(ColorType colorType) {
            _materialController = new HexCellMaterialController(gameObject, _colorMaterialConfig);
            _materialController.Initialize();
            _materialController.UpdateMaterial(colorType);
        }
    }
}

