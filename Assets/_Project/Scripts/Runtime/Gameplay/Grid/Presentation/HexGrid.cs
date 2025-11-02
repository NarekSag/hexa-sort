using UnityEngine;
using System.Collections.Generic;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain.Models;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain.Mappers;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain.Config;

namespace _Project.Scripts.Runtime.Gameplay.Grid.Presentation {
    public class HexGrid {
        private HexGridConfig _config;
        private Dictionary<HexCoordinates, HexStackSlot> _slots;
        private IHexGridMapper _mapper;
        private Transform _transform;
        private bool _isInitialized;

        public void Initialize(HexGridConfig config, IHexGridMapper mapper, Transform parentTransform) {
            if (_isInitialized) {
                Debug.LogWarning("HexGrid is already initialized!");
                return;
            }

            if (config == null || !config.IsValid()) {
                Debug.LogError("Invalid HexGridConfig provided!");
                return;
            }

            if (mapper == null) {
                Debug.LogError("HexGridMapper is null!");
                return;
            }

            if (parentTransform == null) {
                Debug.LogError("Parent Transform is null!");
                return;
            }

            _config = config;
            _mapper = mapper;
            _transform = parentTransform;
            _slots = new Dictionary<HexCoordinates, HexStackSlot>();

            CreateAllSlots();
            _isInitialized = true;
        }

        private void CreateAllSlots() {
            for (int z = 0; z < _config.Height; z++) {
                for (int x = 0; x < _config.Width; x++) {
                    HexCoordinates coordinates = _mapper.GetCoordinateFromOffset(x, z);
                    Vector3 position = _mapper.GetWorldPositionFromOffset(x, z);
                    
                    HexStackSlot slot = CreateSlot(coordinates, position);
                    if (slot != null) {
                        _slots[coordinates] = slot;
                    }
                }
            }
        }

        private HexStackSlot CreateSlot(HexCoordinates coordinates, Vector3 position) {
            if (_config.SlotPrefab == null) {
                Debug.LogError("HexStackSlot prefab is not assigned!");
                return null;
            }

            HexStackSlot slot = Object.Instantiate(_config.SlotPrefab, _transform);
            slot.transform.localPosition = position;
            slot.SetCoordinates(coordinates);
            
            return slot;
        }
    }
}

