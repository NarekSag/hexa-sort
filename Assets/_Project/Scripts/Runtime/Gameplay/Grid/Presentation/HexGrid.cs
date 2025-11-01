using UnityEngine;
using System.Collections.Generic;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain.Models;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain.Mappers;
using _Project.Scripts.Runtime.Gameplay.Grid.Domain.Config;

namespace _Project.Scripts.Runtime.Gameplay.Grid.Presentation {
    public class HexGrid {
        private HexGridConfig _config;
        private Dictionary<HexCoordinates, HexCell> _cells;
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
            _cells = new Dictionary<HexCoordinates, HexCell>();

            CreateAllCells();
            _isInitialized = true;
        }

        private void CreateAllCells() {
            for (int z = 0; z < _config.Height; z++) {
                for (int x = 0; x < _config.Width; x++) {
                    HexCoordinates coordinates = _mapper.GetCoordinateFromOffset(x, z);
                    Vector3 position = _mapper.GetWorldPositionFromOffset(x, z);
                    
                    HexCell cell = CreateCell(coordinates, position);
                    if (cell != null) {
                        _cells[coordinates] = cell;
                    }
                }
            }
        }

        private HexCell CreateCell(HexCoordinates coordinates, Vector3 position) {
            if (_config.CellPrefab == null) {
                Debug.LogError("HexCell prefab is not assigned!");
                return null;
            }

            HexCell cell = Object.Instantiate(_config.CellPrefab, _transform);
            cell.transform.localPosition = position;
            cell.SetCoordinates(coordinates);
            
            return cell;
        }
    }
}

