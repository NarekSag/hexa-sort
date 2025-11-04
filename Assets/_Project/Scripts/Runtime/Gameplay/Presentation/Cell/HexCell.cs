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
            private set => _colorType = value;
        }
        
        public void Initialize(ColorType colorType, int index) 
        {
            _colorType = colorType;
            _collider = GetComponent<Collider>();

            InitializeMaterialController(_colorType);

            transform.localPosition = new Vector3(0, index * Height, 0);
        }

        /// <summary>
        /// Initializes the cell with color type, index, and animation config.
        /// </summary>
        public void Initialize(ColorType colorType, int index, HexAnimationConfig animationConfig) {
            Initialize(colorType, index);
            InitializeAnimator(animationConfig);
        }
        
        private void InitializeAnimator(HexAnimationConfig animationConfig) {
            if (animationConfig == null) {
                return;
            }

            // Get or add animator component
            _animator = GetComponent<HexCellAnimator>();
            if (_animator == null) {
                _animator = gameObject.AddComponent<HexCellAnimator>();
            }

            _animator.Initialize(animationConfig);
        }

        /// <summary>
        /// Gets the animator component for this cell.
        /// </summary>
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

