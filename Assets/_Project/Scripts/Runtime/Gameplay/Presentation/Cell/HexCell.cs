using UnityEngine;
using _Project.Scripts.Runtime.Gameplay.Core.Interfaces;
using _Project.Scripts.Runtime.Gameplay.Core.Models;
using _Project.Scripts.Runtime.Gameplay.Config;

namespace _Project.Scripts.Runtime.Gameplay.Presentation.Cell 
{
    public class HexCell : MonoBehaviour, ICell 
    {
        [SerializeField] private ColorType _colorType;
        [SerializeField] private ColorMaterialConfig _colorMaterialConfig;
        [SerializeField] private HexAnimationConfig _hexAnimationConfig;
            
        private HexCellMaterial _materialController;
        private HexCellAnimator _animator;
            
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
        }
        
        public void Initialize(ColorType colorType, int index) 
        {
            _colorType = colorType;
            _collider = GetComponent<Collider>();

            InitializeMaterialController(_colorType);
            InitializeAnimator();
            transform.localPosition = new Vector3(0, index * Height, 0);
        }
        
        private void InitializeAnimator() {
            if (_hexAnimationConfig == null) {
                return;
            }

            // Get or add animator component
            _animator = GetComponent<HexCellAnimator>();
            if (_animator == null) {
                _animator = gameObject.AddComponent<HexCellAnimator>();
            }

            _animator.Initialize(_hexAnimationConfig);
        }

        public HexCellAnimator GetAnimator() {
            return _animator;
        }
        
        private void InitializeMaterialController(ColorType colorType) {
            _materialController = new HexCellMaterial(gameObject, _colorMaterialConfig);
            _materialController.Initialize();
            _materialController.UpdateMaterial(colorType);
        }
    }
}

