using UnityEngine;

namespace _Project.Scripts.Runtime.Gameplay.Presentation.Grid.Slot
{
    public class HexSlotHighlight : ISlotHighlight
    {
        private MeshRenderer _renderer;
        private Material _originalMaterial;
        private Material _highlightMaterial;
        private bool _isHighlighted;
        private bool _supportsEmission;
        private bool _supportsBaseColor;
        private readonly Color _highlightColor;
        private readonly float _highlightIntensity;

        public HexSlotHighlight(Color highlightColor, float highlightIntensity)
        {
            _highlightColor = highlightColor;
            _highlightIntensity = highlightIntensity;
        }

        public void Initialize(MeshRenderer renderer)
        {
            if (renderer == null) return;

            _renderer = renderer;
            _originalMaterial = renderer.material;

            if (_originalMaterial != null)
            {
                // Cache shader property support to avoid repeated HasProperty calls
                _supportsEmission = _originalMaterial.HasProperty("_EmissionColor");
                _supportsBaseColor = _originalMaterial.HasProperty("_BaseColor");

                // Pre-create highlight material to avoid runtime allocation
                _highlightMaterial = new Material(_originalMaterial);
                ApplyHighlightProperties();
            }
        }

        public void SetHighlighted(bool highlighted)
        {
            if (_isHighlighted == highlighted || _renderer == null) return;

            _isHighlighted = highlighted;
            _renderer.material = highlighted ? _highlightMaterial : _originalMaterial;
        }

        public void RemoveHighlight()
        {
            SetHighlighted(false);
        }

        private void ApplyHighlightProperties()
        {
            if (_highlightMaterial == null) return;

            if (_supportsEmission)
            {
                _highlightMaterial.EnableKeyword("_EMISSION");
                _highlightMaterial.SetColor("_EmissionColor", _highlightColor * _highlightIntensity);
            }
            else if (_supportsBaseColor)
            {
                Color baseColor = _originalMaterial.GetColor("_BaseColor");
                _highlightMaterial.SetColor("_BaseColor", baseColor * _highlightColor);
            }
        }
    }
}

